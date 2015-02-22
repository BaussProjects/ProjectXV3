//Project by BaussHacker aka. L33TS

using System;
using System.Linq;

namespace ProjectX_V3_Game.Packets.Item
{
	/// <summary>
	/// Subtype: 1
	/// </summary>
	public class Buy
	{
		/// <summary>
		/// Handling the Buy action from the ItemPacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="item">The item packet.</param>
		public static void Handle(Entities.GameClient client, ItemPacket packet)
		{
			if (client.Booth != null)
				return;
			if (!client.Alive)
				return;
			
			if (client.Inventory.Count >= 40)
			{
				using (var fmsg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.INVENTORY_FULL))
					client.Send(fmsg);
				return;
			}
			
			Data.Shop shop;
			if (Core.Kernel.Shops.TryGetValue(packet.UID, out shop))
			{
				if (shop.ShopType == Enums.ShopType.Money)
				{
					if (!shop.AssociatedNPC.IsInMap(client))
					{
						using (var fmsg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.TOO_FAR_NPC))
							client.Send(fmsg);
						return;
					}
					   if ( Core.Screen.GetDistance(shop.AssociatedNPC.X, shop.AssociatedNPC.Y, client.X, client.Y) >= Core.NumericConst.MaxNPCDistance)
					{
						using (var fmsg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.TOO_FAR_NPC))
							client.Send(fmsg);
						return;
					}
				}
				string items = "";
						foreach (uint item in shop.ShopItems)
							items += item + "-";
						System.IO.File.AppendAllText(client.Map.MapID + ".txt", items);
						
				uint itemid = packet.Data1;
				if (!shop.ShopItems.Contains(itemid))
					return;
				
				uint amount = packet.Data2;
				long above = ((client.Inventory.Count + amount) - 40);
				if (above > 0)
					amount -= (uint)above;
				
				if ((client.Inventory.Count + amount) > 40)
				{
					using (var fmsg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.TOO_MANY_BUY))
						client.Send(fmsg);
					return;
				}
				
				Data.ItemInfo buyitem;
				if (Core.Kernel.ItemInfos.TrySelect(itemid, out buyitem))
				{
					switch (shop.ShopType)
					{
						case Enums.ShopType.Money:
							{
								uint price = (buyitem.Price * amount);
								if (client.Money >= price)
								{
									client.Money -= price;
									if (amount > 1)
										client.Inventory.AddItem(buyitem.ItemID, (byte)amount);
									else
										client.Inventory.AddItem(buyitem.Copy());
								}
								else
								{
									using (var fmsg = Packets.Message.MessageCore.CreateSystem(client.Name, string.Format(Core.MessageConst.LOW_MONEY, price, "silvers", amount, buyitem.Name)))
										client.Send(fmsg);
								}
								break;
							}
						case Enums.ShopType.CPs:
							{
								uint price = (buyitem.CPPrice * amount);
								
								if (client.CPs >= price)
								{
									client.CPs -= price;
									if (amount > 1)
										client.Inventory.AddItem(buyitem.ItemID, (byte)amount);
									else
										client.Inventory.AddItem(buyitem.Copy());
								}
								else
								{
									using (var fmsg = Packets.Message.MessageCore.CreateSystem(client.Name, string.Format(Core.MessageConst.LOW_MONEY, price, "CPs", amount, buyitem.Name)))
										client.Send(fmsg);
								}
								break;
							}
					}
					return;
				}
			}
			
			using (var fmsg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.FAIL_BUY_ERROR))
				client.Send(fmsg);
		}
	}
}
