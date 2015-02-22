//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Server -> Client
	/// </summary>
	public class CharacterInfoPacket : DataPacket
	{
		public CharacterInfoPacket(StringPacker Names)
			: base((ushort)(112 + Names.Size), PacketType.CharacterInfoPacket)
		{
			Names.AppendAndFinish(this, 110);
		}
		
		/// <summary>
		/// Gets or sets the EntityUID.
		/// </summary>
		public uint EntityUID
		{
			get { return ReadUInt32(4); }
			set { WriteUInt32(value, 4); }
		}
		
		/// <summary>
		/// Gets or sets the Mesh.
		/// </summary>
		public uint Mesh
		{
			get { return ReadUInt32(10); }
			set { WriteUInt32(value, 10); }
		}
		
		/// <summary>
		/// Gets or sets the HairStyle.
		/// </summary>
		public ushort HairStyle
		{
			get { return ReadUInt16(14); }
			set { WriteUInt16(value, 14); }
		}
		
		/// <summary>
		/// Gets or sets the money.
		/// </summary>
		public uint Money
		{
			get { return ReadUInt32(16); }
			set { WriteUInt32(value, 16); }
		}
		
		/// <summary>
		/// Gets or sets the cps.
		/// </summary>
		public uint CPs
		{
			get { return ReadUInt32(20); }
			set { WriteUInt32(value, 20); }
		}
		
		/// <summary>
		/// Gets or sets the experience.
		/// </summary>
		public ulong Experience
		{
			get { return ReadUInt64(24); }
			set { WriteUInt64(value, 24); }
		}
		
		/// <summary>
		/// Gets or sets the strength.
		/// </summary>
		public ushort Strength
		{
			get { return ReadUInt16(52); }
			set { WriteUInt16(value, 52); }
		}
		
		/// <summary>
		/// Gets or sets the agility.
		/// </summary>
		public ushort Agility
		{
			get { return ReadUInt16(54); }
			set { WriteUInt16(value, 54); }
		}
		
		/// <summary>
		/// Gets or sets the vitality.
		/// </summary>
		public ushort Vitality
		{
			get { return ReadUInt16(56); }
			set { WriteUInt16(value, 56); }
		}
		
		/// <summary>
		/// Gets or sets the spirit.
		/// </summary>
		public ushort Spirit
		{
			get { return ReadUInt16(58); }
			set { WriteUInt16(value, 58); }
		}
		
		/// <summary>
		/// Gets or sets the attributepoints.
		/// </summary>
		public ushort AttributePoints
		{
			get { return ReadUInt16(60); }
			set { WriteUInt16(value, 60); }
		}
		
		/// <summary>
		/// Gets or sets the hp.
		/// </summary>
		public ushort HP
		{
			get { return ReadUInt16(62); }
			set { WriteUInt16(value, 62); }
		}
		
		/// <summary>
		/// Gets or sets the mp.
		/// </summary>
		public ushort MP
		{
			get { return ReadUInt16(64); }
			set { WriteUInt16(value, 64); }
		}
		
		/// <summary>
		/// Gets or sets the pkpoints.
		/// </summary>
		public ushort PKPoints
		{
			get { return ReadUInt16(66); }
			set { WriteUInt16(value, 66); }
		}
		
		/// <summary>
		/// Gets or sets the level.
		/// </summary>
		public byte Level
		{
			get { return ReadByte(68); }
			set { WriteByte(value, 68); }
		}
		
		/// <summary>
		/// Gets or sets the class.
		/// </summary>
		public Enums.Class Class
		{
			get { return (Enums.Class)ReadByte(69); }
			set { WriteByte((byte)value, 69); }
		}
		
		/// <summary>
		/// Gets or sets the namedisplayed.
		/// </summary>
		public bool NameDisplayed
		{
			get { return ReadBool(74); }
			set { WriteBool(value, 74); }
		}
		
		/// <summary>
		/// Gets or sets the player title.
		/// </summary>
		public Enums.PlayerTitle PlayerTitle
		{
			get { return (Enums.PlayerTitle)ReadByte(91); }
			set { WriteByte((byte)value, 91); }
		}
		
		/// <summary>
		/// Creates a CharacterInfoPacket based on a client's stats.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <returns>Returns the CharacterInfoPacket.</returns>
		public static CharacterInfoPacket Create(Entities.GameClient client)
		{
			CharacterInfoPacket info = new CharacterInfoPacket(
				new StringPacker(client.Name, "", client.SpouseName));
			info.EntityUID = client.EntityUID;
			info.Mesh = client.Mesh;
			info.HairStyle = client.HairStyle;
			info.Money = client.Money;
			info.CPs = client.CPs;
			info.Experience = client.Experience;
			info.Strength = client.Strength;
			info.Agility = client.Agility;
			info.Vitality = client.Vitality;
			info.Spirit = client.Spirit;
			info.AttributePoints = client.AttributePoints;
			info.HP = (ushort)client.MaxHP;
			info.MP = (ushort)client.MaxMP;
			info.PKPoints = (ushort)client.PKPoints;
			info.Level = client.Level;
			info.Class = client.Class;
			info.NameDisplayed = true;
			info.PlayerTitle = client.PlayerTitle;
			
			return info;
		}
	}
}
