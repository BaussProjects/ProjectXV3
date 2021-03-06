﻿//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.GeneralData
{
	/// <summary>
	/// Subtype: 96
	/// </summary>
	public class SetPkMode
	{
		/// <summary>
		/// Handling the SetPkMode action from GeneralDataPacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="General">The GeneralDataPacket.</param>
		public static void Handle(Entities.GameClient client, GeneralDataPacket General)
		{
			client.PKMode = (Enums.PKMode)General.Data1Low;
			client.Send(General);
		}
	}
}
