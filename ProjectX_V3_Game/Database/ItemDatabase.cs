//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.IO;
using System.IO;
using ProjectX_V3_Lib.Sql;

namespace ProjectX_V3_Game.Database
{
	/// <summary>
	/// The database handler for items.
	/// </summary>
	public class ItemDatabase
	{
		/// <summary>
		/// Loads all the items.
		/// </summary>
		/// <returns>Returns true if the items were loaded.</returns>
		public static bool LoadItemInfos()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			string wrt = "\tLoading Items...";
			/*using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.SELECT, false))
				{
					cmd.Finish("DB_ItemInfo");
				}
				//int count = 0;
				while (sql.Read())
				{
					count++;
					Console.WriteLine("\tLoaded {0} item additions...", count);
					Console.Clear();
				}
			}
			 */
			//Console.WriteLine("SWAG");
			//return true;
			Console.WriteLine(wrt);
			System.Threading.Thread.Sleep(2000);
			int nameduplicates = 0;
			int loaded = 0;
			using (var item = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(item, SqlCommandType.SELECT, false))
				{
					cmd.Finish("DB_ItemInfo");
				}
				while (item.Read())
				{
					Data.ItemInfo info = new ProjectX_V3_Game.Data.ItemInfo();
					info.ItemID = item.ReadUInt32("ItemID");
					info.Name = item.ReadString("Name");
					byte prof = item.ReadByte("Profession");
					if (prof == 190)
						info.Profession = Enums.Class.InternTaoist;
					info.RequiredProf = item.ReadByte("WeaponSkill");
					info.RequiredLevel = item.ReadByte("RequiredLevel");
					info.Sex = (Enums.Sex)Enum.Parse(typeof(Enums.Sex), item.ReadString("Sex"));
					info.RequiredStrength = item.ReadUInt16("RequiredStrength");
					info.RequiredAgility = item.ReadUInt16("RequiredAgility");
					info.RequiredVitality = item.ReadUInt16("RequiredVitality");
					info.RequiredSpirit = item.ReadUInt16("RequiredSpirit");
					info.Monopoly = item.ReadByte("Monopoly");
					info.Weight = item.ReadUInt16("Weight");
					info.Price = item.ReadUInt32("Price");
					info.ActionID = item.ReadUInt32("ActionID");
					info.MaxAttack = item.ReadUInt16("MaxAttack");
					info.MinAttack = item.ReadUInt16("MinAttack");
					info.Defense = item.ReadUInt16("Defense");
					info.Dexterity = item.ReadUInt16("Dexterity");
					info.Dodge = item.ReadUInt16("Dodge");
					info.HP = item.ReadUInt16("Life");
					info.MP = item.ReadUInt16("Mana");
					info.AmountLimit = item.ReadUInt16("Amount");
					if (info.IsArrow())
					{
						info.MaxDura = (short)info.AmountLimit;
						info.CurrentDura = info.MaxDura;
					}
					info.Ident = item.ReadByte("Ident");
					info.StGem1 = item.ReadByte("Gem1");
					info.StGem2 = item.ReadByte("Gem2");
					info.Magic1 = item.ReadUInt16("Magic1");
					info.Magic2 = item.ReadByte("Magic2");
					info.Magic3 = item.ReadByte("Magic3");
					info.Data = item.ReadInt32("Data");
					info.MagicAttack = item.ReadUInt16("MagicAttack");
					info.MagicDefense = item.ReadUInt16("MagicDefense");
					info.AttackRange = item.ReadUInt16("AttackRange");
					info.AttackSpeed = item.ReadUInt16("AttackSpeed");
					info.FrayMode = item.ReadByte("FrayMode");
					info.RepairMode = item.ReadByte("RepairMode");
					info.TypeMask = item.ReadByte("TypeMask");
					info.CPPrice = item.ReadUInt32("EMoneyPrice");
					info.Unknown1 = item.ReadUInt32("Unknown1");
					info.Unknown2 = item.ReadUInt32("Unknown2");
					info.CriticalStrike = item.ReadUInt32("CriticalStrike");
					info.SkillCriticalStrike = item.ReadUInt32("SkillCriticalStrike");
					info.Immunity = item.ReadUInt32("Immunity");
					info.Penetration = item.ReadUInt32("Penetration");
					info.Block = item.ReadUInt32("Block");
					info.BreakThrough = item.ReadUInt32("BreakThrough");
					info.CounterAction = item.ReadUInt32("CounterAction");
					info.StackLimit = item.ReadUInt32("StackLimit");
					info.ResistMetal = item.ReadUInt32("ResistMetal");
					info.ResistWood = item.ReadUInt32("ResistWood");
					info.ResistWater = item.ReadUInt32("ResistWater");
					info.ResistFire = item.ReadUInt32("ResistFire");
					info.ResistEarth = item.ReadUInt32("ResistEarth");
					info.TypeName = item.ReadString("TypeName");
					info.Description = item.ReadString("Description");
					info.NameColor = item.ReadUInt32("NameColor");
					info.DragonSoulPhase = item.ReadUInt32("DragonSoulPhase");
					info.DragonSoulRequirements = item.ReadUInt32("DragonSoulRequirements");
					info.Plus = item.ReadByte("StaticPlus");
					
					if (info.IsShield())
						info.ItemType = Enums.ItemType.Shield;
					else if (info.IsArmor())
						info.ItemType = Enums.ItemType.Armor;
					else if (info.IsHeadgear())
						info.ItemType = Enums.ItemType.Head;
					else if (info.IsOneHand())
						info.ItemType = Enums.ItemType.OneHand;
					else if (info.IsTwoHand())
						info.ItemType = Enums.ItemType.TwoHand;
					else if (info.IsArrow())
						info.ItemType = Enums.ItemType.Arrow;
					else if (info.IsBow())
						info.ItemType = Enums.ItemType.Bow;
					else if (info.IsNecklace())
						info.ItemType = Enums.ItemType.Necklace;
					else if (info.IsRing())
						info.ItemType = Enums.ItemType.Ring;
					else if (info.IsBoots())
						info.ItemType = Enums.ItemType.Boots;
					else if (info.IsGarment())
						info.ItemType = Enums.ItemType.Garment;
					else if (info.IsFan())
						info.ItemType = Enums.ItemType.Fan;
					else if (info.IsTower())
						info.ItemType = Enums.ItemType.Tower;
					else if (info.IsSteed())
						info.ItemType = Enums.ItemType.Steed;
					else if (info.IsBottle())
						info.ItemType = Enums.ItemType.Bottle;
					else if (info.IsMountArmor())
						info.ItemType = Enums.ItemType.SteedArmor;
					else
						info.ItemType = Enums.ItemType.Misc;

					if (Core.Kernel.ItemInfos.Contains(info.Name))
					{
						nameduplicates++;
					}
					if (!Core.Kernel.ItemInfos.TryAddAndDismiss(info.ItemID, info.Name, info))
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("\tFailed to load items. [ADD]" + Core.Kernel.ItemInfos.Count);
						Console.ResetColor();
						return false;
					}
					
					/*if (!Core.Kernel.SortedItemInfos.Contains(info.ItemID))
					{
						if (!Core.Kernel.SortedItemInfos.TryAdd(info.ItemID, info.Name, new System.Collections.Concurrent.ConcurrentDictionary<byte, Data.ItemInfo>()))
						{
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine("\tFailed to load items. [SORTED]" + Core.Kernel.ItemInfos.Count);
							Console.ResetColor();
							return false;
						}
					}
					
					if (!Core.Kernel.SortedItemInfos.selectorCollection2[info.Name].TryAdd(info.Quality, info))
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("\tFailed to load items. [SORTED : NAME]" + Core.Kernel.ItemInfos.Count);
						Console.ResetColor();
						return false;
					}*/
					
					Console.Clear();
					Console.WriteLine("{0}{1}\tLoaded {2} items... Duplicates so far: {3}", wrt, Environment.NewLine, loaded++, nameduplicates);
				}
			}
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\tLoaded {0} Items... Duplicate names: {1}", Core.Kernel.ItemInfos.Count, nameduplicates);
			return true;
		}
		
		/// <summary>
		/// Loads all the item additions.
		/// </summary>
		public static void LoadItemAdditions()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("\tLoading item additions...");

			foreach (string line in System.IO.File.ReadAllLines(ServerDatabase.DatabaseLocation + "\\Misc\\itemAdd.ini"))
			{
				if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))
					continue;
				
				Data.ItemAddition addition = new ProjectX_V3_Game.Data.ItemAddition();
				string[] data = line.Split(' ');
				addition.ItemID = uint.Parse(data[0]);
				addition.Plus = byte.Parse(data[1]);
				addition.HP = ushort.Parse(data[2]);
				addition.MinAttack = uint.Parse(data[3]);
				addition.MaxAttack = uint.Parse(data[4]);
				addition.Defense = ushort.Parse(data[5]);
				addition.MagicAttack = ushort.Parse(data[6]);
				addition.MagicDefense = ushort.Parse(data[7]);
				addition.Dexterity = ushort.Parse(data[8]);
				addition.Dodge = byte.Parse(data[9]);
				
				if (!Core.Kernel.ItemAdditions.ContainsKey(addition.ItemID))
				{
					if (!Core.Kernel.ItemAdditions.TryAdd(addition.ItemID))
					{
						Console.ForegroundColor = ConsoleColor.Red;
						throw new Exception("Failed to load item additions.");
					}
				}
				else
				{
					if (!Core.Kernel.ItemAdditions[addition.ItemID].TryAdd(addition.Plus, addition))
						throw new Exception("Failed to load item additions.");
				}
			}
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\tLoaded {0} item additions...", Core.Kernel.ItemAdditions.Count);
		}
	}
}
