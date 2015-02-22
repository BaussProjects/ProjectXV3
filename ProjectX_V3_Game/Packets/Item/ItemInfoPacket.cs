//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Server -> Client
	/// </summary>
	public class ItemInfoPacket : DataPacket
	{
		public ItemInfoPacket()
			: base(68, PacketType.ItemInfoPacket)
		{
		}
		
		public uint UID
		{
			get { return ReadUInt32(4); }
			set { WriteUInt32(value, 4); }
		}
		
		public uint ItemID
		{
			get { return ReadUInt32(8); }
			set { WriteUInt32(value, 8); }
		}
		
		public ushort CurrentDura
		{
			get { return ReadUInt16(12); }
			set { WriteUInt16(value, 12); }
		}
		
		public ushort MaxDura
		{
			get { return ReadUInt16(14); }
			set { WriteUInt16(value, 14); }
		}
		
		public ushort Data
		{
			get { return ReadUInt16(16); }
			set { WriteUInt16(value, 16); }
		}
		
		public Enums.ItemLocation Location
		{
			get { return (Enums.ItemLocation)ReadUInt16(18); }
			set { WriteUInt16((ushort)value, 18); }
		}
		
		public uint SocketAndRGB
		{
			get { return ReadUInt32(20); }
			set { WriteUInt32(value, 20); }
		}
		
		public Enums.SocketGem Gem1
		{
			get { return (Enums.SocketGem)ReadByte(24); }
			set { WriteByte((byte)value, 24); }
		}
		
		public Enums.SocketGem Gem2
		{
			get { return (Enums.SocketGem)ReadByte(25); }
			set { WriteByte((byte)value, 25); }
		}
		
		public ushort Unknown2
		{
			get { return ReadUInt16(26); }
			set { WriteUInt16(value, 26); }
		}
		
		public ushort RebornEffect
		{
			get { return ReadUInt16(28); }
			set { WriteUInt16(value, 28); }
		}
		
		public byte Plus
		{
			get { return ReadByte(33); }
			set { WriteByte(value, 33); }
		}
		
		public byte Bless
		{
			get { return ReadByte(34); }
			set { WriteByte(value, 34); }
		}
		
		public bool Free
		{
			get { return ReadBool(35); }
			set { WriteBool(value, 35); }
		}
		
		public byte Enchant
		{
			get { return ReadByte(36); }
			set { WriteByte(value, 36); }
		}
		
		public uint GreenText
		{
			get { return ReadUInt32(40); }
			set { WriteUInt32(value, 40); }
		}
		
		public bool Suspicious
		{
			get { return ReadBool(44); }
			set { WriteBool(value, 44); }
		}
		
		public bool Locked
		{
			get { return ReadBool(46); }
			set { WriteBool(value, 46); }
		}
		
		public byte Color
		{
			get { return ReadByte(48); }
			set { WriteByte(value, 48); }
		}
		
		public uint Composition
		{
			get { return ReadUInt32(52); }
			set { WriteUInt32(value, 52); }
		}
		
		public uint INS
		{
			get { return ReadUInt32(56); }
			set { WriteUInt32(value, 56); }
		}
		
		public uint LockedTime
		{
			get { return ReadUInt32(60); }
			set { WriteUInt32(value, 60); }
		}
		
		public ushort Amount
		{
			get { return ReadUInt16(64); }
			set { WriteUInt16(value, 64); }
		}
	}
}
