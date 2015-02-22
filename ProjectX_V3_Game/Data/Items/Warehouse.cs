//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;
using System.Linq;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// Description of Warehouse.
	/// </summary>
	public class Warehouse
	{
		public static ushort WhToPos(uint Where)//Warehouse ID gives Item position
		{
			ushort toRet = 0;
			switch (Where)
			{
				case 8:
					toRet = 5000;
					break;
				case 10012:
					toRet = 5001;
					break;
				case 10028:
					toRet = 5002;
					break;
				case 10011:
					toRet = 5003;
					break;
				case 10027:
					toRet = 5004;
					break;
				case 4101:
					toRet = 5005;
					break;
				case 44:
					toRet = 5006;
					break;
			}
			return toRet;
		}
		public static ushort PosToWH(uint Where)//Item location gives warehouse ID
		{
			ushort toRet = 0;
			switch (Where)
			{
				case 5000:
					toRet = 8;
					break;
				case 5001:
					toRet = 10012;
					break;
				case 5002:
					toRet = 10028;
					break;
				case 5003:
					toRet = 10011;
					break;
				case 5004:
					toRet = 10027;
					break;
				case 5005:
					toRet = 4101;
					break;
				case 5006:
					toRet = 44;
					break;
			}
			return toRet;
		}
		
		/// <summary>
		/// The owner of the inventory.
		/// </summary>
		private Entities.GameClient Owner;
		
		private ushort WhID;
		
		/// <summary>
		/// Creates a new instance of Warehouse.
		/// </summary>
		/// <param name="owner">The owner of the warehouse.</param>
		/// <param name="MapID">The wh id of the warehouse.</param>
		public Warehouse(Entities.GameClient owner, ushort WhID)
		{
			this.Owner = owner;
			warehouseItems = new ConcurrentDictionary<byte, ItemInfo>();
			this.WhID = WhID;
		}
		
		/// <summary>
		/// The collection of items.
		/// </summary>
		private ConcurrentDictionary<byte, ItemInfo> warehouseItems;
		
		/// <summary>
		/// Gets the collection of items.
		/// </summary>
		public ConcurrentDictionary<byte, ItemInfo> WarehouseItems
		{
			get { return warehouseItems; }
		}
		
		/// <summary>
		/// Gets the free spaces (positions) in the warehouse.
		/// </summary>
		public byte FreeSpaces
		{
			get
			{
				for (byte index = 0; index < 20; index++)
				{
					if (!warehouseItems.ContainsKey(index))
						return index;
				}
				return 20; // invalid
			}
		}
		
		/// <summary>
		/// Gets the item count of the warehouse.
		/// </summary>
		public int Count
		{
			get { return WarehouseItems.Count; }
		}
		
		/// <summary>
		/// Adds an item to the warehouse.
		/// </summary>
		/// <param name="item">The item to add.</param>
		/// <returns>Returns true if the item was added.</returns>
		public bool AddItem(ItemInfo item)
		{
			return AddItem(item, FreeSpaces);
		}
		
		/// <summary>
		/// Adds an item to the warehouse.
		/// </summary>
		/// <param name="item">The item to add.</param>
		/// <param name="pos">The position in the inventory.</param>
		/// <returns>Returns true if the item was added.</returns>
		public bool AddItem(ItemInfo item, byte pos)
		{
			if (Count >= 20)
			{
				using (var fmsg = Packets.Message.MessageCore.CreateSystem2(Owner.Name, Core.MessageConst.INVENTORY_FULL))
					Owner.Send(fmsg);
				return false;
			}
			
			if (WarehouseItems.TryAdd(pos, item))
			{
				item.OwnerUID = Owner.EntityUID;
				Database.CharacterDatabase.SaveWarehouse(Owner, item, pos, WhID);
				//SendAll();
				SendSingle(item);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets an item from the warehouse by its uid.
		/// </summary>
		/// <param name="ItemUID">The item uid.</param>
		/// <returns>Returns the item. [null if failed]</returns>
		public ItemInfo GetItemByUID(uint ItemUID)
		{
			byte pos = GetPositionFromItemUID(ItemUID);
			
			ItemInfo rItem;
			if (WarehouseItems.TryGetValue(pos, out rItem))
			{
				return rItem;
			}
			return null;
		}
		
		/// <summary>
		/// Gets an item from the warehouse by its id.
		/// </summary>
		/// <param name="ItemID">The item id.</param>
		/// <returns>Returns the item. [null if failed]</returns>
		public ItemInfo GetItemByID(uint ItemID)
		{
			byte pos = GetPositionFromItemID(ItemID);
			
			ItemInfo rItem;
			if (WarehouseItems.TryGetValue(pos, out rItem))
			{
				return rItem;
			}
			return null;
		}
		
		/// <summary>
		/// Removes an item from the warehouse by its uid.
		/// </summary>
		/// <param name="ItemUID">The item uid.</param>
		/// <returns>Returns the removed item. [null if failed]</returns>
		public ItemInfo RemoveItemByUID(uint ItemUID)
		{
			byte pos = GetPositionFromItemUID(ItemUID);
			
			ItemInfo rItem;
			if (WarehouseItems.TryRemove(pos, out rItem))
			{
				Database.CharacterDatabase.SaveWarehouse(Owner, null, pos, WhID);
				//SendAll();
				return rItem;
			}
			return null;
		}
		
		/// <summary>
		/// Removes an item from the warehouse based on is iteminfo.
		/// </summary>
		/// <param name="info">The item info.</param>
		/// <returns>Returns true if the item was removed.</returns>
		public bool RemoveItem(ItemInfo info)
		{
			foreach (byte key in WarehouseItems.Keys)
			{
				ItemInfo ritem;
				if (WarehouseItems.TryGetValue(key, out ritem))
				{
					if (ritem.UID == info.UID)
					{
						if (WarehouseItems.TryRemove(key, out ritem))
						{
							Database.CharacterDatabase.SaveWarehouse(Owner, null, key, WhID);
							//SendAll();
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
			foreach (byte key in WarehouseItems.Keys)
			{
				ItemInfo finditem;
				if (WarehouseItems.TryGetValue(key, out finditem))
				{
					if (finditem.ItemID == ItemID)
					{
						return key;
					}
				}
			}
			return 20;
		}
		
		/// <summary>
		/// Gets an item position from an item uid.
		/// </summary>
		/// <param name="ItemID">The item id.</param>
		/// <returns>Returns the item uid.</returns>
		public byte GetPositionFromItemUID(uint ItemUID)
		{
			foreach (byte key in WarehouseItems.Keys)
			{
				ItemInfo finditem;
				if (WarehouseItems.TryGetValue(key, out finditem))
				{
					if (finditem.UID == ItemUID)
					{
						return key;
					}
				}
			}
			return 20;
		}
		
		/// <summary>
		/// Searches for an item in the warehouse that has above 0 dura, based on an item ID. (Used for arrows.)
		/// </summary>
		/// <param name="ItemID">The item ID.</param>
		/// <returns>Returns the item found. Null if no items available.</returns>
		public ItemInfo SearchForNonEmpty(uint ItemID)
		{
			foreach (Data.ItemInfo item in warehouseItems.Values)
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
			foreach (byte key in WarehouseItems.Keys)
			{
				ItemInfo finditem;
				if (WarehouseItems.TryGetValue(key, out finditem))
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
		/// Checks if the warehouse contains an item by its item id.
		/// </summary>
		/// <param name="ItemID">The item id.</param>
		/// <returns>Returns true if the item exists in the inventory.</returns>
		public bool ContainsByID(uint ItemID)
		{
			return (GetPositionFromItemID(ItemID) < 20);
		}
		
		/// <summary>
		/// Checks if the warehouse contains an item by its item id.
		/// </summary>
		/// <param name="ItemID">The item id.</param>
		/// <param name="Amount">[out] The total amount of items contained within the inventory.</param>
		/// <returns>Returns true if the item exists in the inventory.</returns>
		public bool ContainsByID(uint ItemID, out byte Amount)
		{
			return (GetUIDFromItemID(ItemID, out Amount) > 0);
		}
		
		/// <summary>
		/// Checks if the warehouse contains an item by its uid.
		/// </summary>
		/// <param name="ItemUID">The uid.</param>
		/// <returns>Returns true if the item exists in the inventory.</returns>
		public bool ContainsByUID(uint ItemUID)
		{
			return (GetPositionFromItemUID(ItemUID) < 20);
		}
		
		/// <summary>
		/// Checks if the warehouse contains an item by position.
		/// </summary>
		/// <param name="pos">The position.</param>
		/// <returns>Returns true if there exists an item at the position.</returns>
		public bool ContainsByPos(byte pos)
		{
			return WarehouseItems.ContainsKey(pos);
		}
		
		/// <summary>
		/// Sends all items in the warehouse to the client. [Used at login]
		/// </summary>
		public void SendAll()
		{
			foreach (Data.ItemInfo item in WarehouseItems.Values)
				SendSingle(item);
		}
		
		public void SendSingle(Data.ItemInfo item)
		{
			using (var warehouse = new Packets.WarehousePacket(item))
			{
				warehouse.WarehouseID = WhID;
				Owner.Send(warehouse);
			}
		}
		
		/// <summary>
		/// Gets an item from the warehouse based on an UID as index.
		/// </summary>
		public ItemInfo this[uint uid]
		{
			get
			{
				byte pos = GetPositionFromItemUID(uid);
				if (pos == 20)
					return null;
				
				return warehouseItems[pos];
			}
		}
		
		/// <summary>
		/// Saves the warehouse. [Currently not used and not really needed.]
		/// </summary>
		public void SaveWarehouse()
		{
			
		}
	}
}
