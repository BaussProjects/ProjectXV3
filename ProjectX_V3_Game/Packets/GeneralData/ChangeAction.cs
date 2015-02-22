//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.GeneralData
{
	/// <summary>
	/// Subtype: 81
	/// </summary>
	public class ChangeAction
	{
		/// <summary>
		/// Handling the ChangeAction action from GeneralDataPacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="General">The GeneralDataPacket.</param>
		public static void Handle(Entities.GameClient client, GeneralDataPacket General)
		{
			Enums.ActionType actiontype = (Enums.ActionType)General.Data1Low;
			client.AttackPacket = null;
			
			if (client.Action != Enums.ActionType.Sit && client.Action != Enums.ActionType.Lie)
			{
				if (actiontype == Enums.ActionType.Sit || actiontype == Enums.ActionType.Lie)
				{
					client.SitTime = DateTime.Now;
					client.UpdateStamina(true);
				}
			}
			
			client.Action = actiontype;
			
			client.SendToScreen(General, false);
		}
	}
}
