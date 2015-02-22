//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Server -> Client
	/// </summary>
	public class SpawnPacket : DataPacket
	{
		public SpawnPacket(StringPacker strings)
			: base((ushort)(218 + strings.Size), PacketType.SpawnPacket)
		{
			strings.AppendAndFinish(this, 218);
		}
		
		public uint Mesh
		{
			get { return ReadUInt32(4); }
			set { WriteUInt32(value, 4); }
		}

		public uint EntityUID
		{
			get { return ReadUInt32(8); }
			set { WriteUInt32(value, 8); }
		}

		public uint GuildID
		{
			get { return ReadUInt32(12); }
			set { WriteUInt32(value, 12); }
		}

		public Enums.GuildRank GuildRank
		{
			get { return (Enums.GuildRank)ReadUInt16(16); }
			set { WriteUInt16((ushort)value, 16); }
		}

		public ulong Effect1
		{
			get { return ReadUInt64(22); }
			set { WriteUInt64(value, 22); }
		}

		public ulong Effect2
		{
			get { return ReadUInt64(30); }
			set { WriteUInt64(value, 30); }
		}

		public uint HelmetID
		{
			get { return ReadUInt32(40); }
			set { WriteUInt32(value, 40); }
		}

		public uint GarmentID
		{
			get { return ReadUInt32(44); }
			set { WriteUInt32(value, 44); }
		}

		public uint ArmorID
		{
			get { return ReadUInt32(48); }
			set { WriteUInt32(value, 48); }
		}

		public uint LeftHandID
		{
			get { return ReadUInt32(52); }
			set { WriteUInt32(value, 52); }
		}

		public uint RightHandID
		{
			get { return ReadUInt32(56); }
			set { WriteUInt32(value, 56); }
		}

		public uint AccessoryRightID
		{
			get { return ReadUInt32(60); }
			set { WriteUInt32(value, 60); }
		}

		public uint AccessoryLeftID
		{
			get { return ReadUInt32(64); }
			set { WriteUInt32(value, 64); }
		}

		public uint SteedID
		{
			get { return ReadUInt32(68); }
			set { WriteUInt32(value, 68); }
		}

		/*public uint Uknown72
		{
			get { return ReadUInt32(72); }
			set { WriteUInt32(value, 72); }
		}*/

		public uint SteedArmor
		{
			get { return ReadUInt32(72); }
			set { WriteUInt32(value, 72); }
		}

		public ushort HP
		{
			get { return ReadUInt16(80); }
			set { WriteUInt16(value, 80); }
		}

		public ushort MobLevel
		{
			get { return ReadUInt16(82); }
			set { WriteUInt16(value, 82); }
		}
		
		public ushort HairStyle
		{
			get { return ReadUInt16(84); }
			set { WriteUInt16(value, 84); }
		}

		public ushort X
		{
			get { return ReadUInt16(86); }
			set { WriteUInt16(value, 86); }
		}

		public ushort Y
		{
			get { return ReadUInt16(88); }
			set { WriteUInt16(value, 88); }
		}

		public byte Direction
		{
			get { return ReadByte(90); }
			set { WriteByte(value, 90); }
		}

		public Enums.ActionType Action
		{
			get { return (Enums.ActionType)ReadByte(91); }
			set { WriteByte((byte)value, 91); }
		}

		public ushort Unknown92
		{
			get { return ReadUInt16(92); }
			set { WriteUInt16(value, 92); }
		}

		public uint Uknown94
		{
			get { return ReadUInt32(94); }
			set { WriteUInt32(value, 94); }
		}

		public byte Reborns
		{
			get { return ReadByte(98); }
			set { WriteByte(value, 98); }
		}

		public ushort Level
		{
			get { return ReadUInt16(99); }
			set { WriteUInt16(value, 99); }
		}

		public uint Away
		{
			get { return ReadUInt32(102); }
			set { WriteUInt32(value, 102); }
		}

		public Enums.NobilityRank NobilityRank
		{
			get { return (Enums.NobilityRank)ReadByte(119); }
			set { WriteByte((byte)value, 119); }
		}

		public ushort ArmorColor
		{
			get { return ReadUInt16(123); }
			set { WriteUInt16(value, 123); }
		}

		public ushort ShieldColor
		{
			get { return ReadUInt16(125); }
			set { WriteUInt16(value, 125); }
		}

		public ushort HelmetColor
		{
			get { return ReadUInt16(127); }
			set { WriteUInt16(value, 127); }
		}

		public uint QuizPoints
		{
			get { return ReadUInt32(129); }
			set { WriteUInt32(value, 129); }
		}

		public byte MountPlus
		{
			get { return ReadByte(133); }
			set { WriteByte(value, 133); }
		}

		public uint MountColor
		{
			get { return ReadUInt32(139); }
			set { WriteUInt32(value, 139); }
		}

		public Enums.PlayerTitle PlayerTitle
		{
			get { return (Enums.PlayerTitle)ReadByte(167); }
			set { WriteByte((byte)value, 167); }
		}

		public bool Boss
		{
			get { return ReadBool(181); }
			set { WriteBool(value, 181); }
		}

		public uint HelmetArtifactID
		{
			get { return ReadUInt32(182); }
			set { WriteUInt32(value, 182); }
		}

		public uint ArmorArtifactID
		{
			get { return ReadUInt32(186); }
			set { WriteUInt32(value, 186); }
		}

		public uint WeaponRightArtifactID
		{
			get { return ReadUInt32(190); }
			set { WriteUInt32(value, 190); }
		}

		public uint WeaponLeftArtifactID
		{
			get { return ReadUInt32(194); }
			set { WriteUInt32(value, 194); }
		}

		public Enums.Class Job
		{
			get { return (Enums.Class)ReadByte(210); }
			set { WriteByte((byte)value, 210); }
		}
	}
}
