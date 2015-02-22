//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Description of ArenaMatchPacket.
	/// </summary>
	public class ArenaMatchPacket : DataPacket
	{
		public ArenaMatchPacket()
			: base(56, PacketType.ArenaMatchPacket)
		{
			
		}
		
		public uint Player1EntityUID
		{
			get { return ReadUInt32(4); }
			set { WriteUInt32(value, 4); }
		}
		
		public string Player1Name
		{
			get { return ReadString(8, 16); }
			set { WriteString(value, 8); }
		}
		
		public int Player1Damage
		{
			get { return ReadInt32(24); }
			set { WriteInt32(value, 24); }
		}
		
		public uint Player2EntityUID
		{
			get { return ReadUInt32(28); }
			set { WriteUInt32(value, 28); }
		}
		
		public string Player2Name
		{
			get { return ReadString(32, 16); }
			set { WriteString(value, 32); }
		}
		
		public int Player2Damage
		{
			get { return ReadInt32(48); }
			set { WriteInt32(value, 48); }
		}
	}
}
