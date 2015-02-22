//Project by BaussHacker aka. L33TS

using System;
using DMapLoader;
using System.Collections.Concurrent;

namespace ProjectX_V3_Game.Maps
{
	/// <summary>
	/// The core handler of dmaps.
	/// </summary>
	public class DMapHandler
	{
		/// <summary>
		/// The dmap server.
		/// </summary>
		private static DMapServer DmapHandler = new DMapServer();
		
		/// <summary>
		/// The collection of dmaps.
		/// </summary>
		public static ConcurrentDictionary<ushort, DMap> DMaps;
		
		/// <summary>
		/// Sets the dmap server + fills the dmap collection.
		/// </summary>
		/// <param name="dmaps">The dmapserver.</param>
		/// <returns>Returns true if the dmap handler was set and the dmap collection was filled correct.</returns>
		public static bool SetHandler(DMapServer dmaps)
		{
			if (!dmaps.Loaded)
			{
				while (!dmaps.Loaded)
					System.Threading.Thread.Sleep(100);
			}
			DmapHandler = dmaps;
			DMaps = new ConcurrentDictionary<ushort, DMap>();
			foreach (int key in DmapHandler.Maps.Keys)
			{
				if (!DMaps.TryAdd((ushort)key, DmapHandler.Maps[key]))
				{
					Console.WriteLine("Failed to add {0} to the dmaps.", key);
					return false;
				}
			}
			return true;
		}
		
		/// <summary>
		/// Checks if a coordinate is valid.
		/// </summary>
		/// <param name="mapid">The mapid to check.</param>
		/// <param name="x">The x coordinate to check.</param>
		/// <param name="y">The y coordinate to check.</param>
		/// <returns>Returns true if the coordinate is valid.</returns>
		public static bool ValidCoord(ushort mapid, ushort x, ushort y)
		{
			if (DMaps == null)
				return true;
			if (!DMaps.ContainsKey(mapid))
				return true;
			
			try
			{
				return DMaps[mapid].Check((int)x, (int)y);
			}
			catch {  return false; }
		}
		
		/// <summary>
		/// Checks if a coordinate is valid.
		/// </summary>
		/// <param name="mapid">The mapid to check.</param>
		/// <param name="x">The x coordinate to check.</param>
		/// <param name="y">The y coordinate to check.</param>
		/// <returns>Returns true if the coordinate is valid.</returns>
		public static bool ValidCoord(int mapid, ushort x, ushort y)
		{
			if (DMaps == null)
				return true;
			if (!DMaps.ContainsKey((ushort)mapid))
				return true;
			
			try
			{
				return DMaps[(ushort)mapid].Check((int)x, (int)y);
			}
			catch {  return false; }
		}
		
		/// <summary>
		/// Checks if a coordinate is valid.
		/// </summary>
		/// <param name="mapid">The mapid to check.</param>
		/// <param name="x">The x coordinate to check.</param>
		/// <param name="y">The y coordinate to check.</param>
		/// <returns>Returns true if the coordinate is valid.</returns>
		public static bool ValidCoord(int mapid, int x, int y)
		{
			if (DMaps == null)
				return true;
			if (!DMaps.ContainsKey((ushort)mapid))
				return true;
			
			try
			{
				return DMaps[(ushort)mapid].Check(x, y);
			}
			catch { return false; }
		}
		
		/// <summary>
		/// Checks if a coordinate is valid.
		/// </summary>
		/// <param name="mapid">The mapid to check.</param>
		/// <param name="x">The x coordinate to check.</param>
		/// <param name="y">The y coordinate to check.</param>
		/// <returns>Returns true if the coordinate is valid.</returns>
		public static bool ValidCoord(ushort mapid, int x, int y)
		{
			if (DMaps == null)
				return true;
			if (!DMaps.ContainsKey(mapid))
				return true;
			
			try
			{
				return DMaps[mapid].Check(x, y);
			}
			catch { return false; }
		}
	}
}
