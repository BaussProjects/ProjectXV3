//Project by BaussHacker aka. L33TS
using System;
using ProjectX_V3_Lib.ThreadSafe;
using System.Drawing;

namespace ProjectX_V3_Game.Entities.AdvancedAI
{
	public class SoulWarsAI
	{
		private static Selector<uint, string, AIBot> BlueTeamBots;
		private static Selector<uint, string, AIBot> RedTeamBots;
		private static Point BlueStatuePoint, RedStatuePoint, SoulStatuePoint;
		
		static SoulWarsAI()
		{
			BlueTeamBots = new Selector<uint, string, AIBot>();
			RedTeamBots = new Selector<uint, string, AIBot>();
			
			BlueStatuePoint = new Point(101, 196);
			RedStatuePoint = new Point(149, 52);
			SoulStatuePoint = new Point(125, 124);
		}
		
		public static void AddBot(bool BlueTeam)
		{
			
		}
		
		public static void BeginBots()
		{
			
		}
		
		public static void DoAction(AIBot bot)
		{
			
		}
		
		private static void Action_A(AIBot bot)
		{
			
		}
		
		private static void Action_B(AIBot bot)
		{
			
		}
		
		private static void Action_C(AIBot bot)
		{
			
		}
	}
}
