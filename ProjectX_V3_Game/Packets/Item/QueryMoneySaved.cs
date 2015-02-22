//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Item
{
	/// <summary>
	/// Description of QueryMoneySaved.
	/// </summary>
	public class QueryMoneySaved
	{
		public static void Handle(Entities.GameClient client, ItemPacket packet)
		{
			if (client.Booth != null)
				return;
			packet.Data1 = client.WarehouseMoney;
			client.Send(packet);
		}
	}
}
