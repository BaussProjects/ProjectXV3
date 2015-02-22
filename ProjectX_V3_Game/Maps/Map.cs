//Project by BaussHacker aka. L33TS

using System;
using DMapLoader;
using ProjectX_V3_Lib.ThreadSafe;
using System.Linq;

namespace ProjectX_V3_Game.Maps
{
	/// <summary>
	/// A map.
	/// </summary>
	[Serializable()]
	public class Map
	{
		/// <summary>
		/// The map id holder.
		/// </summary>
		private ushort mapid;
		
		/// <summary>
		/// Gets the map id.
		/// </summary>
		public ushort MapID
		{
			get { return mapid; }
		}
		
		/// <summary>
		/// The map name.
		/// </summary>
		private string name;
		
		/// <summary>
		/// Gets the map name. [If no name is in database then it returns the id.]
		/// </summary>
		public string Name
		{
			get { return name; }
		}
		
		public bool ShowThunder = false;
		public Enums.Weather Weather;
		public Enums.WeatherIntensity WeatherIntensity;
		public Enums.WeatherAppearance WeatherAppearance;
		public Enums.ConquerAngle WeatherDirection;
		
		/// <summary>
		/// The map flags.
		/// </summary>
		public readonly System.Collections.Concurrent.ConcurrentDictionary<ulong, Enums.MapTypeFlags> Flags;
		
		/// <summary>
		/// The map objects.
		/// </summary>
		public readonly System.Collections.Concurrent.ConcurrentDictionary<uint, Maps.IMapObject> MapObjects;
		
		/// <summary>
		/// The map objects.
		/// </summary>
		public readonly System.Collections.Concurrent.ConcurrentDictionary<uint, Maps.IMapObject> Items;
		
		public ushort DMapInfo;
		
		public ushort InheritanceMap;
		
		public int GetClientCount()
		{
			int counter = 0;
			foreach (Maps.IMapObject MapObject in MapObjects.Values)
			{
				if (MapObject is Entities.GameClient)
					counter += 1;
			}
			return counter;
		}
		
		/// <summary>
		/// Gets the dmap associated with the map.
		/// </summary>
		public DMap DMap
		{
			get
			{
				if (DMapInfo == 0)
					return null;
				
				if (!DMapHandler.DMaps.ContainsKey(DMapInfo))
					return null;
				
				return DMapHandler.DMaps[DMapInfo];
			}
		}

		/// <summary>
		/// The point for revive spot.
		/// </summary>
		public MapPoint RevivePoint;
		
		/// <summary>
		/// The maptype.
		/// </summary>
		public Enums.MapType MapType;
		
		public bool GotKillCons()
		{
			return (MapType != Enums.MapType.FreePK && MapType != Enums.MapType.Tournament
			        && MapType != Enums.MapType.ClanWars && MapType != Enums.MapType.GuildWars);
		}
		
		/// <summary>
		/// Creates a new instance of Map.
		/// </summary>
		/// <param name="mapid">The map id.</param>
		/// <param name="name">The map name.</param>
		public Map(ushort mapid, string name)
		{
			this.mapid = mapid;
			Flags = new System.Collections.Concurrent.ConcurrentDictionary<ulong, ProjectX_V3_Game.Enums.MapTypeFlags>();
			MapObjects = new System.Collections.Concurrent.ConcurrentDictionary<uint, Maps.IMapObject>();
			Items = new System.Collections.Concurrent.ConcurrentDictionary<uint, IMapObject>();
			
			if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
				this.name = this.mapid.ToString();
			else
				this.name = name;
		}
		
		/// <summary>
		/// Checks if the map contains a client by a character name. [Used at login.]
		/// </summary>
		/// <param name="playername">The name.</param>
		/// <param name="entityuid">The entity uid.</param>
		/// <returns>Returns true if the map contains the client.</returns>
		public bool ContainsClientByName(string playername, out uint entityuid)
		{
			entityuid = 0;
			foreach (Maps.IMapObject MapObject in MapObjects.Values)
			{
				if (MapObject is Entities.GameClient)
				{
					if ((MapObject as Entities.GameClient).Name == playername)
					{
						entityuid = MapObject.EntityUID;
						return true;
					}
				}
			}
			return false;
		}
		
		/// <summary>
		/// Checks if a coordinate is valid. [Calls the dmap]
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <returns>Returns true if the coodinate is valid.</returns>
		public bool ValidCoord(ushort x, ushort y)
		{
			return DMapHandler.ValidCoord(mapid, x, y);
		}
		/// <summary>
		/// Checks if a coordinate is valid. [Calls the dmap]
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <returns>Returns true if the coodinate is valid.</returns>
		public bool ValidCoord(int x, int y)
		{
			return DMapHandler.ValidCoord(mapid, x, y);
		}
		
		/// <summary>
		/// Gets a valid move coord based.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <returns>Returns true if the coordinate is valid.</returns>
		public bool ValidMoveCoord<T>(ushort x, ushort y)
		{
			return ValidCoord(x, y) && SearchByLocation<T>(x, y) == null;
		}
		/// <summary>
		/// Tries to let a mapobject enter the map.
		/// </summary>
		/// <param name="MapObject">The map object.</param>
		/// <returns>Returns true if the object was added.</returns>
		public bool EnterMap(Maps.IMapObject MapObject)
		{
			if (MapObject is Data.GroundItem)
			{
				if (Items.ContainsKey(MapObject.EntityUID))
				{
					return false;
				}
				
				Items.TryAdd(MapObject.EntityUID, MapObject);
			}
			else
			{
				if (MapObjects.ContainsKey(MapObject.EntityUID))
				{
					if (MapObject is Entities.GameClient)
						(MapObject as Entities.GameClient).NetworkClient.Disconnect("Multiple map locations.");
					return false;
				}
				
				if (MapObject is Entities.GameClient)
				{
					if (!MapObjects.TryAdd(MapObject.EntityUID, MapObject))
					{
						if (MapObject is Entities.GameClient)
							(MapObject as Entities.GameClient).NetworkClient.Disconnect("Failed to add the client to the map.");
						return false;
					}
				}
				else if (!MapObjects.TryAdd(MapObject.EntityUID, MapObject))
					return false;
			}
			
			MapObject.Map = this;
			return true;
		}
		
		/// <summary>
		/// Tries to make a mapobject leave the map.
		/// </summary>
		/// <param name="MapObject">The map object.</param>
		/// <returns>Returns true if the map object left.</returns>
		public bool LeaveMap(Maps.IMapObject MapObject)
		{
			if (MapObject is Data.GroundItem)
			{
				if (!Items.ContainsKey(MapObject.EntityUID))
				{
					return false;
				}
				
				Maps.IMapObject rObject;
				return Items.TryRemove(MapObject.EntityUID, out rObject);
			}
			else
			{
				MapObject.LastMapID = this.MapID;
				
				if (!MapObjects.ContainsKey(MapObject.EntityUID))
				{
					if (MapObject is Entities.GameClient)
						(MapObject as Entities.GameClient).NetworkClient.Disconnect("Not in the map.");
					return false;
				}
				
				Maps.IMapObject rObject;
				if (!MapObjects.TryRemove(MapObject.EntityUID, out rObject))
				{
					if (MapObject is Entities.GameClient)
						(MapObject as Entities.GameClient).NetworkClient.Disconnect("Could not be removed.");
					return false;
				}
			}
			return true;
		}
		
		/// <summary>
		/// Creates a dynamic map inheriting this map.
		/// </summary>
		/// <param name="dynamicid">[out] The dynamic id.</param>
		/// <returns>Returns the dynamic map.</returns>
		public DynamicMap CreateDynamic(out uint dynamicid)
		{
			dynamicid = Core.UIDGenerators.GetDynamicMapUID();
			DynamicMap dyn = new DynamicMap(dynamicid, this);
			Core.Kernel.DynamicMaps.TryAdd(dyn.DynamicID, dyn.DynamicID.ToString(), dyn);
			return dyn;
		}
		
		/// <summary>
		/// Creates an available location based on a type located in the map.
		/// </summary>
		/// <param name="xmid">The x mid.</param>
		/// <param name="ymid">The y mid.</param>
		/// <param name="range">The range to create the location within.</param>
		/// <returns>Returns the available location. [null if no locations available]</returns>
		public MapPoint CreateAvailableLocation<T>(ushort xmid, ushort ymid, int range)
		{
			if (range <= 1)
				return new MapPoint(MapID, xmid, ymid);
			System.Collections.Generic.List<MapPoint> available = new System.Collections.Generic.List<MapPoint>();
			if (typeof(T) == typeof(Data.GroundItem))
			{
				for (ushort x = (ushort)(xmid - range); x < (ushort)(xmid + range); x++)
				{
					for (ushort y = (ushort)(ymid - range); y < (ushort)(ymid + range); y++)
					{
						T obj = SearchByLocation<T>(x, y);
						if (obj == null)
						{
							if (ValidCoord(x, y))
								available.Add(new MapPoint(MapID, x, y));
						}
					}
				}
			}
			else
			{
				System.Collections.Generic.Dictionary<System.Drawing.Point, Maps.MapPoint> available2 = new System.Collections.Generic.Dictionary<System.Drawing.Point, Maps.MapPoint>();
				foreach (var obj in MapObjects.Values)
				{
					System.Drawing.Point p = new System.Drawing.Point(obj.X, obj.Y);
					if (!available2.ContainsKey(p))
						available2.Add(p, new MapPoint(this.MapID, obj.X, obj.Y));
				}
				for (ushort x = (ushort)(xmid - range); x < (ushort)(xmid + range); x++)
				{
					for (ushort y = (ushort)(ymid - range); y < (ushort)(ymid + range); y++)
					{
						System.Drawing.Point p = new System.Drawing.Point(x, y);
						if (!available2.ContainsKey(p) && ValidCoord(x, y))
							available.Add(new MapPoint(mapid, x, y));
					}
				}
				available2.Clear();
			}
			
			if (available.Count > 0)
			{
				MapPoint[] mappoints = available.ToArray();
				return mappoints[ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(mappoints.Length - 1)];
			}
			else
				return null;
		}
		
		/// <summary>
		/// Searches for a type within the map using a location.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <returns>Returns the found object.</returns>
		public T SearchByLocation<T>(ushort x, ushort y)
		{
			if (typeof(T) == typeof(Data.GroundItem))
			{
				foreach (var obj in Items.Values)
				{
					if (obj is T)
					{
						if (obj.X == x && obj.Y == y)
							return (T)obj;
					}
				}
			}
			else
			{
				foreach (var obj in MapObjects.Values)
				{
					if (obj is T)
					{
						if (obj.X == x && obj.Y == y)
							return (T)obj;
					}
				}
			}
			return default(T);
		}
	}
}
