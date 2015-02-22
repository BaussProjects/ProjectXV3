//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Item
{
	/// <summary>
	/// Description of ActivateAccessory.
	/// </summary>
	public class ActivateAccessory
	{
		public static void Handle(Entities.GameClient client, ItemPacket packet)
		{
			uint uid = packet.UID;
			if (client.Inventory.ContainsByUID(uid))
			{
				Data.ItemInfo useitem = client.Inventory[uid];
				if (!useitem.IsMountArmor())
					return;
				
				client.Equipments.Equip(useitem, Enums.ItemLocation.SteedArmor, true);
			}
		}
	}
}
