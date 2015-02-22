//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.IO;
using System.Collections.Generic;
using ProjectX_V3_Lib.Extensions;

namespace ProjectX_V3_Game.Database
{
	/// <summary>
	/// The shop database.
	/// </summary>
	public class ShopDatabase
	{
		/// <summary>
		/// Loads all regular shops.
		/// </summary>
		/// <returns>Returns true if the shops were loaded.</returns>
		public static bool LoadMoneyShops()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("\tLoading Shops...");
			
			IniFile shop = new IniFile(ServerDatabase.DatabaseLocation + "\\Misc\\Shop.dat", "Header");
			int Amount = shop.ReadInt32("Amount", 0);
			if (Amount > 0)
			{
				for (int i = 0; i < Amount; i++)
				{
					shop.SetSection("Shop" + i.ToString());
					uint npcid = shop.ReadUInt32("ID", 0);
					if (npcid == 0)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("Failed to load shops. [ID]");
						Console.ResetColor();
						return false;
					}
					
					int itemamount = shop.ReadInt32("ItemAmount", 0);
					if (itemamount == 0)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("Failed to load shops. [AMOUNT]");
						Console.ResetColor();
						return false;
					}
					
					Entities.NPC npc;
					if (Core.Kernel.NPCs.TryGetValue(npcid, out npc))
					{
						npc.Name = shop.ReadString("Name", npc.Name);
					}
					
					Data.Shop shopdata = new ProjectX_V3_Game.Data.Shop(npcid, itemamount, Enums.ShopType.Money);
					
					for (int j = 0; j < itemamount; j++)
					{
						shopdata.ShopItems[j] = shop.ReadUInt32("Item" + j, 0);
						if (shopdata.ShopItems[j] == 0)
						{
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine("Failed to load shops. [ITEM]");
							Console.ResetColor();
							return false;
						}
					}
					
					if (Core.Kernel.Shops.ContainsKey(npcid))
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("Failed to add shop {0}", npcid);
						Console.ResetColor();
						continue;
					}
					if (!Core.Kernel.Shops.TryAdd(npcid, shopdata))
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("Failed to load shops. [ADD]");
						Console.ResetColor();
						return false;
					}
				}
			}
			
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\tLoaded {0} Shops...", Core.Kernel.Shops.Count);
			return true;
		}
		
		/// <summary>
		/// Loads all cps shops.
		/// </summary>
		/// <returns>Returns true if the shops were loaded.</returns>
		public static bool LoadCPShops()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("\tLoading CP Shops...");
			
			List<uint> itemids = new List<uint>();
			foreach (string line in System.IO.File.ReadAllLines(ServerDatabase.DatabaseLocation + "\\Misc\\emoneyshop.dat"))
			{
				itemids.Add(uint.Parse(line));
			}
			uint[] items = itemids.ToArray();
			
			IniFile emoneyshop = new IniFile(ServerDatabase.DatabaseLocation + "\\Misc\\emoneyshop.ini", "EMoneyShop");
			int[] ids;
			emoneyshop.ReadString("ShopIDs", "0").Split(',').ConverToInt32(out ids);
			if (ids.Length == 0)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Failed to load shops. [SHOPIDS]");
				Console.ResetColor();
				return false;
			}
			if (ids.Length == 1 && ids[0] == 0)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Failed to load shops. [SHOPIDS]");
				Console.ResetColor();
				return false;
			}
			foreach (int id in ids)
			{
				uint shopid = (uint)id;
				Data.Shop shopdata = new ProjectX_V3_Game.Data.Shop(shopid, items.Length, Enums.ShopType.CPs);
				for (int i = 0; i < items.Length; i++)
					shopdata.ShopItems[i] = items[i];
				
				if (!Core.Kernel.Shops.TryAdd(shopid, shopdata))
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Failed to load cp shops. [ADD]");
					Console.ResetColor();
					return false;
				}
			}
			
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\tLoaded {0} CP Shops with {1} items...", ids.Length, items.Length);
			return true;
		}
	}
}
