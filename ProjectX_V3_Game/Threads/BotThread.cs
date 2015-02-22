//Project by BaussHacker aka. L33TS
using System;
using System.Collections.Concurrent;

namespace ProjectX_V3_Game.Threads
{
	/// <summary>
	/// Description of BotThread.
	/// </summary>
	public class BotThread
	{
		public static ConcurrentDictionary<uint, Entities.AIBot> Bots;
		static BotThread()
		{
			Bots = new ConcurrentDictionary<uint, ProjectX_V3_Game.Entities.AIBot>();
		}
		
		public static void Handle()
		{
			foreach (Entities.AIBot bot in Bots.Values)
			{
				try
				{
					bot.HandleJump();
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
				}
			}
		}
	}
}
