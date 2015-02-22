//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.GeneralData
{
	/// <summary>
	/// Subtype: 77
	/// </summary>
	public class GetWeaponSkillSet
	{
		/// <summary>
		/// Handling the GetWeaponSkillSet action from GeneralDataPacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="General">The GeneralDataPacket.</param>
		public static void Handle(Entities.GameClient client, GeneralDataPacket General)
		{
			client.SpellData.SendProfInfos();
			
			client.Send(General);
		}
	}
}
