//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.ThreadSafe;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// Description of ArenaMatch.
	/// </summary>
	public class ArenaMatch : BattleClass
	{
		public bool FightingMatch = true;
		
		public ArenaMatch(Entities.GameClient Player1, Entities.GameClient Player2)
		{
			Started = false;
			AcceptedFight1 = false;
			AcceptedFight2 = false;
			Player1.ArenaMatch = this;
			this.Player1 = Player1;
			this.Player2 = Player2;
			Watchers = new ConcurrentArrayList<ProjectX_V3_Game.Entities.GameClient>();
		}
		
		public void BeginTimeOutQueue()
		{
			ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(EndMatch_TimeOut, 60000, 0);
		}
		public ConcurrentArrayList<Entities.GameClient> Watchers;
		
		public Entities.GameClient Player1;
		public Entities.GameClient Player2;
		
		public void Quit(Entities.GameClient Player)
		{
			Player1.Equipments.ClearMask();
			if (Player2 != null)
				Player2.Equipments.ClearMask();
			
			if (Player1 == Player)
			{
				Player1Damage = 0;
				Player2Damage = 1;
			}
			else
			{
				Player1Damage = 1;
				Player2Damage = 0;
			}
			
			Started = true;
			EndMatch();
		}
		
		public uint Player1Damage;
		public uint Player2Damage;
		
		public Maps.DynamicMap ArenaMap;
		public uint DynamicID;
		
		public DateTime MatchStartTime;
		
		public bool Started;
		public bool MatchStarted
		{
			get
			{
				return (Started && DateTime.Now >= MatchStartTime.AddMilliseconds(10000));
			}
		}
		
		public bool AcceptedFight1;
		public bool AcceptedFight2;
		
		public static ArenaMatch CreateMatch(Entities.GameClient Player1, Entities.GameClient Player2)
		{
			return new ArenaMatch(Player1, Player2);
		}
		
		public void BeginMatch()
		{
			ArenaQualifier.PlayerQueue.Remove(Player1);
			ArenaQualifier.PlayerQueue.Remove(Player2);
			
			ArenaMap = Core.Kernel.Maps[700].CreateDynamic(out DynamicID);
			
			ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(EndMatch, 120000, 0);
			
			Player1Damage = 0;
			Player2Damage = 0;
			
			MatchStartTime = DateTime.Now.AddMilliseconds(10000);
			Started = true;

			Player1.HP = Player1.MaxHP;
			Player1.Transformation = 0;
			ushort[] PlayerLoc = GenerateRandomLocation();
			Player1.TeleportDynamic(DynamicID, PlayerLoc[0], PlayerLoc[1]);
			
			Player1.RemoveFlag1(Enums.Effect1.Ghost);
			Player1.RemoveFlag1(Enums.Effect1.Dead);
			Player1.RemoveFlag1(Enums.Effect1.Riding);
			
			Player2.HP = Player2.MaxHP;
			Player2.Transformation = 0;
			PlayerLoc = GenerateRandomLocation();
			Player2.TeleportDynamic(DynamicID, PlayerLoc[0], PlayerLoc[1]);
			
			Player2.RemoveFlag1(Enums.Effect1.Ghost);
			Player2.RemoveFlag1(Enums.Effect1.Dead);
			Player2.RemoveFlag1(Enums.Effect1.Riding);
			
			SendMatch();
			SendCountDown(Player1, Player2);
			SendCountDown(Player2, Player1);
			SendPacket();
			
			Player1.Battle = this;
			Player2.Battle = this;
		}
		
		private ushort[] GenerateRandomLocation()
		{
			ushort X = (ushort)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(35, 70);
			ushort Y = (ushort)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(35, 70);
			return new ushort[] { X, Y };
		}
		
		public void EndMatch_TimeOut()
		{
			if (!Started)
				EndMatch();
		}
		public void GiveUp(Entities.GameClient Player)
		{
			if (EndedAlready)
				return;
			EndedAlready = true;
			
			Player1.Equipments.ClearMask();
			Player2.Equipments.ClearMask();
			
			if (Player1 == Player)
			{
				SendGiveUp(Player2);
				SendWin(Player2);
				
				Player1.Arena.Status = Enums.ArenaStatus.WaitingInactive;
				using (var wait = Player1.Arena.Build())
					Player1.Send(wait);
				
				Player2.Arena.Status = Enums.ArenaStatus.WaitingForOpponent;
				using (var wait = Player2.Arena.Build())
					Player2.Send(wait);
			}
			else
			{
				SendGiveUp(Player1);
				SendWin(Player1);
				
				Player1.Arena.Status = Enums.ArenaStatus.WaitingForOpponent;
				using (var wait = Player1.Arena.Build())
					Player1.Send(wait);
				
				Player2.Arena.Status = Enums.ArenaStatus.WaitingInactive;
				using (var wait = Player2.Arena.Build())
					Player2.Send(wait);
			}
			
			Player1.ArenaMatch = null;
			Player2.ArenaMatch = null;
			Player1.Battle = null;
			Player2.Battle = null;
			
			
			uint WinAmount = (uint)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(32, 113);
			uint LoseAmount =  (uint)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)(WinAmount / 3), (int)(WinAmount / 2));
			LoseAmount /= 2;
			
			if (Player != Player1)
			{
				Player1.Arena.ArenaHonorPoints += WinAmount;
				Player2.Arena.ArenaHonorPoints -= LoseAmount;
			}
			else
			{
				Player2.Arena.ArenaHonorPoints += WinAmount;
				Player1.Arena.ArenaHonorPoints -= LoseAmount;
			}
			
			Player1.Arena.Save();
			Player2.Arena.Save();
			
			ArenaQualifier.GetTop10();
			
			Player1 = null;
			Player2 = null;
			
			ArenaQualifier.MatchQueue.Remove(this);
		}
		private bool EndedAlready = false;
		public void EndMatch()
		{
			if (EndedAlready)
				return;
			EndedAlready = true;
			
			bool IsBotMatch = (Player1.IsAIBot || Player2.IsAIBot);
			
			Player1.Equipments.ClearMask();
			if (Player2 != null)
				Player2.Equipments.ClearMask();
			Player1.ArenaMatch = null;
			if (Player2 != null)
				Player2.ArenaMatch = null;
			Player1.Battle = null;
			if (Player2 != null)
				Player2.Battle = null;
			
			SendEnd();
			
			if (Player2 != null)
			{
				uint WinAmount = (uint)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(32, 113);
				uint LoseAmount =  (uint)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)(WinAmount / 3), (int)(WinAmount / 2));
				LoseAmount /= 2;
				
				if (IsBotMatch)
				{
					WinAmount = 0;
					LoseAmount = 0;
				}
				
				if (Player1Damage == Player2Damage)
				{

					Player1.WasInArena = true;
					Player2.WasInArena = true;
					Player1.LostArena = true;
					Player2.LostArena = true;
					
					// draw
					if (!IsBotMatch)
					{
						Player1.Arena.ArenaHonorPoints += LoseAmount;
						Player2.Arena.ArenaHonorPoints += LoseAmount;
						
						using (var msg = Packets.Message.MessageCore.CreateSystem2("ALL", string.Format(Core.MessageConst.ARENA_DRAW, Player1.Name, Player2.Name)))
						{
							Packets.Message.MessageCore.SendGlobalMessage(msg);
						}
					}
				}
				else if (Player1Damage > Player2Damage)
				{
					Player1.WasInArena = true;
					Player2.WasInArena = true;
					Player2.LostArena = true;
					Player1.LostArena = false;
					
					if (!IsBotMatch)
					{
						// player1 win
						Player1.Arena.ArenaHonorPoints += WinAmount;
						Player2.Arena.ArenaHonorPoints -= LoseAmount;
						
						Player1.Arena.ArenaWinsToday++;
						Player1.Arena.ArenaTotalWins++;
						//Player1.Send(arenaAction);
						
						Player2.Arena.ArenaLossToday++;
						Player2.Arena.ArenaTotalLoss++;
						
						using (var msg = Packets.Message.MessageCore.CreateSystem2("ALL", string.Format(Core.MessageConst.ARENA_WIN, Player1.Name, Player2.Name)))
						{
							Packets.Message.MessageCore.SendGlobalMessage(msg);
						}
					}
				}
				else
				{
					Player1.WasInArena = true;
					Player2.WasInArena = true;
					Player1.LostArena = true;
					Player2.LostArena = false;
					
					// player 2 win
					
					if (!IsBotMatch)
					{
						Player2.Arena.ArenaHonorPoints += WinAmount;
						Player1.Arena.ArenaHonorPoints -= LoseAmount;
						
						Player2.Arena.ArenaWinsToday++;
						Player2.Arena.ArenaTotalWins++;
						
						Player1.Arena.ArenaLossToday++;
						Player1.Arena.ArenaTotalLoss++;

						using (var msg = Packets.Message.MessageCore.CreateSystem2("ALL", string.Format(Core.MessageConst.ARENA_WIN, Player2.Name, Player1.Name)))
						{
							Packets.Message.MessageCore.SendGlobalMessage(msg);
						}
					}
				}
			}
			
			if (!Player1.LeaveDynamicMap())
				Player1.NetworkClient.Disconnect("Could not leave arena...");
			Player1.ForceRevive();
			if (Player2 != null)
			{
				if (!Player2.LeaveDynamicMap())
					Player2.NetworkClient.Disconnect("Could not leave arena...");
				Player2.ForceRevive();
			}
			
			KickWatchers();
			
			if (Player2 != null)
			{
				if (!IsBotMatch)
				{
					Player1.Arena.Save();
					Player2.Arena.Save();
				}
			}
			ArenaQualifier.GetTop10();
			
			Player1.Arena.Status = Enums.ArenaStatus.NotSignedUp;
			using (var wait = Player1.Arena.Build())
				Player1.Send(wait);
			
			if (Player2 != null)
			{
				Player2.Arena.Status = Enums.ArenaStatus.NotSignedUp;
				using (var wait = Player2.Arena.Build())
					Player2.Send(wait);
			}
			
			Player1 = null;
			Player2 = null;
			
			ArenaQualifier.MatchQueue.Remove(this);
		}
		
		public void JoinAsWatcher(Entities.GameClient Watcher)
		{
			Watcher.CanAttack = false;
			Watchers.Add(Watcher);
			
			ushort[] Location = GenerateRandomLocation();
			Watcher.TeleportDynamic(DynamicID, Location[0], Location[1]);

			SendPacket();
		}
		
		public bool SpawnInArena(Entities.GameClient Client)
		{
			return !Watchers.Contains(Client);
		}
		
		public void KickWatchers()
		{
			foreach (Entities.GameClient watcher in Watchers.ToDictionary().Values)
			{
				watcher.CanAttack = true;
				if (!watcher.LeaveDynamicMap())
					watcher.NetworkClient.Disconnect("Could not leave dynamic map...");
				else
				{
					watcher.Arena.Status = Enums.ArenaStatus.NotSignedUp;
					using (var wait = watcher.Arena.Build())
						watcher.Send(wait);
				}
			}
			Watchers.Clear();
		}
		
		public void LeaveWatcher(Entities.GameClient Watcher)
		{
			Watcher.CanAttack = true;
			if (!Watcher.LeaveDynamicMap())
				Watcher.NetworkClient.Disconnect("Could not leave dynamic map...");
			else
			{
				Watchers.Remove(Watcher);
				Watcher.Arena.Status = Enums.ArenaStatus.NotSignedUp;
				using (var wait = Watcher.Arena.Build())
					Watcher.Send(wait);
			}
		}
		
		public bool AcceptedFight(Entities.GameClient Player)
		{
			if (Player == Player1)
			{
				return (AcceptedFight2);
			}
			else
			{
				return (AcceptedFight1);
			}
		}
		
		public void SendGiveUp(Entities.GameClient Winner)
		{
			using (var action = new Packets.ArenaActionPacket())
			{
				action.DialogID = 4;
				action.OptionID = 0;
				
				Winner.Send(action);
			}
		}
		public void SendEnd()
		{
			using (var action = new Packets.ArenaActionPacket())
			{
				action.DialogID = 1;
				action.OptionID = 0;
				
				Player1.Send(action);
				Player2.Send(action);
			}
		}
		
		public static void SendWin(Entities.GameClient Winner)
		{
			using (var action = new Packets.ArenaActionPacket())
			{
				action.DialogID = 10;
				action.OptionID = 1;
				
				Winner.Send(action);
			}
		}
		

		public static void SendLose(Entities.GameClient Loser)
		{
			using (var action = new Packets.ArenaActionPacket())
			{
				action.DialogID = 10;
				action.OptionID = 0;
				
				Loser.Send(action);
			}
		}
		
		public void SendMatch()
		{
			using (var action = new Packets.ArenaActionPacket())
			{
				action.DialogID = 6;
				action.OptionID = 5;

				Player1.Send(action);
				if (Player2 != null)
					Player2.Send(action);
			}
		}
		public void SendCountDown(Entities.GameClient From, Entities.GameClient To)
		{
			using (var action = new Packets.ArenaActionPacket())
			{
				action.DialogID = 8;
				action.OptionID = 0;
				
				action.Name = From.Name;
				action.Level = (uint)From.Level;
				action.ArenaPoints = From.Arena.ArenaHonorPoints;
				action.Class = (uint)From.Class;
				action.Rank = From.Arena.ArenaRanking;
				action.EntityUID = From.EntityUID;
				action.Unknown40 = 1;
				
				To.Send(action);
			}
		}
		public void SendCountDown(string Name, uint Level, uint Class, uint Ranking, uint EntityUID, Entities.GameClient To)
		{
			using (var action = new Packets.ArenaActionPacket())
			{
				action.DialogID = 8;
				action.OptionID = 0;
				
				action.Name = Name;
				action.Level = Level;
				action.ArenaPoints = 0;
				action.Class = Class;
				action.Rank = Ranking;
				action.EntityUID = EntityUID;
				action.Unknown40 = 1;
				
				To.Send(action);
			}
		}
		
		public void SendPacket()
		{
			using (var match = new Packets.ArenaMatchPacket())
			{
				match.Player1EntityUID = Player1.EntityUID;
				match.Player1Name = Player1.Name;
				match.Player1Damage = (int)Player1Damage;
				
				match.Player2EntityUID = Player2.EntityUID;
				match.Player2Name = Player2.Name;
				match.Player2Damage = (int)Player2Damage;
				
				Player1.Send(match);
				Player2.Send(match);
				
				foreach (Entities.GameClient client in Watchers.ToDictionary().Values)
				{
					client.Send(match);
				}
			}
		}
		
		public void SendPacket(uint EntityUID, string Name, int HP1, uint EntityUID2, string Name2, int HP2)
		{
			using (var match = new Packets.ArenaMatchPacket())
			{
				match.Player1EntityUID = EntityUID;
				match.Player1Name = Name;
				match.Player1Damage = HP1;
				
				match.Player2EntityUID = EntityUID2;
				match.Player2Name = Name2;
				match.Player2Damage = HP2;
				
				Player1.Send(match);
				
				foreach (Entities.GameClient client in Watchers.ToDictionary().Values)
				{
					client.Send(match);
				}
			}
		}
		
		public override bool HandleBeginAttack(ProjectX_V3_Game.Entities.GameClient Attacker)
		{
			if (!Started)
				return false;
			if (!FightingMatch)
				return false;
			
			return (DateTime.Now >= MatchStartTime);
		}
		public override bool HandleBeginHit_Physical(ProjectX_V3_Game.Entities.GameClient Attacker)
		{
			return true;
		}
		public override bool HandleBeginHit_Ranged(ProjectX_V3_Game.Entities.GameClient Attacker)
		{
			return true;
		}
		public override bool HandleBeginHit_Magic(ProjectX_V3_Game.Entities.GameClient Attacker, Packets.UseSpellPacket usespell)
		{
			return true;
		}
		public override bool HandleAttack(ProjectX_V3_Game.Entities.GameClient Attacker, ProjectX_V3_Game.Entities.GameClient Attacked, ref uint damage)
		{
			if (Attacked.Battle == null)
				return false;
			if (Attacker.Battle == null)
				return false;
			
			if (Attacker == Player1)
				Player1Damage += damage;
			else
				Player2Damage += damage;
			
			SendPacket();
			
			return (Attacked == Player1 || Attacked == Player2);
		}
		public override bool HandleDeath(ProjectX_V3_Game.Entities.GameClient Attacker, ProjectX_V3_Game.Entities.GameClient Attacked)
		{
			if (Attacker == Player1)
			{
				Player1Damage = 1;
				Player2Damage = 0;
			}
			else
			{
				Player2Damage = 1;
				Player1Damage = 0;
			}
			EndMatch();
			return false;
		}
		public override bool HandleRevive(ProjectX_V3_Game.Entities.GameClient Attacked)
		{
			return false;
		}
		public override bool EnterArea(Entities.GameClient client)
		{
			return FightingMatch;
		}
		public override bool LeaveArea(Entities.GameClient client)
		{
			return FightingMatch;
		}
		public override void KillMob(ProjectX_V3_Game.Entities.GameClient Attacker, uint MobUID)
		{
			// do nothing ...
		}
	}
}
