//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Guild
{
	/// <summary>
	/// Description of QuerySyndicateName.
	/// </summary>
	public class QuerySyndicateName
	{
		public static void Handle(Entities.GameClient client, GuildPacket guild)
		{
			uint guilduid = guild.Data;
			if (guilduid > 0)
			{
				Data.Guild realguild;
				if (Core.Kernel.Guilds.TrySelect(guilduid, out realguild))
				{
					using (var stringpacket = new Packets.StringPacket(new StringPacker(realguild.StringInfo)))
					{
						stringpacket.Action = Enums.StringAction.Guild;
						stringpacket.Data = guilduid;
						client.Send(stringpacket);
					}
				}
			}
		}
	}
}
