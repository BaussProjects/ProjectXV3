//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Enums
{
    [Flags]
    public enum Effect1 : ulong
    {
        None = 0UL,
        BlueName = 1UL << 0,
        Poisoned = 1UL << 1,
        FullInvis = 1UL << 2,//(Full invisibility)
        Fade  = 1UL << 3,
        StartXp = 1UL << 4,
        Ghost = 1UL << 5,
        TeamLeader = 1UL << 6,
        StarOfAccuracy = 1UL << 7,
        Shield = 1UL << 8,
        Stig = 1UL << 9,
        Dead = 1UL << 10,
        Invisible = 1UL << 11,// (DOES NOT REMOVE :S)
        Unknown12 = 1UL << 12,
        Unknown13 = 1UL << 13,
        RedName = 1UL << 14,
        BlackName = 1UL << 15,
        Unknown16 = 1UL << 16,
        Unknown17 = 1UL << 17,
        Superman = 1UL << 18,
        ReflecttypeThing = 1UL << 19,
        DifReflectThing = 1UL << 20,
        Unknown21 = 1UL << 21,
        PartiallyInvisible = 1UL << 22,
        Cyclone = 1UL << 23,
        Unknown24 = 1UL << 24,
        Unknown25 = 1UL << 25,
        Unknown26 = 1UL << 26,
        Fly = 1UL << 27,
        Unknown28 = 1UL << 28,
        Unknown29 = 1UL << 29,
        LuckyTime = 1UL << 30,
        Pray = 1UL << 31,
        Cursed = 1UL << 32,
        HeavenBless = 1UL << 33,
        TopGuild = 1UL << 34,
        TopDep = 1UL << 35,
        MonthPk = 1UL << 36,
        WeekPk = 1UL << 37,
        TopWarrior = 1UL << 38,
        TopTro = 1UL << 39,
        TopArcher = 1UL << 40,
        TopWater = 1UL << 41,
        TopFire = 1UL << 42,
        TopNinja = 1UL << 43,
        Unknown44 = 1UL << 44,
        Unknown45 = 1UL << 45,
        Vortex = 1UL << 46,
        FatalStrike = 1UL << 47,
        OrangeHaloGlow = 1UL << 48,
        Unknown49 = 1UL << 49,
        LowVigorUnableToJump = 1UL << 50,
        Riding = 1UL << 50,
        TopSpouse = 1UL << 51,
        SparkleHalo = 1UL << 52,
        NoPotion = 1UL << 53,
        Dazed = 1UL << 54,//no movement
        BlueRestoreAura = 1UL << 55,
        MoveSpeedRecovered = 1UL << 56,
        SuperShieldHalo = 1UL << 57,
        HUGEDazed = 1UL << 58,//no movement
        IceBlock = 1UL << 59, //no movement
        Confused = 1UL << 60,//reverses movement
        Unknown61 = 1UL << 61,
        Unknown62 = 1UL << 62,
        Unknown63 = 1UL << 63
    }
}
