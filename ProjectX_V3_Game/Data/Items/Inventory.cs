//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// An inventory of a client. TODO: Rewrite the save system, because it's slow when adding multiple items (will be fixed in next revision)
	/// </summary>
	public class Inventory
	{
		/// <summary>
		/// The owner of the inventory.
		/// </summary>
		private Entities.GameClient Owner;
		
		/// <summary>
		/// Creates a new instance of Inventory.
		/// </summary>
		/// <param name="owner">The owner of the inventory.</param>
		public Inventory(Entities.GameClient owner)
		{
			this.Owner = owner;
			inventoryItems = new ConcurrentDictionary<byte, ItemInfo>();
		}
		
		/// <summary>
		/// The collection of items.
		/// </summary>
		private ConcurrentDictionary<byte, ItemInfo> inventoryItems;
		
		/// <summary>
		/// Gets the collection of items.
		/// </summary>
		public ConcurrentDictionary<byte, ItemInfo> InventoryItems
		{
			get { return inventoryItems; }
		}
		
		/// <summary>
		/// Gets the free spaces (positions) in the inventory.
		/// </summary>
		public byte FreeSpaces
		{
			get
			{
				for (byte index = 0; index < 40; index++)
				{
					if (!inventoryItems.ContainsKey(index))
						return index;
				}
				return 40; // invalid
			}
		}
		
		/// <summary>
		/// Gets the item count of the inventory.
		/// </summary>
		public int Count
		{
			get { return InventoryItems.Count; }
		}
		
		/// <summary>
		/// Adds an item to the inventory.
		/// </summary>
		/// <param name="item">The item to add.</param>
		/// <returns>Returns true if the item was added.</returns>
		public bool AddItem(ItemInfo item)
		{
			return AddItem(item, FreeSpaces);
		}
		
		/// <summary>
		/// Adds an item to the inventory.
		/// </summary>
		/// <param name="item">The item to add.</param>
		/// <param name="pos">The position in the inventory.</param>
		/// <returns>Returns true if the item was added.</returns>
		public bool AddItem(ItemInfo item, byte pos)
		{
			if (Count >= 40)
			{
				using (var fmsg = Packets.Message.MessageCore.CreateSystem2(Owner.Name, Core.MessageConst.INVENTORY_FULL))
					Owner.Send(fmsg);
				return false;
			}
			
			if (InventoryItems.TryAdd(pos, item))
			{
				item.OwnerUID = Owner.EntityUID;
				item.Location = Enums.ItemLocation.Inventory;
				Database.CharacterDatabase.SaveInventory(Owner, item, pos);
				SendItemToClient(item);
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// Adds an item to the inventory.
		/// </summary>
		/// <param name="ItemID">The id of the item to add.</param>
		/// <returns>Returns true if the item was added.</returns>
		public bool AddItem(uint ItemID)
		{
			ItemInfo original;
			Core.Kernel.ItemInfos.TrySelect(ItemID, out original);
			return AddItem(original.Copy());
		}
		
		/// <summary>
		/// Adds multiple items to the inventory.
		/// </summary>
		/// <param name="ItemID">The id of the item to add.</param>
		/// <param name="amount">The amount of it to add.</param>
		/// <returns>Returns true if the items was added.</returns>
		public bool AddItem(uint ItemID, byte amount = 1)
		{
			if (amount == 0)
				return true;
			
			ItemInfo original;
			Core.Kernel.ItemInfos.TrySelect(ItemID, out original);
			for (byte i = 0; i < amount; i++)
			{
				if (!AddItem(original.Copy()))
					return false;
			}
			return true;
		}
		
		/// <summary>
		/// Adds an item to the inventory.
		/// </summary>
		/// <param name="ItemID">The id of the item to add.</param>
		/// <param name="Plus">The plus stats of the item to add.</param>
		/// <param name="Bless">The bless stats of the item to add.</param>
		/// <param name="Enchant">The enchantment of the item to add.</param>
		/// <param name="Gem1">The first socket gem of the item to add.</param>
		/// <param name="Gem2">The second socket gem of the item to add.</param>
		/// <param name="amount">The amount to add.</param>
		/// <returns>Returns true if the item was added.</returns>
		public bool AddItem(uint ItemID, byte Plus = 0, byte Bless = 0, byte Enchant = 0, Enums.SocketGem Gem1 = 0, Enums.SocketGem Gem2 = 0, byte amount = 1)
		{
			if (amount == 0)
				return true;
			
			ItemInfo original;
			Core.Kernel.ItemInfos.TrySelect(ItemID, out original);
			
			for (byte i = 0; i < amount; i++)
			{
				ItemInfo newitem = original.Copy();
				newitem.Plus = Plus;
				newitem.Bless = Bless;
				newitem.Enchant = Enchant;
				newitem.Gem1 = Gem1;
				newitem.Gem2 = Gem2;
				
				if (!AddItem(newitem))
					return false;
			}
			return true;
		}
		
		/// <summary>
		/// Adds an item to the inventory.
		/// </summary>
		/// <param name="ItemID">The id of the item to add.</param>
		/// <param name="Plus">The plus stats of the item to add.</param>
		/// <param name="Bless">The bless stats of the item to add.</param>
		/// <param name="Enchant">The enchantment of the item to add.</param>
		/// <param name="Gem1">The first socket gem of the item to add.</param>
		/// <param name="Gem2">The second socket gem of the item to add.</param>
		/// <returns>Returns true if the item was added.</returns>
		public bool AddItem(uint ItemID, byte Plus = 0, byte Bless = 0, byte Enchant = 0, Enums.SocketGem Gem1 = 0, Enums.SocketGem Gem2 = 0)
		{
			ItemInfo original;
			Core.Kernel.ItemInfos.TrySelect(ItemID, out original);
			ItemInfo newitem = original.Copy();
			newitem.Plus = Plus;
			newitem.Bless = Bless;
			newitem.Enchant = Enchant;
			newitem.Gem1 = Gem1;
			newitem.Gem2 = Gem2;
			return AddItem(newitem);
		}
		
		/// <summary>
		/// Removes an item from the inventory.
		/// </summary>
		/// <param name="ItemID">The item id.</param>
		/// <returns>Returns true if the item was removed.</returns>
		public bool RemoveItem(uint ItemID)
		{
			byte pos = GetPositionFromItemID(ItemID);
			ItemInfo rItem;
			if (InventoryItems.TryRemove(pos, out rItem))
			{
				Database.CharacterDatabase.SaveInventory(Owner, null, pos);
				RemoveItemFromClient(rItem);
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// Gets an item from the inventory by its uid.
		/// </summary>
		/// <param name="ItemUID">The item uid.</param>
		/// <returns>Returns the item. [null if failed]</returns>
		public ItemInfo GetItemByUID(uint ItemUID)
		{
			byte pos = GetPositionFromItemUID(ItemUID);
			
			ItemInfo rItem;
			if (InventoryItems.TryGetValue(pos, out rItem))
			{
				return rItem;
			}
			return null;
		}
		
		/// <summary>
		/// Gets an item from the inventory by its id.
		/// </summary>
		/// <param name="ItemID">The item id.</param>
		/// <returns>Returns the item. [null if failed]</returns>
		public ItemInfo GetItemByID(uint ItemID)
		{
			byte pos = GetPositionFromItemID(ItemID);
			
			ItemInfo rItem;
			if (InventoryItems.TryGetValue(pos, out rItem))
			{
				return rItem;
			}
			return null;
		}
		
		/// <summary>
		/// Removes an item from the inventory by its uid.
		/// </summary>
		/// <param name="ItemUID">The item uid.</param>
		/// <returns>Returns the removed item. [null if failed]</returns>
		public ItemInfo RemoveItemByUID(uint ItemUID)
		{
			byte pos = GetPositionFromItemUID(ItemUID);
			
			ItemInfo rItem;
			if (InventoryItems.TryRemove(pos, out rItem))
			{
				Database.CharacterDatabase.SaveInventory(Owner, null, pos);
				RemoveItemFromClient(rItem);
				return rItem;
			}
			return null;
		}
		
		/// <summary>
		/// Removes an item from the inventory.
		/// </summary>
		/// <param name="ItemID">The item id.</param>
		/// <param name="Amount">The amount to remove.</param>
		/// <param name="Removed">[out] amount of items removed.</param>
		/// <returns>Returns true if the amount matched the removed items.</returns>
		public bool RemoveItem(uint ItemID, byte Amount, out byte Removed)
		{
			bool failed = false;
			Removed = 0;
			byte contains;
			ContainsByID(ItemID, out contains);
			
			for (byte i = 0; i < Amount; i++)
			{
				if (contains == Removed)
					return !failed;
				
				if (!RemoveItem(ItemID))
					failed = true;
				else
					Removed++;
			}
			
			return !failed;
		}
		
		/// <summary>
		/// Removes an item from the inventory based on is iteminfo.
		/// </summary>
		/// <param name="info">The item info.</param>
		/// <returns>Returns true if the item was removed.</returns>
		public bool RemoveItem(ItemInfo info)
		{
			foreach (byte key in InventoryItems.Keys)
			{
				ItemInfo ritem;
				if (InventoryItems.TryGetValue(key, out ritem))
				{
					if (ritem.UID == info.UID)
					{
						if (InventoryItems.TryRemove(key, out ritem))
						{
							Database.CharacterDatabase.SaveInventory(Owner, null, key);
							RemoveItemFromClient(ritem);
							return true;
						}
						else
							return false;
					}
				}
			}
			return false;
		}
		
		/// <summary>
		/// Gets an item position from an item id.
		/// </summary>
		/// <param name="ItemID">The item id.</param>
		/// <returns>Returns the item uid.</returns>
		public byte GetPositionFromItemID(uint ItemID)
		{
			foreach (byte key in InventoryItems.Keys)
			{
				ItemInfo finditem;
				if (InventoryItems.TryGetValue(key, out finditem))
				{
					if (finditem.ItemID == ItemID)
					{
						return key;
					}
				}
			}
			return 40;
		}
		
		/// <summary>
		/// Gets an item position from an item uid.
		/// </summary>
		/// <param name="ItemID">The item id.</param>
		/// <returns>Returns the item uid.</returns>
		public byte GetPositionFromItemUID(uint ItemUID)
		{
			foreach (byte key in InventoryItems.Keys)
			{
				ItemInfo finditem;
				if (InventoryItems.TryGetValue(key, out finditem))
				{
					if (finditem.UID == ItemUID)
					{
						return key;
					}
				}
			}
			return 40;
		}
		
		/// <summary>
		/// Searches for an item in the inventory that has above 0 dura, based on an item ID. (Used for arrows.)
		/// </summary>
		/// <param name="ItemID">The item ID.</param>
		/// <returns>Returns the item found. Null if no items available.</returns>
		public ItemInfo SearchForNonEmpty(uint ItemID)
		{
			foreach (Data.ItemInfo item in inventoryItems.Values)
			{
				if (item.ItemID == ItemID && item.CurrentDura > 0)
					return item;
			}
			
			return null;
		}
		
		/// <summary>
		/// Gets an uid from an item id.
		/// </summary>
		/// <param name="ItemID">The item id.</param>
		/// <param name="TotalAmounts">[out] The total amount of items contained with the id.</param>
		/// <returns>Returns the item uid.</returns>
		public uint GetUIDFromItemID(uint ItemID, out byte TotalAmounts)
		{
			uint find = 0;
			TotalAmounts = 0;
			foreach (byte key in InventoryItems.Keys)
			{
				ItemInfo finditem;
				if (InventoryItems.TryGetValue(key, out finditem))
				{
					if (finditem.ItemID == ItemID)
					{
						if (TotalAmounts == 0)
							find = finditem.UID;
						TotalAmounts++;
					}
				}
			}
			return find;
		}
		
		/// <summary>
		/// Checks if the inventory contains an item by its item id.
		/// </summary>
		/// <param name="ItemID">The item id.</param>
		/// <returns>Returns true if the item exists in the inventory.</returns>
		public bool ContainsByID(uint ItemID)
		{
			return (GetPositionFromItemID(ItemID) < 40);
		}
		
		/// <summary>
		/// Checks if the inventory contains an item by its item id.
		/// </summary>
		/// <param name="ItemID">The item id.</param>
		/// <param name="Amount">[out] The total amount of items contained within the inventory.</param>
		/// <returns>Returns true if the item exists in the inventory.</returns>
		public bool ContainsByID(uint ItemID, out byte Amount)
		{
			return (GetUIDFromItemID(ItemID, out Amount) > 0);
		}
		
		/// <summary>
		/// Checks if the inventory contains an item by its uid.
		/// </summary>
		/// <param name="ItemUID">The uid.</param>
		/// <returns>Returns true if the item exists in the inventory.</returns>
		public bool ContainsByUID(uint ItemUID)
		{
			return (GetPositionFromItemUID(ItemUID) < 40);
		}
		
		/// <summary>
		/// Checks if the inventory contains an item by position.
		/// </summary>
		/// <param name="pos">The position.</param>
		/// <returns>Returns true if there exists an item at the position.</returns>
		public bool ContainsByPos(byte pos)
		{
			return inventoryItems.ContainsKey(pos);
		}
		
		/// <summary>
		/// Sends the item data to the client.
		/// </summary>
		/// <param name="item">The item.</param>
		public void SendItemToClient(ItemInfo item)
		{
			item.Location = Enums.ItemLocation.Inventory;
			item.SendPacket(Owner, 1);
		}
		
		/// <summary>
		/// Removes the item data from the client.
		/// </summary>
		/// <param name="item">The item.</param>
		public void RemoveItemFromClient(ItemInfo item)
		{
			using (var remove = new Packets.ItemPacket())
			{
				remove.UID = item.UID;
				remove.Action = Enums.ItemAction.Remove;
				Owner.Send(remove);
			}
		}
		
		/// <summary>
		/// Sends all items in the inventory to the client. [Used at login]
		/// </summary>
		public void SendAll()
		{
			foreach (ItemInfo item in InventoryItems.Values)
			{
				SendItemToClient(item);
			}
		}
		
		/// <summary>
		/// Gets an item from the inventory based on an UID as index.
		/// </summary>
		public ItemInfo this[uint uid]
		{
			get
			{
				byte pos = GetPositionFromItemUID(uid);
				if (pos == 40)
					return null;
				
				return inventoryItems[pos];
			}
		}
		
		/// <summary>
		/// Saves the inventory. [Currently not used and not really needed.]
		/// </summary>
		public void SaveInventory()
		{
			
		}
	}
}
