//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Item
{
	/// <summary>
	/// Description of Uplev.
	/// </summary>
	public class Uplev
	{
		public static void Handle(Entities.GameClient client, ItemPacket packet)
		{
			if (client.Booth != null)
				return;
			if (!client.Alive)
				return;
			
			if (!client.Inventory.ContainsByUID(packet.UID) && client.Inventory.ContainsByUID(packet.Data1))
			{
				using (var msg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.ITEM_NOT_FOUND))
					client.Send(msg);
				return;
			}
			
			Data.ItemInfo ToUpgrade = client.Inventory.GetItemByUID(packet.UID);
			if (ToUpgrade.CurrentDura < ToUpgrade.MaxDura)
				return;
			
			Data.ItemInfo Meteor = client.Inventory.GetItemByUID(packet.Data1);
			if (Meteor == null || ToUpgrade == null)
			{
				using (var msg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.ITEM_NOT_FOUND))
					client.Send(msg);
				return;
			}
			if (ToUpgrade.IsGarment() || ToUpgrade.IsArrow() || ToUpgrade.IsBottle() ||
			    ToUpgrade.IsSteed() || ToUpgrade.IsMisc())
			{
				using (var msg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.ITEM_INVALID_UPGRADE))
					client.Send(msg);
				return;
			}
			if (ToUpgrade.RequiredLevel >= 120)
			{
				using (var msg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.ITEM_MAX_LEVEL))
					client.Send(msg);
				return; // max level
			}
			if (Meteor.ItemID != 1088001 && Meteor.ItemID != 1088002)
			{
				using (var msg = Packets.Message.MessageCore.CreateSystem(
					client.Name,
					string.Format(Core.MessageConst.ITEM_AMOUNT_FAIL, "Meteor- or Meteortear's")))
					client.Send(msg);
				return;
			}

			uint NewID = ToUpgrade.ItemID + 10;
			int loop = 4;
			while (!Core.Kernel.ItemInfos.Contains(NewID))
			{
				NewID += 10;
				loop--;
				if (loop <= 0)
					break;
			}
			
			if (Core.Kernel.ItemInfos.Contains(NewID))
			{
				Data.ItemInfo newItem;
				if (Core.Kernel.ItemInfos.TrySelect(NewID, out newItem))
				{
					if (newItem.RequiredLevel > ToUpgrade.RequiredLevel && newItem.TypeName == ToUpgrade.TypeName)
					{
						if (Calculations.BasicCalculations.ChanceSuccess(Core.NumericConst.LevelUpgradeChance))
						{
							Data.ItemInfo NewUpgradedItem = newItem.Copy();
							NewUpgradedItem.SetStats(ToUpgrade);
							
							client.Inventory.RemoveItemByUID(ToUpgrade.UID);
							client.Inventory.RemoveItemByUID(Meteor.UID);
							
							if (Calculations.BasicCalculations.ChanceSuccess(Core.NumericConst.FirstSocketChance) &&
							    NewUpgradedItem.Gem1 == Enums.SocketGem.NoSocket)
							{
								NewUpgradedItem.Gem1 = Enums.SocketGem.EmptySocket;
							}
							else if (Calculations.BasicCalculations.ChanceSuccess(Core.NumericConst.SecondSocketChance) &&
							         NewUpgradedItem.Gem2 == Enums.SocketGem.NoSocket)
							{
								NewUpgradedItem.Gem2 = Enums.SocketGem.EmptySocket;
							}
							
							client.Inventory.AddItem(NewUpgradedItem);
						}
					}
				}
			}
		}
	}
}
