//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Trade
{
	/// <summary>
	/// Subtype: 6
	/// </summary>
	public class AddItem
	{
		/// <summary>
		/// Handles the AddItem subtype of the TradePacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="trade">The trade packet.</param>
		public static void Handle(Entities.GameClient client, TradePacket trade)
		{
			if (!client.Trade.Trading)
				return;
			if (!client.Trade.WindowOpen)
				return;
			
			uint itemuid = trade.TargetUID;
			if (client.Inventory.ContainsByUID(itemuid))
			{
				Data.ItemInfo item = client.Inventory[itemuid];
				
				if (!item.IsValidOffItem())
				{
					using (var fmsg = Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.NO_PERMISSION_ITEM))
					{
						client.Send(fmsg);
					}
					return;
				}
				
				if ((client.Trade.Partner.Inventory.Count + client.Trade.Items.Count) >= 40)
				{
					using (var fmsg = Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.TARGET_FULL_INVENTORY))
					{
						client.Send(fmsg);
					}
					return;
				}
				
				client.Trade.Items.Add(item);
				item.SendPacket(client.Trade.Partner, 2);
			}
		}
	}
}
