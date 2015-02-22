//Project by BaussHacker aka. L33TS
using System;
using ProjectX_V3_Lib.Threading;
using System.Collections.Concurrent;

namespace ProjectX_V3_Game.Threads
{
	/// <summary>
	/// Description of BossThread.
	/// </summary>
	public class BossThread
	{
		private static int BossCount;
		private static int BossThreadCount = 0;
		private static ConcurrentDictionary<int, BossThread> Threads;
		static BossThread()
		{
			Threads = new ConcurrentDictionary<int, BossThread>();
		}
		public static void AddToBossThread(ProjectX_V3_Game.Entities.BossMonster mob)
		{
			if (BossCount > 1000 || Threads.Count == 0)
			{
				BossCount = 0;
				BossThreadCount++;
				if (!Threads.TryAdd(BossThreadCount, new BossThread()))
					throw new Exception("Could not start boss thread...");
			}
			BossThread thread;
			if (!Threads.TryGetValue(BossThreadCount, out thread))
				throw new Exception("Could not get boss thread...");
			if (!thread.SpawnedBosses.TryAdd(mob.EntityUID, mob))
			{
				throw new Exception("Could not add boss to the boss thread...");
			}
			else
				mob.Thread = thread;
			
			BossCount++;
		}
		
		private ConcurrentDictionary<uint, Entities.BossMonster> SpawnedBosses;
		public void RemoveBoss(Entities.BossMonster mob)
		{
			Entities.BossMonster rBoss;
			SpawnedBosses.TryRemove(mob.EntityUID, out rBoss);
		}
		
		public BossThread()
		{
			SpawnedBosses = new ConcurrentDictionary<uint, ProjectX_V3_Game.Entities.BossMonster>();
			
			Start();
		}
		
		public void Start()
		{
			new BaseThread(HandleImmune, 1500, "Boss-Immune Thread").Start();
			new BaseThread(HandleMisc, 500, "Boss-Misc Thread").Start();
		}
		
		void HandleImmune()
		{
			foreach (Entities.BossMonster boss in SpawnedBosses.Values)
			{
				boss.HandleImmunity();
			}
		}
		void HandleMisc()
		{
			foreach (Entities.BossMonster boss in SpawnedBosses.Values)
			{
				boss.HandleAttack();
				boss.HandleMovement();
				boss.HandleSkills();
			}
		}
	}
}
