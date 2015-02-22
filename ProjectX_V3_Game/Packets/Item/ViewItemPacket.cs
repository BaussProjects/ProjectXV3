//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Server -> Client
	/// </summary>
	public class ViewItemPacket : DataPacket
	{
		public ViewItemPacket()
			: base(84, PacketType.ViewItemPacket)
		{
		}
		
		public uint ItemUID
		{
			get { return ReadUInt32(4); }
			set { WriteUInt32(value, 4); }
		}
		
		public uint TargetUID
		{
			get { return ReadUInt32(8); }
			set { WriteUInt32(value, 8); }
		}
		
		public uint Price
		{
			get { return ReadUInt32(12); }
			set { WriteUInt32(value, 12); }
		}
		
		public uint ItemID
		{
			get { return ReadUInt32(16); }
			set { WriteUInt32(value, 16); }
		}
		
		public ushort CurrentDura
		{
			get { return ReadUInt16(20); }
			set { WriteUInt16(value, 20); }
		}
		
		public ushort MaxDura
		{
			get { return ReadUInt16(22); }
			set { WriteUInt16(value, 22); }
		}
		
		public ushort ViewType
		{
			get { return ReadUInt16(24); }
			set { WriteUInt16(value, 24); }
		}
		
		public Enums.ItemLocation Location
		{
			get { return (Enums.ItemLocation)ReadUInt16(26); }
			set { WriteUInt16((ushort)value, 26); }
		}
		
		public uint Unknown2
		{
			get { return ReadUInt16(28); }
			set { WriteUInt32(value, 28); }
		}
		
		public Enums.SocketGem Gem1
		{
			get { return (Enums.SocketGem)ReadByte(32); }
			set { WriteByte((byte)value, 32); }
		}
		
		public Enums.SocketGem Gem2
		{
			get { return (Enums.SocketGem)ReadByte(33); }
			set { WriteByte((byte)value, 33); }
		}
		
		public uint Unknown3
		{
			get { return ReadUInt32(34); }
			set { WriteUInt32(value, 34); }
		}
		
		public ushort Unknown4
		{
			get { return ReadUInt16(38); }
			set { WriteUInt16(value, 38); }
		}
		
		public byte Unknown5
		{
			get { return ReadByte(40); }
			set { WriteByte(value, 40); }
		}
		
		public byte Plus
		{
			get { return ReadByte(41); }
			set { WriteByte(value, 41); }
		}
		
		public byte Bless
		{
			get { return ReadByte(42); }
			set { WriteByte(value, 42); }
		}
		
		public bool Free
		{
			get { return ReadBool(43); }
			set { WriteBool(value, 43); }
		}
		
		public byte Enchant
		{
			get { return ReadByte(44); }
			set { WriteByte(value, 44); }
		}
		
		public bool Suspicious
		{
			get { return ReadBool(53); }
			set { WriteBool(value, 53); }
		}
		
		public byte Color
		{
			get { return ReadByte(56); }
			set { WriteByte(value, 56); }
		}
		
		public uint Composition
		{
			get { return ReadUInt32(60); }
			set { WriteUInt32(value, 60); }
		}
	}
}
