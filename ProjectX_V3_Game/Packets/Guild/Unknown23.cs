﻿//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Guild
{
	/// <summary>
	/// Description of Unknown23.
	/// </summary>
	public class Unknown23
	{
		public static void Handle(Entities.GameClient client, GuildPacket guild)
		{
			using (var msg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.USE_GUILD_DIRECTOR))
				client.Send(msg);
		}
	}
}
