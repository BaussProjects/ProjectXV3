//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Server -> Client
	/// </summary>
	public class SendSpellPacket : DataPacket
	{
		public SendSpellPacket()
			: base(20, PacketType.SendSpellPacket)
		{
		}
		
		public uint Experience
		{
			get { return ReadUInt32(4); }
			set { WriteUInt32(value, 4); }
		}
		
		public ushort ID
		{
			get { return ReadUInt16(8); }
			set { WriteUInt16(value, 8); }
		}
		
		public ushort Level
		{
			get { return ReadUInt16(10); }
			set { WriteUInt16(value, 10); }
		}
	}
}
