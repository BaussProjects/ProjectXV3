//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Item
{
	/// <summary>
	/// Subtype: ?
	/// </summary>
	public class Use
	{
		/// <summary>
		/// Handling the Use action from the ItemPacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="item">The item packet.</param>
		public static void Handle(Entities.GameClient client, ItemPacket packet)
		{		
			uint uid = packet.UID;
			if (client.Inventory.ContainsByUID(uid))
			{
				Data.ItemInfo useitem = client.Inventory[uid];
				if (useitem.IsMisc())
				{
					if (!Core.Kernel.ItemScriptEngine.Invoke(useitem.ItemID, new object[] { client, useitem }))
					{
						using (var fmsg = Packets.Message.MessageCore.CreateSystem(client.Name, string.Format(Core.MessageConst.INVALD_ITEM_USE, useitem.Name, useitem.ItemID)))
							client.Send(fmsg);
					}
				}
				else
				{
					if (!client.Alive)
						return;
					
					Enums.ItemLocation loc = (Enums.ItemLocation)packet.Data1;
					client.Equipments.Equip(useitem, loc, true);
				}
			}
			//	else (Reason this is commented out is because it disconnected the client if switching equips too fast lmao.
			//		client.NetworkClient.Disconnect("Using invalid item.");
		}
	}
}
