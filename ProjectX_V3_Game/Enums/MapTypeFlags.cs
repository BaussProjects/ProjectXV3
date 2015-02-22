//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Enums
{
	/// <summary>
	/// The different types of flags a map can have.
	/// </summary>
    [Flags]
    public enum MapTypeFlags : ulong
    {
        Normal = 0,
        PkField = 1 << 0,//0x1
        ChangeMapDisable = 1 << 1,//0x2
        RecordDisable = 1 << 2,//0x4
        PkDisable = 1 << 3,//0x8
        BoothEnable = 1 << 4,//0x10
        TeamDisable = 1 << 5,//0x20
        TeleportDisable = 1 << 6,
        GuildMap = 1 << 7,
        PrisonMap = 1 << 8,
        WingDisable = 1 << 9,
        Family = 1 << 10,
        MineField = 1 << 11,
        PkGame = 1 << 12,
        NeverWound = 1 << 13,
        DeadIsland = 1 << 14
    }
}
