//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Guild
{
	/// <summary>
	/// Description of InviteJoin.
	/// </summary>
	public class InviteJoin
	{
		public static void Handle(Entities.GameClient client, GuildPacket guild)
		{
			if (guild.Data == 0)
				return;
			if (guild.Data == client.EntityUID)
				return;
			if (client.Guild == null)
				return;
			if (client.ApplyGuildMemberUID == 0)
			{
				Entities.GameClient inviteto;
				if (Core.Kernel.Clients.TrySelect(guild.Data, out inviteto))
				{
					if (!inviteto.IsInMap(client))
						return;
					
					if (inviteto.Guild == null && client.Guild != null)
					{
						if (client.GuildMemberInfo.Rank == Enums.GuildRank.DeputyLeader ||
						    client.GuildMemberInfo.Rank == Enums.GuildRank.GuildLeader)
						{
							inviteto.ApplyGuildMemberUID = client.EntityUID;
							using (var invite = new Packets.GuildPacket(new Packets.StringPacker()))
							{
								invite.Action = Enums.GuildAction.InviteJoin;
								invite.Data = client.EntityUID;
								inviteto.Send(invite);
							}
						}
					}
				}
				return;
			}
			
			if (client.ApplyGuildMemberUID != guild.Data)
				return;
			client.ApplyGuildMemberUID = 0;
			
			Entities.GameClient newclient;
			if (Core.Kernel.Clients.TrySelect(guild.Data, out newclient))
			{
				if (!newclient.IsInMap(client))
					return;
				
				if (newclient.Guild == null && client.Guild != null)
				{
					if (client.GuildMemberInfo.Rank == Enums.GuildRank.DeputyLeader ||
					    client.GuildMemberInfo.Rank == Enums.GuildRank.GuildLeader)
					{
						client.Guild.AddMember(newclient);
						newclient.LoadGuildInfo();
						//newclient.Screen.FullUpdate();
					}
				}
			}
		}
	}
}
