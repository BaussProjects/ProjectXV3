﻿//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Calculations
{
	/// <summary>
	/// Calculations based on levels.
	/// </summary>
	public class LevelCalculations
	{
		/// <summary>
		/// The prof exp table.
		/// </summary>
		static uint[] ProfExp = new uint[21] { 0, 1200, 68000, 250000, 640000, 1600000, 4000000,
			10000000, 22000000, 40000000, 90000000, 95000000, 142500000, 213750000, 320625000,
			480937500, 721406250, 1082109375, 1623164063, 2100000000, 0 };
		
		/// <summary>
		/// The level exp table.
		/// </summary>
		static ulong[] LevelExp = new ulong[] { 0, 120, 180, 240, 360, 600, 960, 1200, 2400, 3600,
 8400, 12000, 14400, 18000, 21600, 22646, 32203, 37433, 47556, 56609, 68772, 70515,
 75936, 97733, 114836, 120853, 123981, 126720, 145878, 173436, 197646, 202451, 212160,
 244190, 285823, 305986, 312864, 324480, 366168, 433959, 460590, 506738, 569994, 728527,
 850829, 916479, 935118, 940800, 1076593, 1272780, 1357994, 1384861, 1478400, 1632438,
 1903104, 2066042, 2104924, 1921085, 2417202, 2853462, 3054574, 3111217, 3225600, 3810962,
 4437896, 4880605, 4970962, 5107200, 5652518, 6579162, 6877991, 7100700, 7157657, 9106860,
 10596398, 11220549, 11409192, 11424000, 12882952, 15172807, 15896990, 16163799, 16800000,
 19230280, 22365208, 23819312, 24219528, 24864000, 27200077, 32033165, 33723801, 34291317,
 34944000, 39463523, 45878567, 48924236, 49729220, 51072000, 55808379, 64870058, 68391931,
 69537026, 76422968, 96950789, 112676755, 120090482, 121798280, 127680000, 137446887,
 193715970, 408832150, 454674685, 461125885, 469189885, 477253885, 480479485, 485317885,
 493381885, 580580046, 717424987, 564548116, 677457740, 812949288, 975539145, 1170646974,
 1404776369, 1685731643, 2022877971, 2147283647, 2147283647, 8589134588, 25767403764,
 77302211292, 231906633876, 347859950814, 521789926221, };
		
		/// <summary>
		/// Gets the prof exp based on a level.
		/// </summary>
		/// <param name="Level">The level.</param>
		/// <returns>Returns the required exp.</returns>
		public static uint GetProfExperience(byte Level)
		{
			return ProfExp[Level];
		}
		
		/// <summary>
		/// Gets the level exp based on a level.
		/// </summary>
		/// <param name="Level">The level.</param>
		/// <returns>Returns the required exp.</returns>
		public static ulong GetLevelExperience(byte Level)
		{
			return LevelExp[Level];
		}
		
		public static ulong GetRangeExperience(byte StartLevel, byte EndLevel)
		{
			ulong exp = 0;
			for (byte i = StartLevel; i <= EndLevel; i++)
				exp += GetLevelExperience(i);
			if (exp > 0)
				exp /= Core.NumericConst.ExpRate;
			return exp;
		}
	}
}
