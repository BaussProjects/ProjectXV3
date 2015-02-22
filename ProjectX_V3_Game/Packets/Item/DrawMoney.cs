//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Item
{
	/// <summary>
	/// Description of DrawMoney.
	/// </summary>
	public class DrawMoney
	{
		public static void Handle(Entities.GameClient client, ItemPacket packet)
		{
			if (client.Booth != null)
				return;
			if (client.WarehouseMoney < packet.Data1)
				return;
			client.WarehouseMoney -= packet.Data1;
			client.Money += packet.Data1;
		}
	}
}
