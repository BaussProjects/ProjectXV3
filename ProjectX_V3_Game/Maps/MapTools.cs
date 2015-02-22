//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Maps
{
	/// <summary>
	/// Different tools to help with map handling. TODO: Rewrite this shit to be more dynamic (eg. using the created folders in the database)
	/// </summary>
	public class MapTools
	{
		/// <summary>
		/// Gets the mappoint for the startmap.
		/// </summary>
		/// <param name="mapid">The mapid.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="lastmap">The last mapid.</param>
		/// <returns>Returns the mappoint to start at.</returns>
		public static MapPoint GetStartMap(ushort mapid, ushort x, ushort y, ushort lastmap)
		{
			MapPoint defmap = GetMapPoint(lastmap);
			if (!Core.Kernel.Maps.Contains(mapid))
				return defmap;
			
			switch (mapid)
			{
				case 6000:
					return new MapPoint(6000, 55, 55);
				case 6001:
					return new MapPoint(6001, 55, 55);
				case 1036:
					return defmap;
					
				default:
					return new MapPoint(mapid, x, y);
			}
		}
		
		/// <summary>
		/// Gets a default mappoint based on a mapid.
		/// </summary>
		/// <param name="mapid">The mapid.</param>
		/// <returns>Returns the mappoint.</returns>
		public static MapPoint GetMapPoint(ushort mapid)
		{
			if (Core.Kernel.Maps.Contains(mapid))
			{
				if (Core.Kernel.Maps[mapid].RevivePoint != null)
					return Core.Kernel.Maps[mapid].RevivePoint;
			}
			return new MapPoint(1002, 400, 400);
		}
	}
}
