//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// An item on the ground.
	/// </summary>
	public class GroundItem : Maps.IMapObject
	{
		/// <summary>
		/// Creates a new instance of GroundItem.
		/// </summary>
		/// <param name="item">The actual item info.</param>
		public GroundItem(ItemInfo item)
		{
			this.item = item;
			_screen = new ProjectX_V3_Game.Core.Screen(this);
		}
		
		/// <summary>
		/// The time the item was dropped.
		/// </summary>
		public DateTime DropTime = DateTime.Now;
		
		/// <summary>
		/// If the drop is a player drop.
		/// </summary>
		public bool PlayerDrop = false;
		
		/// <summary>
		/// The item holder.
		/// </summary>
		private ItemInfo item;
		
		/// <summary>
		/// The owner UID.
		/// </summary>
		public uint OwnerUID = 0;
		
		/// <summary>
		/// Gets the item info.
		/// </summary>
		public ItemInfo Item
		{
			get { return item; }
		}
		
		/// <summary>
		/// Gets or sets the entity UID of the item.
		/// </summary>
		public uint EntityUID
		{
			get { return item.UID; }
			set { throw new Exception("DO NOT SET THE UID FOR THIS"); }
		}
		
		/// <summary>
		/// Gets a boolean defining whether the client can update its own spawn and/or spawn to others.
		/// </summary>
		public bool CanUpdateSpawn
		{
			get
			{
				return Map != null;
			}
		}
		
		/// <summary>
		/// The screen holder.
		/// </summary>
		private Core.Screen _screen;
		
		/// <summary>
		/// Gets the screen.
		/// </summary>
		public Core.Screen Screen
		{
			get { return _screen; }
		}
		
		/// <summary>
		/// The lastmapid holder.
		/// </summary>
		private ushort _lastmapid;
		
		/// <summary>
		/// Gets or sets the last map.
		/// </summary>
		public ushort LastMapID
		{
			get { return _lastmapid; }
			set { _lastmapid = value; }
		}
		
		/// <summary>
		/// The x coordinate holder.
		/// </summary>
		private ushort _x;
		
		/// <summary>
		/// Gets or sets the x coordinate.
		/// </summary>
		public ushort X
		{
			get { return _x; }
			set
			{
				_x = value;
			}
		}
		
		/// <summary>
		/// The y coordinate holder.
		/// </summary>
		private ushort _y;
		
		/// <summary>
		/// Gets or sets the y coordinate.
		/// </summary>
		public ushort Y
		{
			get { return _y; }
			set
			{
				_y = value;
			}
		}
		
		/// <summary>
		/// The dynamic map holder.
		/// </summary>
		private Maps.DynamicMap _dynamicMap;
		
		/// <summary>
		/// Gets or sets the dynamic map for the item.
		/// </summary>
		public Maps.DynamicMap DynamicMap
		{
			get { return _dynamicMap; }
			set { _dynamicMap = value; }
		}
		
		/// <summary>
		/// The map holder.
		/// </summary>
		private Maps.Map _map;
		
		/// <summary>
		/// Gets or sets the map.
		/// </summary>
		public Maps.Map Map
		{
			get { return _map; }
			set { _map = value; }
		}
		
		/// <summary>
		/// The direction holder.
		/// </summary>
		private byte _direction;
		
		/// <summary>
		/// Gets or sets the direction.
		/// </summary>
		public byte Direction
		{
			get { return _direction; }
			set { _direction = value; }
		}
		
		/// <summary>
		/// The money value.
		/// </summary>
		public uint Money;
		
		/// <summary>
		/// The delayed task id.
		/// </summary>
		public uint TaskID;
		
		/// <summary>
		/// The type of drop.
		/// </summary>
		public Enums.DropItemType DropType;
		
		public bool IsInMap(Maps.IMapObject MapObject)
		{
			if (DynamicMap != null)
			{
				if (MapObject.DynamicMap == null)
					return false;
				
				if (DynamicMap.DynamicID != MapObject.DynamicMap.DynamicID)
					return false;
			}
			return Map.MapID == MapObject.Map.MapID;
		}
		
		[Obsolete("This method should never be called.")]
		public Packets.SpawnPacket CreateSpawnPacket()
		{
			throw new Exception("DO NOT CALL THIS!");
		}
		
		/// <summary>
		/// Creates the spawn packet for the item.
		/// </summary>
		/// <param name="droptype">The drop type.</param>
		/// <returns>Returns the packet.</returns>
		public Packets.GroundItemPacket CreateItemSpawnPacket(ushort droptype)
		{
			Packets.GroundItemPacket ground = new ProjectX_V3_Game.Packets.GroundItemPacket();
			ground.UID = Item.UID;
			ground.ItemID = Item.ItemID;
			ground.X = X;
			ground.Y = Y;
			ground.DropType = droptype;
			return ground;
		}
	}
}
