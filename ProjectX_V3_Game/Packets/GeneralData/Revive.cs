//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.GeneralData
{
	/// <summary>
	/// Subtype: 94
	/// </summary>
	public class Revive
	{
		/// <summary>
		/// Handling the Revive action from GeneralDataPacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="General">The GeneralDataPacket.</param>
		public static void Handle(Entities.GameClient client, GeneralDataPacket General)
		{
			if (client.Alive)
				return;
			
			// check tournament spawns etc. do ReviveHere() and Teleport() for tournaments ;)
			client.Revive();
		}
	}
}
