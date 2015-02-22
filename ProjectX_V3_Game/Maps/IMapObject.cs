//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Maps
{
	/// <summary>
	/// The base map object interface.
	/// </summary>
	public interface IMapObject
	{
		uint EntityUID { get; set; } // also ObjectUID... Bad naming, I know! xD
		
		Maps.Map Map { get; set; }
		
		ushort LastMapID { get; set; }
		ushort X { get; set; }
		ushort Y { get; set; }
		Maps.DynamicMap DynamicMap { get; set; }
		
		byte Direction { get; set; }
		
		Core.Screen Screen { get; }
		
		bool CanUpdateSpawn { get; }
		
		bool IsInMap(IMapObject MapObject);
		
		Packets.SpawnPacket CreateSpawnPacket();
	}
}
