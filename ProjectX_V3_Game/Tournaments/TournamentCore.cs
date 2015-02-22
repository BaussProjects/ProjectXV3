//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;
using System.Linq;
using ProjectX_V3_Lib.ThreadSafe;

namespace ProjectX_V3_Game.Tournaments
{
	/// <summary>
	/// Description of TournamentCore.
	/// </summary>
	public class TournamentCore
	{
		private static ConcurrentDictionary<string, TournamentBase> TournamentPool;
		public static ConcurrentDictionary<string, TournamentBase> CurrentPool;
		
		static TournamentCore()
		{
			TournamentPool = new ConcurrentDictionary<string, TournamentBase>();
			CurrentPool = new ConcurrentDictionary<string, TournamentBase>();
		}
		
		public static void StartPool()
		{
			// ADD TOURNAMENTS HERE ...
			TournamentPool.TryAdd("LastManStanding", new LastManStanding());

			new System.Threading.Thread(Run).Start();
		}
		
		static void Run()
		{
			while (true)
			{
				try
				{
					foreach (TournamentBase Tournament in TournamentPool.Values)
					{
						if (Tournament.StartHours.Contains(DateTime.Now.Hour) && DateTime.Now.Minute == Tournament.StartMinute)
						{
							try
							{
								Tournament.Start();
								CurrentPool.TryAdd(Tournament.Name, Tournament);
							}
							catch { }
							System.Threading.Thread.Sleep(60000);
							
							if (CurrentPool.ContainsKey(Tournament.Name))
							{
								TournamentBase rT;
								CurrentPool.TryRemove(Tournament.Name, out rT);
								try
								{
									int EndTime = Tournament.MeasureEndTime();
									Tournament.Send();
									
									System.Threading.Thread.Sleep(EndTime);
									Tournament.End();
								}
								catch { System.Threading.Thread.Sleep(2000); }
							}
						}
					}
				}
				catch { }
				System.Threading.Thread.Sleep(100);
			}
		}
		public static bool SignedUp(Entities.GameClient client, string Tournament, out bool AlreadySigned)
		{
			AlreadySigned = false;
			if (!CurrentPool.ContainsKey(Tournament))
				return false;
			TournamentBase tournament;
			if (!CurrentPool.TryGetValue(Tournament, out tournament))
				return false;
			return tournament.SignUp(client, out AlreadySigned);
		}
	}
}
