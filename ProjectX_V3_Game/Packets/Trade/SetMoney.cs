//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Trade
{
	/// <summary>
	/// Subtype: 7
	/// </summary>
	public class SetMoney
	{
		/// <summary>
		/// Handles the SetMoney subtype of the TradePacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="trade">The trade packet.</param>
		public static void Handle(Entities.GameClient client, TradePacket trade)
		{
			if (!client.Trade.Trading)
				return;
			if (!client.Trade.WindowOpen)
				return;
			
			uint money = trade.TargetUID;
			if (money > client.Money)
			{
				using (var fmsg = Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.LOW_MONEY_TRADE))
				{
					client.Send(fmsg);
				}
				return;
			}
			
			client.Trade.Money = money;
			trade.TradeType = Enums.TradeType.ShowMoney;
			client.Trade.Partner.Send(trade);
		}
	}
}
