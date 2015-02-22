//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Enums
{
    public enum StringAction : byte
    {
        None = 0,
        Fireworks,
        CreateGuild,
        Guild,
        ChangeTitle,
        DeleteRole = 5,
        Mate,
        QueryNpc,
        Wanted,
        MapEffect,
        RoleEffect = 10,
        MemberList,
        KickoutGuildMember,
        QueryWanted,
        QueryPoliceWanted,
        PoliceWanted = 15,
        QueryMate,
        AddDicePlayer,
        DeleteDicePlayer,
        DiceBonus,
        PlayerWave = 20,
        SetAlly,
        SetEnemy,
        WhisperWindowInfo = 26
    }
}
