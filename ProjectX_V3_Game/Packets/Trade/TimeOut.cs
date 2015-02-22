//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Trade
{
	/// <summary>
	/// Subtype: 17
	/// </summary>
	public class TimeOut
	{
		/// <summary>
		/// Handles the TimeOut subtype of the TradePacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="trade">The trade packet.</param>
		public static void Handle(Entities.GameClient client, TradePacket trade)
		{
			if (client.Trade.Partner != null)
			{
				client.Trade.Partner.Trade.Reset();
			}
			client.Trade.Reset();
		}
	}
}
