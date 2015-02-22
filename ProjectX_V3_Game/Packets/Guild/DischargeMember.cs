//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Guild
{
	/// <summary>
	/// Description of DischargeMember.
	/// </summary>
	public class DischargeMember
	{
		public static void Handle(Entities.GameClient client, GuildPacket guild)
		{
			if (client.Guild != null)
			{
				if (client.GuildMemberInfo.Rank == Enums.GuildRank.GuildLeader)
				{
					client.Guild.KickMember(guild.Strings[0]);
				}
			}
		}
	}
}
