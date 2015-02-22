//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.ThreadSafe;
using System.Collections.Concurrent;
using ProjectX_V3_Lib.ScriptEngine;

namespace ProjectX_V3_Game.Core
{
	/// <summary>
	/// The kernel of the server.
	/// </summary>
	public class Kernel
	{
		/// <summary>
		/// The collection of the clients.
		/// </summary>
		public static readonly Selector<uint, string, Entities.GameClient> Clients;
		
		/// <summary>
		/// The collection of maps.
		/// </summary>
		public static Selector<ushort, string, Maps.Map> Maps;
		
		/// <summary>
		/// The collection of dynamic maps.
		/// </summary>
		public static Selector<uint, string, ProjectX_V3_Game.Maps.DynamicMap> DynamicMaps;
		
		/// <summary>
		/// The collection of npcs.
		/// </summary>
		public static ConcurrentDictionary<uint, Entities.NPC> NPCs;
		
		/// <summary>
		/// The collection of shops.
		/// </summary>
		public static ConcurrentDictionary<uint, Data.Shop> Shops;
		
		/// <summary>
		/// The collection of items.
		/// </summary>
		public static Selector<uint, string, Data.ItemInfo> ItemInfos;
		
		public static Selector<uint, string, ConcurrentDictionary<byte, Data.ItemInfo>> SortedItemInfos;
		
		/// <summary>
		/// The script engine for npcs.
		/// </summary>
		public static ScriptEngine ScriptEngine;
		
		/// <summary>
		/// The script engine for items.
		/// </summary>
		public static ScriptEngine ItemScriptEngine;
		
		/// <summary>
		/// The spell infos.
		/// </summary>
		public static MultiConcurrentDictionary<ushort, byte, Data.Spell> SpellInfos;
		
		/// <summary>
		/// The weapon spells.
		/// </summary>
		public static ConcurrentDictionary<ushort, ushort> WeaponSpells;
		
		/// <summary>
		/// The item additions.
		/// </summary>
		public static MultiConcurrentDictionary<uint, byte, Data.ItemAddition> ItemAdditions;

		/// <summary>
		/// The drop data.
		/// </summary>
		public static ConcurrentDictionary<int, Data.DropData> DropData;
		
		/// <summary>
		/// The monster data.
		/// </summary>
		public static ConcurrentDictionary<int, Entities.Monster> Monsters;
		
		public static Selector<uint, string, Data.Guild> Guilds;
		
        public static readonly DateTime UnixEpoch;
        
        public static ConcurrentDictionary<byte, ushort[]> ArcherStats;
        public static ConcurrentDictionary<byte, ushort[]> NinjaStats;
        public static ConcurrentDictionary<byte, ushort[]> MonkStats;
        public static ConcurrentDictionary<byte, ushort[]> WarriorStats;
        public static ConcurrentDictionary<byte, ushort[]> TrojanStats;
        public static ConcurrentDictionary<byte, ushort[]> TaoistStats;
        public static ConcurrentDictionary<PortalPoint, PortalPoint> Portals;
		static Kernel()
		{
			Clients = new Selector<uint, string, Entities.GameClient>();
			Maps = new Selector<ushort, string, ProjectX_V3_Game.Maps.Map>();
			DynamicMaps = new Selector<uint, string, ProjectX_V3_Game.Maps.DynamicMap>();
			NPCs = new ConcurrentDictionary<uint, Entities.NPC>();
			ItemInfos = new Selector<uint, string, ProjectX_V3_Game.Data.ItemInfo>();
			SortedItemInfos = new Selector<uint, string, ConcurrentDictionary<byte, ProjectX_V3_Game.Data.ItemInfo>>();
			Shops = new ConcurrentDictionary<uint, ProjectX_V3_Game.Data.Shop>();
			SpellInfos = new MultiConcurrentDictionary<ushort, byte, ProjectX_V3_Game.Data.Spell>();
			WeaponSpells = new ConcurrentDictionary<ushort, ushort>();
			ItemAdditions = new MultiConcurrentDictionary<uint, byte, ProjectX_V3_Game.Data.ItemAddition>();
			DropData = new ConcurrentDictionary<int, ProjectX_V3_Game.Data.DropData>();
			Monsters = new ConcurrentDictionary<int, ProjectX_V3_Game.Entities.Monster>();
			Guilds = new Selector<uint, string, ProjectX_V3_Game.Data.Guild>();
			
			ArcherStats = new ConcurrentDictionary<byte, ushort[]>();
			NinjaStats = new ConcurrentDictionary<byte, ushort[]>();
			MonkStats = new ConcurrentDictionary<byte, ushort[]>();
			WarriorStats = new ConcurrentDictionary<byte, ushort[]>();
			TrojanStats = new ConcurrentDictionary<byte, ushort[]>();
			TaoistStats = new ConcurrentDictionary<byte, ushort[]>();
			
			UnixEpoch = new DateTime(1970, 1, 1);
			
			Portals = new ConcurrentDictionary<PortalPoint, PortalPoint>();
		}
		
		public static bool GotPermission(Enums.PlayerPermission Permission)
		{
			const byte disallow = 1;
			return ((byte)Permission) <= disallow;
		}
		/// <summary>
		/// Checks if a name is banned.
		/// </summary>
		/// <param name="Name">The name to check.</param>
		/// <returns>Returns true if the name is banned.</returns>
		public static bool IsBannedName(string Name)
		{
			string name = Name.ToUpper();
			if (name.Contains("[GM]"))
				return true;
			if (name.Contains("[PM]"))
				return true;
			if (name.Contains("[VIP]"))
				return true;
			if (name.Contains("[MOD]"))
				return true;
			if (name.Contains("(GM)"))
				return true;
			if (name.Contains("(PM)"))
				return true;
			if (name.Contains("(VIP)"))
				return true;
			if (name.Contains("(MOD)"))
				return true;
			
			return false;
		}
		
		/// <summary>
		/// Gets the base class based on the different class types.
		/// </summary>
		/// <param name="_class">The class type.</param>
		/// <returns>Returns the base class.</returns>
		public static Enums.Class GetBaseClass(Enums.Class _class)
		{
			byte classid = (byte)_class;

			if (classid > 145 || classid == 0)
				return Enums.Class.Unknown;
			
			if (classid >= 100)
				return (Enums.Class)((classid / 100) * 100);
			else
				return (Enums.Class)((classid / 10) * 10);
		}
		
		/// <summary>
		/// Checks whether a class promotion is above the first promotion-
		/// </summary>
		/// <param name="_class">The class.</param>
		/// <returns>Returns true if the promotion is above the first.</returns>
		public static bool AboveFirstPromotion(Enums.Class _class)
		{
			byte classid = (byte)_class;
			if (classid > 145 || classid == 0)
				return true;
			byte promoteclassid = (byte)(((byte)GetBaseClass(_class)) + 1);
			return (classid >= promoteclassid);
		}
		
		public static uint TimeGet(Enums.TimeType type = Enums.TimeType.Millisecond)
		{
			var time = 0u;

			switch (type)
			{
				case Enums.TimeType.Second:
					{
						time = (uint)(DateTime.UtcNow - UnixEpoch).TotalSeconds;
						break;
					}
				case Enums.TimeType.Minute:
					{
						var now = DateTime.Now;
						time = (uint)(now.Year % 100 * 100000000 +
						              (now.Month + 1) * 1000000 +
						              now.Day * 10000 +
						              now.Hour * 100 +
						              now.Minute);
						break;
					}
				case Enums.TimeType.Hour:
					{
						var now = DateTime.Now;
						time = (uint)(now.Year % 100 * 1000000 +
						              (now.Month + 1) * 10000 +
						              now.Day * 100 +
						              now.Hour);
						break;
					}
				case Enums.TimeType.Day:
					{
						var now = DateTime.Now;
						time = (uint)(now.Year * 10000 +
						              now.Month * 100 +
						              now.Day);
						break;
					}
				case Enums.TimeType.DayTime:
					{
						var now = DateTime.Now;
						time = (uint)(now.Hour * 10000 +
						              now.Minute * 100 +
						              now.Second);
						break;
					}
				case Enums.TimeType.Stamp:
					{
						var now = DateTime.Now;
						time = (uint)((now.Month + 1) * 100000000 +
						              now.Day * 1000000 +
						              now.Hour * 10000 +
						              now.Minute * 100 +
						              now.Second);
						break;
					}
				default:
					{
						time = (uint)Environment.TickCount;
						break;
					}
			}

			return time;
		}
		
	}
}
