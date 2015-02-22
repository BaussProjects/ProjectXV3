//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.GeneralData
{
	/// <summary>
	/// Subtype: 76
	/// </summary>
	public class GetAssociation
	{
		/// <summary>
		/// Handling the GetAssociation action from GeneralDataPacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="General">The GeneralDataPacket.</param>
		public static void Handle(Entities.GameClient client, GeneralDataPacket General)
		{
			// Load friends [If not loaded]
			// Send friends
			client.Send(General);
		}
	}
}
