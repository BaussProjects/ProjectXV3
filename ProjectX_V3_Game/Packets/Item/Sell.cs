//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Item
{
	/// <summary>
	/// Subtype: 2
	/// </summary>
	public class Sell
	{
		/// <summary>
		/// Handling the Sell action from the ItemPacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="item">The item packet.</param>
		public static void Handle(Entities.GameClient client, ItemPacket packet)
		{
			if (client.Booth != null)
				return;
			if (!client.Alive)
				return;
			
			Data.Shop shop;
			if (Core.Kernel.Shops.TryGetValue(packet.UID, out shop))
			{
				if (Core.Screen.GetDistance(shop.AssociatedNPC.X, shop.AssociatedNPC.Y, client.X, client.Y) >= Core.NumericConst.MaxNPCDistance)
				{
					using (var fmsg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.TOO_FAR_NPC))
						client.Send(fmsg);
					return;
				}
				Data.ItemInfo sellitem = client.Inventory.GetItemByUID(packet.Data1);
				if (!sellitem.IsValidOffItem())
				{
					using (var fmsg = Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.NO_PERMISSION_ITEM))
					{
						client.Send(fmsg);
					}
					return;
				}
				
				uint giveback = (uint)(sellitem.Price / 3);
				if (client.Inventory.RemoveItemByUID(packet.Data1) != null)
				{
					client.Money += giveback;
				}
			}
		}
	}
}
