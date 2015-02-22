//Project by BaussHacker aka. L33TS
using System;
using System.Threading;
using System.Collections.Concurrent;

namespace ProjectX_V3_Game.Threads
{
	/// <summary>
	/// Description of ActionThread.
	/// </summary>
	public class ActionThread
	{
		public class ThreadAction
		{
			public ThreadStart action;
			public uint ActionID;
			public int Interval;
			public DateTime LastExecute;
		}
		
		public static ConcurrentDictionary<uint, ThreadAction> Actions;
		static ActionThread()
		{
			Actions = new ConcurrentDictionary<uint, ThreadAction>();
		}
		
		public static ThreadAction AddAction(ThreadStart tAction, int Interval)
		{
			ThreadAction action = new ActionThread.ThreadAction();
			action.ActionID = Core.UIDGenerators.GetActionUID();
			action.LastExecute = DateTime.Now;
			action.Interval = Interval;
			action.action = tAction;
			Actions.TryAdd(action.ActionID, action);
			return action;
		}
		
		public static void Handle()
		{
			foreach (ThreadAction action in Actions.Values)
			{
				try
				{
					if (DateTime.Now >= action.LastExecute.AddMilliseconds(action.Interval))
					{
						action.LastExecute = DateTime.Now;
						action.action.Invoke();
					}
				}
				catch { }
			}
		}
	}
}
