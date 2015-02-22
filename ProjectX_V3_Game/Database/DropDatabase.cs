//Project by BaussHacker aka. L33TS

using System;
using System.IO;
using ProjectX_V3_Lib.IO;
using ProjectX_V3_Lib.Sql;

namespace ProjectX_V3_Game.Database
{
	/// <summary>
	/// The database handling drop data.
	/// </summary>
	public class DropDatabase
	{
		/// <summary>
		/// Loads all the drop data.
		/// </summary>
		/// <returns>Returns true if the drops were loaded.</returns>
		public static bool LoadDropData()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("\tLoading Drops...");

			using (var drop = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(drop, SqlCommandType.SELECT, false))
				{
					cmd.Finish("DB_MapDrops");
				}
				while (drop.Read())
				{
					int dropid = drop.ReadInt32("MapID");
					Data.DropData data = new ProjectX_V3_Game.Data.DropData();
					data.MinGoldDrop = drop.ReadUInt32("MinMoney");
					data.MaxGoldDrop = drop.ReadUInt32("MaxMoney");
					data.CPsDropChance = drop.ReadByte("CPsChance");
					data.MinCPsDrop = drop.ReadUInt32("MinCPs");
					data.MaxCPsDrop = drop.ReadUInt32("MaxCPs");
					data.DragonballChance = drop.ReadByte("DragonballChance");
					data.MeteorChance = drop.ReadByte("MeteorChance");
					data.FirstSocketChance = drop.ReadByte("FirstSocketChance");
					data.SecondSocketChance = drop.ReadByte("SecondSocketChance");
					data.PlusChance = drop.ReadByte("PlusChance");
					data.QualityChance = drop.ReadByte("QualityChance");
					data.MinPlus = drop.ReadByte("MinPlus");
					data.MaxPlus = drop.ReadByte("MaxPlus");
					data.BlessChance = drop.ReadByte("BlessChance");
					foreach (string item in drop.ReadString("ItemList").Split('-'))
					{
						if (!string.IsNullOrWhiteSpace(item))
						{
							data.ItemDrops.Add(uint.Parse(item));
						}
					}
					
					if (!Core.Kernel.DropData.TryAdd(dropid, data))
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("Failed to load drops. Failed at ID: {0}", dropid);
						Console.ResetColor();
						return false;
					}
				}
			}
			
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\tLoaded {0} Drops...", Core.Kernel.DropData.Count);
			return true;
		}
	}
}
