//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.GeneralData
{
	/// <summary>
	/// Description of QueryTeamMember.
	/// </summary>
	public class QueryTeamMember
	{
		public static void Handle(Entities.GameClient client, GeneralDataPacket general)
		{
			if (client.Team == null)
				return;
			
			uint TargetID = general.Data1;
			Entities.GameClient TeamMember;
			if (client.Team.Members.TryGetValue(TargetID, out TeamMember))
			{
				general.Data3Low = TeamMember.X;
				general.Data3High = TeamMember.Y;
				client.Send(general);
			}
		}
	}
}
