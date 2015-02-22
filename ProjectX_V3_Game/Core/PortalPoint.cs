//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Core
{
	/// <summary>
	/// Description of PortalPoint.
	/// </summary>
	public struct PortalPoint
	{
		public ushort MapID;
		public ushort X;
		public ushort Y;
		public PortalPoint(ushort map, ushort x, ushort y)
		{
			MapID = map;
			X = x;
			Y = y;
		}
	}
}
