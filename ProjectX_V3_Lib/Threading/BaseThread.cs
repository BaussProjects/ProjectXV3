using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ProjectX_V3_Lib.Threading
{
	/// <summary>
	/// A delegate used in the thread.
	/// </summary>
	public delegate void ThreadAction();

	/// <summary>
	/// This class is a base thread for multi-threading.
	/// </summary>
	[Serializable()]
	public class BaseThread
	{
		/// <summary>
		/// The running thread.
		/// </summary>
		private ThreadStart ThreadRun
		{
			get
			{
				return new ThreadStart(delegate
				                       {
				                       	while (true)
				                       	{
				                       		lasttime = DateTime.Now;
				                       		try
				                       		{
				                       			Action.Invoke();
				                       		}
				                       		catch { }
				                       		Thread.Sleep(Interval);
				                       	}
				                       });
			}
		}

		/// <summary>
		/// The checking thread.
		/// </summary>
		private ThreadStart ThreadCheck
		{
			get
			{
				return new ThreadStart(delegate
				                       {
				                       	while (true)
				                       	{
				                       		if (DateTime.Now >= lasttime2.AddMilliseconds((2500)))
				                       		{
				                       			lasttime2 = DateTime.Now;

				                       			if (DateTime.Now >= lasttime.AddMilliseconds(5000 + Interval) || !running_thread.IsAlive)
				                       			{
				                       				if (AllowReset)
				                       				{
				                       					Console.WriteLine("Restarting thread {0}", ThreadName);
				                       					Stop();
				                       					running_thread = new Thread(ThreadRun);
				                       					running_thread.Start();
				                       				}
				                       			}
				                       		}
				                       		Thread.Sleep(2000);
				                       	}
				                       });
			}
		}

		private Thread running_thread;
		private Thread check_thread;
		private DateTime lasttime = DateTime.Now;
		private DateTime lasttime2 = DateTime.Now;
		private bool AllowReset = false;
		private ThreadAction Action;
		private int Interval;

		/// <summary>
		/// Gets the thread name.
		/// </summary>
		public string ThreadName
		{
			get
			{
				return running_thread.Name;
			}
		}

		/// <summary>
		/// Creating a new instance of ConquerOnline.MultiThreading.BaseThread.
		/// </summary>
		/// <param name="Action">The thread action.</param>
		/// <param name="Interval">The interval.</param>
		/// <param name="ThreadName">The thread name.</param>
		public BaseThread(ThreadAction Action, int Interval, string ThreadName)
		{
			this.Action = Action;
			this.Interval = Interval;
			running_thread = new Thread(ThreadRun);
			running_thread.Name = ThreadName;
			check_thread = new Thread(ThreadCheck);
		}

		
		/// <summary>
		/// Starts the thread.
		/// </summary>
		
		public void Start()
		{
			running_thread.Start();
			check_thread.Start();

			if (!string.IsNullOrWhiteSpace(ThreadName))
				Console.WriteLine("Started thread: {0}.", ThreadName);
		}

		/// <summary>
		/// Stops the thread.
		/// </summary>
		public void Stop()
		{
			try
			{
				running_thread.Abort();
			}
			catch { }
		}

		/// <summary>
		/// Forcing a stop on both the checking thread and running thread.
		/// </summary>
		public void ForceStop()
		{
			try
			{
				check_thread.Abort();
			}
			catch { }
			Stop();
		}

		~BaseThread()
		{
			ForceStop();
		}
	}
}
