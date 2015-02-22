//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.GeneralData
{
	/// <summary>
	/// Subtype: 75
	/// </summary>
	public class GetItemSet
	{
		/// <summary>
		/// Handling the GetItemSet action from GeneralDataPacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="General">The GeneralDataPacket.</param>
		public static void Handle(Entities.GameClient client, GeneralDataPacket General)
		{
			client.Send(General);
			
			client.LoginProtection = DateTime.Now;
			client.LoggedIn = true;
			
			client.Inventory.SendAll();
			
			client.Equipments.SendItemInfos();
			client.Equipments.SendGears();
			
			client.Screen.UpdateScreen(null);
		}
	}
}
