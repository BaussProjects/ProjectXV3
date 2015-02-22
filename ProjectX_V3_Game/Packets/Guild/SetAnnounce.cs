//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Guild
{
	/// <summary>
	/// Description of SetAnnounce.
	/// </summary>
	public class SetAnnounce
	{
		public static void Handle(Entities.GameClient client, GuildPacket guild)
		{
			if (client.Guild != null)
			{
				if (client.GuildMemberInfo.Rank == Enums.GuildRank.GuildLeader)
					client.Guild.SetBullentin(client, guild.Strings[0]);
			}
		}
	}
}
