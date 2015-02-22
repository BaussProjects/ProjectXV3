//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.GeneralData
{
	/// <summary>
	/// Description of EndFly.
	/// </summary>
	public class EndFly
	{
		public static void Handle(Entities.GameClient client, GeneralDataPacket General)
		{
			client.RemoveFlag1(Enums.Effect1.Fly);
		}
	}
}
