//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Enums
{
	public enum InteractAction : uint
	{
		None = 0,
		Steal,
		Attack,
		Heal,
		Poison,
		Assassinate = 5,
		Freeze,
		Unfreeze,
		Court,
		Marry,
		Divorce = 10,
		PresentMoney,
		PresentItem,
		SendFlowers,
		Kill = 14,
		JoinGuild = 15,
		AcceptGuildMember,
		KickoutGuildMember,
		PresentPower,
		QueryInfo,
		RushAttack = 20,
		Unknown21,
		AbortMagic,
		ReflectWeapon,
		MagicAttack,
		Unknown = 25,
		ReflectMagic,
		Dash,
		Shoot,
		Quarry,
		Chop = 30,
		Hustle,
		Soul,
		AcceptMerchant,
		CounterKill = 43,
		CounterKillSwitch = 44,
		FatalStrike =45,
		/// <summary>
		/// [46]
		/// Used to request player-player interactions like hug, kiss, etc.
		/// </summary>
		InteractRequest = 46,

		/// <summary>
		/// [47]
		/// Used to confirm player-player interactions like hug, kiss, etc.
		/// </summary>
		InteractConfirm,
		Interact,
		InteractUnknown,
		InteractStop
	}
}
