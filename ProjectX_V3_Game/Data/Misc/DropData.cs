//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;
using ProjectX_V3_Lib.Extensions;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// Data for drops.
	/// </summary>
	[Serializable()]
	public class DropData
	{
		/// <summary>
		/// The minimum amount of gold to drop.
		/// </summary>
		public uint MinGoldDrop = 0;
		
		/// <summary>
		/// The maximum amount of gold to drop.
		/// </summary>
		public uint MaxGoldDrop = 0;
		
		/// <summary>
		/// The chance of dropping CPs.
		/// </summary>
		public byte CPsDropChance = 0;
		
		/// <summary>
		/// The minimum amount of CPs to drop.
		/// </summary>
		public uint MinCPsDrop = 0;
		
		/// <summary>
		/// The maximum amount of CPs to drop.
		/// </summary>
		public uint MaxCPsDrop = 0;
		
		/// <summary>
		/// The items to drop.
		/// </summary>
		public ConcurrentBag<uint> ItemDrops = new ConcurrentBag<uint>();
		
		public byte QualityChance = 0;
		
		public byte PlusChance = 0;
		
		public byte MinPlus = 0;
		
		public byte MaxPlus = 0;
		
		public byte NormalGemChance = 0;
		
		public byte RefinedGemChance = 0;
		
		public byte SuperGemChance = 0;
		
		public byte DragonballChance = 0;
		
		public byte MeteorChance = 0;
		
		public byte FirstSocketChance = 0;
		
		public byte SecondSocketChance = 0;
		
		public byte BlessChance = 0;
		
		/// <summary>
		/// Drops the items and gold associated with the drop data.
		/// </summary>
		/// <param name="client">The client to receive cps if any. Put null if not a client drop.</param>
		/// <param name="MidLocation">The location to star the drop from.</param>
		public void Drop(Entities.GameClient client, Maps.MapPoint MidLocation)
		{
			if (Calculations.BasicCalculations.ChanceSuccess(CPsDropChance))
			{
				// for cps bags just add them to the items
				// or copy the item drops already existing above and below
				if (client != null)
					client.CPs += (uint)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)MinCPsDrop, (int)MaxCPsDrop);
			}
			else
			{
				Maps.MapPoint Location = MidLocation.Map.CreateAvailableLocation<Data.GroundItem>(MidLocation.X, MidLocation.Y, 5);
				if (Location != null)
				{
					// perhaps some vip thing to loot gold in bag, rather than ground ??
					
					uint DropMoney = (uint)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)MinGoldDrop, (int)MaxGoldDrop);
					if (DropMoney > 0)
					{
						uint ItemID = 1090000;
						if (DropMoney > 10000)
							ItemID = 1091020;
						else if (DropMoney > 5000)
							ItemID = 1091010;
						else if (DropMoney > 1000)
							ItemID = 1091000;
						else if (DropMoney > 100)
							ItemID = 1090020;
						else if (DropMoney > 50)
							ItemID = 1090010;
						Data.ItemInfo item;
						if (Core.Kernel.ItemInfos.TrySelect(ItemID, out item))
						{
							item = item.Copy(); // necessary to not edit original data
							
							Data.GroundItem ground = new Data.GroundItem(item);
							if (client != null)
							{
								ground.PlayerDrop = false;
								ground.OwnerUID = client.EntityUID;
								ground.DropTime = DateTime.Now;
							}
							ground.DropType = Enums.DropItemType.Gold;
							ground.Money = DropMoney;
							ground.X = Location.X;
							ground.Y = Location.Y;
							Location.Map.EnterMap(ground);
							ground.TaskID = ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(
								() => {
									Location.Map.LeaveMap(ground);
									ground.Screen.ClearScreen();
								},
								Core.TimeIntervals.DroppedItemRemove);
							ground.Screen.UpdateScreen(null);
						}
					}
				}
			}

			// could use else if, however we do want to drop gold/cps and items at the same time
			if (Calculations.BasicCalculations.ChanceSuccess(33))
			{
				if (ItemDrops.Count > 0)
				{
					uint[] itemids = ItemDrops.ToArray();
					uint ItemID = itemids[ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(0, itemids.Length - 1)];

					Data.ItemInfo item;
					// check for chances between the different types of items ex. armor, bow etc. item.IsArmor() etc.
					if (Core.Kernel.ItemInfos.TrySelect(ItemID, out item))
					{
						Maps.MapPoint Location = MidLocation.Map.CreateAvailableLocation<Data.GroundItem>(MidLocation.X, MidLocation.Y, 5);
						if (Location != null)
						{
							item = item.Copy(); // necessary
							if (item.IsArmor() || item.IsBow() || item.IsRing() ||
							    item.IsOneHand() || item.IsTwoHand() ||item.IsBoots() ||
							    item.IsHeadgear() || item.IsShield() || item.IsNecklace())
							{
								if (Calculations.BasicCalculations.ChanceSuccess(PlusChance))
								{
									byte chance = (byte)(MaxPlus * (MaxPlus + 1));
									byte offchance = (byte)(chance / MaxPlus);
									item.Plus = MinPlus;
									
									while (Calculations.BasicCalculations.ChanceSuccess(chance))
									{
										item.Plus += 1;
										chance -= offchance;
									}
								}
								if (Calculations.BasicCalculations.ChanceSuccess(QualityChance))
								{
									item = Core.Kernel.ItemInfos[item.ItemID + 3].Copy();
									if (Calculations.BasicCalculations.ChanceSuccess(20))
									{
										item = Core.Kernel.ItemInfos[item.ItemID + 1].Copy();
										if (Calculations.BasicCalculations.ChanceSuccess(15))
										{
											item = Core.Kernel.ItemInfos[item.ItemID + 1].Copy();
											if (Calculations.BasicCalculations.ChanceSuccess(10))
											{
												item = Core.Kernel.ItemInfos[item.ItemID + 1].Copy();
											}
										}
									}
								}
								
								if (Calculations.BasicCalculations.ChanceSuccess(FirstSocketChance) ||
								    item.IsOneHand() && Calculations.BasicCalculations.ChanceSuccess(FirstSocketChance * 2) ||
								    item.IsTwoHand() && Calculations.BasicCalculations.ChanceSuccess(FirstSocketChance * 2))
								{
									item.Gem1 = Enums.SocketGem.EmptySocket;
									if (Calculations.BasicCalculations.ChanceSuccess(SecondSocketChance))
										item.Gem2 = Enums.SocketGem.EmptySocket;
								}
								
								if (Calculations.BasicCalculations.ChanceSuccess(BlessChance))
									item.Bless = 1;
							}
							
							Data.GroundItem ground = new Data.GroundItem(item);
							if (client != null)
							{
								ground.PlayerDrop = false;
								ground.OwnerUID = client.EntityUID;
								ground.DropTime = DateTime.Now;
							}
							ground.DropType = Enums.DropItemType.Item;
							ground.X = Location.X;
							ground.Y = Location.Y;
							Location.Map.EnterMap(ground);
							ground.TaskID = ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(
								() => {
									Location.Map.LeaveMap(ground);
									ground.Screen.ClearScreen();
								},
								Core.TimeIntervals.DroppedItemRemove);
							ground.Screen.UpdateScreen(null);
						}
					}
				}
			}
			else if (Calculations.BasicCalculations.ChanceSuccess(MeteorChance))
			{
				Data.ItemInfo item;
				
				if (Core.Kernel.ItemInfos.TrySelect(1088001, out item))
				{
					Maps.MapPoint Location = MidLocation.Map.CreateAvailableLocation<Data.GroundItem>(MidLocation.X, MidLocation.Y, 5);
					if (Location != null)
					{
						item = item.Copy(); // necessary
						
						Data.GroundItem ground = new Data.GroundItem(item);
						if (client != null)
						{
							ground.PlayerDrop = false;
							ground.OwnerUID = client.EntityUID;
							ground.DropTime = DateTime.Now;
						}
						ground.DropType = Enums.DropItemType.Item;
						ground.X = Location.X;
						ground.Y = Location.Y;
						Location.Map.EnterMap(ground);
						ground.TaskID = ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(
							() => {
								Location.Map.LeaveMap(ground);
								ground.Screen.ClearScreen();
							},
							Core.TimeIntervals.DroppedItemRemove);
						ground.Screen.UpdateScreen(null);
					}
				}
			}
			else if (Calculations.BasicCalculations.ChanceBigSuccess(10, DragonballChance))
			{
				Data.ItemInfo item;
				
				if (Core.Kernel.ItemInfos.TrySelect(1088000, out item))
				{
					Maps.MapPoint Location = MidLocation.Map.CreateAvailableLocation<Data.GroundItem>(MidLocation.X, MidLocation.Y, 5);
					if (Location != null)
					{
						item = item.Copy(); // necessary
						
						Data.GroundItem ground = new Data.GroundItem(item);
						if (client != null)
						{
							ground.PlayerDrop = false;
							ground.OwnerUID = client.EntityUID;
							ground.DropTime = DateTime.Now;
						}
						ground.DropType = Enums.DropItemType.Item;
						ground.X = Location.X;
						ground.Y = Location.Y;
						Location.Map.EnterMap(ground);
						ground.TaskID = ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(
							() => {
								Location.Map.LeaveMap(ground);
								ground.Screen.ClearScreen();
							},
							Core.TimeIntervals.DroppedItemRemove);
						ground.Screen.UpdateScreen(null);
					}
				}
			}
			else if (Calculations.BasicCalculations.ChanceSuccess(NormalGemChance))
			{
				Data.ItemInfo item;
				
				Enums.SocketGem gem = (Enums.SocketGem)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.NextEnum(typeof(Enums.SocketGem));
				switch (gem)
				{
					case Enums.SocketGem.NormalPhoenixGem:
					case Enums.SocketGem.RefinedPhoenixGem:
					case Enums.SocketGem.SuperPhoenixGem:
						gem = Enums.SocketGem.NormalPhoenixGem;
						break;
						
					case Enums.SocketGem.NormalDragonGem:
					case Enums.SocketGem.RefinedDragonGem:
					case Enums.SocketGem.SuperDragonGem:
						gem = Enums.SocketGem.NormalDragonGem;
						break;
						
					case Enums.SocketGem.NormalRainbowGem:
					case Enums.SocketGem.RefinedRainbowGem:
					case Enums.SocketGem.SuperRainbowGem:
						gem = Enums.SocketGem.NormalRainbowGem;
						break;
						
					case Enums.SocketGem.NormalVioletGem:
					case Enums.SocketGem.RefinedVioletGem:
					case Enums.SocketGem.SuperVioletGem:
						gem = Enums.SocketGem.NormalVioletGem;
						break;
						
					case Enums.SocketGem.NormalFuryGem:
					case Enums.SocketGem.RefinedFuryGem:
					case Enums.SocketGem.SuperFuryGem:
						gem = Enums.SocketGem.NormalFuryGem;
						break;
						
					case Enums.SocketGem.NormalKylinGem:
					case Enums.SocketGem.RefinedKylinGem:
					case Enums.SocketGem.SuperKylinGem:
						gem = Enums.SocketGem.NormalKylinGem;
						break;
						
					case Enums.SocketGem.NormalMoonGem:
					case Enums.SocketGem.RefinedMoonGem:
					case Enums.SocketGem.SuperMoonGem:
						gem = Enums.SocketGem.NormalMoonGem;
						break;
						
					default:
						return;
				}
				
				uint gemid = (uint)(((uint)gem) + 700000);
				
				if (Calculations.BasicCalculations.ChanceSuccess(RefinedGemChance))
				{
					gemid += 1;
					if (Calculations.BasicCalculations.ChanceSuccess(SuperGemChance))
						gemid += 1;
				}
				
				if (Core.Kernel.ItemInfos.TrySelect(gemid, out item))
				{
					Maps.MapPoint Location = MidLocation.Map.CreateAvailableLocation<Data.GroundItem>(MidLocation.X, MidLocation.Y, 5);
					if (Location != null)
					{
						item = item.Copy(); // necessary
						
						Data.GroundItem ground = new Data.GroundItem(item);
						if (client != null)
						{
							ground.PlayerDrop = false;
							ground.OwnerUID = client.EntityUID;
							ground.DropTime = DateTime.Now;
						}
						ground.DropType = Enums.DropItemType.Item;
						ground.X = Location.X;
						ground.Y = Location.Y;
						Location.Map.EnterMap(ground);
						ground.TaskID = ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(
							() => {
								Location.Map.LeaveMap(ground);
								ground.Screen.ClearScreen();
							},
							Core.TimeIntervals.DroppedItemRemove);
						ground.Screen.UpdateScreen(null);
					}
				}
			}
		}
		
		public DropData Copy()
		{
			return this.DeepClone();
		}
	}
}
