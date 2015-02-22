//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Enums
{
	/// <summary>
	/// Description of TeamAction.
	/// </summary>
	public enum TeamAction : uint
	{
		Create = 0x00,
        RequestJoin = 0x01,
        LeaveTeam = 0x02,
        AcceptInvite = 0x03,
        RequestInvite = 0x04,
        AcceptJoin = 0x05,
        Dismiss = 0x06,
        Kick = 0x07,
        Leader = 15,
	}
}
