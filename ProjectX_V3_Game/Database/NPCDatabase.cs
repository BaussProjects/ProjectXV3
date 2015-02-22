//Project by BaussHacker aka. L33TS

using System;
using System.IO;
using ProjectX_V3_Lib.IO;
using ProjectX_V3_Lib.Sql;

namespace ProjectX_V3_Game.Database
{
	/// <summary>
	/// The NPC Database handler.
	/// </summary>
	public class NPCDatabase
	{
		/// <summary>
		/// Loads all the npc informations and spawns.
		/// </summary>
		/// <returns>Returns true if the the infos/spawns were loaded.</returns>
		public static bool LoadNPCInfo()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("\tLoading NPCs...");
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.SELECT, false))
				{
					cmd.Finish("DB_NPCInfo");
				}
				while (sql.Read())
				{
					Entities.NPC npc = new ProjectX_V3_Game.Entities.NPC();
					npc.EntityUID = sql.ReadUInt32("NPCID");
					ushort mapid = sql.ReadUInt16("MapID");
					if (mapid == 0)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("Failed to load npcs. [MAPID]");
						Console.ResetColor();
						return false;
					}
					
					Maps.Map map;
					Core.Kernel.Maps.TrySelect(mapid, out map);
					npc.Map = map;
					if (!npc.Map.EnterMap(npc))
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("Failed to load npcs. [MAP]");
						Console.ResetColor();
						return false;
					}
					
					npc.Mesh = sql.ReadUInt16("Mesh");
					npc.Flag = sql.ReadUInt32("Flag");
					npc.Name = sql.ReadString("NPCName");
					npc.X = sql.ReadUInt16("X");
					npc.Y = sql.ReadUInt16("Y");
					npc.NPCType = (Enums.NPCType)Enum.Parse(typeof(Enums.NPCType), sql.ReadString("Type"));			
					
					npc.Avatar = sql.ReadByte("Avatar");

					if (!Core.Kernel.NPCs.TryAdd(npc.EntityUID, npc))
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("Failed to load npcs. [ADD]");
						Console.ResetColor();
						return false;
					}
				}
			}
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\tLoaded {0} NPCs...", Core.Kernel.NPCs.Count);
			return true;
		}

		/// <summary>
		/// Loads all the shopflag informations and spawns.
		/// </summary>
		/// <returns>Returns true if the the infos/spawns were loaded.</returns>
		public static bool LoadShopFlags()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("\tLoading ShopFlags...");
			int NpcCount = Core.Kernel.NPCs.Count;
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.SELECT, false))
				{
					cmd.Finish("DB_ShopFlags");
				}
				while (sql.Read())
				{
					Entities.NPC npc = new ProjectX_V3_Game.Entities.NPC();
					npc.EntityUID = sql.ReadUInt32("ShopFlagID");
					ushort mapid = sql.ReadUInt16("ShopFlagMapID");
					if (mapid == 0)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("Failed to load shopflags. [MAPID]");
						Console.ResetColor();
						return false;
					}
					
					Maps.Map map;
					Core.Kernel.Maps.TrySelect(mapid, out map);
					npc.Map = map;
					if (!npc.Map.EnterMap(npc))
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("Failed to load shopflags. [MAP]");
						Console.ResetColor();
						return false;
					}
					
					npc.Mesh = sql.ReadUInt16("ShopFlagMesh");
					npc.Flag = sql.ReadUInt32("ShopFlagFlag");
					npc.Name = "";
					npc.X = sql.ReadUInt16("ShopFlagX");
					npc.Y = sql.ReadUInt16("ShopFlagY");
					npc.NPCType = Enums.NPCType.ShopFlag;							
					npc.Avatar = 0;

					if (!Core.Kernel.NPCs.TryAdd(npc.EntityUID, npc))
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("Failed to load shopflags. [ADD]");
						Console.ResetColor();
						return false;
					}
				}
			}
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\tLoaded {0} ShopFlags...", (Core.Kernel.NPCs.Count - NpcCount));
			return true;
		}
		
		/// <summary>
		/// Creates a new npc and spawns it.
		/// </summary>
		/// <param name="id">The id of the npc.</param>
		/// <param name="name">The name of the npc.</param>
		/// <param name="location">The location of the npc.</param>
		/// <param name="mesh">The mesh of the npc.</param>
		/// <param name="avatar">The avatar of the npc.</param>
		/// <param name="npctype">The type of the npc.</param>
		/// <param name="flag">The flat of the npc.</param>
		public static void CreateNPC(uint id, string name, Maps.MapPoint location, ushort mesh, byte avatar, Enums.NPCType npctype = Enums.NPCType.Normal, ushort flag = 2)
		{
			Entities.NPC npc = new ProjectX_V3_Game.Entities.NPC();
			
			npc.EntityUID = id;
			npc.Mesh = (ushort)(mesh * 10);
			npc.Flag = flag;
			npc.Name = name;
			npc.X = location.X;
			npc.Y = location.Y;
			npc.NPCType = npctype;
			npc.Avatar = avatar;
			
			if (!location.Map.EnterMap(npc))
			{
				return;
			}
			
			if (Core.Kernel.NPCs.TryAdd(npc.EntityUID, npc))
			{
				IniFile npcini = new IniFile(ServerDatabase.DatabaseLocation + "\\NPCInfo\\" + id + ".ini", "Info");
				npcini.WriteString("Name", name);
				npcini.WriteString("Type", npctype.ToString());
				npcini.Write<ushort>("MapID", location.MapID);
				npcini.Write<ushort>("X", location.X);
				npcini.Write<ushort>("Y", location.Y);
				npcini.Write<ushort>("Flag", flag);
				npcini.Write<ushort>("Mesh", mesh);
				npcini.Write<byte>("Avatar", avatar);
				
				npc.Screen.UpdateScreen(null);
			}
		}
		
		/// <summary>
		/// Deletes an npc by its id.
		/// </summary>
		/// <param name="id">The id.</param>
		public static void DeleteNPC(uint id)
		{
			Entities.NPC rnpc;
			if (Core.Kernel.NPCs.TryRemove(id, out rnpc))
			{
				rnpc.Map.LeaveMap(rnpc);
				rnpc.Screen.ClearScreen();
				
				System.IO.File.Delete(ServerDatabase.DatabaseLocation + "\\NPCInfo\\" + id + ".ini");
			}
		}
	}
}
