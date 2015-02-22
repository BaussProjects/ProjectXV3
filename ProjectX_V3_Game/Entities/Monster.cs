//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Extensions;
using System.Collections.Concurrent;

namespace ProjectX_V3_Game.Entities
{
	/// <summary>
	/// A monster.
	/// </summary>
	[Serializable()]
	public class Monster : IEntity, Maps.IMapObject
	{
		private static ConcurrentDictionary<uint, uint> useduids;
		static Monster()
		{
			useduids = new ConcurrentDictionary<uint, uint>();
		}
		
		public Monster()
		{
			_screen = new ProjectX_V3_Game.Core.Screen(this);
			_baseentity = new BaseEntity(this);
			
		}
		
		#region Global Variables
		/// <summary>
		/// The last time there was a poison attack.
		/// </summary>
		public DateTime LastPoison = DateTime.Now;
		
		public ushort PoisonEffect = 0;
		public int MobID;
		protected bool CanRevive = true;
		public uint Mesh = 0;
		public uint MinAttack = 0;
		public uint MaxAttack = 0;
		public uint Defense = 0;
		public byte Dexterity = 0;
		public byte Dodge = 0;
		public int AttackRange = 1;
		public int ViewRange = 15;
		public int AttackSpeed = 500;
		public int MoveSpeed = 500;
		public int AttackType = 2;
		public Enums.MonsterBehaviour Behaviour = Enums.MonsterBehaviour.Normal;
		public int MagicType = 0;
		public int MagicDefense = 0;
		public int MagicHitRate = 0;
		public System.Collections.Concurrent.ConcurrentBag<ushort> Skills = new System.Collections.Concurrent.ConcurrentBag<ushort>();
		public ulong ExtraExperience = 100;
		public uint ExtraDamage = 0;
		public bool Boss = false;
		public uint Action = 0;
		
		// these are used to send a monster back to is original location
		// ex. after dying
		public int OriginalRange = 25;
		public ushort OriginalX = 0;
		public ushort OriginalY = 0;
		
		public DateTime DieTime = DateTime.Now;
		
		public Data.DropData DropData;
		
		public DateTime MoveTime = DateTime.Now;
		
		public DateTime AttackTime = DateTime.Now;
		
		public IEntity Target;
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
		/// Gets or sets whether the mob is alive.
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
			get
			{
				return Map != null;
			}
		}
		
		/// <summary>
		/// The name holder.
		/// </summary>
		private string _name;
		
		/// <summary>
		/// Gets or sets the name of the mob.
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
		/// Gets or sets the entity UID of the mob.
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
		private int _mp = 9999;
		
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
		private int _maxmp = 9999;
		
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
		/// Gets or sets the dynamic map for the mob.
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
		
		public Packets.SpawnPacket CreateSpawnPacket()
		{
			Packets.SpawnPacket spawn = new ProjectX_V3_Game.Packets.SpawnPacket(new Packets.StringPacker(Name, "", ""));
			spawn.Mesh = Mesh;
			spawn.EntityUID = EntityUID;
			spawn.Effect1 = _statusflag1;
			spawn.Effect2 = 0;

			spawn.MobLevel = Level;
			spawn.HP = Boss ? (ushort)((long)HP * 10000 / MaxHP) :(ushort) HP;
			spawn.Boss = Boss;
			spawn.HP = (ushort)HP;
			spawn.X = X;
			spawn.Y = Y;
			spawn.Direction = Direction;

			return spawn;
		}
		
		public Monster Copy()
		{
			Monster nmonster = this.DeepClone();
			nmonster.EntityUID = Core.UIDGenerators.GetMonsterUID();
			//do
			//{
			//	nmonster.EntityUID = (uint)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(400001, 499999);
			//}
			//while (useduids.ContainsKey(nmonster.EntityUID) && !useduids.TryAdd(nmonster.EntityUID, nmonster.EntityUID));
			
			return nmonster;
		}
		#endregion
		
		#region Methods
		#region StatusEffect1
		/// <summary>
		/// Adds a status effect.
		/// </summary>
		/// <param name="effect">The effect.</param>
		/// <param name="time">Milliseconds before remove.</param>
		public void AddStatusEffect1(Enums.Effect1 effect, int time = 0) // 0 = perm
		{
			if (!ContainsFlag1(effect))
			{
				if (!ContainsFlag1(Enums.Effect1.Dead) && !ContainsFlag1(Enums.Effect1.Ghost) || effect == Enums.Effect1.TeamLeader)
				{
					StatusFlag1 |= (ulong)effect;
					if (time > 0)
					{
						ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(
							() => { RemoveFlag1(effect); }, time);
					}
					Screen.FullUpdate();
				}
			}
		}
		
		/// <summary>
		/// Checks whether the status already contains an effect..
		/// </summary>
		/// <param name="effect">The effect</param>
		/// <returns>Returns true if the status contains the effect.</returns>
		public bool ContainsFlag1(Enums.Effect1 effect)
		{
			ulong aux = StatusFlag1;
			aux &= ~(ulong)effect;
			return !(aux == StatusFlag1);
		}
		
		/// <summary>
		/// Removes an effect from the client.
		/// </summary>
		/// <param name="effect">The effect.</param>
		public void RemoveFlag1(Enums.Effect1 effect)
		{
			if (ContainsFlag1(effect))
			{
				StatusFlag1 &= ~(ulong)effect;
				Screen.FullUpdate();
			}
		}
		#endregion
		
		public void Kill(IEntity killer, uint damage = 0)
		{
			if (killer != null)
			{
				if (killer is GameClient)
				{
					//Quests.QuestCore.SetQuestKills((killer as GameClient), MobID);
					if (DropData != null)
						DropData.Drop((killer as GameClient), new Maps.MapPoint(Map.MapID, X, Y));
					
					if (damage < 3)
						damage = 3;
					if (killer.Level > Level)
						damage = 3;
					if (Level > (killer.Level - 10))
						(killer as GameClient).AddExp((ulong)(((damage * 2) / 3) + ExtraExperience));
				}
			}
			else if (DropData != null)
				DropData.Drop(null, new Maps.MapPoint(Map.MapID, X, Y));
			StatusFlag1 = 32;
			DieTime = DateTime.Now;
			// removes it from the screen after 1 sec of dying
			// also adds it again after 10 secs
			if (CanRevive)
			{
				ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(() =>
				                                                       {
				                                                       	Screen.UpdateScreen(null);
				                                                       	ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(() =>
				                                                       	                                                       {
				                                                       	                                                       	Revive();
				                                                       	                                                       }, 10000);
				                                                       }, 1000);
			}
		}
		
		public void Revive()
		{
			StatusFlag1 = 0;
			if (Behaviour == Enums.MonsterBehaviour.PhysicalGuard ||
			    Behaviour == Enums.MonsterBehaviour.MagicGuard ||
			    Behaviour == Enums.MonsterBehaviour.DeathGuard ||
			    Behaviour == Enums.MonsterBehaviour.ReviverGuard1 ||
			    Behaviour == Enums.MonsterBehaviour.ReviverGuard2)
			{
				X = OriginalX;
				Y = OriginalY;
			}
			else
			{
				Maps.MapPoint point = Map.CreateAvailableLocation<Monster>(OriginalX, OriginalY, OriginalRange);
				X = point.X;
				Y = point.Y;
			}
			HP = MaxHP;
			MP = MaxMP;
			Alive = true;
			Direction = (byte)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(0, 8);
			Screen.UpdateScreen(null);
		}
		#endregion
		
		#region Misc
		#endregion
	}
}
