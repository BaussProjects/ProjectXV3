//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Server -> Client
	/// </summary>
	public class CharacterStatsPacket : DataPacket
	{
		public CharacterStatsPacket()
			: base(136, PacketType.CharacterStatsPacket)
		{
			
		}
		
		public CharacterStatsPacket(DataPacket inPacket)
			: base(inPacket)
		{
			
		}
		
		public uint EntityUID
		{
			get { return ReadUInt32(4); }
			set { WriteUInt32(value, 4); }
		}
		
		public uint HP
		{
			get { return ReadUInt32(8); }
			set { WriteUInt32(value, 8); }
		}
		
		public uint MP
		{
			get { return ReadUInt32(12); }
			set { WriteUInt32(value, 12); }
		}
		
		public uint MaxAttack
		{
			get { return ReadUInt32(16); }
			set { WriteUInt32(value, 16); }
		}
		
		public uint MinAttack
		{
			get { return ReadUInt32(20); }
			set { WriteUInt32(value, 20); }
		}
		
		public uint PhysicalDefense
		{
			get { return ReadUInt32(24); }
			set { WriteUInt32(value, 24); }
		}
		public uint MagicAttack
		{
			get { return ReadUInt32(28); }
			set { WriteUInt32(value, 28); }
		}
		public uint MagicDefense
		{
			get { return ReadUInt32(32); }
			set { WriteUInt32(value, 32); }
		}
		public uint Dodge
		{
			get { return ReadUInt32(36); }
			set { WriteUInt32(value, 36); }
		}
		public uint Agility
		{
			get { return ReadUInt32(40); }
			set { WriteUInt32(value, 40); }
		}
		public uint Accuracy
		{
			get { return ReadUInt32(44); }
			set { WriteUInt32(value, 44); }
		}
		public uint AttackBoost
		{
			get { return ReadUInt32(48); }
			set { WriteUInt32(value, 48); }
		}
		public uint MagicAttackBoost
		{
			get { return ReadUInt32(52); }
			set { WriteUInt32(value, 52); }
		}
		public uint MagicDefensePercentage
		{
			get { return ReadUInt32(56); }
			set { WriteUInt32(value, 56); }
		}
		public uint Bless
		{
			get { return ReadUInt32(64); }
			set { WriteUInt32(value, 28); }
		}
		public uint CriticalStrike
		{
			get { return ReadUInt32(68); }
			set { WriteUInt32(value, 68); }
		}
		public uint SkillCriticalStrike
		{
			get { return ReadUInt32(72); }
			set { WriteUInt32(value, 72); }
		}
		public uint Immunity
		{
			get { return ReadUInt32(76); }
			set { WriteUInt32(value, 76); }
		}
		public uint Penetration
		{
			get { return ReadUInt32(80); }
			set { WriteUInt32(value, 80); }
		}
		public uint Block
		{
			get { return ReadUInt32(84); }
			set { WriteUInt32(value, 84); }
		}
		public uint BreakThrough
		{
			get { return ReadUInt32(88); }
			set { WriteUInt32(value, 88); }
		}
		public uint CounterAction
		{
			get { return ReadUInt32(92); }
			set { WriteUInt32(value, 92); }
		}
		public uint Detoxication
		{
			get { return ReadUInt32(96); }
			set { WriteUInt32(value, 96); }
		}
		public uint FinalPhysicalDamage
		{
			get { return ReadUInt32(100); }
			set { WriteUInt32(value, 100); }
		}
		public uint FinalMagicDamage
		{
			get { return ReadUInt32(104); }
			set { WriteUInt32(value, 104); }
		}
		public uint FinalDefense
		{
			get { return ReadUInt32(108); }
			set { WriteUInt32(value, 108); }
		}
		public uint FinalMagicDefense
		{
			get { return ReadUInt32(112); }
			set { WriteUInt32(value, 112); }
		}
		public uint MetalDefense
		{
			get { return ReadUInt32(116); }
			set { WriteUInt32(value, 166); }
		}
		public uint WoodDefense
		{
			get { return ReadUInt32(120); }
			set { WriteUInt32(value, 120); }
		}
		public uint WaterDefense
		{
			get { return ReadUInt32(124); }
			set { WriteUInt32(value, 124); }
		}
		public uint FireDefense
		{
			get { return ReadUInt32(128); }
			set { WriteUInt32(value, 128); }
		}
		public uint EarthDefense
		{
			get { return ReadUInt32(132); }
			set { WriteUInt32(value, 132); }
		}
		
		public static void Handle(Entities.GameClient client, DataPacket packet)
		{
			using (var stats = new CharacterStatsPacket(packet))
			{
				if (stats.EntityUID != client.EntityUID)
				{
					Entities.GameClient viewclient;
					if (Core.Kernel.Clients.TrySelect(stats.EntityUID, out viewclient))
						client.Send(viewclient.CreateStatsPacket());
				}
				else
					client.Send(client.CreateStatsPacket());
			}
		}
	}
}
