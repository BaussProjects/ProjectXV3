//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;

namespace ProjectX_V3_Game.Threads
{
	/// <summary>
	/// Description of BroadcastThread.
	/// </summary>
	public class BroadcastThread
	{
		private static ConcurrentQueue<Packets.MessagePacket> Broadcasts;
		private static Packets.MessagePacket LastBroadcast;
		static BroadcastThread()
		{
			Broadcasts = new ConcurrentQueue<ProjectX_V3_Game.Packets.MessagePacket>();
		}
		public static void AddBroadcast(Packets.MessagePacket broadcast)
		{
			Broadcasts.Enqueue(broadcast);
		}
		
		public static Packets.MessagePacket GetLastBroadcast()
		{
			if (LastBroadcast == null)
					return null;
				return new Packets.MessagePacket(new ProjectX_V3_Lib.Network.DataPacket(LastBroadcast));
		}
		
		public static void Handle()
		{
			if (Broadcasts.Count > 0)
			{
				Packets.MessagePacket broadcast;
				if (Broadcasts.TryDequeue(out broadcast))
				{
					Packets.Message.MessageCore.SendGlobalMessage(broadcast);
					LastBroadcast = broadcast;
				}
				broadcast.Dispose();
			}
			System.Threading.Thread.Sleep(Core.TimeIntervals.BroadcastWaitTime);
		}
	}
}
