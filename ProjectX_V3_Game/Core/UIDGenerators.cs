//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;

namespace ProjectX_V3_Game.Core
{
	/// <summary>
	/// Description of UIDGenerators.
	/// If this causes dead-locks then uncomment the copy methods in Monster / Item, however UID generating will cause problems for mobs atm. using that
	/// Will look the actual error up for it later
	/// </summary>
	public class UIDGenerators
	{
		public static uint GetItemUID()
		{
			lock (SyncRoot)
			{
				return ItemUID++;
			}
		}

		public static uint GetMonsterUID()
		{
			lock (SyncRoot2)
			{
				return MonsterUID++;
			}
		}
		
		public static uint GetGuildUID()
		{
			lock (SyncRoot3)
			{
				return GuildUID++;
			}
		}
		
		public static uint GetDynamicMapUID()
		{
			lock (SyncRoot4)
			{
				return DynamicMapID++;
			}
		}
		
		public static uint GetActionUID()
		{
			lock (SyncRoot5)
			{
				return ActionUID++;
		
			}
		}
		
		public static uint GetBotUID()
		{
			lock (SyncRoot6)
			{
				return BotUID++;
		
			}
		}
		
		static UIDGenerators()
		{
			System.Threading.Interlocked.CompareExchange(ref SyncRoot, new object(), null);
			System.Threading.Interlocked.CompareExchange(ref SyncRoot2, new object(), null);
			System.Threading.Interlocked.CompareExchange(ref SyncRoot3, new object(), null);
			System.Threading.Interlocked.CompareExchange(ref SyncRoot4, new object(), null);
			System.Threading.Interlocked.CompareExchange(ref SyncRoot5, new object(), null);
			System.Threading.Interlocked.CompareExchange(ref SyncRoot6, new object(), null);
		}
		
		static uint MonsterUID = 400001;
		static uint ItemUID = 1;
		static uint GuildUID = 1;
		static uint DynamicMapID = 1;
		static uint ActionUID = 1;
		static uint BotUID = 1;
		
		/// <summary>
		/// The synchronization root.
		/// </summary>
		private static object SyncRoot;
		/// <summary>
		/// The synchronization root.
		/// </summary>
		private static object SyncRoot2;
		/// <summary>
		/// The synchronization root.
		/// </summary>
		private static object SyncRoot3;
		/// <summary>
		/// The synchronization root.
		/// </summary>
		private static object SyncRoot4;
		/// <summary>
		/// The synchronization root.
		/// </summary>
		private static object SyncRoot5;
		/// <summary>
		/// The synchronization root.
		/// </summary>
		private static object SyncRoot6;
	}
}
