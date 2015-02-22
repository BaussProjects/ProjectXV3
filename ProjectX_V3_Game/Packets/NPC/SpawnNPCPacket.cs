//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Server -> Client
	/// </summary>
	public class SpawnNPCPacket : DataPacket
	{
		public SpawnNPCPacket(StringPacker namestring)
			: base((ushort)(24 + namestring.Size), PacketType.SpawnNPCPacket)
		{
			namestring.AppendAndFinish(this, 22);
		}
		
		public uint EntityUID
		{
			get { return ReadUInt32(4); }
			set { WriteUInt32(value, 4); }
		}
		
		public uint NPCID
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
		
		public ushort Mesh
		{
			get { return ReadUInt16(16); }
			set { WriteUInt16(value, 16); }
		}
		
		public uint Flag
		{
			get { return ReadUInt32(18); }
			set { WriteUInt32(value, 18); }
		}
	}
}
