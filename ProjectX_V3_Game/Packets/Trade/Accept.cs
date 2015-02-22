//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Trade
{
	/// <summary>
	/// Subtype: 10
	/// </summary>
	public class Accept
	{
		/// <summary>
		/// Handles the Accept subtype of the TradePacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="trade">The trade packet.</param>
		public static void Handle(Entities.GameClient client, TradePacket trade)
		{
			if (!client.Trade.Trading)
				return;
			if (!client.Trade.WindowOpen)
				return;
			
			if (!client.Trade.Accepted && !client.Trade.PartnerAccepted)
			{
				client.Trade.Accepted = true;
				client.Trade.Partner.Trade.Accepted = true;
				trade.TargetUID = client.EntityUID;
				client.Trade.Partner.Send(trade);
			}
			else if (client.Trade.Accepted && client.Trade.Partner.Trade.Accepted)
			{
				Console.WriteLine(trade.TargetUID);
				
				bool trade_success = true;
				foreach (Data.ItemInfo item in client.Trade.Items)
				{
					if (!client.Inventory.ContainsByUID(item.UID))
						trade_success = false;
				}
				foreach (Data.ItemInfo item in client.Trade.PartnerItems)
				{
					if (!client.Trade.Partner.Inventory.ContainsByUID(item.UID))
						trade_success = false;
				}
				
				if (client.Money < client.Trade.Money)
					trade_success = false;
				if (client.Trade.Partner.Money < client.Trade.PartnerMoney)
					trade_success = false;
				
				if (client.CPs < client.Trade.CPs)
					trade_success = false;
				if (client.Trade.Partner.CPs < client.Trade.PartnerCPs)
					trade_success = false;
				
				if (trade_success)
				{
					if (!client.NetworkClient.Connected)
						return;
					if (!client.Trade.Partner.NetworkClient.Connected)
						return;
					
					foreach (Data.ItemInfo item in client.Trade.Items)
					{
						if (!client.Inventory.ContainsByUID(item.UID))
							continue; // double checking
						
						if (client.Inventory.RemoveItemByUID(item.UID) != null)
						{
							client.Trade.Partner.Inventory.AddItem(item);
						}
					}
					foreach (Data.ItemInfo item in client.Trade.PartnerItems)
					{
						if (!client.Trade.Partner.Inventory.ContainsByUID(item.UID))
							continue; // double checking
						
						if (client.Trade.Partner.Inventory.RemoveItemByUID(item.UID) != null)
						{
							client.Inventory.AddItem(item);
						}
					}
					
					if (client.Money >= client.Trade.Money)
					{
						client.Money -= client.Trade.Money;
						client.Trade.Partner.Money += client.Trade.Money;
					}
					if (client.Trade.Partner.Money >= client.Trade.PartnerMoney)
					{
						client.Trade.Partner.Money -= client.Trade.PartnerMoney;
						client.Money += client.Trade.PartnerMoney;
					}
					
					if (client.CPs >= client.Trade.CPs)
					{
						client.CPs -= client.Trade.CPs;
						client.Trade.Partner.CPs += client.Trade.CPs;
					}
					if (client.Trade.Partner.CPs >= client.Trade.PartnerCPs)
					{
						client.Trade.Partner.CPs -= client.Trade.PartnerCPs;
						client.CPs += client.Trade.PartnerCPs;
					}

					Entities.GameClient partner = client.Trade.Partner;
					
					partner.Trade.Reset();
					client.Trade.Reset();
					
					trade.TradeType = Enums.TradeType.HideTable;
					trade.TargetUID = partner.EntityUID;
					client.Send(trade);
					trade.TargetUID = client.EntityUID;
					partner.Send(trade);
					
					using (var tmsg = Message.MessageCore.Create(Enums.ChatType.TopLeft, System.Drawing.Color.LimeGreen, "SYSTEM", client.Name, Core.MessageConst.TRADE_SUCCESS))
					{
						client.Send(tmsg);
					}
					using (var tmsg = Message.MessageCore.Create(Enums.ChatType.TopLeft, System.Drawing.Color.LimeGreen, "SYSTEM", partner.Name, Core.MessageConst.TRADE_SUCCESS))
					{
						partner.Send(tmsg);
					}
				}
				else
				{
					Entities.GameClient partner = client.Trade.Partner;
					
					partner.Trade.Reset();
					client.Trade.Reset();
					
					trade.TradeType = Enums.TradeType.HideTable;
					trade.TargetUID = partner.EntityUID;
					client.Send(trade);
					trade.TargetUID = client.EntityUID;
					partner.Send(trade);
					
					using (var tmsg = Message.MessageCore.Create(Enums.ChatType.TopLeft, System.Drawing.Color.LimeGreen, "SYSTEM", client.Name, Core.MessageConst.TRADE_FAIL))
					{
						client.Send(tmsg);
					}
					using (var tmsg = Message.MessageCore.Create(Enums.ChatType.TopLeft, System.Drawing.Color.LimeGreen, "SYSTEM", partner.Name, Core.MessageConst.TRADE_FAIL))
					{
						partner.Send(tmsg);
					}
				}
			}
		}
	}
}
