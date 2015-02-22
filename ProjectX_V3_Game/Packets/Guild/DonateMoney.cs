//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Guild
{
	/// <summary>
	/// Description of DonateMoney.
	/// </summary>
	public class DonateMoney
	{
		public static void Handle(Entities.GameClient client, GuildPacket guild)
		{
			if (client.Guild != null)
				client.Guild.DonateMoney(client, guild.Data);
		}
	}
}
