//Project by BaussHacker aka. L33TS
using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets.Location
{
	/// <summary>
	/// Description of SobSpawnPacket.
	/// </summary>
	public class SobSpawnPacket : DataPacket
	{
		public SobSpawnPacket(StringPacker strings)
			: base((ushort)(32 + strings.Size), PacketType.SobSpawnPacket)
		{
			strings.AppendAndFinish(this, 30);
		}
		public SobSpawnPacket()
			: base(32, PacketType.SobSpawnPacket)
		{
			
		}
		
		public uint EntityUID
		{
			get { return ReadUInt32(4); }
			set { WriteUInt32(value, 4); }
		}
		
		public uint MaxHealth
		{
			get { return ReadUInt32(12); }
			set { WriteUInt32(value, 12); }
		}
		
		public uint Health
		{
			get { return ReadUInt32(16); }
			set { WriteUInt32(value, 16); }
		}
		
		public ushort X
		{
			get { return ReadUInt16(20); }
			set { WriteUInt16(value, 20); }
		}
		
		public ushort Y
		{
			get { return ReadUInt16(22); }
			set { WriteUInt16(value, 22); }
		}
		
		public ushort Mesh
		{
			get { return ReadUInt16(24); }
			set { WriteUInt16(value, 24); }
		}
		
		public ushort Flag
		{
			get { return ReadUInt16(26); }
			set { WriteUInt16(value, 26); }
		}
		
		public ushort SobType
		{
			get { return ReadUInt16(28); }
			set { WriteUInt16(value, 28); }
		}
	}
}
