using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace ProjectX_V3_Lib.Threading
{
	internal class DelayedAction
	{
		internal DateTime allowedTime;
		internal ThreadAction threadAction;
		internal uint actionID;
		internal int repeat;
		internal int repeated = 0;
	}

	/// <summary>
	/// This class is used to handle delayed tasks.
	/// </summary>
	public class DelayedTask
	{
		/// <summary>
		/// The thread wrapping around all the tasks.
		/// </summary>
		private static BaseThread baseThread;

		/// <summary>
		/// The collection of all the threading objects and their tasks.
		/// </summary>
		private static ConcurrentDictionary<uint, DelayedAction> taskObjects;

		static uint TaskID = 0;
		/// <summary>
		/// The synchronization root.
		/// </summary>
		private static object SyncRoot;
		
		static DelayedTask()
		{
			System.Threading.Interlocked.CompareExchange(ref SyncRoot, new object(), null);
			
			taskObjects = new ConcurrentDictionary<uint, DelayedAction>();
			baseThread = new BaseThread(new ThreadAction(() =>
			                                             {
			                                             	foreach (DelayedAction action in taskObjects.Values)
			                                             	{
			                                             		try
			                                             		{
			                                             			action.threadAction.Invoke();
			                                             		}
			                                             		catch (Exception e) { Console.WriteLine(e.ToString()); }
			                                             	}
			                                             }), 100, "DelayedTask");
			baseThread.Start();
		}

		/// <summary>
		/// Starting a delayed task.
		/// </summary>
		/// <param name="delayedAction">The delayed action.</param>
		/// <param name="waitTime">The timespan before invoking the action. (Milliseconds.)</param>
		/// <param name="repeat">The amount of times the action should be repeated.</param>
		public static uint StartDelayedTask(ThreadAction delayedAction, int waitTime, int repeat = 0)
		{
			DelayedAction taskAction = new DelayedAction();
			lock (SyncRoot)
			{
				taskAction.actionID = TaskID;
				TaskID++;
			}

			taskAction.allowedTime = DateTime.Now.AddMilliseconds(waitTime);
			taskAction.repeat = repeat;
			taskAction.threadAction = new ThreadAction(() =>
			                                           {
			                                           	if (DateTime.Now >= taskAction.allowedTime)
			                                           	{
			                                           		delayedAction.Invoke();
			                                           		if (taskAction.repeated >= taskAction.repeat)
			                                           		{
			                                           			DelayedAction outDA;
			                                           			taskObjects.TryRemove(taskAction.actionID, out outDA);
			                                           		}
			                                           		taskAction.repeated++;
			                                           	}
			                                           });
			taskObjects.TryAdd(taskAction.actionID, taskAction);
			return taskAction.actionID;
		}
		
		public static void Remove(uint actionID)
		{
			DelayedAction raction;
			taskObjects.TryRemove(actionID, out raction);
		}
	}
}
