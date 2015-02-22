//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Enums
{
	/// <summary>
	/// The different types of actions ItemPacket handles.
	/// </summary>
	public enum ItemAction : uint
	{
		/// <summary>
		/// [00]
		/// </summary>
		None = 0,

		/// <summary>
		/// [01]
		/// </summary>
		Buy = 1,

		/// <summary>
		/// [02]
		/// </summary>
		Sell = 2,

		/// <summary>
		/// [03]
		/// </summary>
		Remove = 3,

		/// <summary>
		/// [04]
		/// </summary>
		Use = 4,

		/// <summary>
		/// [05]
		/// </summary>
		Equip = 5,

		/// <summary>
		/// [06]
		/// </summary>
		Unequip = 6,

		/// <summary>
		/// [07]
		/// </summary>
		SplitItem = 7,

		/// <summary>
		/// [08]
		/// </summary>
		CombineItem = 8,

		/// <summary>
		/// [09]
		/// </summary>
		QueryMoneySaved = 9,

		/// <summary>
		/// [10]
		/// </summary>
		SaveMoney = 10,

		/// <summary>
		/// [11]
		/// </summary>
		DrawMoney = 11,

		/// <summary>
		/// [12]
		/// </summary>
		DropMoney = 12,

		/// <summary>
		/// [13]
		/// </summary>
		SpendMoney = 13,

		/// <summary>
		/// [14]
		/// </summary>
		Repair = 14,

		/// <summary>
		/// [15]
		/// </summary>
		RepairAll = 15,

		/// <summary>
		/// [16]
		/// </summary>
		Ident = 16,

		/// <summary>
		/// [17]
		/// </summary>
		Durability = 17,

		/// <summary>
		/// [18]
		/// </summary>
		DropEquipment = 18,

		/// <summary>
		/// [19]
		/// </summary>
		Improve = 19,

		/// <summary>
		/// [20]
		/// </summary>
		Uplev = 20,

		/// <summary>
		/// [21]
		/// </summary>
		BoothQuery = 21,

		/// <summary>
		/// [22]
		/// </summary>
		BoothAdd = 22,

		/// <summary>
		/// [23]
		/// </summary>
		BoothDelete = 23,

		/// <summary>
		/// [24]
		/// </summary>
		BoothBuy = 24,

		/// <summary>
		/// [25]
		/// </summary>
		SynchroAmount = 25,

		/// <summary>
		/// [26]
		/// </summary>
		Fireworks = 26,

		/// <summary>
		/// [27]
		/// </summary>
		Ping = 27,

		/// <summary>
		/// [28]
		/// </summary>
		Enchant = 28,

		/// <summary>
		/// [29]
		/// </summary>
		BoothAddCP = 29,

		/// <summary>
		/// [33]
		/// </summary>
		PkItemRedeem = 33,
		
		/// <summary>
		/// [37]
		/// </summary>
		Drop = 37,

		Bless = 40,
		ActivateAccessory = 41,

		SocketEquipment = 43,

		/// <summary>
		/// [46]
		/// </summary>
		DisplayGears = 46,

		/// <summary>
		/// [52]
		/// </summary>
		RequestItemTooltip = 52,
	}
}
