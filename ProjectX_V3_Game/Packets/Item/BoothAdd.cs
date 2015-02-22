//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Item
{
	/// <summary>
	/// Description of BoothAdd.
	/// </summary>
	public class BoothAdd
	{
		public static void Handle(Entities.GameClient client, ItemPacket packet)
		{
			if (client.Booth == null)
				return;
			uint uid = packet.UID;
			if (client.Inventory.ContainsByUID(uid))
			{
				Data.BoothItem boothItem = new ProjectX_V3_Game.Data.BoothItem();
				boothItem.ItemUID = uid;
				boothItem.Price = packet.Data1;
				if (client.Booth.BoothItems.TryAdd(uid, boothItem))
				{
					client.Send(packet);
				}
			}
		}
	}
}
