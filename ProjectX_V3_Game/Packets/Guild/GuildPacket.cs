//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Server -> Client
	/// Client -> Server
	/// </summary>
	public class GuildPacket : DataPacket
	{
		public GuildPacket(StringPacker strings)
			: base((ushort)(28 + strings.Size), PacketType.GuildPacket)
		{
			strings.AppendAndFinish(this, 24);
		}
		
		public GuildPacket(DataPacket inPacket)
			: base(inPacket)
		{
			if (inPacket.ReadByte(24) > 0)
				Strings = StringPacker.Analyze(this, 24);
		}
		
		public Enums.GuildAction Action
		{
			get { return (Enums.GuildAction)ReadUInt32(4); }
			set { WriteUInt32((uint)value, 4); }
		}
		
		public uint Data
		{
			get { return ReadUInt32(8); }
			set { WriteUInt32(value, 8); }
		}
		
		public int RequiredLevel
		{
			get { return ReadInt32(12); }
			set { WriteInt32(value, 12); }
		}
		
		public int RequiredMetempsychosis
		{
			get { return ReadInt32(16); }
			set { WriteInt32(value, 16); }
		}
		
		public int RequiredProfession
		{
			get { return ReadInt32(20); }
			set { WriteInt32(value, 20); }
		}
		
		public readonly string[] Strings;
		
		public static void Handle(Entities.GameClient client, DataPacket packet)
		{
			using (var guild = new GuildPacket(packet))
			{
				switch (guild.Action)
				{
					case Enums.GuildAction.QuerySyndicateName:
						Guild.QuerySyndicateName.Handle(client, guild);
						break;
						
					case Enums.GuildAction.ApplyJoin:
						Guild.ApplyJoin.Handle(client, guild);
						break;
						
					case Enums.GuildAction.InviteJoin:
						Guild.InviteJoin.Handle(client, guild);
						break;
						
					case Enums.GuildAction.LeaveSyndicate:
						Guild.LeaveSyndicate.Handle(client, guild);
						break;
						
					case Enums.GuildAction.SetAnnounce:
						Guild.SetAnnounce.Handle(client, guild);
						break;
						
					case Enums.GuildAction.QuerySyndicateAttribute:
						Guild.QuerySyndicateAttribute.Handle(client, guild);
						break;
						
					case Enums.GuildAction.DonateMoney:
						Guild.DonateMoney.Handle(client, guild);
						break;
						
					case Enums.GuildAction.DonateEMoney:
						Guild.DonateEMoney.Handle(client, guild);
						break;
						
					case Enums.GuildAction.DischargeMember:
						Guild.DischargeMember.Handle(client, guild);
						break;
						
					case Enums.GuildAction.ClearAlly:
						Guild.ClearAlly.Handle(client, guild);
						break;
						
					case Enums.GuildAction.ClearEnemy:
						Guild.ClearEnemy.Handle(client, guild);
						break;
						
					case Enums.GuildAction.SetAlly:
						Guild.SetAlly.Handle(client, guild);
						break;
						
					case Enums.GuildAction.SetEnemy:
						Guild.DischargeMember.Handle(client, guild);
						break;
						
					case Enums.GuildAction.PromoteInfo:
						Guild.DischargeMember.Handle(client, guild);
						break;
						
					case Enums.GuildAction.PromoteMember:
						Guild.DischargeMember.Handle(client, guild);
						break;
						
					case Enums.GuildAction.SetRequirement:
						Guild.DischargeMember.Handle(client, guild);
						break;
					default:
						Console.WriteLine("Unknown Guild Packet: {0} From {1}", guild.Action.ToString(), client.Name);
						break;
				}
			}
		}
	}
}
