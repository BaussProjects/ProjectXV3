//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Item
{
	/// <summary>
	/// Description of Bless.
	/// </summary>
	public class Bless
	{
		public static void Handle(Entities.GameClient client, ItemPacket packet)
		{
			if (client.Booth != null)
				return;

			if (client.Inventory.ContainsByUID(packet.UID))
			{
				Data.ItemInfo ToBless = client.Inventory.GetItemByUID(packet.UID);
				if (ToBless.CurrentDura < ToBless.MaxDura)
				{
					using (var msg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.ITEM_LOW_DURA))
						client.Send(msg);
					return;
				}
				if (ToBless.IsGarment() || ToBless.IsArrow() || ToBless.IsBottle() ||
				    ToBless.IsSteed() || ToBless.IsMisc())
				{
					using (var msg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.ITEM_INVALID_UPGRADE))
						client.Send(msg);
					return;
				}
				
				if (ToBless != null)
				{
					byte RequiredTortoiseGems = 5;
					byte SetBless = 1;
					switch (ToBless.Bless)
					{
						case 0:
							RequiredTortoiseGems = 5;
							break;
						case 1:
							RequiredTortoiseGems = 1;
							SetBless = 3;
							break;
						case 3:
							RequiredTortoiseGems = 3;
							SetBless = 5;
							break;
						case 5:
							RequiredTortoiseGems = 5;
							SetBless = 7;
							break;
						default:
							return;
					}
					byte TortoiseAmount;
					if (!client.Inventory.ContainsByID(700073, out TortoiseAmount))
					{
						using (var msg = Packets.Message.MessageCore.CreateSystem(
							client.Name,
							string.Format(Core.MessageConst.ITEM_AMOUNT_FAIL, "TortoiseGem's")))
							client.Send(msg);
						return;
					}
					if (TortoiseAmount < RequiredTortoiseGems)
					{
						using (var msg = Packets.Message.MessageCore.CreateSystem(
							client.Name,
							string.Format(Core.MessageConst.ITEM_AMOUNT_FAIL, "TortoiseGem's")))
							client.Send(msg);
						return;
					}
					byte Removed;
					client.Inventory.RemoveItem(700073, RequiredTortoiseGems, out Removed);
					Database.CharacterDatabase.SaveInventory(client, ToBless, client.Inventory.GetPositionFromItemUID(ToBless.UID));
					ToBless.Bless = SetBless;
					ToBless.SendPacket(client, 3);
					client.Send(packet);
				}
			}
		}
	}
}
