//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;
using ProjectX_V3_Lib.IO;
using ProjectX_V3_Lib.Extensions;
using System.Linq;
using ProjectX_V3_Lib.Sql;

namespace ProjectX_V3_Game.Database
{
	/// <summary>
	/// A database associated with the characters.
	/// </summary>
	public class CharacterDatabase
	{
		#region Old
		/*
		/// <summary>
		/// The different types of flags for the database files.
		/// </summary>
		public enum DatabaseFlag
		{
			CharacterFile = 0,
			SpouseFile = 1,
			InventoryFile = 2,
			EquipmentFile = 3,
			ProfFile = 4,
			SpellFile = 5
		}
		
		/// <summary>
		/// The database files associated with the character.
		/// </summary>
		public readonly ConcurrentDictionary<DatabaseFlag, IniFile> databaseFiles;
		
		private Entities.GameClient client;
		
		/// <summary>
		/// Creates a new instance of CharacterDatabase.
		/// </summary>
		/// <param name="client">The client that is associated with the database.</param>
		/// <param name="UID">The database UID.</param>
		public CharacterDatabase(Entities.GameClient client, int UID)
		{
			this.client = client;
			databaseFiles = new ConcurrentDictionary<DatabaseFlag, IniFile>();
			AddFile(DatabaseFlag.CharacterFile, "Characters", "Character", UID);
			UpdateSpouse(UID);
			AddFile(DatabaseFlag.InventoryFile, "Inventories", "Inventory", UID);
			AddFile(DatabaseFlag.EquipmentFile, "Equipments", "Equips", UID);
			AddFile(DatabaseFlag.ProfFile, "Profs", "Profs", UID);
			AddFile(DatabaseFlag.SpellFile, "Spells", "Spells", UID);
		}
		
		/// <summary>
		/// Adds a spouse.
		/// </summary>
		/// <param name="UID">The UID.</param>
		/// <param name="SpouseUID">The spouse UID.</param>
		public void AddSpouse(int UID, int SpouseUID)
		{
			RemoveSpouse(UID, false);
			
			AddFile(DatabaseFlag.SpouseFile, "Characters", "Character", SpouseUID);
			
			databaseFiles[DatabaseFlag.CharacterFile].Write<int>("SpouseUID", SpouseUID);
		}
		
		/// <summary>
		/// Updates the spouse.
		/// </summary>
		/// <param name="UID">The UID.</param>
		public void UpdateSpouse(int UID)
		{
			client.SpouseDatabaseUID = databaseFiles[DatabaseFlag.CharacterFile].ReadInt32("SpouseUID", 0);
			if (client.SpouseDatabaseUID > 0)
			{
				if (!databaseFiles.ContainsKey(DatabaseFlag.SpouseFile))
					AddFile(DatabaseFlag.SpouseFile, "Characters", "Character", client.SpouseDatabaseUID);
				if (databaseFiles[DatabaseFlag.SpouseFile].ReadInt32("SpouseUID", 0) == 0)
				{
					RemoveSpouse(UID);
				}
			}
		}
		
		/// <summary>
		/// Removes a spouse.
		/// </summary>
		/// <param name="UID">The UID.</param>
		/// <param name="removeuid">Boolean defining whether to remove the UID or not.</param>
		public void RemoveSpouse(int UID, bool removeuid = true)
		{
			if (databaseFiles.ContainsKey(DatabaseFlag.SpouseFile))
			{
				IniFile rfile;
				databaseFiles.TryRemove(DatabaseFlag.SpouseFile, out rfile);
				if (removeuid)
					client.SpouseDatabaseUID = 0;
				databaseFiles[DatabaseFlag.CharacterFile].Write<int>("SpouseUID", 0);
			}
		}
		
		/// <summary>
		/// Adds a file to the database.
		/// </summary>
		/// <param name="flag">The database flag.</param>
		/// <param name="name">The folder name.</param>
		/// <param name="section">The file section.</param>
		/// <param name="UID">The database UID.</param>
		private void AddFile(DatabaseFlag flag, string name, string section, int UID)
		{
			if (!databaseFiles.TryAdd(flag, new IniFile(ServerDatabase.DatabaseLocation + "\\" + name + "\\" + UID + ".ini", section)))
			{
				throw new Exception("Failed to initialize \"" + flag + "\" for UID: " + UID);
			}
		}
		
		/// <summary>
		/// Gets a database file depending on a flag index.
		/// </summary>
		public IniFile this[DatabaseFlag flag]
		{
			get
			{
				if (!databaseFiles.ContainsKey(flag))
					return new IniFile("DO_NOT_EVER_MAKE_THIS_FILE.ini", "DO_NOT_EVER_MAKE_THIS_FILE.ini");
				
				return databaseFiles[flag];
			}
		}
		 */
		#endregion
		
		private ConcurrentDictionary<byte, IniFile> ItemFiles;
		private ConcurrentDictionary<ushort, IniFile> WarehouseFiles;
		private static readonly ushort[] whids = new ushort[]
		{
			8,
			10012,
			10028,
			10011,
			1027,
			4101,
			44
		};
		
		public CharacterDatabase(int DatabaseUID)
		{
			ItemFiles = new ConcurrentDictionary<byte, IniFile>();
			WarehouseFiles = new ConcurrentDictionary<ushort, IniFile>();
			foreach (ushort MapID in whids)
			{
				if (!WarehouseFiles.TryAdd(MapID, new IniFile(Database.ServerDatabase.DatabaseLocation + "\\Warehouses\\" + DatabaseUID + "_" + MapID + ".ini")))
					throw new Exception("Failed to add warehouse id " + MapID + " of " + DatabaseUID);
			}
			if (!ItemFiles.TryAdd(0, new IniFile(Database.ServerDatabase.DatabaseLocation + "\\Inventories\\" + DatabaseUID + ".ini")))
				throw new Exception("Failed to add inventory of " + DatabaseUID);
			if (!ItemFiles.TryAdd(1, new IniFile(Database.ServerDatabase.DatabaseLocation + "\\Equipments\\" + DatabaseUID + ".ini")))
				throw new Exception("Failed to add equipment of " + DatabaseUID);
		}
		/// <summary>
		/// Loads all the data of a character.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="newchar">[out] if true then the character is not yet made.</param>
		/// <returns>Returns true.</returns>
		public static bool LoadCharacter(Entities.GameClient client, out bool newchar)
		{
			newchar = false;
			
			try
			{
				client.CharDB = new CharacterDatabase(client.DatabaseUID);
				
				#region Load Stats ; Loads all the main stats of the character (name, level, coordinates etc.)
				using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
				{
					using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.SELECT, true))
					{
						cmd.AddWhereValue("PlayerID", client.DatabaseUID);
						cmd.Finish("DB_Players");
					}

					if (!sql.Read())
						return false;
					
					if (sql.ReadBoolean("PlayerNew"))
					{
						newchar = true;
						return false;
					}
					client.Name = sql.ReadString("PlayerName");
					if (string.IsNullOrEmpty(client.Name) || string.IsNullOrWhiteSpace(client.Name))
						return false;
					
					client.Permission = (Enums.PlayerPermission)Enum.Parse(typeof(Enums.PlayerPermission), sql.ReadString("PlayerPermission"));
					client.Faction = (Enums.Faction)Enum.Parse(typeof(Enums.Faction), sql.ReadString("PlayerFaction"));
					client.Avatar = sql.ReadUInt16("PlayerAvatar");
					client.Model = sql.ReadUInt16("PlayerModel");
					client.HairStyle = sql.ReadUInt16("PlayerHairStyle");
					client.Money = sql.ReadUInt32("PlayerMoney");
					client.WarehouseMoney = sql.ReadUInt32("PlayerWarehouseMoney");
					client.CPs = sql.ReadUInt32("PlayerCPs");
					client.BoundCPs = sql.ReadUInt32("PlayerBoundCPs");
					client.Strength = sql.ReadUInt16("PlayerStrength");
					client.Agility = sql.ReadUInt16("PlayerAgility");
					client.Vitality = sql.ReadUInt16("PlayerVitality");
					client.Spirit = sql.ReadUInt16("PlayerSpirit");
					client.AttributePoints = sql.ReadUInt16("PlayerAttributePoints");
					client.MaxHP = sql.ReadInt32("PlayerMaxHP");
					client.HP = sql.ReadInt32("PlayerHP");
					if (client.HP <= 0)
						client.HP = 1;
					client.MaxMP = sql.ReadInt32("PlayerMaxMP");
					client.MP = sql.ReadInt32("PlayerMP");
					client.PKPoints = sql.ReadInt16("PlayerPKPoints");
					client.Level = sql.ReadByte("PlayerLevel");
					client.Experience = sql.ReadUInt64("PlayerExperience");
					client.Class = (Enums.Class)Enum.Parse(typeof(Enums.Class), sql.ReadString("PlayerClass"));
					client.PlayerTitle = (Enums.PlayerTitle)Enum.Parse(typeof(Enums.PlayerTitle), sql.ReadString("PlayerTitle"));
					client.Account = sql.ReadString("PlayerAccount");
					client.Reborns = sql.ReadByte("PlayerReborns");
					client.SpouseDatabaseUID = sql.ReadInt32("PlayerSpouseID");
					client.QuestPoints = sql.ReadUInt32("PlayerQuestPoints");
					/*string n = sql.ReadObject("PlayerCurrentQuest").ToString();
					if (!string.IsNullOrWhiteSpace(n))
						client.CurrentQuest = client.Quests[n];*/
					Maps.MapPoint startmap = Maps.MapTools.GetStartMap(
						sql.ReadUInt16("PlayerMapID"),
						sql.ReadUInt16("PlayerX"),
						sql.ReadUInt16("PlayerY"),
						sql.ReadUInt16("PlayerLastMapID"));
					
					client.Map = startmap.Map;
					client.LastMapID = client.Map.MapID;
					client.X = startmap.X;
					client.Y = startmap.Y;
					client.LastMapX = client.X;
					client.LastMapY = client.Y;
					client.LastX = client.X;
					client.LastY = client.Y;
					uint entityuid;
					Maps.IMapObject rObject;
					if (client.Map.ContainsClientByName(client.Name, out entityuid))
						client.Map.MapObjects.TryRemove(entityuid, out rObject);
					
					if (!client.Map.EnterMap(client))
						return false;
				}
				#endregion
				
				#region Load Inventory ; Loads the inventory of the character.
				IniFile itemfile = client.CharDB.ItemFiles[0];
				if (itemfile.Exists())
				{
					string[] sections = itemfile.GetSectionNames(255);
					if (sections.Length > 0)
					{
						int[] positions;
						sections.ConverToInt32(out positions);

						for (int i = 0; i < 40; i++)
						{
							if (positions.Contains(i))
							{
								itemfile.SetSection(i.ToString());
								uint itemid = itemfile.ReadUInt32("ItemID", 0);
								if (itemid == 0)
									return false;
								
								Data.ItemInfo item;
								Data.ItemInfo original;
								if (!Core.Kernel.ItemInfos.TrySelect(itemid, out original))
									return false;
								item = original.Copy();
								
								item.Plus = itemfile.ReadByte("Plus", 0);
								item.Bless = itemfile.ReadByte("Bless", 0);
								item.Enchant = itemfile.ReadByte("Enchant", 0);
								item.Gem1 = (Enums.SocketGem)Enum.Parse(typeof(Enums.SocketGem), itemfile.ReadString("Gem1", "NoSocket"));
								item.Gem2 = (Enums.SocketGem)Enum.Parse(typeof(Enums.SocketGem), itemfile.ReadString("Gem2", "NoSocket"));
								item.Location = Enums.ItemLocation.Inventory;
								item.CurrentDura = itemfile.ReadInt16("CurrentDura", 0);
								item.MaxDura = itemfile.ReadInt16("MaxDura", 0);
								item.Color = (Enums.ItemColor)Enum.Parse(typeof(Enums.ItemColor), itemfile.ReadString("Color", "Orange"));
								item.SocketAndRGB = itemfile.ReadUInt32("SocketProgress", 0);
								
//							// other data
//							public uint SocketAndRGB = 0;
//							public ushort CurrentDura = 100;
//							public ushort MaxDura = 100;
//							public ushort RebornEffect = 0;
//							public bool Free = false;
//							public uint GreenText = 0;
//							public uint INS = 0;
//							public bool Suspicious = false;
//							public bool Locked = false;
//							public uint Composition = 0;
//							public uint LockedTime = 0;
//							public ushort Amount = 0;
//							public byte Color = 0;
								
								if (!client.Inventory.AddItem(item, (byte)i))
									return false;
							}
						}
					}
				}
				#endregion
				
				#region Load Equipments ; Loads the equipments of the character.
				IniFile equipfile = client.CharDB.ItemFiles[1];
				if (equipfile.Exists())
				{
					for (byte i = 1; i <= 32; i++)
					{
						equipfile.SetSection(i.ToString());
						uint itemid = equipfile.ReadUInt32("ID", 0);
						if (itemid > 0)
						{
							Data.ItemInfo item;
							Data.ItemInfo original;
							if (!Core.Kernel.ItemInfos.TrySelect(itemid, out original))
								return false;
							item = original.Copy();
							
							item.Plus = equipfile.ReadByte("Plus", 0);
							item.Bless = equipfile.ReadByte("Bless", 0);
							item.Enchant = equipfile.ReadByte("Enchant", 0);
							item.Gem1 = (Enums.SocketGem)Enum.Parse(typeof(Enums.SocketGem), equipfile.ReadString("Gem1", "NoSocket"));
							item.Gem2 = (Enums.SocketGem)Enum.Parse(typeof(Enums.SocketGem), equipfile.ReadString("Gem2", "NoSocket"));
							item.CurrentDura = equipfile.ReadInt16("CurrentDura", 0);
							item.MaxDura = equipfile.ReadInt16("MaxDura", 0);
							item.Color = (Enums.ItemColor)Enum.Parse(typeof(Enums.ItemColor), equipfile.ReadString("Color", "Orange"));
							item.SocketAndRGB = equipfile.ReadUInt32("SocketProgress", 0);
							
							// FIX REMOVE!!
							client.Equipments.Equip(item, (Enums.ItemLocation)i, false);
						}
					}
				}
				#endregion
				
				#region Load Prof ; Loads the prof-skills of the character.
				using (var prof = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
				{
					using (var cmd = new SqlCommandBuilder(prof, SqlCommandType.SELECT, true))
					{
						cmd.AddWhereValue("PlayerID", client.DatabaseUID);
						cmd.Finish("DB_PlayerProfs");
					}
					while (prof.Read())
					{
						Data.SpellInfo profinfo = new ProjectX_V3_Game.Data.SpellInfo();
						profinfo.ID = prof.ReadUInt16("Prof");
						profinfo.Level = prof.ReadUInt16("ProfLevel");
						profinfo.Experience = prof.ReadUInt32("ProfExperience");
						client.SpellData.AddProf(profinfo);
					}
				}
				#endregion
				
				#region Load Skills ; Loads the skills of the character.
				using (var spell = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
				{
					using (var cmd = new SqlCommandBuilder(spell, SqlCommandType.SELECT, true))
					{
						cmd.AddWhereValue("PlayerID", client.DatabaseUID);
						cmd.Finish("DB_PlayerSpells");
					}
					while (spell.Read())
					{
						Data.SpellInfo spellinfo = new ProjectX_V3_Game.Data.SpellInfo();
						spellinfo.ID = spell.ReadUInt16("SpellSkillID");
						spellinfo.Level = spell.ReadUInt16("SpellLevel");
						spellinfo.Experience = spell.ReadUInt32("SpellExperience");
						client.SpellData.AddSpell(spellinfo);
					}
				}
				#endregion
				
				#region Load Banks ; Loads the warehouses of the character.
				foreach (ushort WhID in whids)
				{
					IniFile warehousefile = client.CharDB.WarehouseFiles[WhID];
					if (warehousefile.Exists())
					{
						string[] sections = warehousefile.GetSectionNames(255);
						if (sections.Length > 0)
						{
							int[] positions;
							sections.ConverToInt32(out positions);

							for (int i = 0; i < 40; i++)
							{
								if (positions.Contains(i))
								{
									warehousefile.SetSection(i.ToString());
									uint itemid = warehousefile.ReadUInt32("ItemID", 0);
									if (itemid == 0)
										return false;
									
									Data.ItemInfo item;
									Data.ItemInfo original;
									if (!Core.Kernel.ItemInfos.TrySelect(itemid, out original))
										return false;
									item = original.Copy();
									
									item.Plus = warehousefile.ReadByte("Plus", 0);
									item.Bless = warehousefile.ReadByte("Bless", 0);
									item.Enchant = warehousefile.ReadByte("Enchant", 0);
									item.Gem1 = (Enums.SocketGem)Enum.Parse(typeof(Enums.SocketGem), warehousefile.ReadString("Gem1", "NoSocket"));
									item.Gem2 = (Enums.SocketGem)Enum.Parse(typeof(Enums.SocketGem), warehousefile.ReadString("Gem2", "NoSocket"));
									item.Location = Enums.ItemLocation.Inventory;
									item.CurrentDura = warehousefile.ReadInt16("CurrentDura", 0);
									item.MaxDura = warehousefile.ReadInt16("MaxDura", 0);
									item.Color = (Enums.ItemColor)Enum.Parse(typeof(Enums.ItemColor), warehousefile.ReadString("Color", "Orange"));
									item.SocketAndRGB = warehousefile.ReadUInt32("SocketProgress", 0);
									
//							// other data
//							public uint SocketAndRGB = 0;
//							public ushort CurrentDura = 100;
//							public ushort MaxDura = 100;
//							public ushort RebornEffect = 0;
//							public bool Free = false;
//							public uint GreenText = 0;
//							public uint INS = 0;
//							public bool Suspicious = false;
//							public bool Locked = false;
//							public uint Composition = 0;
//							public uint LockedTime = 0;
//							public ushort Amount = 0;
//							public byte Color = 0;
									
									if (!client.Warehouses[WhID].AddItem(item, (byte)i))
										return false;
								}
							}
						}
					}
				}
				#endregion
				
				#region Load Guild ; Loads the guild of the character.
				// Look Packets.GeneralData.GetSynAttr.cs
				#endregion
				
				#region Load Association ; Loads the character associations.
				#endregion
				
				#region Load Quests ; Loads the quests of the character.
				/*using (var quest = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
				{
					using (var cmd = new SqlCommandBuilder(quest, SqlCommandType.SELECT, true))
					{
						cmd.AddWhereValue("PlayerID", client.DatabaseUID);
						cmd.Finish("DB_Quests");
					}
					while (quest.Read())
					{
						string Name = quest.ReadString("QuestName");
						int Progress = quest.ReadInt32("QuestProgress");
						
						if (Progress == -1)
							client.Quests[Name].Finished = true;
						else
						{
							client.Quests[Name].QuestProgress = (ushort)Progress;
							client.Quests[Name].LoadInfoString(quest.ReadString("QuestInfo"));
						}
					}
				}*/
				#endregion
				
				client.BaseEntity.SetBaseStats();
				client.BaseEntity.CalculateBaseStats();
				
				UpdateSpouse(client);
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				return false;
			}
		}
		
		/// <summary>
		/// Loads the offline message query.
		/// </summary>
		/// <param name="client">The client.</param>
		public static void LoadMessageQuery(Entities.GameClient client)
		{
			string file = Database.ServerDatabase.DatabaseLocation + "\\MessageQuery\\" + client.Name + ".ini";
			IniFile whisper = new ProjectX_V3_Lib.IO.IniFile(file, "Whisper");
			if (!whisper.Exists())
				return;
			
			int count = whisper.ReadInt32("Count", 0);
			for (int i = 1; i <= count; i++)
			{
				whisper.SetSection(i.ToString());
				try
				{
					using (var wmsg = Packets.Message.MessageCore.Create(Enums.ChatType.Whisper,
					                                                     "[Offline]" + whisper.ReadString("From", "."),
					                                                     client.Name,
					                                                     whisper.ReadString("Message", ".")))
					{
						wmsg.FromMesh = whisper.ReadUInt32("Mesh", client.Mesh);
						wmsg.ToMesh = client.Mesh;
						client.Send(wmsg);
					}
				}
				catch { }
			}
			
			System.IO.File.Delete(file);
		}

		public static void UpdateCharacter(Entities.GameClient client, string Column, object Value)
		{
			if (client.IsAIBot)
				return;
			
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.UPDATE, true))
				{
					cmd.AddWhereValue("PlayerID", client.DatabaseUID);
					cmd.AddUpdateValue(Column, Value);
					cmd.Finish("DB_Players");
				}
				sql.Execute();
			}
		}
		
		public static string GetSpouseName(Entities.GameClient client)
		{
			if (client.SpouseDatabaseUID == 0 || client.IsAIBot)
				return "None";
			
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.SELECT, true))
				{
					cmd.AddWhereValue("PlayerID", client.SpouseDatabaseUID);
					cmd.Finish("DB_Players");
				}
				if (sql.Read())
					return sql.ReadString("PlayerName");
			}
			return "None";
		}
		
		public static uint GetSpouseEntityUID(Entities.GameClient client)
		{
			if (client.SpouseDatabaseUID == 0)
				return 0;
			
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.SELECT, true))
				{
					cmd.AddWhereValue("PlayerID", client.SpouseDatabaseUID);
					cmd.Finish("DB_Players");
				}
				if (sql.Read())
					return sql.ReadUInt32("PlayerLastEntityUID");
			}
			return 0;
		}
		
		public static SqlHandler OpenRead(Entities.GameClient client, string Table)
		{
			if (client.IsAIBot)
				return null;
			SqlHandler handler = new SqlHandler(Program.Config.ReadString("GameConnectionString"));
			using (var cmd = new SqlCommandBuilder(handler, SqlCommandType.SELECT, true))
			{
				cmd.AddWhereValue("PlayerID", client.DatabaseUID);
				cmd.Finish(Table);
			}
			if (!handler.Read())
				return null;
			
			return handler;
		}
		public static void AddSpouse(Entities.GameClient client1, Entities.GameClient client2)
		{
			client1.SpouseDatabaseUID = client2.DatabaseUID;
			client2.SpouseDatabaseUID = client1.DatabaseUID;
			
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.UPDATE, true))
				{
					cmd.AddWhereValue("PlayerID", client1.DatabaseUID);
					cmd.AddUpdateValue("PlayerSpouseID", client2.DatabaseUID);
					cmd.Finish("DB_Players");
				}
				sql.Execute();
				sql.Forward(Program.Config.ReadString("GameConnectionString"), "");
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.UPDATE, true))
				{
					cmd.AddWhereValue("PlayerID", client2.DatabaseUID);
					cmd.AddUpdateValue("PlayerSpouseID", client1.DatabaseUID);
					cmd.Finish("DB_Players");
				}
				sql.Execute();
			}
		}
		
		public static void UpdateSpouse(Entities.GameClient client)
		{
			if (client.SpouseDatabaseUID == 0)
				return;
			
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.SELECT, true))
				{
					cmd.AddWhereValue("PlayerID", client.SpouseDatabaseUID);
					cmd.Finish("DB_Players");
				}
				
				if (!sql.Read())
				{
					RemoveSpouse(client);
				}
				else if (sql.ReadInt32("PlayerSpouseID") != client.DatabaseUID)
				{
					RemoveSpouse(client);
				}
			}
		}
		
		public static void RemoveSpouse(Entities.GameClient client)
		{
			client.SpouseDatabaseUID = 0;
			Save(client);
		}
		
		/// <summary>
		/// Creates a new character.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="name">The character name.</param>
		/// <param name="job">The job id.</param>
		/// <param name="model">The model.</param>
		/// <returns>Returns true if the character was created successfully.</returns>
		public static bool CreateCharacter(Entities.GameClient client, string name, byte job, ushort model)
		{
			try
			{
				client.Name = name;
				client.BaseEntity.SetBaseStats();
				client.BaseEntity.CalculateBaseStats();
				
				using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
				{
					using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.UPDATE, true))
					{
						cmd.AddWhereValue("PlayerID", client.DatabaseUID);
						cmd.AddUpdateValue("PlayerName", name);//1010 61 109
						cmd.AddUpdateValue("PlayerMapID", (ushort)1010);
						cmd.AddUpdateValue("PlayerLastMapID", (ushort)1010);
						cmd.AddUpdateValue("PlayerX", (ushort)61);
						cmd.AddUpdateValue("PlayerY", (ushort)109);
						cmd.AddUpdateValue("PlayerMoney", (uint)1020);
						cmd.AddUpdateValue("PlayerCPs", (uint)0);
						cmd.AddUpdateValue("PlayerBoundCPs", (uint)0);
						cmd.AddUpdateValue("PlayerLevel", (byte)1);
						cmd.AddUpdateValue("PlayerExperience", (ulong)0);
						cmd.AddUpdateValue("PlayerAvatar", (ushort)AvatarDatabase.GenerateAvatar(model >= 2000));
						cmd.AddUpdateValue("PlayerHairStyle", (ushort)410);
						cmd.AddUpdateValue("PlayerModel", (ushort)model);
						cmd.AddUpdateValue("PlayerClass", ((Enums.Class)job).ToString());
						cmd.AddUpdateValue("PlayerQuizPoints", (uint)0);
						cmd.AddUpdateValue("PlayerPKPoints", (short)0);
						cmd.AddUpdateValue("PlayerMaxHP", (int)client.MaxHP);
						cmd.AddUpdateValue("PlayerMaxMP", (int)client.MaxMP);
						cmd.AddUpdateValue("PlayerHP", (int)client.MaxHP);
						cmd.AddUpdateValue("PlayerMP", (int)client.MaxMP);
						cmd.AddUpdateValue("PlayerStrength", (ushort)client.Strength);
						cmd.AddUpdateValue("PlayerAgility", (ushort)client.Agility);
						cmd.AddUpdateValue("PlayerVitality", (ushort)client.Vitality);
						cmd.AddUpdateValue("PlayerSpirit", (ushort)client.Spirit);
						cmd.AddUpdateValue("PlayerAttributePoints", (ushort)0);
						cmd.AddUpdateValue("PlayerStamina", (byte)0);
						cmd.AddUpdateValue("PlayerReborns", (byte)0);
						cmd.AddUpdateValue("PlayerSpouseID", (int)0);
						cmd.AddUpdateValue("PlayerTitle", Enums.PlayerTitle.None.ToString());
						cmd.AddUpdateValue("PlayerStatusFlag", (ulong)0);
						cmd.AddUpdateValue("PlayerNew", false);
						cmd.AddUpdateValue("PlayerGuildRank", Enums.GuildRank.None.ToString());
						cmd.AddUpdateValue("PlayerGuild", "");
						cmd.AddUpdateValue("PlayerGuildDonation", (uint)0);
						cmd.AddUpdateValue("PlayerWarehouseMoney", (uint)0);
						cmd.AddUpdateValue("TournyDeaths", (uint)0);
						cmd.AddUpdateValue("TournyKills", (uint)0);
						cmd.AddUpdateValue("TournyDeaths", (uint)0);
						cmd.AddUpdateValue("PlayerQuestPoints", (uint)0);
						cmd.AddUpdateValue("PlayerPermission", "Normal");
						cmd.AddUpdateValue("PlayerFaction", "None");
						
						cmd.Finish("DB_Players");
					}
					sql.Execute();
				}
				switch ((Enums.Class)job)
				{
					case Enums.Class.InternTrojan:
					case Enums.Class.InternWarrior:
					case Enums.Class.InternNinja:
						{
							IniFile itemfile = client.CharDB.ItemFiles[0];
							#region Weapon
							itemfile.SetSection("0");
							itemfile.Write<uint>("ItemID", 410301);
							itemfile.Write<byte>("Plus", 0);
							itemfile.Write<byte>("Bless", 0);
							itemfile.Write<byte>("Enchant", 0);
							itemfile.WriteString("Gem1", "NoSocket");
							itemfile.WriteString("Gem2", "NoSocket");
							itemfile.Write<short>("CurrentDura", 100);
							itemfile.Write<short>("MaxDura", 100);
							itemfile.WriteString("Color", "Orange");
							itemfile.Write<uint>("SocketProgress", 0);
							#endregion
							#region Coat
							itemfile.SetSection("1");
							itemfile.Write<uint>("ItemID", 132005);
							itemfile.Write<byte>("Plus", 0);
							itemfile.Write<byte>("Bless", 0);
							itemfile.Write<byte>("Enchant", 0);
							itemfile.WriteString("Gem1", "NoSocket");
							itemfile.WriteString("Gem2", "NoSocket");
							itemfile.Write<short>("CurrentDura", 100);
							itemfile.Write<short>("MaxDura", 100);
							itemfile.WriteString("Color", "Orange");
							itemfile.Write<uint>("SocketProgress", 0);
							#endregion
							#region Potions
							for (int i = 2; i <= 6; i++)
							{
								itemfile.SetSection(i.ToString());
								itemfile.Write<uint>("ItemID", 1000000);
								itemfile.Write<byte>("Plus", 0);
								itemfile.Write<byte>("Bless", 0);
								itemfile.Write<byte>("Enchant", 0);
								itemfile.WriteString("Gem1", "NoSocket");
								itemfile.WriteString("Gem2", "NoSocket");
								itemfile.Write<short>("CurrentDura", 100);
								itemfile.Write<short>("MaxDura", 100);
								itemfile.WriteString("Color", "Orange");
								itemfile.Write<uint>("SocketProgress", 0);
							}
							#endregion
							break;
						}
					case Enums.Class.InternArcher:
						{
							IniFile itemfile = client.CharDB.ItemFiles[0];
							#region Weapon
							itemfile.SetSection("0");
							itemfile.Write<uint>("ItemID", 500301);
							itemfile.Write<byte>("Plus", 0);
							itemfile.Write<byte>("Bless", 0);
							itemfile.Write<byte>("Enchant", 0);
							itemfile.WriteString("Gem1", "NoSocket");
							itemfile.WriteString("Gem2", "NoSocket");
							itemfile.Write<short>("CurrentDura", 100);
							itemfile.Write<short>("MaxDura", 100);
							itemfile.WriteString("Color", "Orange");
							itemfile.Write<uint>("SocketProgress", 0);
							#endregion
							#region Coat
							itemfile.SetSection("1");
							itemfile.Write<uint>("ItemID", 132005);
							itemfile.Write<byte>("Plus", 0);
							itemfile.Write<byte>("Bless", 0);
							itemfile.Write<byte>("Enchant", 0);
							itemfile.WriteString("Gem1", "NoSocket");
							itemfile.WriteString("Gem2", "NoSocket");
							itemfile.Write<short>("CurrentDura", 100);
							itemfile.Write<short>("MaxDura", 100);
							itemfile.WriteString("Color", "Orange");
							itemfile.Write<uint>("SocketProgress", 0);
							#endregion
							#region Potions
							for (int i = 2; i <= 6; i++)
							{
								itemfile.SetSection(i.ToString());
								itemfile.Write<uint>("ItemID", 1000000);
								itemfile.Write<byte>("Plus", 0);
								itemfile.Write<byte>("Bless", 0);
								itemfile.Write<byte>("Enchant", 0);
								itemfile.WriteString("Gem1", "NoSocket");
								itemfile.WriteString("Gem2", "NoSocket");
								itemfile.Write<short>("CurrentDura", 100);
								itemfile.Write<short>("MaxDura", 100);
								itemfile.WriteString("Color", "Orange");
								itemfile.Write<uint>("SocketProgress", 0);
							}
							#endregion
							break;
						}
					case Enums.Class.InternTaoist:
						{
							IniFile itemfile = client.CharDB.ItemFiles[0];
							#region Weapon
							itemfile.SetSection("0");
							itemfile.Write<uint>("ItemID", 421301);
							itemfile.Write<byte>("Plus", 0);
							itemfile.Write<byte>("Bless", 0);
							itemfile.Write<byte>("Enchant", 0);
							itemfile.WriteString("Gem1", "NoSocket");
							itemfile.WriteString("Gem2", "NoSocket");
							itemfile.Write<short>("CurrentDura", 100);
							itemfile.Write<short>("MaxDura", 100);
							itemfile.WriteString("Color", "Orange");
							itemfile.Write<uint>("SocketProgress", 0);
							#endregion
							#region Coat
							itemfile.SetSection("1");
							itemfile.Write<uint>("ItemID", 132005);
							itemfile.Write<byte>("Plus", 0);
							itemfile.Write<byte>("Bless", 0);
							itemfile.Write<byte>("Enchant", 0);
							itemfile.WriteString("Gem1", "NoSocket");
							itemfile.WriteString("Gem2", "NoSocket");
							itemfile.Write<short>("CurrentDura", 100);
							itemfile.Write<short>("MaxDura", 100);
							itemfile.WriteString("Color", "Orange");
							itemfile.Write<uint>("SocketProgress", 0);
							#endregion
							#region Potions
							for (int i = 2; i <= 3; i++)
							{
								itemfile.SetSection(i.ToString());
								itemfile.Write<uint>("ItemID", 1001000);
								itemfile.Write<byte>("Plus", 0);
								itemfile.Write<byte>("Bless", 0);
								itemfile.Write<byte>("Enchant", 0);
								itemfile.WriteString("Gem1", "NoSocket");
								itemfile.WriteString("Gem2", "NoSocket");
								itemfile.Write<short>("CurrentDura", 100);
								itemfile.Write<short>("MaxDura", 100);
								itemfile.WriteString("Color", "Orange");
								itemfile.Write<uint>("SocketProgress", 0);
							}
							for (int i = 4; i <= 6; i++)
							{
								itemfile.SetSection(i.ToString());
								itemfile.Write<uint>("ItemID", 1000000);
								itemfile.Write<byte>("Plus", 0);
								itemfile.Write<byte>("Bless", 0);
								itemfile.Write<byte>("Enchant", 0);
								itemfile.WriteString("Gem1", "NoSocket");
								itemfile.WriteString("Gem2", "NoSocket");
								itemfile.Write<short>("CurrentDura", 100);
								itemfile.Write<short>("MaxDura", 100);
								itemfile.WriteString("Color", "Orange");
								itemfile.Write<uint>("SocketProgress", 0);
							}
							#endregion
							break;
						}
				}
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				return false;
			}
		}
		
		public static bool CharacterExists(string Name)
		{
			try
			{
				using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
				{
					using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.SELECT, true))
					{
						cmd.AddWhereValue("PlayerName", Name);
						cmd.Finish("DB_Players");
					}
					return sql.Read();
				}
			}
			catch { return false; }
		}
		#region Save
		
		#region Regular
		/// <summary>
		/// Saves the basic information of a client.
		/// </summary>
		/// <param name="client">The client.</param>
		public static void Save(Entities.GameClient client)
		{
			try
			{
				if (!client.LoggedIn || client.IsAIBot)
					return;
				
				using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
				{
					using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.UPDATE, true))
					{
						cmd.AddWhereValue("PlayerID", client.DatabaseUID);
						cmd.AddUpdateValue("PlayerMapID", client.Map.MapID);
						cmd.AddUpdateValue("PlayerLastMapID", client.LastMapID);
						cmd.AddUpdateValue("PlayerX", client.X);
						cmd.AddUpdateValue("PlayerY", client.Y);
						cmd.AddUpdateValue("PlayerSpouseID", client.SpouseDatabaseUID);
						cmd.AddUpdateValue("TournyKills", client.TournamentInfo.TotalKills);
						cmd.AddUpdateValue("TournyDeaths", client.TournamentInfo.TotalDeaths);
						if (client.Guild == null)
						{
							cmd.AddUpdateValue("PlayerGuildRank", "None");
							cmd.AddUpdateValue("PlayerGuild", "");
							cmd.AddUpdateValue("PlayerGuildDonation", (uint)0);
							cmd.AddUpdateValue("PlayerGuildCPDonation", (uint)0);
						}
						else
						{
							cmd.AddUpdateValue("PlayerGuildRank", client.GuildMemberInfo.Rank.ToString());
							cmd.AddUpdateValue("PlayerGuild", client.Guild.Name);
							cmd.AddUpdateValue("PlayerGuildDonation", client.GuildMemberInfo.MoneyDonation);
							cmd.AddUpdateValue("PlayerGuildCPDonation", client.GuildMemberInfo.CPDonation);
						}
						cmd.Finish("DB_Players");
					}
					sql.Execute();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Failed to save {0}.", client.Name);
				Console.WriteLine(e.ToString());
			}
		}
		
		public static void SaveQuest(Entities.GameClient client, string Name, int Progress, string InfoString) // -1 = finished
		{
			bool Exists = false;
			using (var existsql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var existcmd = new SqlCommandBuilder(existsql, SqlCommandType.SELECT, true))
				{
					existcmd.AddWhereValue("PlayerID", client.DatabaseUID);
					existcmd.AddWhereValue("QuestName", Name);
					existcmd.Finish("DB_Quests");
				}
				Exists = existsql.Read();
			}
			
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				if (Exists)
				{
					using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.UPDATE, true))
					{
						cmd.AddWhereValue("PlayerID", client.DatabaseUID);
						cmd.AddWhereValue("QuestName", Name);
						cmd.AddUpdateValue("QuestProgress", Progress.ToString());
						cmd.AddUpdateValue("QuestInfo", InfoString);
						cmd.Finish("DB_Quests");
					}
				}
				else
				{
					using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.INSERT, false))
					{
						cmd.AddInsertValue("PlayerID", client.DatabaseUID);
						cmd.AddInsertValue("QuestName", Name);
						cmd.AddInsertValue("QuestProgress", Progress.ToString());
						cmd.AddInsertValue("QuestInfo", InfoString);
						cmd.Finish("DB_Quests");
					}
				}
				sql.Execute();
			}
		}

		public static void SaveTourny(Entities.GameClient client)
		{
			if (!client.LoggedIn || client.IsAIBot)
				return;
			
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.UPDATE, true))
				{
					cmd.AddWhereValue("PlayerID", client.DatabaseUID);
					cmd.AddUpdateValue("TournyKills", client.TournamentInfo.TotalKills);
					cmd.AddUpdateValue("TournyDeaths", client.TournamentInfo.TotalDeaths);

					cmd.Finish("DB_Players");
				}
				sql.Execute();
			}
		}
		
		public static void SaveGuildRank(int UID, Enums.GuildRank rank)
		{
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.UPDATE, true))
				{
					cmd.AddWhereValue("PlayerID", UID);
					cmd.AddUpdateValue("PlayerGuildRank", rank.ToString());
					cmd.Finish("DB_Players");
				}
				sql.Execute();
			}
		}
		public static void RemoveGuild(int UID, bool MakeMember = false)
		{
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.UPDATE, true))
				{
					cmd.AddWhereValue("PlayerID", UID);
					if (!MakeMember)
					{
						cmd.AddUpdateValue("PlayerGuildRank", "None");
						cmd.AddUpdateValue("PlayerGuild", "");
						cmd.AddUpdateValue("PlayerGuildDonation", (uint)0);
						cmd.AddUpdateValue("PlayerGuildCPDonation", (uint)0);
					}
					else
					{
						cmd.AddUpdateValue("PlayerGuildRank", "Member");
					}
					cmd.Finish("DB_Players");
				}
				sql.Execute();
			}
		}
		#endregion
		
		#region Inventory
		/// <summary>
		/// Saves inventory.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="item">The item. [Null for removing]</param>
		/// <param name="pos">The position.</param>
		public static void SaveInventory(Entities.GameClient client, Data.ItemInfo item, byte pos)
		{
			if (!client.LoggedIn || client.IsAIBot)
				return;
			
			IniFile itemfile = client.CharDB.ItemFiles[0];
			if (item == null)
			{
				itemfile.DeleteSection(((byte)pos).ToString());
			}
			else
			{
				itemfile.SetSection(((byte)pos).ToString());
				itemfile.Write<uint>("ItemID", item.ItemID);
				itemfile.Write<byte>("Plus", item.Plus);
				itemfile.Write<byte>("Bless", item.Bless);
				itemfile.Write<byte>("Enchant", item.Enchant);
				itemfile.WriteString("Gem1", item.Gem1.ToString());
				itemfile.WriteString("Gem2", item.Gem2.ToString());
				itemfile.Write<short>("CurrentDura", item.CurrentDura);
				itemfile.Write<short>("MaxDura", item.MaxDura);
				itemfile.WriteString("Color", item.Color.ToString());
				itemfile.Write<uint>("SocketProgress", item.SocketAndRGB);
				/*		public uint SocketAndRGB = 0;
		public ushort CurrentDura = 100;
		public ushort MaxDura = 100;
		public ushort RebornEffect = 0;
		public bool Free = false;
		public uint GreenText = 0;
		public uint INS = 0;
		public bool Suspicious = false;
		public bool Locked = false;
		public uint Composition = 0;
		public uint LockedTime = 0;
		public ushort Amount = 0;
		public byte Color = 0;*/
			}
		}
		
		#endregion
		
		#region Warehouse
		/// <summary>
		/// Saves Warehouse.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="item">The item. [Null for removing]</param>
		/// <param name="pos">The position.</param>
		/// <param name="MapID">The mapid.</param>
		public static void SaveWarehouse(Entities.GameClient client, Data.ItemInfo item, byte pos, ushort whID)
		{
			if (!client.LoggedIn || client.IsAIBot)
				return;
			
			IniFile itemfile = client.CharDB.WarehouseFiles[whID];
			if (item == null)
			{
				itemfile.DeleteSection(((byte)pos).ToString());
			}
			else
			{
				itemfile.SetSection(((byte)pos).ToString());
				itemfile.Write<uint>("ItemID", item.ItemID);
				itemfile.Write<byte>("Plus", item.Plus);
				itemfile.Write<byte>("Bless", item.Bless);
				itemfile.Write<byte>("Enchant", item.Enchant);
				itemfile.WriteString("Gem1", item.Gem1.ToString());
				itemfile.WriteString("Gem2", item.Gem2.ToString());
				itemfile.Write<short>("CurrentDura", item.CurrentDura);
				itemfile.Write<short>("MaxDura", item.MaxDura);
				itemfile.WriteString("Color", item.Color.ToString());
				itemfile.Write<uint>("SocketProgress", item.SocketAndRGB);
				/*		public uint SocketAndRGB = 0;
		public ushort CurrentDura = 100;
		public ushort MaxDura = 100;
		public ushort RebornEffect = 0;
		public bool Free = false;
		public uint GreenText = 0;
		public uint INS = 0;
		public bool Suspicious = false;
		public bool Locked = false;
		public uint Composition = 0;
		public uint LockedTime = 0;
		public ushort Amount = 0;
		public byte Color = 0;*/
			}
		}
		
		#endregion
		
		#region Equips
		/// <summary>
		/// Saves equipments.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="item">The equipment item. [null for removing]</param>
		/// <param name="pos">The position.</param>
		public static void SaveEquipment(Entities.GameClient client, Data.ItemInfo item, Enums.ItemLocation pos)
		{
			if (!client.LoggedIn || client.IsAIBot)
				return;
			
			IniFile itemfile = client.CharDB.ItemFiles[1];
			if (item == null)
			{
				itemfile.DeleteSection(((byte)pos).ToString());
			}
			else
			{
				itemfile.SetSection(((byte)pos).ToString());
				itemfile.Write<uint>("ID", item.ItemID);
				itemfile.Write<byte>("Plus", item.Plus);
				itemfile.Write<byte>("Bless", item.Bless);
				itemfile.Write<byte>("Enchant", item.Enchant);
				itemfile.WriteString("Gem1", item.Gem1.ToString());
				itemfile.WriteString("Gem2", item.Gem2.ToString());
				itemfile.Write<short>("CurrentDura", item.CurrentDura);
				itemfile.Write<short>("MaxDura", item.MaxDura);
				itemfile.WriteString("Color", item.Color.ToString());
				itemfile.Write<uint>("SocketProgress", item.SocketAndRGB);
				
				/*		public uint SocketAndRGB = 0;
		public ushort CurrentDura = 100;
		public ushort MaxDura = 100;
		public ushort RebornEffect = 0;
		public bool Free = false;
		public uint GreenText = 0;
		public uint INS = 0;
		public bool Suspicious = false;
		public bool Locked = false;
		public uint Composition = 0;
		public uint LockedTime = 0;
		public ushort Amount = 0;
		public byte Color = 0;*/
			}
		}
		#endregion
		
		#region Prof
		/// <summary>
		/// Saves profs.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="prof">The prof.</param>
		public static void SaveProf(Entities.GameClient client, Data.SpellInfo prof)
		{
			if (!client.LoggedIn || client.IsAIBot)
				return;
			bool Exists = false;
			using (var existsql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var existcmd = new SqlCommandBuilder(existsql, SqlCommandType.SELECT, true))
				{
					existcmd.AddWhereValue("PlayerID", client.DatabaseUID);
					existcmd.AddWhereValue("Prof", prof.ID);
					existcmd.Finish("DB_PlayerProfs");
				}
				Exists = existsql.Read();
			}
			
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				if (Exists)
				{
					using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.UPDATE, true))
					{
						cmd.AddWhereValue("PlayerID", client.DatabaseUID);
						cmd.AddWhereValue("Prof", prof.ID);
						cmd.AddUpdateValue("ProfLevel", prof.Level);
						cmd.AddUpdateValue("ProfExperience", prof.Experience);
						cmd.Finish("DB_PlayerProfs");
					}
				}
				else
				{
					using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.INSERT, false))
					{
						cmd.AddInsertValue("PlayerID", client.DatabaseUID);
						cmd.AddInsertValue("Prof", prof.ID);
						cmd.AddInsertValue("ProfLevel", prof.Level);
						cmd.AddInsertValue("ProfExperience", prof.Experience);
						cmd.Finish("DB_PlayerProfs");
					}
				}
				
				sql.Execute();
			}
		}
		#endregion
		
		#region Spell
		/// <summary>
		/// Saves spells.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="spell">The spell.</param>
		public static void SaveSpell(Entities.GameClient client, Data.SpellInfo spell)
		{
			if (!client.LoggedIn || client.IsAIBot)
				return;
			
			bool Exists = false;
			using (var existsql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var existcmd = new SqlCommandBuilder(existsql, SqlCommandType.SELECT, true))
				{
					existcmd.AddWhereValue("PlayerID", client.DatabaseUID);
					existcmd.AddWhereValue("SpellSkillID", spell.ID);
					existcmd.Finish("DB_PlayerSpells");
				}
				Exists = existsql.Read();
			}
			
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				if (Exists)
				{
					using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.UPDATE, true))
					{
						cmd.AddWhereValue("PlayerID", client.DatabaseUID);
						cmd.AddWhereValue("SpellSkillID", spell.ID);
						cmd.AddUpdateValue("SpellLevel", spell.Level);
						cmd.AddUpdateValue("SpellExperience", spell.Experience);
						cmd.Finish("DB_PlayerSpells");
					}
				}
				else
				{
					using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.INSERT, false))
					{
						cmd.AddInsertValue("PlayerID", client.DatabaseUID);
						cmd.AddInsertValue("SpellSkillID", spell.ID);
						cmd.AddInsertValue("SpellLevel", spell.Level);
						cmd.AddInsertValue("SpellExperience", spell.Experience);
						cmd.Finish("DB_PlayerSpells");
					}
				}
				
				sql.Execute();
			}
		}
		#endregion
		
		#endregion
	}
}