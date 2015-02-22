//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Trade
{
	/// <summary>
	/// Subtype: 2
	/// </summary>
	public class Close
	{
		/// <summary>
		/// Handles the Close subtype of the TradePacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="trade">The trade packet.</param>
		public static void Handle(Entities.GameClient client, TradePacket trade)
		{
			if (client.Trade.Partner != null)
			{
				Entities.GameClient partner = client.Trade.Partner;
				
				partner.Trade.Reset();
				client.Trade.Reset();
				
				trade.TradeType = Enums.TradeType.HideTable;
				trade.TargetUID = partner.EntityUID;
				client.Send(trade);						
				trade.TargetUID = client.EntityUID;
				partner.Send(trade);
			}
		}
	}
}
