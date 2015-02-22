//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Enums
{
	/// <summary>
	/// The different classes
	/// </summary>
	public enum Class : byte
	{
		Unknown = 0,
		InternTrojan = 10,
		Trojan = 11,
		VeteranTrojan = 12,
		TigerTrojan = 13,
		DragonTrojan = 14,
		TrojanMaster = 15,
		InternWarrior = 20,
		Warrior = 21,
		BrassWarrior = 22,
		SilverWarrior = 23,
		GoldWarrior = 24,
		WarriorMaster = 25,
		InternArcher = 40,
		Archer = 41,
		EagleArcher = 42,
		TigerArcher = 43,
		DragonArcher = 44,
		ArcherMaster = 45,
		InternNinja = 50,
		Ninja = 51,
		MiddleNinja = 52,
		DarkNinja = 53,
		NinjaMaster = 54,
		InternMonk_InternSaint = 60,
		Monk_Saint = 61,
		DhyanaMonk_DhyanaSaint = 62,
		DharmaMonk_DharmaSaint = 63,
		PrajnaMonk_PrajnaSaint = 64,
		NirvanaMonk_NirvanaSaint = 65,
		InternTaoist = 100,
		Taoist = 101,
		WaterTaoist = 132,
		WaterWizard = 133,
		WaterMaster = 134,
		WaterSaint = 135,
		FireTaoist = 142,
		FireWizard = 143,
		FireMaster = 144,
		FireSaint = 145,
		
		// custom ones below, never send to client!
		// used for ais
		
		// monster classes
		MonsterBaby = 200,
		Monster = 201,
		MonsterKing = 202,
		MonsterBoss = 203,
		
		// duelist | inheriting TrojanMaster as well...
		DuelistEasy = 210,
		Duelist = 211,
		DuelistMedium = 212,
		DuelistHard = 213,
		DuelistInsane = 214,
		DuelistImpossible = 215, // teleports
		
		// anything other eg. sob
		Other = 225
	}
}
