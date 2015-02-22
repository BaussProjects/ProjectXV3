//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.GeneralData
{
	/// <summary>
	/// Subtype: ?
	/// </summary>
	public class ViewEquipment
	{
		/// <summary>
		/// Handling the QueryEquipment & QueryFriendEquip action from GeneralDataPacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="General">The GeneralDataPacket.</param>
		public static void Handle(Entities.GameClient client, GeneralDataPacket General)
		{
			Entities.GameClient target;
			if (Core.Kernel.Clients.TrySelect(General.Data1, out target))
			{
				client.Send(target.CreateSpawnPacket());
				foreach (Data.ItemInfo equip in target.Equipments.Equips.Values)
				{
					equip.SendViewPacket(target.EntityUID, client);
				}
				using (var msg = Packets.Message.MessageCore.CreateSystem(
					target.Name,
					string.Format(Core.MessageConst.VIEW_EQUIP, client.Name)))
					target.Send(msg);
				using (var stringpacket = new Packets.StringPacket(new StringPacker(target.SpouseName)))
				{
					stringpacket.Action = Enums.StringAction.QueryMate;
					client.Send(stringpacket);
				}
			}
		}
	}
}
