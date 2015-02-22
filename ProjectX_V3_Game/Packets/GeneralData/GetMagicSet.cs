//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.GeneralData
{
	/// <summary>
	/// Subtype: 78
	/// </summary>
	public class GetMagicSet
	{
		/// <summary>
		/// Handling the GetMagicSet action from GeneralDataPacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="General">The GeneralDataPacket.</param>
		public static void Handle(Entities.GameClient client, GeneralDataPacket General)
		{
			client.SpellData.SendSpellInfos();
			
			client.Send(General);
		}
	}
}
