//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.GeneralData
{
	/// <summary>
	/// Subtype: 79
	/// </summary>
	public class ChangeDirection
	{
		/// <summary>
		/// Handling the ChangeDirection action from GeneralDataPacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="General">The GeneralDataPacket.</param>
		public static void Handle(Entities.GameClient client, GeneralDataPacket General)
		{
			client.Direction = (byte)General.Direction;
			client.AttackPacket = null;
			
			client.SendToScreen(General, false);
		}
	}
}
