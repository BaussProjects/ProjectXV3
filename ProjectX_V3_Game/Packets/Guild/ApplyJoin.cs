//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Guild
{
	/// <summary>
	/// Description of ApplyJoin.
	/// </summary>
	public class ApplyJoin
	{
		public static void Handle(Entities.GameClient client, GuildPacket guild)
		{
			if (guild.Data == 0)
				return;
			if (guild.Data == client.EntityUID)
				return;
			
			if (client.ApplyGuildMemberUID == 0)
			{
				Entities.GameClient requestto;
				if (Core.Kernel.Clients.TrySelect(guild.Data, out requestto))
				{
					if (!requestto.IsInMap(client))
						return;
					
					if (requestto.Guild != null && client.Guild == null)
					{
						if (requestto.GuildMemberInfo.Rank == Enums.GuildRank.DeputyLeader ||
						    requestto.GuildMemberInfo.Rank == Enums.GuildRank.GuildLeader)
						{
							requestto.ApplyGuildMemberUID = client.EntityUID;
							
							using (var request = new Packets.GuildPacket(new Packets.StringPacker()))
							{
								request.Action = Enums.GuildAction.ApplyJoin;
								request.Data = client.EntityUID;
								requestto.Send(request);
							}
						}
					}
				}
				return;
			}
			
			if (client.ApplyGuildMemberUID != guild.Data)
				return;
			client.ApplyGuildMemberUID = 0;
			
			Entities.GameClient invitefrom;
			if (Core.Kernel.Clients.TrySelect(guild.Data, out invitefrom))
			{
				if (!invitefrom.IsInMap(client))
					return;
									
				if (invitefrom.Guild != null && client.Guild == null)
				{
					if (invitefrom.GuildMemberInfo.Rank == Enums.GuildRank.DeputyLeader ||
					    invitefrom.GuildMemberInfo.Rank == Enums.GuildRank.GuildLeader)
					{
						invitefrom.Guild.AddMember(client);
						client.LoadGuildInfo();
						//newclient.Screen.FullUpdate();
					}
				}
			}
		}
	}
}
