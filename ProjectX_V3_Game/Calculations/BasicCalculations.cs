//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Calculations
{
	/// <summary>
	/// Basic calculations used.
	/// </summary>
	public class BasicCalculations
	{
		/// <summary>
		/// Gets the ghost transformation ID.
		/// </summary>
		/// <param name="model">The model to base it off.</param>
		/// <returns>Returns the ghost model.</returns>
		public static ushort GetGhostTransform(ushort model)
		{
			ushort transform = 98;
			if (model % 10 < 3)
				transform = 99;
			return transform;
		}
		
		/// <summary>
		/// Gets a boolean that is defining whether the percentage chance was a success.
		/// </summary>
		/// <param name="percent">The percentage chance.</param>
		/// <returns>Returns true if success.</returns>
		public static bool ChanceSuccess(int percent)
		{
			if (percent == 0)
				return false;
			
			return (ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(0, 100) < percent);
		}
		
		/// <summary>
		/// Gets a boolean that is defining whether the percentage chance was a success.
		/// Both percentages must succeed.
		/// </summary>
		/// <param name="percent">The percentage chance.</param>
		/// <returns>Returns true if success.</returns>
		public static bool ChanceBigSuccess(int percent_a, int percent_b)
		{
			if (percent_a == 0)
				return false;
			if (!ChanceSuccess(percent_a))
				return false;
			
			return (ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(0, 100) < percent_b);
		}
		
		public static byte NeededDragonballs(Entities.GameClient client, byte pos)
		{
			if (!client.Equipments.Contains((Enums.ItemLocation)pos))
				return 0;
			return NeededDragonballs(client.Equipments.Equips[(Enums.ItemLocation)pos].Quality);
		}
		
		public static byte NeededDragonballs(byte Quality)
		{
			if (Quality <= 5)
				return 1;
			else
			{
				switch (Quality)
				{
					case 6:
						return 2;
					case 7:
						return 4;
					case 8:
						return 6;
					default:
					case 9:
						return 0;
				}
			}
		}
		public static byte NeededMeteors(Entities.GameClient client, byte pos)
		{
			if (!client.Equipments.Contains((Enums.ItemLocation)pos))
				return 0;
			return NeededMeteors(client.Equipments.Equips[(Enums.ItemLocation)pos].RequiredLevel);
		}
		public static byte NeededMeteors(byte ItemLevel)
		{
			if (ItemLevel >= 100)
				return 40;
			else if (ItemLevel >= 75)
				return 30;
			else if (ItemLevel >= 60)
				return 25;
			else if (ItemLevel >= 50)
				return 15;
			else if (ItemLevel >= 40)
				return 5;
			else if (ItemLevel >= 30)
				return 2;
			else
				return 1;
		}
		
		public static int PlusRequirements(byte plus)
		{
			switch (plus)
			{
				case 0:
					{
						return 20;
					}
				case 1:
					{
						return 20;
					}
				case 2:
					{
						return 80;
					}
				case 3:
					{
						return 240;
					}
				case 4:
					{
						return 720;
					}
				case 5:
					{
						return 2160;
					}
				case 6:
					{
						return 6480;
					}
				case 7:
					{
						return 19440;
					}
				case 8:
					{
						return 58320;
					}
				case 9:
					{
						return 116640;
					}
				case 10:
					{
						return 233280;
					}
				case 11:
					{
						return 466560;
					}
				default:
					{
						return -1;
					}
			}
		}
		
		public static uint CompositionPoints(byte plus)
		{
			uint pts = 0;
			switch (plus)
			{
				case 1:
					pts = 10;
					break;
				case 2:
					pts = 40;
					break;
				case 3:
					pts = 120;
					break;
				case 4:
					pts = 360;
					break;
				case 5:
					pts = 1080;
					break;
				case 6:
					pts = 3240;
					break;
				case 7:
					pts = 9720;
					break;
				case 8:
					pts = 29160;
					break;
				case 9:
					pts = 87480;
					break;
				case 10:
					pts = 262440;
					break;
				case 11:
					pts = 787320;
					break;
			}
			return pts;
		}
	}
}
