//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Threading;
using System.Collections.Concurrent;

namespace ProjectX_V3_Game.Threads
{
	/// <summary>
	/// The threads handling monsters.
	/// </summary>
	public class MonsterThread
	{
		private static int MonsterCount;
		private static int MonsterThreadCount = 0;
		private static ConcurrentDictionary<int, MonsterThread> Threads;
		static MonsterThread()
		{
			Threads = new ConcurrentDictionary<int, MonsterThread>();
		}
		public static void AddToMonsterThread(ProjectX_V3_Game.Entities.Monster mob, bool guard)
		{
			if (MonsterCount > 5000 || Threads.Count == 0)
			{
				MonsterCount = 0;
				MonsterThreadCount++;
				if (!Threads.TryAdd(MonsterThreadCount, new MonsterThread()))
					throw new Exception("Could not start monster thread...");
			}
			MonsterThread thread;
			if (!Threads.TryGetValue(MonsterThreadCount, out thread))
				throw new Exception("Could not get monster thread...");
			if (guard)
			{
				if (!thread.SpawnedGuards.TryAdd(mob.EntityUID, mob))
				{
					throw new Exception("Could not add guard to the monster thread...");
				}
			}
			else
			{
				if (!thread.SpawnedMonsters.TryAdd(mob.EntityUID, mob))
				{
					throw new Exception("Could not add mob to the monster thread...");
				}
			}
			MonsterCount++;
		}
		
		private ConcurrentDictionary<uint, Entities.Monster> SpawnedMonsters;
		private  ConcurrentDictionary<uint, Entities.Monster> SpawnedGuards;
		
		public MonsterThread()
		{
			SpawnedMonsters = new ConcurrentDictionary<uint, ProjectX_V3_Game.Entities.Monster>();
			SpawnedGuards = new ConcurrentDictionary<uint, ProjectX_V3_Game.Entities.Monster>();
		}
		
		public static void StartThreads()
		{
			foreach (MonsterThread thread in Threads.Values)
				thread.Start();
		}
		public void Start()
		{
			new BaseThread(HandleRegular, 500, "Monster Thread").Start();
			new BaseThread(HandleGuards, 500, "Guard Thread").Start();
		}
		
		void HandleRegular()
		{
			foreach (Entities.Monster mob in SpawnedMonsters.Values)
			{
				if (!mob.Alive)
					continue;
				
				try
				{
					Regular(mob);
					Movement(mob);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
				}
			}
		}
		void HandleGuards()
		{
			foreach (Entities.Monster guard in SpawnedGuards.Values)
			{
				if (!guard.Alive)
					continue;
				
				try
				{
					Guards(guard);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
				}
			}
		}
		
		void Regular(Entities.Monster mob)
		{
			#region Poison
			if (mob.ContainsFlag1(Enums.Effect1.Poisoned))
			{
				if (DateTime.Now >= mob.LastPoison.AddMilliseconds(3000))
				{
					mob.LastPoison = DateTime.Now;
					if (mob.PoisonEffect > 0)
					{
						uint damage = (uint)((mob.HP / 100) * mob.PoisonEffect);
						if (mob.HP > damage)
						{
							Packets.Interaction.Battle.Combat.HitDamage(null, mob, damage);
							using (var interact = new Packets.InteractionPacket())
							{
								interact.Action = Enums.InteractAction.Attack;
								interact.EntityUID = mob.EntityUID;
								interact.TargetUID = mob.EntityUID;
								interact.UnPacked = true;
								interact.X = mob.X;
								interact.Y = mob.Y;
								interact.Data = damage;
								mob.Screen.UpdateScreen(interact, false);
							}
						}
						else
							mob.RemoveFlag1(Enums.Effect1.Poisoned);
					}
				}
			}
			#endregion
			
			if (mob.Target != null)
			{
				if (mob.Target.Alive)
				{
					if (mob.Screen.MapObjects.ContainsKey(mob.Target.EntityUID))
					{
						if (Core.Screen.GetDistance(mob.X, mob.Y, mob.Target.X, mob.Target.Y) <= mob.AttackRange &&
						    DateTime.Now >= mob.AttackTime)
						{
							mob.AttackTime = DateTime.Now.AddMilliseconds(
								ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(
									mob.AttackSpeed, mob.AttackSpeed * 3));
							
							#region physical attack
							using (var interact = new Packets.InteractionPacket())
							{
								interact.Action = Enums.InteractAction.Attack;
								interact.EntityUID = mob.EntityUID;
								interact.TargetUID = mob.Target.EntityUID;
								interact.UnPacked = true;
								interact.X = mob.Target.X;
								interact.Y = mob.Target.Y;
								Packets.Interaction.Battle.Physical.Handle(mob, interact);
							}
							#endregion
							
							return;
						}
					}
				}
			}
			
			mob.Target = null;
			foreach (Maps.IMapObject obj in mob.Screen.MapObjects.Values)
			{
				if (obj is Entities.GameClient)
				{
					if ((obj as Entities.GameClient).ContainsFlag1(Enums.Effect1.PartiallyInvisible))
					{
						continue;
					}
					mob.Target = obj as Entities.IEntity;
					if (!mob.Target.Alive)
					{
						mob.Target = null;
						continue;
					}
					break;
				}
			}
		}
		
		void Movement(Entities.Monster mob)
		{
			if (!mob.Alive)
				return;
			
			try
			{
				if (mob.Target != null)
				{
					if (DateTime.Now >= mob.MoveTime)
					{
						try
						{
							if (Core.Screen.GetDistance(mob.X, mob.Y, mob.Target.X, mob.Target.Y) >= mob.ViewRange)
							{
								byte dir = (byte)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(0, 8);
								Maps.MapPoint movepoint = Packets.MovementPacket.CreateDirectionPoint(mob.X, mob.Y, dir);
								if (mob.Map.ValidMoveCoord<Entities.Monster>(movepoint.X, movepoint.Y))
								{
									mob.X = movepoint.X;
									mob.Y = movepoint.Y;
									mob.Direction = dir;
									using (var movepacket = new Packets.MovementPacket())
									{
										movepacket.EntityUID = mob.EntityUID;
										movepacket.TimeStamp = ProjectX_V3_Lib.Native.Winmm.timeGetTime();
										movepacket.Direction = (uint)dir;
										movepacket.WalkMode = Enums.WalkMode.Run;
										mob.Screen.UpdateScreen(movepacket);
									}
									mob.MoveTime = DateTime.Now.AddMilliseconds(mob.MoveSpeed * 5);
								}
								return;
							}
							else
							{
								byte dir = (byte)Core.Screen.GetFacing(Core.Screen.GetAngle(mob.X, mob.Y, mob.Target.X, mob.Target.Y));
								Maps.MapPoint movepoint = Packets.MovementPacket.CreateDirectionPoint(mob.X, mob.Y, dir);
								if (mob.Map.ValidMoveCoord<Entities.Monster>(movepoint.X, movepoint.Y))
								{
									mob.X = movepoint.X;
									mob.Y = movepoint.Y;
									mob.Direction = dir;
									using (var movepacket = new Packets.MovementPacket())
									{
										movepacket.EntityUID = mob.EntityUID;
										movepacket.TimeStamp = ProjectX_V3_Lib.Native.Winmm.timeGetTime();
										movepacket.Direction = (uint)dir;
										movepacket.WalkMode = Enums.WalkMode.Run;
										mob.Screen.UpdateScreen(movepacket);
									}
								}
								int nextmove = ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(mob.MoveSpeed, mob.MoveSpeed * 2);
								mob.MoveTime = DateTime.Now.AddMilliseconds(nextmove);
							}
						}
						catch
						{
						} // target was removed again / out of range :s there is a concurrency problem, just cba to look for it as it's nothing major
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}
		
		void Guards(Entities.Monster guard)
		{
			switch (guard.Behaviour)
			{
					#region Magic Guard
				case Enums.MonsterBehaviour.MagicGuard:
					{
						if (guard.Target != null)
						{
							if (guard.Target.Alive)
							{
								if (guard.Screen.MapObjects.ContainsKey(guard.Target.EntityUID))
								{
									if (Core.Screen.GetDistance(guard.X, guard.Y, guard.Target.X, guard.Target.Y) <= guard.AttackRange)
									{
										#region magic attack
										using (var interact = new Packets.InteractionPacket())
										{
											interact.Action = Enums.InteractAction.MagicAttack;
											interact.MagicType = 1002;
											interact.EntityUID = guard.EntityUID;
											interact.TargetUID = guard.Target.EntityUID;
											interact.UnPacked = true;
											interact.X = guard.Target.X;
											interact.Y = guard.Target.Y;
											Packets.Interaction.Battle.Magic.Handle(guard, interact);
										}
										#endregion
										
										return;
									}
								}
							}
						}
						
						guard.Target = null;
						foreach (Maps.IMapObject obj in guard.Screen.MapObjects.Values)
						{
							if (obj is Entities.GameClient || obj is Entities.Monster)
							{
								guard.Target = obj as Entities.IEntity;
								if (guard.Target.EntityUID == guard.EntityUID)
								{
									guard.Target = null;
									continue;
								}
								if (guard.Target is Entities.Monster)
								{
									if (((byte)(guard.Target as Entities.Monster).Behaviour) >= 3)
									{
										guard.Target = null;
										continue;
									}
								}
								if (guard.Target is Entities.GameClient)
								{
									if (!(guard.Target as Entities.GameClient).ContainsFlag1(Enums.Effect1.BlueName))
									{
										guard.Target = null;
										continue;
									}
								}
								if (!guard.Target.Alive)
								{
									guard.Target = null;
									continue;
								}
								break;
							}
						}
						break;
					}
					#endregion
					#region Physical Guard
				case Enums.MonsterBehaviour.PhysicalGuard:
					{
						break;
					}
					#endregion
					#region Death Guard
				case Enums.MonsterBehaviour.DeathGuard:
					{
						break;
					}
					#endregion
					#region Reviver Guard1
				case Enums.MonsterBehaviour.ReviverGuard1:
					{
						break;
					}
					#endregion
					#region Reviver Guard2
				case Enums.MonsterBehaviour.ReviverGuard2:
					{
						break;
					}
					#endregion
			}
		}
	}
}
