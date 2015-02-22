//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Client -> Server
	/// Server -> Client
	/// </summary>
	public class InteractionPacket : DataPacket
	{
		public InteractionPacket(DataPacket inPacket)
			: base(inPacket)
		{
		}
		public InteractionPacket()
			: base(40, PacketType.InteractionPacket)
		{
			
		}
		
		public bool UnPacked = false;
		
		public ushort WeaponTypeRight = 0, WeaponTypeLeft = 0;
		
		public ProjectX_V3_Lib.Time.SystemTime TimeStamp
		{
			get { return new ProjectX_V3_Lib.Time.SystemTime(ReadUInt32(4)); }
			set { WriteUInt32(value, 4); }
		}
		
		public uint EntityUID
		{
			get { return ReadUInt32(8); }
			set { WriteUInt32(value, 8); }
		}
		
		public uint TargetUID
		{
			get { return ReadUInt32(12); }
			set { WriteUInt32(value, 12); }
		}
		
		public ushort X
		{
			get { return ReadUInt16(16); }
			set { WriteUInt16(value, 16); }
		}
		
		public ushort Y
		{
			get { return ReadUInt16(18); }
			set { WriteUInt16(value, 18); }
		}
		
		public Enums.InteractAction Action
		{
			get { return (Enums.InteractAction)ReadUInt32(20); }
			set { WriteUInt32((uint)value, 20); }
		}
		
		public uint Data
		{
			get { return ReadUInt32(24); }
			set { WriteUInt32(value, 24); }
		}
		
		public ushort Damage
		{
			get { return (ushort)Data; }
			set { Data = (uint)((KoCount << 16) | value); }
		}
		public ushort KoCount
		{
			get { return (ushort)(Data >> 16); }
			set { Data = (uint)((value << 16) | Damage); }
		}
		public ushort MagicType
		{
			get { return (ushort)Data; }
			set { Data = (uint)((MagicLevel << 16) | value); }
		}

		public ushort MagicLevel
		{
			get { return (ushort)(Data >> 16); }
			set { Data = (uint)((value << 16) | MagicType); }
		}
		
		public uint Unknown
		{
			get { return ReadUInt32(28); }
			set { WriteUInt32(value, 28); }
		}
		
		public uint ActivationType
		{
			get { return ReadUInt32(32); }
			set { WriteUInt32(value, 32); }
		}
		
		public uint ActivationValue
		{
			get { return ReadUInt32(36); }
			set { WriteUInt32(value, 36); }
		}
		
		public static void Handle(Entities.GameClient client, DataPacket packet)
		{
			using (var interact = new InteractionPacket(packet))
			{
				switch (interact.Action)
				{
						#region Marriage
					case Enums.InteractAction.Marry:
						Interaction.Misc.Marry.Handle(client, interact);
						break;
						#endregion
						#region Court
					case Enums.InteractAction.Court:
						Interaction.Misc.Court.Handle(client, interact);
						break;
						#endregion
						#region Combat
					case Enums.InteractAction.Attack:
					case Enums.InteractAction.MagicAttack:
					case Enums.InteractAction.Shoot:
						Interaction.Battle.Combat.Handle(client, interact);
						break;
						#endregion
				}
			}
		}
	}
}
