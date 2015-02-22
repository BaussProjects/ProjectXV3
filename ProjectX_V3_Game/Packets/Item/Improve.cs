//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Item
{
	/// <summary>
	/// Description of Improve.
	/// </summary>
	public class Improve
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
			{
				using (var msg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.ITEM_LOW_DURA))
					client.Send(msg);
				return;
			}
			
			Data.ItemInfo Dragonball = client.Inventory.GetItemByUID(packet.Data1);
			if (Dragonball == null || ToUpgrade == null)
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
			if (ToUpgrade.Quality >= 9)
			{
				using (var msg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.ITEM_SUPER))
					client.Send(msg);
				return; // super
			}
			if (Dragonball.ItemID != 1088000)
			{
				using (var msg = Packets.Message.MessageCore.CreateSystem(
					client.Name,
					string.Format(Core.MessageConst.ITEM_AMOUNT_FAIL, "Dragonball's")))
					client.Send(msg);
				return;
			}

			uint NewID = ToUpgrade.ItemID;
			if (ToUpgrade.Quality >= 6)
				NewID = (ToUpgrade.ItemID + 1);
			else
			{
				while ((NewID % 10) < 6)
				{
					NewID++;
				}
			}
			
			if (Core.Kernel.ItemInfos.Contains(NewID))
			{
				Data.ItemInfo newItem;
				if (Core.Kernel.ItemInfos.TrySelect(NewID, out newItem))
				{
					if (newItem.Quality > ToUpgrade.Quality && newItem.Name == ToUpgrade.Name)
					{
						if (Calculations.BasicCalculations.ChanceSuccess(Core.NumericConst.QualityUpgradeChance))
						{
							Data.ItemInfo NewUpgradedItem = newItem.Copy();
							NewUpgradedItem.SetStats(ToUpgrade);
							
							client.Inventory.RemoveItemByUID(ToUpgrade.UID);
							client.Inventory.RemoveItemByUID(Dragonball.UID);
							
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
