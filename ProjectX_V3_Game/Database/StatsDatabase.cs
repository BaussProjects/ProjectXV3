//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.IO;

namespace ProjectX_V3_Game.Database
{
	/// <summary>
	/// Description of StatsDatabase.
	/// </summary>
	public class StatsDatabase
	{
		public static bool Load()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("\tLoading Stats...");
			
			#region TrojanStats
			{
				string file = "TrojanStats";
				string[] lines = System.IO.File.ReadAllLines(Database.ServerDatabase.DatabaseLocation + "\\Stats\\" + file + ".dat");
				for (int i = 0; i < lines.Length; i++)
				{
					string line = lines[i];
					if (!string.IsNullOrWhiteSpace(line))
					{
						string[] fulldata = line.Split('=');
						string[] data = fulldata[1].Split(',');
						ushort str = ushort.Parse(data[0]);
						ushort vit = ushort.Parse(data[1]);
						ushort agi = ushort.Parse(data[2]);
						ushort spi = ushort.Parse(data[3]);
						if (!Core.Kernel.TrojanStats.TryAdd(byte.Parse(fulldata[0]),
						                                    new ushort[]
						                                    {
						                                    	str, vit, agi, spi
						                                    }))
						{
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine("Failed to load stats. [TROJAN]");
							Console.ResetColor();
							return false;
						}
					}
				}
			}
			#endregion
			#region WarriorStats
			{
				string file = "WarriorStats";
				string[] lines = System.IO.File.ReadAllLines(Database.ServerDatabase.DatabaseLocation + "\\Stats\\" + file + ".dat");
				for (int i = 0; i < lines.Length; i++)
				{
					string line = lines[i];
					if (!string.IsNullOrWhiteSpace(line))
					{
						string[] fulldata = line.Split('=');
						string[] data = fulldata[1].Split(',');
						ushort str = ushort.Parse(data[0]);
						ushort vit = ushort.Parse(data[1]);
						ushort agi = ushort.Parse(data[2]);
						ushort spi = ushort.Parse(data[3]);
						if (!Core.Kernel.WarriorStats.TryAdd(byte.Parse(fulldata[0]),
						                                     new ushort[]
						                                     {
						                                     	str, vit, agi, spi
						                                     }))
						{
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine("Failed to load stats. [WARRIOR]");
							Console.ResetColor();
							return false;
						}
					}
				}
			}
			#endregion
			#region ArcherStats
			{
				string file = "ArcherStats";
				string[] lines = System.IO.File.ReadAllLines(Database.ServerDatabase.DatabaseLocation + "\\Stats\\" + file + ".dat");
				for (int i = 0; i < lines.Length; i++)
				{
					string line = lines[i];
					if (!string.IsNullOrWhiteSpace(line))
					{
						string[] fulldata = line.Split('=');
						string[] data = fulldata[1].Split(',');
						ushort str = ushort.Parse(data[0]);
						ushort vit = ushort.Parse(data[1]);
						ushort agi = ushort.Parse(data[2]);
						ushort spi = ushort.Parse(data[3]);
						if (!Core.Kernel.ArcherStats.TryAdd(byte.Parse(fulldata[0]),
						                                    new ushort[]
						                                    {
						                                    	str, vit, agi, spi
						                                    }))
						{
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine("Failed to load stats. [ARCHER]");
							Console.ResetColor();
							return false;
						}
					}
				}
			}
			#endregion
			#region NinjaStats
			{
				string file = "NinjaStats";
				string[] lines = System.IO.File.ReadAllLines(Database.ServerDatabase.DatabaseLocation + "\\Stats\\" + file + ".dat");
				for (int i = 0; i < lines.Length; i++)
				{
					string line = lines[i];
					if (!string.IsNullOrWhiteSpace(line))
					{
						string[] fulldata = line.Split('=');
						string[] data = fulldata[1].Split(',');
						ushort str = ushort.Parse(data[0]);
						ushort vit = ushort.Parse(data[1]);
						ushort agi = ushort.Parse(data[2]);
						ushort spi = ushort.Parse(data[3]);
						if (!Core.Kernel.NinjaStats.TryAdd(byte.Parse(fulldata[0]),
						                                   new ushort[]
						                                   {
						                                   	str, vit, agi, spi
						                                   }))
						{
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine("Failed to load stats. [NINJA]");
							Console.ResetColor();
							return false;
						}
					}
				}
			}
			#endregion
			#region MonkStats
			{
				string file = "MonkStats";
				string[] lines = System.IO.File.ReadAllLines(Database.ServerDatabase.DatabaseLocation + "\\Stats\\" + file + ".dat");
				for (int i = 0; i < lines.Length; i++)
				{
					string line = lines[i];
					if (!string.IsNullOrWhiteSpace(line))
					{
						string[] fulldata = line.Split('=');
						string[] data = fulldata[1].Split(',');
						ushort str = ushort.Parse(data[0]);
						ushort vit = ushort.Parse(data[1]);
						ushort agi = ushort.Parse(data[2]);
						ushort spi = ushort.Parse(data[3]);
						if (!Core.Kernel.MonkStats.TryAdd(byte.Parse(fulldata[0]),
						                                  new ushort[]
						                                  {
						                                  	str, vit, agi, spi
						                                  }))
						{
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine("Failed to load stats. [MONK]");
							Console.ResetColor();
							return false;
						}
					}
				}
			}
			#endregion
			#region TaoistStats
			{
				string file = "TaoistStats";
				string[] lines = System.IO.File.ReadAllLines(Database.ServerDatabase.DatabaseLocation + "\\Stats\\" + file + ".dat");
				for (int i = 0; i < lines.Length; i++)
				{
					string line = lines[i];
					if (!string.IsNullOrWhiteSpace(line))
					{
						string[] fulldata = line.Split('=');
						string[] data = fulldata[1].Split(',');
						ushort str = ushort.Parse(data[0]);
						ushort vit = ushort.Parse(data[1]);
						ushort agi = ushort.Parse(data[2]);
						ushort spi = ushort.Parse(data[3]);
						if (!Core.Kernel.TaoistStats.TryAdd(byte.Parse(fulldata[0]),
						                                    new ushort[]
						                                    {
						                                    	str, vit, agi, spi
						                                    }))
						{
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine("Failed to load stats. [TAOIST]");
							Console.ResetColor();
							return false;
						}
					}
				}
			}
			#endregion
			
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\tLoaded Stats...");
			
			return true;
		}
	}
}
