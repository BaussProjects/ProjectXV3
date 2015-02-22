//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Item
{
	/// <summary>
	/// Description of SaveMoney.
	/// </summary>
	public class SaveMoney
	{
		public static void Handle(Entities.GameClient client, ItemPacket packet)
		{
			if (client.Booth != null)
				return;
			if (client.Money < packet.Data1)
				return;
			client.Money -= packet.Data1;
			client.WarehouseMoney += packet.Data1;
		}
	}
}
