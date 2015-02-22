//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.GeneralData
{
	/// <summary>
	/// Subtype: 97
	/// </summary>
	public class GetSynAttr
	{
		/// <summary>
		/// Handling the GetSynAttr action from GeneralDataPacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="General">The GeneralDataPacket.</param>
		public static void Handle(Entities.GameClient client, GeneralDataPacket General)
		{
			client.LoadGuildInfo();
			
			Core.LoginHandler.Handle(client);
			
			client.Teleport(client.Map.MapID, client.X, client.Y); // poor fix, but updates lastmap to proper ...
		}
	}
}
