//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Threading;

namespace ProjectX_V3_Game.Threads
{
	/// <summary>
	/// Handling all threads.
	/// </summary>
	public class GlobalThreads
	{
		public static void Start()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Starting the global threads...");
			
			new BaseThread(PlayerThread.Handle, 100, "PlayerThread").Start();
			
			new BaseThread(BroadcastThread.Handle, 500, "BroadcastThread").Start();
			
			new BaseThread(DynamicMapThread.Handle, 10000, "DynamicMapThread").Start();
			
			#if WEATHER
			new BaseThread(WeatherThread.Handle, 100, "WeatherThread").Start();
			#endif
						
			new BaseThread(BotThread.Handle, 500, "DuelBotThread").Start();
			
			//new BaseThread(SystemThreads.HandleMessages, 300000, "SystemMessageThread").Start();

			#if TOURNAMENTS
			Tournaments.TournamentCore.StartPool();
			#endif
			
			MonsterThread.StartThreads();
					
			new BaseThread(ActionThread.Handle, 100, "ActionThread").Start();
			
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Started global threads...");
			Console.ResetColor();
		}
	}
}
