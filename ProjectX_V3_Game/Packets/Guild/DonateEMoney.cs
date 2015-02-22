//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Guild
{
	/// <summary>
	/// Description of DonateEMoney.
	/// </summary>
	public class DonateEMoney
	{
		public static void Handle(Entities.GameClient client, GuildPacket guild)
		{
			if (client.Guild != null)
				client.Guild.DonateCPs(client, guild.Data);
		}
	}
}
