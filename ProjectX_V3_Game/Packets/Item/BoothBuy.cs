//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Item
{
	/// <summary>
	/// Description of BoothBuy.
	/// </summary>
	public class BoothBuy
	{
		public static void Handle(Entities.GameClient client, ItemPacket packet)
		{
			uint uid = packet.UID;
			Data.Booth Booth = Data.Booth.FindBooth(uid);
			if (Booth != null)
			{
				if (!Booth.ShopOwner.IsInMap(client))
					return;
				
				if (!Core.Screen.ValidDistance(client.X, client.Y, Booth.ShopOwner.X, Booth.ShopOwner.Y))
					return;
				Data.BoothItem boothItem;
				if (Booth.BoothItems.TryGetValue(uid, out boothItem))
				{
					if (boothItem.IsCP && client.CPs < boothItem.Price)
					{
						using (var msg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.LOW_CPS))
							client.Send(msg);
						return;
					}
					else if (client.Money < boothItem.Price)
					{
						using (var msg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.LOW_MONEY2))
							client.Send(msg);
						return;
					}
					if (Booth.BoothItems.TryRemove(uid, out boothItem))
					{
						if (boothItem.IsCP)
						{
							client.CPs -= boothItem.Price;
							Booth.ShopOwner.CPs += boothItem.Price;
						}
						else
						{
							client.Money -= boothItem.Price;
							Booth.ShopOwner.Money += boothItem.Price;
						}
						
						client.Send(packet);
						
						packet.Action = Enums.ItemAction.BoothDelete;
						Booth.ShopOwner.Send(packet);
						
						Data.ItemInfo item = Booth.ShopOwner.Inventory.RemoveItemByUID(uid);
						client.Inventory.AddItem(item);
						
						using (var msg = Packets.Message.MessageCore.CreateSystem(
							Booth.ShopOwner.Name,
							string.Format(Core.MessageConst.BOOTH_BUY, client.Name, item.Name)))
							Booth.ShopOwner.Send(msg);
						
						Database.CharacterDatabase.SaveInventory(client, item, client.Inventory.GetPositionFromItemUID(uid));
					}
				}
			}
		}
	}
}
