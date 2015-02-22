//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;
using ProjectX_V3_Lib.ThreadSafe;
using System.Linq;
using ProjectX_V3_Lib.Threading;

namespace ProjectX_V3_Game.Entities
{
	/// <summary>
	/// Description of BossMonster.
	/// </summary>
	[Serializable()]
	public abstract class BossMonster : Monster
	{

		public BossMonster()
			: base()
		{
			BossSkills = new ConcurrentDictionary<ushort, Data.AdvancedSkill>();
			BossAlwaysSkills = new ConcurrentDictionary<ushort, ProjectX_V3_Game.Data.AdvancedSkill>();
			
			Creatures = new ConcurrentDictionary<uint, BossCreature>();
			ProtectedPlayers = new ConcurrentArrayList<GameClient>();
			
			CanRevive = false;
			IsSpawned = false;
		}
		
		public Threads.BossThread Thread;
		
		protected ConcurrentDictionary<ushort, Data.AdvancedSkill> BossSkills;
		protected ConcurrentDictionary<ushort, Data.AdvancedSkill> BossAlwaysSkills;
		public ConcurrentArrayList<Entities.GameClient> ProtectedPlayers;
		public DateTime BossSkillTime = DateTime.Now;
		public int BossSkillSpeed;
		private bool IsSpawned;
		
		protected ConcurrentArrayList<Entities.GameClient> GetPossibleTargets()
		{
			ConcurrentArrayList<Entities.GameClient> Targets = new ConcurrentArrayList<GameClient>();
			if (Screen.MapObjects.Count == 0)
				return Targets;
			
			foreach (Maps.IMapObject MapObject in Screen.MapObjects.Values)
			{
				if (ProtectedPlayers.Contains((MapObject as Entities.GameClient)))
					continue;
				if (MapObject is Entities.GameClient)
				{
					if ((MapObject as Entities.GameClient).Alive)
						Targets.Add(MapObject as Entities.GameClient);
				}
			}
			return Targets;
		}

		public void HandleAttack()
		{
			if (!IsSpawned)
				return;
			
			if (this.Target != null)
			{
				if (this.Target.Alive)
				{
					if (this.Screen.MapObjects.ContainsKey(this.Target.EntityUID))
					{
						if (Core.Screen.GetDistance(this.X, this.Y, this.Target.X, this.Target.Y) <= this.AttackRange &&
						    DateTime.Now >= this.AttackTime)
						{
							this.AttackTime = DateTime.Now.AddMilliseconds(
								ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(
									this.AttackSpeed, this.AttackSpeed * 3));
							
							#region physical attack
							using (var interact = new Packets.InteractionPacket())
							{
								interact.Action = Enums.InteractAction.Attack;
								interact.EntityUID = this.EntityUID;
								interact.TargetUID = this.Target.EntityUID;
								interact.UnPacked = true;
								interact.X = this.Target.X;
								interact.Y = this.Target.Y;
								Packets.Interaction.Battle.Physical.Handle(this, interact);
							}
							#endregion
							
							return;
						}
					}
				}
			}
			
			this.Target = null;
			foreach (Maps.IMapObject obj in this.Screen.MapObjects.Values)
			{
				if (obj is Entities.GameClient)
				{
					if (ProtectedPlayers.Contains((obj as Entities.GameClient)))
						continue;
					
					this.Target = obj as Entities.IEntity;
					if (!this.Target.Alive)
					{
						this.Target = null;
						continue;
					}
					break;
				}
			}
		}
		
		public void HandleSkills()
		{
			if (!IsSpawned)
				return;
			
			if (BossSkills.Count > 0 && Creatures.Count == 0)
			{
				ConcurrentArrayList<Entities.GameClient> PossibleTargets = GetPossibleTargets();
				if (PossibleTargets.Count > 0)
				{
					if (DateTime.Now >= BossSkillTime)
					{
						BossSkillTime = DateTime.Now.AddMilliseconds(ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(BossSkillSpeed * 2));
						
						ushort[] rSkills = BossSkills.Keys.ToArray();
						if (rSkills.Length > 0)
						{
							ushort[] Skills = new ushort[rSkills.Length * 4]; // fixing random index issue with .NET's random ...
							Array.Copy(rSkills, 0, Skills, 0, rSkills.Length);
							Array.Copy(rSkills, 0, Skills, rSkills.Length, rSkills.Length);
							ushort Skill = Skills[ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(Skills.Length - 1)];
							
							BossSkills[Skill].Use(this, PossibleTargets.ToDictionary().Values.ToArray());
						}
						
						foreach (Data.AdvancedSkill skill in BossAlwaysSkills.Values)
						{
							skill.Use(this, PossibleTargets.ToDictionary().Values.ToArray());
						}
					}
				}
			}
		}
		
		public void HandleMovement()
		{
			if (!IsSpawned)
				return;
			
			
			if (!Alive)
				return;
			
			try
			{
				if (Target != null)
				{
					if (DateTime.Now >= this.MoveTime)
					{
						if (Core.Screen.GetDistance(this.X, this.Y, this.Target.X, this.Target.Y) >= this.ViewRange)
						{
							byte dir = (byte)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(0, 8);
							Maps.MapPoint movepoint = Packets.MovementPacket.CreateDirectionPoint(this.X, this.Y, dir);
							if (this.Map.ValidMoveCoord<Entities.Monster>(movepoint.X, movepoint.Y))
							{
								this.X = movepoint.X;
								this.Y = movepoint.Y;
								this.Direction = dir;
								using (var movepacket = new Packets.MovementPacket())
								{
									movepacket.EntityUID = this.EntityUID;
									movepacket.TimeStamp = ProjectX_V3_Lib.Native.Winmm.timeGetTime();
									movepacket.Direction = (uint)dir;
									movepacket.WalkMode = Enums.WalkMode.Run;
									this.Screen.UpdateScreen(movepacket);
								}
								this.MoveTime = DateTime.Now.AddMilliseconds(this.MoveSpeed * 5);
							}
						}
						else
						{
							byte dir = (byte)Core.Screen.GetFacing(Core.Screen.GetAngle(this.X, this.Y, this.Target.X, this.Target.Y));
							Maps.MapPoint movepoint = Packets.MovementPacket.CreateDirectionPoint(this.X, this.Y, dir);
							if (this.Map.ValidMoveCoord<Entities.Monster>(movepoint.X, movepoint.Y))
							{
								this.X = movepoint.X;
								this.Y = movepoint.Y;
								this.Direction = dir;
								using (var movepacket = new Packets.MovementPacket())
								{
									movepacket.EntityUID = this.EntityUID;
									movepacket.TimeStamp = ProjectX_V3_Lib.Native.Winmm.timeGetTime();
									movepacket.Direction = (uint)dir;
									movepacket.WalkMode = Enums.WalkMode.Run;
									this.Screen.UpdateScreen(movepacket);
								}
							}
							int nextmove = ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(this.MoveSpeed, this.MoveSpeed * 2);
							this.MoveTime = DateTime.Now.AddMilliseconds(nextmove);
						}
					}
				}
			}
			catch
			{
				
			}
		}
		
		public void HandleImmunity()
		{
			if (!IsSpawned)
				return;
			
			if (!Alive)
				return;
			
			if (!CanBeAttacked)
			{
				using (var str = new Packets.StringPacket(new Packets.StringPacker("bossimmunity")))
				{
					str.Action = Enums.StringAction.RoleEffect;
					str.Data = EntityUID;
					Screen.UpdateScreen(str);
				}
			}
		}
		
		public bool CanBeAttacked
		{
			get
			{
				return IsSpawned && Creatures.Count == 0;
			}
		}
		
		public ConcurrentDictionary<uint, BossCreature> Creatures;
		
		public abstract void ON_TELEPORTED();
		
		public void Teleport(ushort X, ushort Y)
		{
			this.X = X;
			this.Y = Y;
			Screen.UpdateScreen(null, false);
		}

		public bool Teleport(ushort mapid, ushort x, ushort y)
		{
			if (!Core.Kernel.Maps.Contains(mapid))
				return false;
			Maps.Map tmap;
			if (!Core.Kernel.Maps.TrySelect(mapid, out tmap))
				return false;
			
			return Teleport(tmap, x, y);
		}

		public bool Teleport(Maps.MapPoint point)
		{
			return Teleport(point.MapID, point.X, point.Y);
		}
		
		public bool Teleport(Maps.Map map, ushort x, ushort y)
		{
			if (!Core.Kernel.Maps.Contains(map.MapID))
				return false;
			
			if (Map != null)
			{
				Map.LeaveMap(this);
			}
			
			if (!map.EnterMap(this))
				return false;
			
			Target = null;

			this.X = x;
			this.Y = y;
			Screen.UpdateScreen(null, false);
			ON_TELEPORTED();
			
			return true;
		}
		
		public void Jump(ushort X, ushort Y)
		{
			// TODO: GeneralData ...
		}
		
		private bool permRemoved = false;
		public void RespawnBoss()
		{
			if (permRemoved)
				return;
			
			foreach (BossCreature creature in Creatures.Values)
				creature.Abort();
			Creatures.Clear();
			IsSpawned = true;
			StatusFlag1 = 0;
			HP = MaxHP;
			MP = MaxMP;
			Alive = true;
			Direction = (byte)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(0, 8);
			Screen.UpdateScreen(null);
		}
		public void AbortBoss(bool PERM_REMOVE)
		{
			permRemoved = PERM_REMOVE;
			IsSpawned = false;
			Map.LeaveMap(this);
			
			foreach (BossCreature creature in Creatures.Values)
				creature.Abort();
			Creatures.Clear();
			
			if (PERM_REMOVE)
			{
				Thread.RemoveBoss(this);
			}
			
			if (Map != null && PERM_REMOVE)
				Map.LeaveMap(this);
			
			Screen.UpdateScreen(null);
		}
		
		public abstract void ON_DEATH(Entities.GameClient Killer);
	}
}
