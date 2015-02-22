//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Enums
{
	/// <summary>
	/// The different types of actions a player can perform.
	/// </summary>
    public enum ActionType : byte
    {
        None = 0,
        Dance = 1,
        Jump = 100,
        Happy = 150,
        Angry = 160,
        Sad = 170,
        Wave = 190,
        Bow = 200,
        Kneel = 210,
        Cool = 230,
        Sit = 250,
        Lie = 0x0E
    }
}
