//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;
using System.Reflection;
using ProjectX_V3_Lib.Extensions;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// The actual item info.
	/// </summary>
	[Serializable()]
	public class ItemInfo
	{
		private static ConcurrentDictionary<uint, uint> useduids;
		static ItemInfo()
		{
			useduids = new ConcurrentDictionary<uint, uint>();
		}
		
		public ItemInfo()
		{
			// not currently in use...
		}
		
		public void Drop(Maps.Map map, ushort MidLocationX, ushort MidLocationY)
		{
			Maps.MapPoint Location = map.CreateAvailableLocation<Data.GroundItem>(MidLocationX, MidLocationY,  10);
			if (Location != null)
			{
				Data.GroundItem ground = new Data.GroundItem(this);
				
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
		
		// itemtype.dat data:
		public uint UID = 10000000;
		public uint OwnerUID = 0;
		public uint ItemID;
		public ushort BaseID
		{
			get
			{
				return (ushort)(ItemID / 1000);
			}
		}
		public string Name;
		public Enums.Class Profession;
		public byte RequiredProf;
		public byte RequiredLevel;
		public Enums.Sex Sex;
		public ushort RequiredStrength;
		public ushort RequiredAgility;
		public ushort RequiredVitality;
		public ushort RequiredSpirit;
		public byte Monopoly;
		public ushort Weight;
		public uint Price;
		public uint ActionID;
		public ushort MaxAttack;
		public ushort MinAttack;
		public ushort Defense;
		public ushort Dexterity;
		public ushort Dodge;
		public ushort HP;
		public ushort MP;
		//public ushort StAmount;
		public ushort AmountLimit;
		public byte Ident;
		public byte StGem1;
		public byte StGem2;
		public ushort Magic1;
		public byte Magic2;
		public byte Magic3;
		public int Data;
		public ushort MagicAttack;
		public ushort MagicDefense;
		public ushort AttackRange;
		public ushort AttackSpeed;
		public byte FrayMode;
		public byte RepairMode;
		public byte TypeMask;
		public uint CPPrice;
		public uint Unknown1;
		public uint Unknown2;
		public uint CriticalStrike;
		public uint SkillCriticalStrike;
		public uint Immunity;
		public uint Penetration;
		public uint Block;
		public uint BreakThrough;
		public uint CounterAction;
		public uint StackLimit;
		public uint ResistMetal;
		public uint ResistWood;
		public uint ResistWater;
		public uint ResistFire;
		public uint ResistEarth;
		public string TypeName;
		public string Description;
		public uint NameColor;
		public uint DragonSoulPhase;
		public uint DragonSoulRequirements;
		
		public int BattleMonsterID = 0;
		
		// and item data:
		public byte Plus = 0;
		public byte Bless = 0;
		public byte Enchant = 0;
		public Enums.SocketGem Gem1 = 0;
		public Enums.SocketGem Gem2 = 0;
		
		// other data
		public Enums.ItemLocation Location = Enums.ItemLocation.Inventory;
		public uint SocketAndRGB = 0;
		private short currentDura = 100;
		public short CurrentDura
		{
			get { return currentDura; }
			set
			{
				value = (short)(value < 0 ? 0 : value);
				value = (short)(value > MaxDura ? MaxDura : value);
				currentDura = value;
			}
		}
		public short MaxDura = 100;
		public ushort RebornEffect = 0;
		public bool Free = false;
		public uint GreenText = 0;
		public uint INS = 0;
		public bool Suspicious = false;
		public bool Locked = false;
		public uint Composition = 0;
		public uint LockedTime = 0;
		public Enums.ItemColor Color = Enums.ItemColor.Orange;
		
		public Enums.ItemType ItemType;
		
		/// <summary>
		/// Gets the item addition.
		/// </summary>
		public ItemAddition Addition
		{
			//get { return (Plus > 0 ? Core.Kernel.ItemAdditions[ItemID][Plus] : null); }
			get
			{
				if (Plus == 0)
					return null;
				uint type = ItemID;
				if (type / 100000 == 4 || type / 100000 == 5)
				{
					type = (type / 100000) * 111000 + (type / 10 * 10 % 1000);
				}
				else
				{
					type = type / 10 * 10;
				}
				
				if (!Core.Kernel.ItemAdditions.ContainsKey(type))
					return null;
				if (!Core.Kernel.ItemAdditions[type].ContainsKey(Plus))
					return null;
				
				return Core.Kernel.ItemAdditions[type][Plus];
			}
		}
		
		/// <summary>
		/// Copies the current item (+ stats) into a new item with a new uid.
		/// </summary>
		/// <returns>Returns the new item.</returns>
		public ItemInfo Copy()
		{
			ItemInfo item = this.DeepClone(); //(ItemInfo)this.MemberwiseClone();
			item.UID = Core.UIDGenerators.GetItemUID();
			//do
			//{
			//	item.UID = (uint)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(1000000, 99999999);
			//}
			//while (useduids.ContainsKey(item.UID) && !useduids.TryAdd(item.UID, item.UID));
			
			return item;
		}
		
		/// <summary>
		/// Sends the item data to a client.
		/// </summary>
		/// <param name="client">The client to send it to.</param>
		/// <param name="data">The packet data.</param>
		public void SendPacket(Entities.GameClient client, ushort data)
		{
			if (!client.LoggedIn)
				return;
			
			using (var itempacket = new Packets.ItemInfoPacket())
			{
				itempacket.UID = UID;
				itempacket.ItemID = ItemID;
				itempacket.CurrentDura = (ushort)CurrentDura;
				itempacket.MaxDura = (ushort)MaxDura;
				itempacket.Data = data;
				itempacket.Location = Location;
				itempacket.SocketAndRGB = SocketAndRGB;
				itempacket.Gem1 = Gem1;
				itempacket.Gem2 = Gem2;
				itempacket.Unknown2 = 0;
				itempacket.RebornEffect = RebornEffect;
				itempacket.Plus = Plus;
				itempacket.Bless = Bless;
				itempacket.Free = Free;
				itempacket.Enchant = Enchant;
				itempacket.GreenText = GreenText;
				itempacket.Suspicious = Suspicious;
				itempacket.Locked = Locked;
				itempacket.Color = (byte)Color;
				itempacket.Composition = Composition;
				itempacket.INS = INS;
				itempacket.LockedTime = LockedTime;
				if (IsArrow())
					itempacket.Amount = AmountLimit;

				client.Send(itempacket);
			}
		}
		
		public ProjectX_V3_Game.Packets.ViewItemPacket CreateViewPacket(uint owner, ushort viewtype)
		{
			var view = new ProjectX_V3_Game.Packets.ViewItemPacket();
			view.ViewType = viewtype;
			view.TargetUID = owner;
			view.ItemID = ItemID;
			view.ItemUID = UID;
			view.CurrentDura = (ushort)CurrentDura;
			view.MaxDura = (ushort)MaxDura;
			view.Location = Location;
			view.Gem1 = Gem1;
			view.Gem2 = Gem2;
			view.Plus = Plus;
			view.Bless = Bless;
			view.Free = Free;
			view.Enchant = Enchant;
			view.Suspicious = Suspicious;
			view.Color = (byte)Color;
			view.Composition = Composition;
			return view;
		}
		public void SendViewPacket(uint owner, Entities.GameClient client, ushort viewtype = 4)
		{
			using (var view = CreateViewPacket(owner, viewtype))
			{
				client.Send(view);
			}
		}
		
		public bool IsValidOffItem()
		{
			if (ItemID >= 70019001 && ItemID <= 70129001)
				return false;
			
			return !Free && !Suspicious && !Locked;
		}
		/// <summary>
		/// Checks whether the item is a shield.
		/// </summary>
		/// <returns>Returns true if the item is a shield.</returns>
		public bool IsShield()
		{
			return (ItemID >= 900000 && ItemID <= 900999);
		}
		
		/// <summary>
		/// Checks whether the item is an armor.
		/// </summary>
		/// <returns>Returns true if the item is an armor.</returns>
		public bool IsArmor()
		{
			return (ItemID >= 130000 && ItemID <= 136999);
		}
		
		/// <summary>
		/// Checks whether the item is a headgear.
		/// </summary>
		/// <returns>Returns true if the item is a headgear.</returns>
		public bool IsHeadgear()
		{
			return ((ItemID >= 111000 && ItemID <= 118999) || (ItemID >= 123000 && ItemID <= 123999) || (ItemID >= 141999 && ItemID <= 143999));
		}
		/// <summary>
		/// Checks whether the item is an one hander.
		/// </summary>
		/// <returns>Returns true if the item is an one hander.</returns>
		public bool IsOneHand()
		{
			return (ItemID >= 410000 && ItemID <= 499999 || ItemID >= 601000 && ItemID <= 601999 || ItemID >= 610000 && ItemID <= 610999);
		}
		
		public bool IsBacksword()
		{
			return (ItemID >= 421000 && ItemID <= 421999);
		}
		
		public bool IsSwordOrBlade()
		{
			return (ItemID >= 410000 && ItemID <= 421999);
		}
		
		/// <summary>
		/// Checks whether the item is a two hander.
		/// </summary>
		/// <returns>Returns true if the item is a two hander.</returns>
		public bool IsTwoHand()
		{
			return (ItemID > 510000 && ItemID < 599999);
		}
		/// <summary>
		/// Checks whether the item is an arrow.
		/// </summary>
		/// <returns>Returns true if the item is an arrow.</returns>
		public bool IsArrow()
		{
			return (ItemID >= 1050000 && ItemID <= 1051000);
		}
		
		/// <summary>
		/// Checks whether the item is a bow.
		/// </summary>
		/// <returns>Returns true if the item is a bow.</returns>
		public bool IsBow()
		{
			return (ItemID >= 500000 && ItemID < 510000);
		}
		/// <summary>
		/// Checks whether the item is a necklace.
		/// </summary>
		/// <returns>Returns true if the item is a necklace.</returns>
		public bool IsNecklace()
		{
			return (ItemID >= 120000 && ItemID <= 121999);
		}
		
		/// <summary>
		/// Checks whether the item is a ring.
		/// </summary>
		/// <returns>Returns true if the item is a ring.</returns>
		public bool IsRing()
		{
			return (ItemID >= 150000 && ItemID <= 159999);
		}
		
		/// <summary>
		/// Checks whether the item is boots.
		/// </summary>
		/// <returns>Returns true if the item is boots.</returns>
		public bool IsBoots()
		{
			return (ItemID >= 160000 && ItemID <= 160999);
		}
		
		/// <summary>
		/// Checks whether the item is a garment.
		/// </summary>
		/// <returns>Returns true if the item is a garment.</returns>
		public bool IsGarment()
		{
			if (ItemID == 134155 || ItemID == 131155 || ItemID == 133155 || ItemID == 130155)
				return true;
			return (ItemID >= 181000 && ItemID <= 194999);
		}
		
		/// <summary>
		/// Checks whether the item is a fan.
		/// </summary>
		/// <returns>Returns true if the item is a fan.</returns>
		public bool IsFan()
		{
			return (ItemID >= 201000 && ItemID <= 201999);
		}
		
		/// <summary>
		/// Checks whether the item is a tower.
		/// </summary>
		/// <returns>Returns true if the item is a tower.</returns>
		public bool IsTower()
		{
			return (ItemID >= 202000 && ItemID <= 202999);
		}
		
		/// <summary>
		/// Checks whether the item is a steed.
		/// </summary>
		/// <returns>Returns true if the item is a steed.</returns>
		public bool IsSteed()
		{
			return (ItemID == 300000);
		}
		
		/// <summary>
		/// Checks whether the item is a bottle.
		/// </summary>
		/// <returns>Returns true if the item is a bottle.</returns>
		public bool IsBottle()
		{
			return (ItemID >= 2100025 && ItemID <= 2100095);
		}
		
		public bool IsMountArmor()
		{
			return (ItemID >= 200000 && ItemID <= 200420);
		}
		
		/// <summary>
		/// Checks whether the item is a misc item.
		/// </summary>
		/// <returns>Returns true if the item is a misc item.</returns>
		public bool IsMisc()
		{
			return (!IsHeadgear() && !IsArmor() &&
			        !IsShield() && !IsOneHand() &&
			        !IsTwoHand() && !IsNecklace() &&
			        !IsRing() && !IsArrow() &&
			        !IsBow() && !IsBoots() &&
			        !IsGarment() && !IsFan() &&
			        !IsTower() && !IsBottle() &&
			        !IsSteed() && !IsMountArmor());
		}
		
		public byte Quality
		{
			get { return (byte)(ItemID % 10); }
		}
		
		public void SetStats(ItemInfo fromItem)
		{
			Plus = fromItem.Plus;
			Bless = fromItem.Bless;
			Enchant = fromItem.Enchant;
			Gem1 = fromItem.Gem1;
			Gem2 = fromItem.Gem2;
			
			SocketAndRGB = fromItem.SocketAndRGB;
			CurrentDura = fromItem.CurrentDura;
			MaxDura = fromItem.MaxDura;
			RebornEffect = fromItem.RebornEffect;
			Free = fromItem.Free;
			GreenText = fromItem.GreenText;
			INS = fromItem.INS;
			Suspicious = fromItem.Suspicious;
			Locked = fromItem.Locked;
			Composition = fromItem.Composition;
			LockedTime = fromItem.LockedTime;
			Color = fromItem.Color;
			MaxDura = fromItem.MaxDura;
			CurrentDura = fromItem.CurrentDura;
		}
	}
}
