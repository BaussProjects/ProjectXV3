//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Guild
{
	/// <summary>
	/// Description of LeaveSyndicate.
	/// </summary>
	public class LeaveSyndicate
	{
		public static void Handle(Entities.GameClient client, GuildPacket guild)
		{
			if (client.Guild != null)
			{
				if (client.GuildMemberInfo.Rank != Enums.GuildRank.GuildLeader)
				{
					client.Guild.RemoveMember(client.GuildMemberInfo);
				}
			}
		}
	}
}
