//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// [[ same type as GroundItemPacket ]]
	/// </summary>
	public class MapEffectPacket : DataPacket
	{
		public MapEffectPacket()
			: base(32, PacketType.GroundItemPacket)
		{
			WriteUInt16(0xd, 18);
		}
		public bool Shake = false;
		public bool Zoom = false;
		public bool Darkness = false;
		
		public void AppendFlags()
		{
			WriteInt32((Shake ? 1 : 0) | (Zoom ? 2 : 0) | (Darkness ? 4 : 0), 4);
		}
		
		public uint EntityUID
		{
			get { return ReadUInt32(8); }
			set { WriteUInt32(value, 8); }
		}
		
		public ushort X
		{
			get { return ReadUInt16(12); }
			set { WriteUInt16(value, 12); }
		}
		
		public ushort Y
		{
			get { return ReadUInt16(14); }
			set { WriteUInt16(value, 14); }
		}
	}
}
