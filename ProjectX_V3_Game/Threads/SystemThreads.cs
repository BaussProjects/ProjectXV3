//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;

namespace ProjectX_V3_Game.Threads
{
	/// <summary>
	/// Description of SystemThreads.
	/// </summary>
	public class SystemThreads
	{
		public static ConcurrentDictionary<int, string> SystemMessages = new ConcurrentDictionary<int, string>();
		
		public static void HandleMessages()
		{
			string Msg;
			int Index = ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(SystemMessages.Count);
			if (SystemMessages.TryGetValue(Index, out Msg))
			{
				using (var msg = Packets.Message.MessageCore.CreateSystem("ALL", Msg)) // fix server ...
					Packets.Message.MessageCore.SendGlobalMessage(msg);
			}
		}
	}
}
