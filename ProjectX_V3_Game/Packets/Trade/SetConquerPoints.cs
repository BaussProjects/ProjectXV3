//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Trade
{
	/// <summary>
	/// Subtype: 12
	/// </summary>
	public class SetConquerPoints
	{
		/// <summary>
		/// Handles the SetConquerPoints subtype of the TradePacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="trade">The trade packet.</param>
		public static void Handle(Entities.GameClient client, TradePacket trade)
		{
			if (!client.Trade.Trading)
				return;
			if (!client.Trade.WindowOpen)
				return;
			
			uint cps = trade.TargetUID;
			if (cps > client.CPs)
			{
				using (var fmsg = Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.LOW_CPS_TRADE))
				{
					client.Send(fmsg);
				}
				return;
			}
			
			client.Trade.CPs = cps;
			trade.TradeType = Enums.TradeType.ShowConquerPoints;
			client.Trade.Partner.Send(trade);
		}
	}
}
