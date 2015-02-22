//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Server -> Client
	/// </summary>
	public class GuildAttributePacket : DataPacket
	{
		public GuildAttributePacket()
			: base(92, PacketType.GuildAttributePacket)
		{
		}
		
		public uint GuildID
		{
			get { return ReadUInt32(4); }
			set { WriteUInt32(value, 4); }
		}
		
		public uint Unknown8
		{
			get { return ReadUInt32(8); }
			set { WriteUInt32(value, 8); }
		}
		
		public long Fund
		{
			get { return ReadInt64(12); }
			set { WriteInt64(value, 12); }
		}
		
		public int CPs
		{
			get { return ReadInt32(20); }
			set { WriteInt32(value, 20); }
		}
		
		public int Amount
		{
			get { return ReadInt32(24); }
			set { WriteInt32(value, 24); }
		}
		
		public Enums.GuildRank Rank
		{
			get { return (Enums.GuildRank)ReadUInt16(28); }
			set { WriteUInt16((ushort)value, 28); }
		}
		
		public string GuildLeader
		{
			get { return ReadString(32, 16); }
			set { WriteString(value, 32); }
		}
		
		public int RequiredLevel
		{
			get { return ReadInt32(48); }
			set { WriteInt32(value, 48); }
		}
		
		public int RequiredMetempsychosis
		{
			get { return ReadInt32(52); }
			set { WriteInt32(value, 52); }
		}
		
		public int RequiredProfession
		{
			get { return ReadInt32(56); }
			set { WriteInt32(value, 56); }
		}
		
		public byte Level
		{
			get { return ReadByte(60); }
			set { WriteByte(value, 60); }
		}
		
		public ushort Unknown61
		{
			get { return ReadUInt16(61); }
			set { WriteUInt16(value, 61); }
		}
		
		public uint Unknown63
		{
			get { return ReadUInt32(63); }
			set { WriteUInt32(value, 63); }
		}
		
		public uint EnrollmentDate
		{
			get { return ReadUInt32(67); }
			set { WriteUInt32(value, 67); }
		}
		
		public byte Unknown71
		{
			get { return ReadByte(71); }
			set { WriteByte(value, 71); }
		}
		
		public uint Unknown72
		{
			get { return ReadUInt32(72); }
			set { WriteUInt32(value, 72); }
		}
		
		public uint Unknown76
		{
			get { return ReadUInt32(77); }
			set { WriteUInt32(value, 77); }
		}
		
		public uint Unknown80
		{
			get { return ReadUInt32(80); }
			set { WriteUInt32(value, 80); }
		}
		
		public uint Unknown84
		{
			get { return ReadUInt32(84); }
			set { WriteUInt32(value, 84); }
		}
		
		public uint Unknown88
		{
			get { return ReadUInt32(88); }
			set { WriteUInt32(value, 88); }
		}
	}
}
