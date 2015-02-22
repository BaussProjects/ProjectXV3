//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Item
{
	/// <summary>
	/// Subtype: ?
	/// </summary>
	public class Unequip
	{
		/// <summary>
		/// Handling the Unequip action from the ItemPacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="item">The item packet.</param>
		public static void Handle(Entities.GameClient client, ItemPacket packet)
		{
			if (client.Booth != null)
				return;
			if (!client.Alive)
				return;
			
			client.Equipments.Unequip((Enums.ItemLocation)packet.Data1);
		}
	}
}
