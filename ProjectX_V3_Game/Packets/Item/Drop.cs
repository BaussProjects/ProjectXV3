//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Item
{
	/// <summary>
	/// Subtype: 37
	/// </summary>
	public class Drop
	{
		/// <summary>
		/// Handling the Drop action from the ItemPacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="item">The item packet.</param>
		public static void Handle(Entities.GameClient client, ItemPacket packet)
		{
			if (client.Booth != null)
				return;
			if (!client.Alive)
				return;
			if (client.Map.MapType == Enums.MapType.Shared)
				return;
			
			if (client.Inventory.ContainsByUID(packet.UID))
			{
				Maps.MapPoint Location = client.Map.CreateAvailableLocation<Data.GroundItem>(client.X, client.Y, 3);
				
				if (Location != null)
				{
					Data.ItemInfo dropitem = client.Inventory.GetItemByUID(packet.UID);
					
					if (dropitem != null)
					{
						if (!dropitem.IsValidOffItem())
						{
							using (var fmsg = Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.NO_PERMISSION_ITEM))
							{
								client.Send(fmsg);
							}
							return;
						}
						client.Inventory.RemoveItemByUID(packet.UID);
						
						Data.GroundItem ground = new Data.GroundItem(dropitem);
						ground.PlayerDrop = true;
						ground.DropType = Enums.DropItemType.Item;
						ground.X = Location.X;
						ground.Y = Location.Y;
						Location.Map.EnterMap(ground);
						ground.TaskID = ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(
							() => {
								Location.Map.LeaveMap(ground);
								ground.Screen.ClearScreen();
							},
							Core.TimeIntervals.DroppedItemRemove);
						ground.Screen.UpdateScreen(null);
					}
				}
			}
		}
	}
}
