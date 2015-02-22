//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Trade
{
	/// <summary>
	/// Subtype: 1
	/// </summary>
	public class Request
	{
		/// <summary>
		/// Handles the Request subtype of the TradePacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="trade">The trade packet.</param>
		public static void Handle(Entities.GameClient client, TradePacket trade)
		{
			if (client.Trade.Trading && client.Trade.Partner == null)
			{
				using (var fmsg = Message.MessageCore.CreateSystem2(client.Name, Core.MessageConst.SELF_ALREADY_TRADE))
					client.Send(fmsg);
				return;
			}
			else if (client.Trade.Trading)
			{
				if (!client.Trade.Requesting)
				{
					trade.TradeType = Enums.TradeType.ShowTable;
					trade.TargetUID = client.Trade.Partner.EntityUID;
					client.Send(trade);
					trade.TargetUID = client.EntityUID;
					client.Trade.Partner.Send(trade);
					
					client.Trade.WindowOpen = true;
					client.Trade.Partner.Trade.WindowOpen = true;
				}
				return;
			}
			
			if (client.Map.MapObjects.ContainsKey(trade.TargetUID))//(Core.Kernel.Clients.Contains(trade.TargetUID))
			{
				Maps.IMapObject obj;
				if (client.Map.MapObjects.TryGetValue(trade.TargetUID, out obj))//(Core.Kernel.Clients.TrySelect(trade.TargetUID, out target))
				{
					if (!obj.IsInMap(client))
						return;
					if (client.Map.MapType == Enums.MapType.Shared)
						return;
					
					Entities.GameClient target = (Entities.GameClient)obj;
					if (!target.Trade.Trading)
					{
						client.Trade.Begin(target);
						target.Trade.Begin(client);
						target.Trade.Requesting = false;
						client.Trade.Requesting = true;
						trade.TargetUID = client.EntityUID;
						target.Send(trade);
					}
					else
					{
						using (var fmsg = Message.MessageCore.CreateSystem2(client.Name, Core.MessageConst.ALREADY_TRADE))
							client.Send(fmsg);
					}
				}
				else
				{
					using (var fmsg = Message.MessageCore.CreateSystem2(client.Name, Core.MessageConst.TARGET_BUSY))
						client.Send(fmsg);
				}
			}
			else
			{
				client.NetworkClient.Disconnect(string.Format("Invalid trade target. TargetUID {0}", trade.TargetUID));
			}
		}
	}
}
