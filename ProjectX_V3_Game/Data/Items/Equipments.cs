//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// Description of Equipments.
	/// </summary>
	public class Equipments
	{
		/// <summary>
		/// The equipment items.
		/// </summary>
		private ConcurrentDictionary<Enums.ItemLocation, ItemInfo> EquipmentItems;
		
		/// <summary>
		/// Masked equipment garment.
		/// </summary>
		public ItemInfo MaskedGarment;
		
		/// <summary>
		/// Masked equipment right hand.
		/// </summary>
		public ItemInfo MaskedRightHand;
		
		/// <summary>
		/// Masked equipment left hand.
		/// </summary>
		public ItemInfo MaskedLeftHand;
		
		/// <summary>
		/// The owner of the equipments.
		/// </summary>
		public Entities.GameClient Owner;
		
		/// <summary>
		/// Creates a new instance of Equipments.
		/// </summary>
		/// <param name="owner">The owner of the equipments.</param>
		public Equipments(Entities.GameClient owner)
		{
			this.Owner = owner;
			EquipmentItems = new ConcurrentDictionary<ProjectX_V3_Game.Enums.ItemLocation, ItemInfo>();
		}
		
		/// <summary>
		/// Gets an item from the equipments based on an ItemLocation as index.
		/// </summary>
		public ItemInfo this[Enums.ItemLocation pos]
		{
			get
			{
				if (!Contains(pos))
					return null;
				if (pos == Enums.ItemLocation.Garment && MaskedGarment != null)
					return MaskedGarment;
				if (pos == Enums.ItemLocation.WeaponR && MaskedRightHand != null)
					return MaskedRightHand;
				if (pos == Enums.ItemLocation.WeaponL && MaskedLeftHand != null)
					return MaskedLeftHand;
				
				return EquipmentItems[pos];
			}
		}
		
		public ushort GetNumberOfPlus()
		{
			ushort Plus = 0;
			foreach (ItemInfo item in Equips.Values)
			{
				Plus += item.Plus;
			}
			return Plus;
		}
		
		/// <summary>
		/// The equipments.
		/// </summary>
		public ConcurrentDictionary<Enums.ItemLocation, ItemInfo> Equips
		{
			get { return EquipmentItems; }
		}
		
		/// <summary>
		/// Checks whether the a position has an item equipped or not.
		/// </summary>
		/// <param name="pos">The position.</param>
		/// <returns>Returns true if an item is equipped at the position.</returns>
		public bool Contains(Enums.ItemLocation pos)
		{
			return EquipmentItems.ContainsKey(pos);
		}
		
		/// <summary>
		/// Equips a new item.
		/// </summary>
		/// <param name="item">The item to equip.</param>
		/// <param name="pos">The position to equip the item at.</param>
		/// <param name="unequip">Boolean defining whether to unequip an existing item at the position.</param>
		/// <returns>Returns true if the item was equipped.</returns>
		public bool Equip(ItemInfo item, Enums.ItemLocation pos, bool unequip = true, bool force_equip = false)
		{
			if (!force_equip)
			{
				#region Sex check
				if (item.Sex == Enums.Sex.Female && Owner.Sex != Enums.Sex.Female)
				{
					using (var fmsg = Packets.Message.MessageCore.CreateSystem2(Owner.Name, Core.MessageConst.ITEM_FEMALE))
						Owner.Send(fmsg);
					
					return false;
				}
				#endregion

				#region Job Check
//			if (item.Profession != Enums.Class.Unknown && Core.Kernel.GetBaseClass(item.Profession) != Enums.Class.Unknown)
//			{
//				Enums.Class promoteclass = Core.Kernel.GetBaseClass(Owner.Class);
//				if (promoteclass != Core.Kernel.GetBaseClass(item.Profession))
//				{
//					using (var fmsg = Packets.Message.MessageCore.CreateSystem2(Owner.Name, string.Format(Core.MessageConst.ITEM_JOB_INVALID, Core.Kernel.GetBaseClass(item.Profession).ToString())))
//						Owner.Send(fmsg);
//
//					return false;
//				}
//				else if (!Core.Kernel.AboveFirstPromotion(Owner.Class))
//				{
//					using (var fmsg = Packets.Message.MessageCore.CreateSystem2(Owner.Name, string.Format(Core.MessageConst.ITEM_JOB_INVALID, Core.Kernel.GetBaseClass(item.Profession).ToString())))
//						Owner.Send(fmsg);
//
//					return false;
//				}
//			}
				#endregion
				
				#region Level Check
				if (Owner.Level < item.RequiredLevel)
				{
					using (var fmsg = Packets.Message.MessageCore.CreateSystem2(Owner.Name, Core.MessageConst.LEVEL_LOW))
						Owner.Send(fmsg);
					
					return false;
				}
				#endregion
				
				#region Stats Check
				if (Owner.Strength < item.RequiredStrength ||
				    Owner.Agility < item.RequiredAgility ||
				    Owner.Vitality < item.RequiredVitality ||
				    Owner.Spirit < item.RequiredSpirit)
				{
					using (var fmsg = Packets.Message.MessageCore.CreateSystem2(Owner.Name, Core.MessageConst.STATS_LOW))
						Owner.Send(fmsg);
					
					return false;
				}
				#endregion
				
				#region Prof Check
//			if (Owner.Level < item.RequiredProf)
//			{
//				using (var fmsg = Packets.Message.MessageCore.CreateSystem2(Owner.Name, Core.MessageConst.LEVEL_LOW))
//					Owner.Send(fmsg);
//
//				return false;
//			}
				#endregion
			}
			
			if (!force_equip)
			{
				switch (pos)
				{
						#region Head
					case Enums.ItemLocation.Head:
						{
							if (item.ItemType != Enums.ItemType.Head)
							{
								using (var fmsg = Packets.Message.MessageCore.CreateSystem2(Owner.Name, Core.MessageConst.INVALID_ITEM_TYPE))
									Owner.Send(fmsg);
								return false;
							}
							break;
						}
						#endregion
						#region Necklace
					case Enums.ItemLocation.Necklace:
						{
							if (item.ItemType != Enums.ItemType.Necklace)
							{
								using (var fmsg = Packets.Message.MessageCore.CreateSystem2(Owner.Name, Core.MessageConst.INVALID_ITEM_TYPE))
									Owner.Send(fmsg);
								return false;
							}
							break;
						}
						#endregion
						#region Ring
					case Enums.ItemLocation.Ring:
						{
							if (item.ItemType != Enums.ItemType.Ring)
							{
								using (var fmsg = Packets.Message.MessageCore.CreateSystem2(Owner.Name, Core.MessageConst.INVALID_ITEM_TYPE))
									Owner.Send(fmsg);
								return false;
							}
							break;
						}
						#endregion
						#region Armor
					case Enums.ItemLocation.Armor:
						{
							if (item.ItemType != Enums.ItemType.Armor)
							{
								using (var fmsg = Packets.Message.MessageCore.CreateSystem2(Owner.Name, Core.MessageConst.INVALID_ITEM_TYPE))
									Owner.Send(fmsg);
								return false;
							}
							break;
						}
						#endregion
						#region Boots
					case Enums.ItemLocation.Boots:
						{
							if (item.ItemType != Enums.ItemType.Boots)
							{
								using (var fmsg = Packets.Message.MessageCore.CreateSystem2(Owner.Name, Core.MessageConst.INVALID_ITEM_TYPE))
									Owner.Send(fmsg);
								return false;
							}
							break;
						}
						#endregion
						#region Bottle
					case Enums.ItemLocation.Bottle:
						{
							if (item.ItemType != Enums.ItemType.Bottle)
							{
								using (var fmsg = Packets.Message.MessageCore.CreateSystem2(Owner.Name, Core.MessageConst.INVALID_ITEM_TYPE))
									Owner.Send(fmsg);
								return false;
							}
							break;
						}
						#endregion
						#region Garment
					case Enums.ItemLocation.Garment:
						{
							if (MaskedGarment != null)
								return false;
							
							if (item.ItemType != Enums.ItemType.Garment)
							{
								using (var fmsg = Packets.Message.MessageCore.CreateSystem2(Owner.Name, Core.MessageConst.INVALID_ITEM_TYPE))
									Owner.Send(fmsg);
								return false;
							}
							break;
						}
						#endregion
						#region Steed
					case Enums.ItemLocation.Steed:
						{
							if (Owner.ContainsFlag1(Enums.Effect1.Riding))
								return false;
							
							if (item.ItemType != Enums.ItemType.Steed)
							{
								using (var fmsg = Packets.Message.MessageCore.CreateSystem2(Owner.Name, Core.MessageConst.INVALID_ITEM_TYPE))
									Owner.Send(fmsg);
								return false;
							}
							break;
						}
						#endregion
						#region SteedArmor
					case Enums.ItemLocation.SteedArmor:
						{
							if (Owner.ContainsFlag1(Enums.Effect1.Riding))
								return false;
							
							if (item.ItemType != Enums.ItemType.SteedArmor)
							{
								using (var fmsg = Packets.Message.MessageCore.CreateSystem2(Owner.Name, Core.MessageConst.INVALID_ITEM_TYPE))
									Owner.Send(fmsg);
								return false;
							}
							break;
						}
						#endregion
						#region Fan
					case Enums.ItemLocation.Fan:
						{
							return false;
							/*if (item.ItemType != Enums.ItemType.Fan)
						{
							using (var fmsg = Packets.Message.MessageCore.CreateSystem2(Owner.Name, Core.MessageConst.INVALID_ITEM_TYPE))
								Owner.Send(fmsg);
							return false;
						}
						break;*/
						}
						#endregion
						#region Tower
					case Enums.ItemLocation.Tower:
						{
							return false;
							/*if (item.ItemType != Enums.ItemType.Tower)
						{
							using (var fmsg = Packets.Message.MessageCore.CreateSystem2(Owner.Name, Core.MessageConst.INVALID_ITEM_TYPE))
								Owner.Send(fmsg);
							return false;
						}
						break;*/
						}
						#endregion
						#region Right
					case Enums.ItemLocation.WeaponR:
						{
							if (MaskedRightHand != null)
								return false;
							
							if (item.ItemType != Enums.ItemType.OneHand && item.ItemType != Enums.ItemType.TwoHand &&
							    item.ItemType != Enums.ItemType.Bow)
							{
								using (var fmsg = Packets.Message.MessageCore.CreateSystem2(Owner.Name, Core.MessageConst.INVALID_ITEM_TYPE))
									Owner.Send(fmsg);
								return false;
							}
							
							if (Contains(Enums.ItemLocation.WeaponL) && item.ItemType == Enums.ItemType.TwoHand)
							{
								using (var fmsg = Packets.Message.MessageCore.CreateSystem2(Owner.Name, Core.MessageConst.TWO_HAND_EQUIP_FAIL))
									Owner.Send(fmsg);
								return false;
							}
							break;
						}
						#endregion
						#region Left
					case Enums.ItemLocation.WeaponL:
						{
							if (MaskedLeftHand != null)
								return false;
							
							if (item.ItemType != Enums.ItemType.OneHand && item.ItemType != Enums.ItemType.Shield && item.ItemType != Enums.ItemType.Arrow)
							{
								using (var fmsg = Packets.Message.MessageCore.CreateSystem2(Owner.Name, Core.MessageConst.INVALID_ITEM_TYPE))
									Owner.Send(fmsg);
								return false;
							}
							
							if (item.ItemType == Enums.ItemType.Shield || item.ItemType == Enums.ItemType.Arrow)
							{
								if (!Contains(Enums.ItemLocation.WeaponR))
								{
									using (var fmsg = Packets.Message.MessageCore.CreateSystem2(Owner.Name, Core.MessageConst.INVALID_ITEM_TYPE))
										Owner.Send(fmsg);
									return false;
								}
							}
							
							if (!Contains(Enums.ItemLocation.WeaponR) && item.ItemType != Enums.ItemType.Shield && item.ItemType != Enums.ItemType.Arrow)
								pos = Enums.ItemLocation.WeaponR;
							break;
						}
						#endregion
						
						#region default
					default:
						{
							using (var fmsg = Packets.Message.MessageCore.CreateSystem2(Owner.Name, Core.MessageConst.INVALID_ITEM_POS))
								Owner.Send(fmsg);
							return false;
						}
						#endregion
				}
			}
			if (Contains(pos))
			{
				if (unequip)
				{
					if (!Unequip(pos))
						return false;
				}
				else if (!force_equip)
				{
					ItemInfo ritem;
					if (!EquipmentItems.TryRemove(pos, out ritem))
						return false;
				}
			}
			
			item.Location = pos;
			if (Owner.Inventory.ContainsByUID(item.UID) && !force_equip)
			{
				if (Owner.Inventory.RemoveItemByUID(item.UID) != null)
				{
					item.SendPacket(Owner, 1);
					if (EquipmentItems.TryAdd(pos, item))
					{
						if (Owner.LoggedIn)
						{
							Database.CharacterDatabase.SaveEquipment(Owner, item, pos);
							
							Owner.BaseEntity.CalculateBaseStats();
							
							using (var equippack = new Packets.ItemPacket())
							{
								equippack.UID = item.UID;
								equippack.Action = Enums.ItemAction.Equip;
								equippack.Data1Low = (ushort)pos;
								Owner.Send(equippack);
							}
							SendGears();
						}
						return true;
					}
				}
			}
			else if (!force_equip)
			{
				item.SendPacket(Owner, 1);
				if (EquipmentItems.TryAdd(pos, item))
				{
					if (Owner.LoggedIn)
					{
						Database.CharacterDatabase.SaveEquipment(Owner, item, pos);
						
						Owner.BaseEntity.CalculateBaseStats();
						
						using (var equippack = new Packets.ItemPacket())
						{
							equippack.UID = item.UID;
							equippack.Action = Enums.ItemAction.Equip;
							equippack.Data1Low = (ushort)pos;
							Owner.Send(equippack);
						}
						SendGears();
					}
					return true;
				}
			}
			else
			{
				item.SendPacket(Owner, 1);
				if (Owner.LoggedIn)
				{
					Database.CharacterDatabase.SaveEquipment(Owner, item, pos);
					
					Owner.BaseEntity.CalculateBaseStats();
					
					using (var equippack = new Packets.ItemPacket())
					{
						equippack.UID = item.UID;
						equippack.Action = Enums.ItemAction.Equip;
						equippack.Data1Low = (ushort)pos;
						Owner.Send(equippack);
					}
					SendGears();
				}
			}
			return false;
		}
		
		
		public void ForceEquipments(Equipments equips)
		{
			EquipmentItems = equips.EquipmentItems;
		}
		public void Swap(ItemInfo item, ItemInfo item2, Enums.ItemLocation pos)
		{
			ItemInfo ritem;
			if (EquipmentItems.TryRemove(pos, out ritem))
			{
				Owner.BaseEntity.CalculateBaseStats();

				SendGears();
				
				Equip(item2, pos, false);
			}
		}
		
		/// <summary>
		/// Replaces an equipment with a new item. This will REMOVE an existing item.
		/// </summary>
		/// <param name="item">The new item to equip.</param>
		/// <param name="pos">The position.</param>
		/// <returns>Returns true if the item was replaced.</returns>
		public bool Replace(ItemInfo item, Enums.ItemLocation pos)
		{
			return Equip(item, pos, false);
		}
		
		/// <summary>
		/// Adds a masked equipment.
		/// </summary>
		/// <param name="item">The masked item.</param>
		/// <param name="pos">The position.</param>
		/// <returns>Returns true if the mask was added.</returns>
		public bool AddMask(ItemInfo item, Enums.ItemLocation pos)
		{
			if (pos != Enums.ItemLocation.Garment && pos != Enums.ItemLocation.WeaponR && pos != Enums.ItemLocation.WeaponL)
				return false;
			
			if (pos == Enums.ItemLocation.Garment)
				MaskedGarment = item;
			else if (pos == Enums.ItemLocation.WeaponR)
				MaskedRightHand = item;
			else
				MaskedLeftHand = item;
			
			item.SendPacket(Owner, 1);
			using (var equippack = new Packets.ItemPacket())
			{
				equippack.UID = item.UID;
				equippack.Action = Enums.ItemAction.Equip;
				equippack.Data1Low = (ushort)pos;
				Owner.Send(equippack);
			}
			SendGears();
			return true;
		}
		
		/// <summary>
		/// Adds a masked equipment based on an item ID.
		/// </summary>
		/// <param name="ItemID">The masked item ID.</param>
		/// <param name="pos">The position.</param>
		/// <returns>Returns true if the mask was added.</returns>
		public bool AddMask(uint ItemID, Enums.ItemLocation pos)
		{
			return AddMask(Core.Kernel.ItemInfos[ItemID].Copy(), pos);
		}
		
		/// <summary>
		/// Unequips an item.
		/// </summary>
		/// <param name="pos">The position.</param>
		/// <returns>Returns true if the item was unequipped.</returns>
		public bool Unequip(Enums.ItemLocation pos)
		{
			if (Owner.ContainsFlag1(Enums.Effect1.Riding))
				return false;
			
			if (MaskedGarment != null)
				return false;
			if (MaskedLeftHand != null)
				return false;
			if (MaskedRightHand != null)
				return false;
			
			if (Owner.Inventory.Count < 40)
			{
				ItemInfo ritem;
				if (EquipmentItems.TryRemove(pos, out ritem))
				{
					Database.CharacterDatabase.SaveEquipment(Owner, null, pos);
					
					Owner.BaseEntity.CalculateBaseStats();

					using (var unequippack = new Packets.ItemPacket())
					{
						unequippack.UID = ritem.UID;
						unequippack.Action = Enums.ItemAction.Unequip;
						unequippack.Data1Low = (ushort)ritem.Location;
						Owner.Send(unequippack);
					}
					Owner.Inventory.AddItem(ritem);
					SendGears();
					return true;
				}
			}
			else
			{
				using (var fmsg = Packets.Message.MessageCore.CreateSystem2(Owner.Name, Core.MessageConst.INVENTORY_FULL))
					Owner.Send(fmsg);
			}
			return false;
		}
		
		/// <summary>
		/// Clears all masked items.
		/// </summary>
		public void ClearMask()
		{
			MaskedGarment = null;
			if (Contains(Enums.ItemLocation.Garment))
				Equip(EquipmentItems[Enums.ItemLocation.Garment], Enums.ItemLocation.Garment, false, true);
			MaskedRightHand = null;
			if (Contains(Enums.ItemLocation.WeaponR))
				Equip(EquipmentItems[Enums.ItemLocation.WeaponR], Enums.ItemLocation.WeaponR, false, true);
			MaskedLeftHand = null;
			if (Contains(Enums.ItemLocation.WeaponL))
				Equip(EquipmentItems[Enums.ItemLocation.WeaponL], Enums.ItemLocation.WeaponL, false, true);
			SendGears();
		}
		
		/// <summary>
		/// Sends all the gears to the client and screen.
		/// </summary>
		public void SendGears()
		{
			Owner.SendToScreen(Owner.CreateSpawnPacket(), false);
			using (var itempacket = new Packets.ItemPacket())
			{
				itempacket.Action = Enums.ItemAction.DisplayGears;
				itempacket.UID = Owner.EntityUID;
				itempacket.Data1 = 255;
				itempacket.SetGears(this);
				Owner.Send(itempacket);
			}
		}
		
		/// <summary>
		/// Sends the item infos to the client.
		/// </summary>
		public void SendItemInfos()
		{
			foreach (Enums.ItemLocation loc in EquipmentItems.Keys)
			{
				ItemInfo item = EquipmentItems[loc];
				item.SendPacket(Owner, 1);
				
				using (var equippack = new Packets.ItemPacket())
				{
					equippack.UID = item.UID;
					equippack.Action = Enums.ItemAction.Equip;
					equippack.Data1Low = (ushort)loc;
					Owner.Send(equippack);
				}
			}
		}
		
		/// <summary>
		/// Saves the equipments [Currently unused and not really needed.]
		/// </summary>
		public void SaveEquipments()
		{
			
		}
	}
}
