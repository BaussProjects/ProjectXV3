//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Client -> Server
	/// Server -> Client
	/// </summary>
	public class ItemPacket : DataPacket
	{
		public ItemPacket()
			: base(92, Packets.PacketType.ItemPacket)
		{
			
		}
		public ItemPacket(DataPacket inPacket)
			: base(inPacket)
		{
			
		}
		
		/// <summary>
		/// Gets the item action.
		/// </summary>
		public Enums.ItemAction Action
		{
			get { return (Enums.ItemAction)ReadUInt32(12); }
			set { WriteUInt32((uint)value, 12); }
		}
		
		public uint UID
		{
			get { return ReadUInt32(4); }
			set { WriteUInt32(value, 4); }
		}
		
		public uint Data1
		{
			get { return ReadUInt32(8); }
			set { WriteUInt32(value, 8); }
		}
		
		public uint Data2
		{
			get { return ReadUInt32(20); }
			set { WriteUInt32(value, 20); }
		}
		
		/// <summary>
		/// Offset [8/0x08]
		/// </summary>
		public ushort Data1Low
		{
			get { return (ushort)Data1; }
			set { Data1 = (uint)((Data1High << 16) | value); }
		}

		/// <summary>
		/// Offset [10/0x0a]
		/// </summary>
		public ushort Data1High
		{
			get { return (ushort)(Data1 >> 16); }
			set { Data1 = (uint)((value << 16) | Data1Low); }
		}
		
		public void SetGears(Data.Equipments equips)
		{
			if (equips.Contains(Enums.ItemLocation.Head))
				WriteUInt32(equips[Enums.ItemLocation.Head].UID, 32);
			if (equips.Contains(Enums.ItemLocation.Necklace))
				WriteUInt32(equips[Enums.ItemLocation.Necklace].UID, 36);
			if (equips.Contains(Enums.ItemLocation.Armor))
				WriteUInt32(equips[Enums.ItemLocation.Armor].UID, 40);
			
			if (equips.MaskedRightHand != null)
				WriteUInt32(equips.MaskedRightHand.UID, 44);
			else if (equips.Contains(Enums.ItemLocation.WeaponR))
				WriteUInt32(equips[Enums.ItemLocation.WeaponR].UID, 44);
			
			if (equips.MaskedLeftHand != null)
				WriteUInt32(equips.MaskedLeftHand.UID, 48);
			else if (equips.Contains(Enums.ItemLocation.WeaponL))
				WriteUInt32(equips[Enums.ItemLocation.WeaponL].UID, 48);
			
			if (equips.Contains(Enums.ItemLocation.Ring))
				WriteUInt32(equips[Enums.ItemLocation.Ring].UID, 52);
			if (equips.Contains(Enums.ItemLocation.Bottle))
				WriteUInt32(equips[Enums.ItemLocation.Bottle].UID, 56);
			if (equips.Contains(Enums.ItemLocation.Boots))
				WriteUInt32(equips[Enums.ItemLocation.Boots].UID, 60);
			
			if (equips.MaskedGarment != null)
				WriteUInt32(equips.MaskedGarment.UID, 64);
			else if (equips.Contains(Enums.ItemLocation.Garment))
				WriteUInt32(equips[Enums.ItemLocation.Garment].UID, 64);
			
			//	if (equips.Contains(Enums.ItemLocation.Fan))
			//		WriteUInt32(equips[Enums.ItemLocation.Fan].UID, 68);
			//	if (equips.Contains(Enums.ItemLocation.Tower))
			//WriteUInt32(equips[Enums.ItemLocation.Tower].UID, 72);
			if (equips.Contains(Enums.ItemLocation.SteedArmor))
				WriteUInt32(equips[Enums.ItemLocation.SteedArmor].UID, 76);
			else if (equips.Contains(Enums.ItemLocation.Steed))
				WriteUInt32(equips[Enums.ItemLocation.Steed].UID, 76);
			//if (equips.Contains(Enums.ItemLocation.Head))
			//	WriteUInt32(equips[Enums.ItemLocation.Head].UID, 80);
		}
		// GEARS
		
		/*            packet.Id = *((uint*)(ptr + 4));
            packet.Data1 = *((uint*)(ptr + 8));
            packet.Action = *((ItemAction*)(ptr + 12));
            packet.Timestamp = *((SystemTime*)(ptr + 16));
            packet.Data2 = *((uint*)(ptr + 20));
            packet.Gear1 = *((uint*)(ptr + 32));
            packet.Gear2 = *((uint*)(ptr + 36));
            packet.Gear3 = *((uint*)(ptr + 40));
            packet.Gear4 = *((uint*)(ptr + 44));
            packet.Gear5 = *((uint*)(ptr + 48));
            packet.Gear6 = *((uint*)(ptr + 52));
            packet.Gear7 = *((uint*)(ptr + 56));
            packet.Gear8 = *((uint*)(ptr + 60));
            packet.Gear9 = *((uint*)(ptr + 64));
            packet.Gear16 = *((uint*)(ptr + 68));
            packet.Gear15 = *((uint*)(ptr + 72));
            packet.Gear17 = *((uint*)(ptr + 76));
            packet.Gear18 = *((uint*)(ptr + 80)); */
		
		/// <summary>
		/// Handling the ItemPacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="packet">The packet.</param>
		public static void Handle(Entities.GameClient client, DataPacket packet)
		{
			using (var item = new ItemPacket(packet))
			{
				if (item.Action != Enums.ItemAction.Ping)
				{
					if (client.Trade.Trading)
						return;
				}
				switch (item.Action)
				{
						#region Ping : 27
					case Enums.ItemAction.Ping:
						Item.Ping.Handle(client, item);
						break;
						#endregion
						#region Drop : 37
					case Enums.ItemAction.Drop:
						Item.Drop.Handle(client, item);
						break;
						#endregion
						#region Buy : 1
					case Enums.ItemAction.Buy:
						Item.Buy.Handle(client, item);
						break;
						#endregion
						#region Sell : 2
					case Enums.ItemAction.Sell:
						Item.Sell.Handle(client, item);
						break;
						#endregion
						#region Use
					case Enums.ItemAction.Use:
						Item.Use.Handle(client, item);
						break;
						#endregion
						#region Unequip : ?
					case Enums.ItemAction.Unequip:
						Item.Unequip.Handle(client, item);
						break;
						#endregion
						#region Improve : ?
					case Enums.ItemAction.Improve:
						Item.Improve.Handle(client, item);
						break;
						#endregion
						#region Uplev : ?
					case Enums.ItemAction.Uplev:
						Item.Uplev.Handle(client, item);
						break;
						#endregion
						#region ActivateAccessory : ?
					case Enums.ItemAction.ActivateAccessory:
						Item.ActivateAccessory.Handle(client, item);
						break;
						#endregion
						#region QueryMoneySaved : ?
					case Enums.ItemAction.QueryMoneySaved:
						Item.QueryMoneySaved.Handle(client, item);
						break;
						#endregion
						#region SaveMoney : ?
					case Enums.ItemAction.SaveMoney:
						Item.SaveMoney.Handle(client, item);
						break;
						#endregion
						#region DrawMoney : ?
					case Enums.ItemAction.DrawMoney:
						Item.DrawMoney.Handle(client, item);
						break;
						#endregion
						#region Bless : ?
					case Enums.ItemAction.Bless:
						Item.Bless.Handle(client, item);
						break;
						#endregion
						#region BoothAdd : ?
					case Enums.ItemAction.BoothAdd:
						Item.BoothAdd.Handle(client, item);
						break;
						#endregion
						#region BoothAddCP : ?
					case Enums.ItemAction.BoothAddCP:
						Item.BoothAddCP.Handle(client, item);
						break;
						#endregion
						#region BoothBuy : ?
					case Enums.ItemAction.BoothBuy:
						Item.BoothBuy.Handle(client, item);
						break;
						#endregion
						#region BoothDelete : ?
					case Enums.ItemAction.BoothDelete:
						Item.BoothDelete.Handle(client, item);
						break;
						#endregion
						#region BoothDelete : ?
					case Enums.ItemAction.BoothQuery:
						Item.BoothQuery.Handle(client, item);
						break;
						#endregion
						
						#region default
					default:
						Console.WriteLine("Unhandled item packet: {0} User: {1}", item.Action, client.Name);
						break;
						#endregion
				}
			}
		}
	}
}
