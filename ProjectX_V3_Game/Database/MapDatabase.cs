//Project by BaussHacker aka. L33TS

using System;
using System.IO;
using ProjectX_V3_Lib.IO;
using ProjectX_V3_Lib.Sql;

namespace ProjectX_V3_Game.Database
{
	/// <summary>
	/// The map database handler.
	/// </summary>
	public class MapDatabase
	{
		/// <summary>
		/// Loads the dmaps.
		/// </summary>
		public static void LoadDMaps()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("\tLoading Dmaps...");
			DMapLoader.DMapServer dmaps = new DMapLoader.DMapServer();
			dmaps.ConquerPath = ServerDatabase.DatabaseLocation + "\\Maps";
			dmaps.Load(true);
			Maps.DMapHandler.SetHandler(dmaps);
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\tLoaded {0} Dmaps...", Maps.DMapHandler.DMaps.Count);
		}
		
		/// <summary>
		/// Loads the map info.
		/// </summary>
		/// <returns>Returns true if the maps were loaded.</returns>
		public static bool LoadMaps()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("\tLoading Maps...");
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.SELECT, false))
				{
					cmd.Finish("DB_MapInfo");
				}
				while (sql.Read())
				{

					Maps.Map map = new ProjectX_V3_Game.Maps.Map(
						sql.ReadUInt16("MapID"),
						sql.ReadString("Name"));

					if (map.MapID == 0)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("Failed to load maps. [MAPID]");
						Console.ResetColor();
						return false;
					}
					map.MapType = (Enums.MapType)Enum.Parse(typeof(Enums.MapType), sql.ReadString("MapType"));
					ushort dmap = sql.ReadUInt16("DMapInfo");
					if (dmap > 0)
						map.DMapInfo = dmap;
					
					map.InheritanceMap = sql.ReadUInt16("InheritanceMap");
					
					using (var sqlflags = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
					{
						using (var cmd2 = new SqlCommandBuilder(sqlflags, SqlCommandType.SELECT, true))
						{
							cmd2.AddWhereValue("MapID", map.MapID);
							cmd2.Finish("DB_MapFlags");
						}
						while (sqlflags.Read())
						{
							Enums.MapTypeFlags flag = (Enums.MapTypeFlags)Enum.Parse(typeof(Enums.MapTypeFlags), sqlflags.ReadString("Flag"));
							
							if (!map.Flags.TryAdd((ulong)flag, flag))
							{
								Console.ForegroundColor = ConsoleColor.Red;
								Console.WriteLine("Failed to load maps. [FLAGS] Failed at ID: {0}", map.MapID);
								Console.ResetColor();
								return false;
							}
						}
					}
					
					if (!Core.Kernel.Maps.TryAdd(map.MapID, map.Name, map))
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("Failed to load maps. [MAP] Failed at ID: {0}", map.MapID);
						Console.ResetColor();
						return false;
					}
				}
			}
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\tLoaded {0} Maps...", Core.Kernel.Maps.Count);
			return true;
		}
		
		/// <summary>
		/// Adds a new map to the database and server.
		/// </summary>
		/// <param name="mapid">The new map id.</param>
		/// <returns>Returns true if the map was added.</returns>
		public static bool AddMap(ushort mapid)
		{
			string name = "UnknownMap" + ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next();
			Maps.Map map = new ProjectX_V3_Game.Maps.Map(mapid, name);
			Enums.MapTypeFlags flag = Enums.MapTypeFlags.Normal;
			if (!map.Flags.TryAdd((ulong)flag, flag))
				return false;
			map.MapType = Enums.MapType.Normal;
			
			IniFile ini = new IniFile(Database.ServerDatabase.DatabaseLocation + "\\MapInfo\\" + mapid + ".ini", "MapInfo");
			ini.Write<ushort>("ID", mapid);
			ini.WriteString("Name", name);
			ini.WriteString("MapType", "Normal");
			ini.SetSection("Flags");
			ini.Write<int>("Count", 1);
			ini.WriteString("0", "Normal");
			
			return Core.Kernel.Maps.TryAddAndDismiss(mapid, name, map);
		}
	}
}
