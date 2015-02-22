//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Item
{
	/// <summary>
	/// Subtype: 27
	/// </summary>
	public class Ping
	{
		/// <summary>
		/// Handling the Ping action from the ItemPacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="item">The item packet.</param>
		public static void Handle(Entities.GameClient client, ItemPacket item)
		{
			client.Send(item);
		}
	}
}
