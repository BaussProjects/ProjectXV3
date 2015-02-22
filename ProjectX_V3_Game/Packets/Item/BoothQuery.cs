//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Item
{
	/// <summary>
	/// Description of BoothQuery.
	/// </summary>
	public class BoothQuery
	{
		public static void Handle(Entities.GameClient client, ItemPacket packet)
		{
			uint uid = packet.UID;
			Data.Booth Booth = Data.Booth.FindBoothFromNPC(uid);
			if (Booth != null)
			{
				if (!Booth.ShopOwner.IsInMap(client))
					return;
				
				if (!Core.Screen.ValidDistance(client.X, client.Y, Booth.ShopOwner.X, Booth.ShopOwner.Y))
					return;
				
				foreach (Data.BoothItem boothItem in Booth.BoothItems.Values)
				{
					Data.ItemInfo item = boothItem.GetInfo(Booth.ShopOwner);
					var viewpacket = item.CreateViewPacket(uid, 1);
					if (boothItem.IsCP)
						viewpacket.ViewType = 3;
					viewpacket.Price = boothItem.Price;
					client.Send(viewpacket);
				}
			}
		}
	}
}
