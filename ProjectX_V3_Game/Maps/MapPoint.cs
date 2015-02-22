//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Maps
{
	/// <summary>
	/// A specific map location.
	/// </summary>
	public class MapPoint
	{
		/// <summary>
		/// The map id.
		/// </summary>
		public ushort MapID;
		
		/// <summary>
		/// Gets the map based on is map id.
		/// </summary>
		public Map Map
		{
			get { return Core.Kernel.Maps[MapID]; }
		}
		
		/// <summary>
		/// The x coordinate.
		/// </summary>
		public ushort X;
		
		/// <summary>
		/// The y coordinate.
		/// </summary>
		public ushort Y;
		
		/// <summary>
		/// Creates a new instance of MapPoint.
		/// </summary>
		/// <param name="mapid">The map id.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public MapPoint(ushort mapid, ushort x, ushort y)
		{
			this.MapID = mapid;
			this.X = x;
			this.Y = y;
		}
	}
}
