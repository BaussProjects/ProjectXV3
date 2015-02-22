//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;
using ProjectX_V3_Lib.ThreadSafe;
using System.Linq;
using ProjectX_V3_Lib.Threading;

namespace ProjectX_V3_Game.Entities
{
	/// <summary>
	/// Description of BossCreature.
	/// </summary>
	[Serializable()]
	public class BossCreature : Monster
	{
		private BaseThread AttackThread, MoveThread;
		private BossMonster BossMob;
		private IEntity FixedTarget;
		
		public BossCreature()
			: base()
		{

		}
		
		public void SetData(BossMonster boss, IEntity FixedTarget)
		{
			if (FixedTarget != null)
				this.FixedTarget = FixedTarget;
			
			this.BossMob = boss;
			ProtectedPlayers = boss.ProtectedPlayers;
			
			AttackThread = new BaseThread(Creature_Attack_Thread, 500, "");
			AttackThread.Start();
			
			MoveThread = new BaseThread(Creature_Move_Thread, 500, "");
			MoveThread.Start();
			
			CanRevive = false;
			this.BossMob.Creatures.TryAdd(EntityUID, this);
		}
		public ConcurrentArrayList<Entities.GameClient> ProtectedPlayers;

		private void Creature_Attack_Thread()
		{
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
			
			if (FixedTarget != null)
				this.Target = FixedTarget;
			else
			{
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
		}
		
		private void Creature_Move_Thread()
		{
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
			
			return true;
		}
		
		public void Jump(ushort X, ushort Y)
		{
			// TODO: GeneralData ...
		}
		
		public void Abort()
		{
			Map.LeaveMap(this);
			
			AttackThread.ForceStop();
			MoveThread.ForceStop();
			
			if (BossMob != null)
			{
				BossCreature rCreature;
				BossMob.Creatures.TryRemove(this.EntityUID, out rCreature);
			}
		}
	}
}
