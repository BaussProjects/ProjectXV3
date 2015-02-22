//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;

namespace ProjectX_V3_Game.Tournaments
{
	/// <summary>
	/// Description of LastManStanding.
	/// </summary>
	public class LastManStanding : TournamentBase
	{
		private ConcurrentDictionary<uint, Entities.GameClient> Players;
		
		private const string START_MSG = "LastManStanding has started. You have 1 minute to sign up!";
		private const string SEND_MSG = "The LastManStanding battle begins in 10 seconds... 5 hits and you're out!";
		private const string END_MSG = "LastManStanding has ended, the winner was {0}";
		private const string BEGIN_FIGHT = "Fight!!!";
		private const string HEALTH = "You current have {0}/5 HP.";
		
		private const uint WIN_SCORE = 112;
		private const uint EXTRA_SCORE = 13;
		private const int NUMBER_OF_MINS = 12;
		private const int AFK_TIME = 25000;
		private readonly Maps.MapPoint MapLocation;
		public LastManStandingBattleClass BattleClass;
		
		private DateTime TournamentStartTime;
		private bool Ended = false;
		private bool Started = false;
		private bool WasSend = false;
		
		public class LastManStandingBattleClass : Data.BattleClass
		{
			public LastManStanding LastManStanding;
			
			public override bool HandleBeginAttack(ProjectX_V3_Game.Entities.GameClient Attacker)
			{
				return (DateTime.Now >= LastManStanding.TournamentStartTime);
			}
			public override bool HandleBeginHit_Physical(ProjectX_V3_Game.Entities.GameClient Attacker)
			{
				return false;
			}
			public override bool HandleBeginHit_Ranged(ProjectX_V3_Game.Entities.GameClient Attacker)
			{
				return false;
			}
			public override bool HandleBeginHit_Magic(ProjectX_V3_Game.Entities.GameClient Attacker, Packets.UseSpellPacket usespell)
			{
				if (usespell.SpellID != 1045 && usespell.SpellID != 1046)
					return false;
				return true;
			}
			public override bool HandleAttack(ProjectX_V3_Game.Entities.GameClient Attacker, ProjectX_V3_Game.Entities.GameClient Attacked, ref uint damage)
			{
				damage = 1;
				
				Attacked.TournamentScore.CurrentHP -= 1;
				LastManStanding.SendMessage(Attacked, string.Format(LastManStanding.HEALTH, Attacked.TournamentScore.CurrentHP));
				
				if (Attacked.TournamentScore.CurrentHP <= 0)
				{
					Attacker.TournamentScore.CurrentScore += 1;
					Attacker.TournamentInfo.TotalKills += 1;
					
					Entities.GameClient rClient;
					LastManStanding.Players.TryRemove(Attacked.EntityUID, out rClient);
					
					Attacked.Battle = null;
					Attacked.Equipments.ClearMask();
					if (Attacked.TournamentScore.CurrentScore >= 5)
						Attacked.TournamentInfo.TournamentPoints += LastManStanding.EXTRA_SCORE;
					
					Attacked.TournamentInfo.TotalDeaths -= 1;
					
					Attacked.Teleport(Attacked.LastMapID, Attacked.LastMapX, Attacked.LastMapY);
					
					LastManStanding.UpdateBroadcast();
					LastManStanding.CheckForWin();
				}

				return false;
			}
			public override bool HandleDeath(ProjectX_V3_Game.Entities.GameClient Attacker, ProjectX_V3_Game.Entities.GameClient Attacked)
			{
				return false;
			}
			public override bool HandleRevive(ProjectX_V3_Game.Entities.GameClient Killed)
			{
				return false;
			}
			public override bool EnterArea(Entities.GameClient client)
			{
				return true;
			}
			public override bool LeaveArea(Entities.GameClient client)
			{
				return true;
			}
			public override void KillMob(ProjectX_V3_Game.Entities.GameClient Attacker, uint MobUID)
			{
				// do nothing ...
			}
		}
		
		public LastManStanding()
			: base("LastManStanding", 00, 4, 16)
		{
			Players = new ConcurrentDictionary<uint, ProjectX_V3_Game.Entities.GameClient>();
			MapLocation = new ProjectX_V3_Game.Maps.MapPoint(50001, 46, 45);
			BattleClass = new LastManStanding.LastManStandingBattleClass();
			BattleClass.LastManStanding = this;
			
			new ProjectX_V3_Lib.Threading.BaseThread(CHECK_PLAYERS_THREAD, 5000, "LastManStandingThread").Start();
		}
		
		public bool CheckForWin()
		{
			if (Players.Count == 1 && WasSend)
			{
				WasSend = false;
				
				string WinnerName = "NONE";
				foreach (Entities.GameClient Player in Players.Values)
				{
					WinnerName = Player.Name;
					Player.TournamentInfo.TournamentPoints += WIN_SCORE;
				}
				End();
				SendMessage(string.Format(END_MSG, WinnerName));
				return false;
			}
			
			return true;
		}
		private void CHECK_PLAYERS_THREAD()
		{
			if (!CheckForWin())
				return;

			if (Players.Count > 0)
			{
				foreach (Entities.GameClient Player in Players.Values)
				{
					if (Player != null)
					{
						if (!Player.LoggedIn || DateTime.Now >= Player.LastMovement.AddMilliseconds(AFK_TIME) && WasSend)
						{
							// afk or lost a failed connection ...
							if (Player.Map.MapID == MapLocation.MapID)
							{
								Entities.GameClient rPlayer;
								Players.TryRemove(Player.EntityUID, out rPlayer);
								Player.NetworkClient.Disconnect("AFK: LastManStanding...");
							}
							Player.Battle = null;
						}
					}
				}
			}
		}
		
		public override int MeasureEndTime()
		{
			return (60000 * NUMBER_OF_MINS);
		}
		public override void Start()
		{
			Players.Clear();
			
			SendMessage(START_MSG);
			SendMessageBC(START_MSG);
		}
		public override void Send()
		{
			Started = true;
			WasSend = false;
			TournamentStartTime = DateTime.Now.AddMilliseconds(10000);
			foreach (Entities.GameClient Player in Players.Values)
			{
				Player.Battle = BattleClass;
				Player.Teleport(MapLocation);
				Player.Equipments.AddMask(410009, Enums.ItemLocation.WeaponR);
				Player.Equipments.AddMask(420009, Enums.ItemLocation.WeaponL);
				Player.TournamentScore.Clear();
				Player.TournamentScore.CurrentHP = 5;
				Player.LastMovement = DateTime.Now;
			}
			SendMessage(SEND_MSG);
			SendMessageBC(SEND_MSG);
			System.Threading.Thread.Sleep(10000);
			SendMessage(BEGIN_FIGHT);
			UpdateBroadcast();
			WasSend = true;
		}
		
		public override void End()
		{
			if (Ended)
				return;
			Ended = true;
			Started = false;
			foreach (Entities.GameClient Player in Players.Values)
			{
				Player.Equipments.ClearMask();
				Player.Battle = null;
				Player.Teleport(Player.LastMapID, Player.LastMapX, Player.LastMapY);
			}
			Players.Clear();
		}
		public override bool SignUp(ProjectX_V3_Game.Entities.GameClient client, out bool AlreadySigned)
		{
			AlreadySigned = false;
			if (Started)
				return false;
			AlreadySigned = !Players.TryAdd(client.EntityUID, client);
			return true;
		}
		
		public override void UpdateBroadcast()
		{
			foreach (Entities.GameClient client in Players.Values)
			{
				#region Clear
				using (var msg = Packets.Message.MessageCore.ClearScore())
				{
					client.Send(msg);
				}
				#endregion
				
				#region Players In Map
				using (var msg = Packets.Message.MessageCore.CreateScore(string.Format("Players Back: {0}", Players.Count)))
				{
					client.Send(msg);
				}
				#endregion
				#region Score
				using (var msg = Packets.Message.MessageCore.CreateScore(string.Format("Kills: {0}", client.TournamentScore.CurrentScore)))
				{
					client.Send(msg);
				}
				#endregion
				#region Total Score
				using (var msg = Packets.Message.MessageCore.CreateScore(string.Format("Total-Kills: {0}", client.TournamentInfo.TotalKills)))
				{
					client.Send(msg);
				}
				using (var msg = Packets.Message.MessageCore.CreateScore(string.Format("Total-Deaths: {0}", client.TournamentInfo.TotalDeaths)))
				{
					client.Send(msg);
				}
				using (var msg = Packets.Message.MessageCore.CreateScore(string.Format("Kill-Ratio: {0}", client.TournamentInfo.Ratio)))
				{
					client.Send(msg);
				}
				#endregion
			}
		}
	}
}
