//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;

namespace ProjectX_V3_Game.Threads
{
	/// <summary>
	/// Description of ArenaThread.
	/// </summary>
	public class ArenaThread
	{
		public static ConcurrentDictionary<uint, Entities.GameClient> Clients;
		static ArenaThread()
		{
			Clients = new ConcurrentDictionary<uint, ProjectX_V3_Game.Entities.GameClient>();
		}
		
		public static void Handle()
		{
			foreach (Entities.GameClient client in Clients.Values)
			{

			}
		}
	}
}
