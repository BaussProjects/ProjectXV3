//Project by BaussHacker aka. L33TS
using System;

namespace ProjectX_V3_Game.Entities
{
	/// <summary>
	/// Description of Sob.
	/// </summary>
	public class Sob : IEntity, Maps.IMapObject
	{
		private NPC associatedNPC;
		
		public Sob(NPC associatedNPC)
		{
			this.associatedNPC = associatedNPC;
			_baseentity = new BaseEntity(this);
			_screen = new ProjectX_V3_Game.Core.Screen(this);
		}
		
		#region Global Variables
		/// <summary>
		/// The mesh of the sob.
		/// </summary>
		public ushort Mesh;
		
		/// <summary>
		/// The flag of the sob.
		/// </summary>
		public ushort Flag;

		#endregion
		
		#region Properties
		/// <summary>
		/// The first status flag holder.
		/// </summary>
		private ulong _statusflag1;
		
		/// <summary>
		/// Gets or sets the first status flag.
		/// </summary>
		public ulong StatusFlag1
		{
			get { return _statusflag1; }
			set
			{
				_statusflag1 = value;
			}
		}
		
		/// <summary>
		/// The alive holder.
		/// </summary>
		private bool _alive = true;
		
		/// <summary>
		/// Gets or sets whether the npc is alive. (Although it's always alive ^~^)
		/// </summary>
		public bool Alive
		{
			get { return _alive; }
			set { _alive = value; }
		}
		
		/// <summary>
		/// The base entity holder.
		/// </summary>
		private BaseEntity _baseentity;
		
		/// <summary>
		/// Gets the base entity holder.
		/// </summary>
		public BaseEntity BaseEntity
		{
			get { return _baseentity; }
		}
		
		/// <summary>
		/// The reborn holder.
		/// </summary>
		private byte _reborns;
		
		/// <summary>
		/// Gets or sets the reborns of the character.
		/// </summary>
		public byte Reborns
		{
			get { return _reborns; }
			set
			{
				_reborns = value;
				this.BaseEntity.CalculateBaseStats();
			}
		}
		
		/// <summary>
		/// The class holder.
		/// </summary>
		private Enums.Class _class = Enums.Class.Other;
		
		/// <summary>
		/// Gets or sets the class.
		/// </summary>
		public Enums.Class Class
		{
			get { return _class; }
			set
			{
				_class = value;
				this.BaseEntity.CalculateBaseStats();
			}
		}
		
		/// <summary>
		/// Gets a boolean defining whether the client can update its own spawn and/or spawn to others.
		/// </summary>
		public bool CanUpdateSpawn
		{
			get { return true; }
		}
		
		/// <summary>
		/// The name holder.
		/// </summary>
		private string _name;
		
		/// <summary>
		/// Gets or sets the name of the npc.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
		
		/// <summary>
		/// The entity uid holder.
		/// </summary>
		private uint _entityuid;
		
		/// <summary>
		/// Gets or sets the entity UID of the npc.
		/// </summary>
		public uint EntityUID
		{
			get { return _entityuid; }
			set { _entityuid = value; }
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
		/// The hp holder.
		/// </summary>
		private int _hp;
		
		/// <summary>
		/// Gets or sets the hp.
		/// </summary>
		public int HP
		{
			get { return _hp; }
			set
			{
				_hp = value;
			}
		}
		
		/// <summary>
		/// The mp holder.
		/// </summary>
		private int _mp;
		
		/// <summary>
		/// Gets or sets the mp.
		/// </summary>
		public int MP
		{
			get { return _mp; }
			set
			{
				_mp = value;
			}
		}
		
		/// <summary>
		/// The max hp holder.
		/// </summary>
		private int _maxhp;
		
		/// <summary>
		/// Gets or sets the max hp.
		/// </summary>
		public int MaxHP
		{
			get { return _maxhp; }
			set
			{
				_maxhp = value;
			}
		}
		
		/// <summary>
		/// The max mp holder.
		/// </summary>
		private int _maxmp;
		
		/// <summary>
		/// Gets or sets the maxmp.
		/// </summary>
		public int MaxMP
		{
			get { return _maxmp; }
			set
			{
				_maxmp = value;
			}
		}
		
		/// <summary>
		/// The level holder.
		/// </summary>
		private byte _level;
		
		/// <summary>
		/// Gets or sets the level.
		/// </summary>
		public byte Level
		{
			get { return _level; }
			set
			{
				_level = value;
			}
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
		/// Gets or sets the dynamic map for the npc.
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
		/// The strength holder.
		/// </summary>
		private ushort _strength;
		
		/// <summary>
		/// Gets or sets the strength.
		/// </summary>
		public ushort Strength
		{
			get { return _strength; }
			set
			{
				_strength = value;
				this.BaseEntity.CalculateBaseStats();
			}
		}
		
		/// <summary>
		/// The agility holder.
		/// </summary>
		private ushort _agility;
		
		/// <summary>
		/// Gets or sets the agility.
		/// </summary>
		public ushort Agility
		{
			get { return _agility; }
			set
			{
				_agility = value;
				this.BaseEntity.CalculateBaseStats();
			}
		}

		/// <summary>
		/// The vitality holder.
		/// </summary>
		private ushort _vitality;
		
		/// <summary>
		/// Gets or sets the vitality.
		/// </summary>
		public ushort Vitality
		{
			get { return _vitality; }
			set
			{
				_vitality = value;
				this.BaseEntity.CalculateBaseStats();
			}
		}
		
		/// <summary>
		/// The spirit holder.
		/// </summary>
		private ushort _spirit;
		
		/// <summary>
		/// Gets or sets the spirit.
		/// </summary>
		public ushort Spirit
		{
			get { return _spirit; }
			set
			{
				_spirit = value;
				this.BaseEntity.CalculateBaseStats();
			}
		}
		#endregion
		
		#region Core Methods
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
		/// Calls the npc dialog associated with the npc.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="option">The dialog option.</param>
		public void CallDialog(GameClient client, byte option)
		{
			if (associatedNPC != null)
				associatedNPC.CallDialog(client, option);
		}
		#endregion
		
		#region Methods
		private int MoveCount = 0;
		private bool LastLeft = false;
		public void Move()
		{		
			new ProjectX_V3_Lib.Threading.BaseThread(MoveT, 500, "").Start();
		}
		private void MoveT()
		{
			if (LastLeft)
			{
				// move right
				/*using (var movepacket = new Packets.MovementPacket())
				{
					movepacket.EntityUID = mob.EntityUID;
					movepacket.TimeStamp = ProjectX_V3_Lib.Native.Winmm.timeGetTime();
					movepacket.Direction = (uint)Enums.ConquerAngle.East;
					movepacket.WalkMode = Enums.WalkMode.Run;
					mob.Screen.UpdateScreen(movepacket);
				}*/
			}
			else
			{
				// move left
				/*using (var movepacket = new Packets.MovementPacket())
				{
					movepacket.EntityUID = mob.EntityUID;
					movepacket.TimeStamp = ProjectX_V3_Lib.Native.Winmm.timeGetTime();
					movepacket.Direction = (uint)Enums.ConquerAngle.West;
					movepacket.WalkMode = Enums.WalkMode.Run;
					mob.Screen.UpdateScreen(movepacket);
				}*/
			}
			if (MoveCount >= 5)
			{
				LastLeft = !LastLeft;
				MoveCount = 0;
			}
		}
		#endregion
		
		#region Misc
		#endregion
	}
}
