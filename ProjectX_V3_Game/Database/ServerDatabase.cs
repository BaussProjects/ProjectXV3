//Project by BaussHacker aka. L33TS

using System;
using System.IO;
using ProjectX_V3_Lib.IO;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Database
{
	/// <summary>
	/// The database handler for the game server.
	/// </summary>
	public class ServerDatabase
	{
		/// <summary>
		/// The location of the database.
		/// </summary>
		public static readonly string DatabaseLocation;
		
		static ServerDatabase()
		{
			string dbloc = "\\CODB\\game";

			foreach (DriveInfo drives in DriveInfo.GetDrives())
			{
				if (Directory.Exists(drives.Name + dbloc))
				{
					DatabaseLocation = drives.Name + dbloc;
					return;
				}
			}
			DatabaseLocation = Environment.CurrentDirectory + dbloc;
		}
		
		/// <summary>
		/// Sets the login info of the character or creates the base character file if first time logging in.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="Account">The account.</param>
		/// <param name="DatabaseUID">The database UID.</param>
		/// <param name="EntityUID">The entity UID.</param>
		public static void SetLoginOrCreate(SocketClient client, string Account, int DatabaseUID, uint EntityUID)
		{
			IniFile characterfile = new IniFile(DatabaseLocation + "\\Characters\\" + DatabaseUID + ".ini", "Character");
			if (!characterfile.Exists())
				characterfile.Write<bool>("New", true);
			characterfile.WriteString("Account", Account);
			characterfile.Write<uint>("LastEntityUID", EntityUID);
		}
		
		/// <summary>
		/// Loads the database.
		/// </summary>
		public static bool Load()
		{
			if (!Directory.Exists(DatabaseLocation))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("The database does not exist.");
				return false;
			}
			
			try
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("Starting to load the server...");

				System.Threading.Thread.Sleep(2000);
				
				Console.Clear();
				if (!StatsDatabase.Load())
					return false;
				
				Console.Clear();
				if (!PortalDatabase.LoadPortals())
					return false;
				
				Console.Clear();
				if (!DropDatabase.LoadDropData())
					return false;
				
				Console.Clear();
				ItemDatabase.LoadItemAdditions();
				
				Console.Clear();
				if (!SpellDatabase.LoadSpells())
					return false;
				
				Console.Clear();
				AvatarDatabase.LoadAvatars();
				
				Console.Clear();
				MapDatabase.LoadDMaps();
				
				Console.Clear();
				if (!MapDatabase.LoadMaps())
					return false;
				
				Console.Clear();
				if (!ReviveDatabase.LoadReviveSpots())
					return false;
				
				Console.Clear();
				if (!MonsterDatabase.LoadMonsterInfo())
					return false;
				
				Console.Clear();
				if (!MonsterDatabase.LoadMonsterSpawns())
					return false;
				
				Console.Clear();
				if (!NPCDatabase.LoadNPCInfo())
					return false;
				
				Console.Clear();
				if (!NPCDatabase.LoadShopFlags())
					return false;
				
				Console.Clear();
				ScriptDatabase.LoadSettings();
				
				Console.Clear();
				ScriptDatabase.LoadNPCScripts();
				
				Console.Clear();
				if (!ShopDatabase.LoadMoneyShops())
					return false;
				
				Console.Clear();
				if (!ShopDatabase.LoadCPShops())
					return false;
				
				Console.Clear();
				if (!ItemDatabase.LoadItemInfos())
					return false;
				
				Console.Clear();
				ScriptDatabase.LoadItemScripts();
				
				Console.Clear();
				if (!GuildDatabase.LoadGuilds())
					return false;
				
				Console.Clear();
				if (!NobilityDatabase.LoadNobilityBoard())
					return false;
				
				Console.Clear();
				if (!ArenaDatabase.LoadArenaQualifiers())
					return false;
				
				Console.Clear();
				if (!SystemDatabase.LoadSystemVariables())
					return false;
				
				//Console.Clear();
				//if (!SystemDatabase.LoadSystemMessages())
				//	return false;
				
				BotDatabase.LoadBots();
				
				Console.Clear();
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("The database was loaded.");
				return true;
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e.ToString());
				return false;
			}
		}
	}
}
