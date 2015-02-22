//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Enums
{
	/// <summary>
	/// The different types of updates the UpdatePacket can perform.
	/// </summary>
	public enum UpdateType : uint
    {
        None = unchecked((uint)-1),
        HP = 0,
        MaxHitpoints = 1,
        Mana = 2,
        MaxMana = 3,
        Money = 4,
        Experience = 5,
        PkPt = 6,
        Job = 7,
        Stamina = 8,
        WarehouseMoney = 9,
        Stats = 10,
        Mesh = 11,
        Level = 12,
        Spirit = 13,
        Vitality = 14,
        Strength = 15,
        Agility = 16,
        HeavensBlessing = 17,
        DoubleExpTimer = 18,
        CursedTimer = 20,
        Reborn = 22,
        StatusEffect = 25,
        Hair = 26,
        XPPct = 27,
        LuckyTimeTimer = 28,
        CP = 29,
        OnlineTraining = 31,
        ExtraBattlePower = 36,
        Merchant = 38,
        VIPLevel = 39,
        QuizPoints = 40,
        EnlightPoints = 41,
        BonusBP = 44,
        BoundCp = 45,
        AzureShield = 49,
    }
}
