//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Guild
{
	/// <summary>
	/// Description of QuerySyndicateAttribute.
	/// </summary>
	public class QuerySyndicateAttribute
	{
		public static void Handle(Entities.GameClient client, GuildPacket guild)
		{
			if (client.Guild != null)
			{
				client.SendGuild();
				using (var announce = new Packets.GuildPacket(new Packets.StringPacker(client.Guild.Bullentin)))
				{
					announce.Data = Core.Kernel.TimeGet(Enums.TimeType.Day);
					announce.Action = Enums.GuildAction.SetAnnounce;
					client.Send(announce);
				}
			}
		}
	}
}
