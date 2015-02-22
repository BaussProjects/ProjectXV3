//Project by BaussHacker aka. L33TS

using System;
using System.Linq;
using System.Collections.Concurrent;
using ProjectX_V3_Lib.ThreadSafe;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// Description of ArenaQualifier.
	/// </summary>
	public class ArenaQualifier
	{
		private static ConcurrentDictionary<int, ArenaInfo> ArenaStats;
		static ArenaQualifier()
		{
			ArenaStats = new ConcurrentDictionary<int, ArenaInfo>();
			PlayerQueue = new ConcurrentArrayList<ProjectX_V3_Game.Entities.GameClient>();
			MatchQueue = new ConcurrentArrayList<Data.ArenaMatch>();
		}
		
		public static ConcurrentArrayList<Entities.GameClient> PlayerQueue;
		public static ConcurrentArrayList<Data.ArenaMatch> MatchQueue;
		
		public static bool AddArenaInfo(ArenaInfo arena)
		{
			return ArenaStats.TryAdd(arena.DatabaseID, arena);
		}
		
		public static void SetArenaInfo(Entities.GameClient client)
		{
			if (ArenaStats.ContainsKey(client.DatabaseUID))
			{
				ArenaInfo arena;
				if (ArenaStats.TryGetValue(client.DatabaseUID, out arena))
				{
					client.Arena = arena;
					arena.Owner = client;
				}
			}
		}
		
		public static ArenaInfo[] GetTop10()
		{
			System.Collections.Generic.KeyValuePair<int, ArenaInfo>[]
				OrderStats = ArenaStats.OrderBy(don => don.Value.ArenaTotal).ToArray();
			
			Array.Reverse(OrderStats);
			
			ArenaInfo[] TopArena = new ArenaInfo[10];
			if (OrderStats.Length < 10)
				TopArena = new ArenaInfo[OrderStats.Length];
			
			for (int i = 0; i < OrderStats.Length; i++)
			{

				if (i < 10)
				{
					TopArena[i] = OrderStats[i].Value;
					TopArena[i].ArenaRanking = (uint)i;
				}
				else
					OrderStats[i].Value.ArenaRanking = (uint)i;
			}
			return TopArena;
		}
		
		public static void JoinArena(Entities.GameClient Player)
		{
			if (Player.Battle != null)
				return;
			if (Player.DynamicMap != null)
				return;
			if (Player.ArenaMatch != null)
				return;
			
			if (PlayerQueue.Contains(Player))
			{
				
				Player.Arena.Status = Enums.ArenaStatus.NotSignedUp;
				using (var wait = Player.Arena.Build())
					Player.Send(wait);
				
				Player.Arena.Status = Enums.ArenaStatus.WaitingForOpponent;
				using (var wait = Player.Arena.Build())
					Player.Send(wait);
			}
			else
			{
				Player.Arena.Status = Enums.ArenaStatus.WaitingForOpponent;
				using (var wait = Player.Arena.Build())
					Player.Send(wait);
			}
			if (MatchQueue.Count > 0)
			{
				using (var matches = new Packets.ArenaBattleInfoPacket(MatchQueue.ToDictionary().Values.ToArray()))
				{
					matches.Page = 1;
					matches.SetSize();
					Player.Send(matches);
				}
			}
			PlayerQueue.Add(Player);
			uint DelayedID = 0;
			DelayedID = ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(() =>
			                                                                   {
			                                                                   	if (HandleMatchUp(Player, FindMatchUp(Player)))
			                                                                   	{
			                                                                   		ProjectX_V3_Lib.Threading.DelayedTask.Remove(DelayedID);
			                                                                   	}
			                                                                   }, 10000, 0);
		}
		private static bool HandleMatchUp(Entities.GameClient Player, Entities.GameClient Opponent)
		{
			if (Player.ArenaMatch != null)
				return true;
			if (Opponent == null)
				return false;
			
			ArenaMatch match = ArenaMatch.CreateMatch(Player, Opponent);
			Player.ArenaMatch.BeginTimeOutQueue();
			
			Player.Arena.Status = Enums.ArenaStatus.WaitingInactive;
			using (var wait = Player.Arena.Build())
				Player.Send(wait);
			
			Opponent.Arena.Status = Enums.ArenaStatus.WaitingInactive;
			using (var wait = Opponent.Arena.Build())
				Opponent.Send(wait);
			
			Player.ArenaMatch = match;
			Opponent.ArenaMatch = match;
			
			using (var request = new Packets.ArenaActionPacket())
			{
				request.Name = Player.Name;
				request.Level = (uint)Player.Level;
				request.ArenaPoints = Player.Arena.ArenaPoints;
				request.Class = (uint)Player.Class;
				request.DialogID = Packets.ArenaActionPacket.StartCountDown;
				request.Rank = Player.Arena.ArenaRanking;
				request.EntityUID = Player.EntityUID;
				request.Unknown40 = 1;
				Player.Send(request);
			}
			using (var request = new Packets.ArenaActionPacket())
			{
				request.Name = Opponent.Name;
				request.Level = (uint)Opponent.Level;
				request.ArenaPoints = Opponent.Arena.ArenaPoints;
				request.Class = (uint)Opponent.Class;
				request.DialogID = Packets.ArenaActionPacket.StartCountDown;
				request.Rank = Opponent.Arena.ArenaRanking;
				request.EntityUID = Opponent.EntityUID;
				request.Unknown40 = 1;
				Opponent.Send(request);
			}
			
			return true;
		}
		public static void AcceptArena(Entities.GameClient Player)
		{
			if (Player.ArenaMatch == null)
				return;
			
			if (Player.ArenaMatch.AcceptedFight(Player))
			{
				Player.ArenaMatch.BeginMatch();
				MatchQueue.Add(Player.ArenaMatch);
			}
			else
			{
				if (Player == Player.ArenaMatch.Player1)
					Player.ArenaMatch.AcceptedFight1 = true;
				else
					Player.ArenaMatch.AcceptedFight2 = true;
			}
		}
		public static void QuitArena(Entities.GameClient Player)
		{
			if (Player.ArenaMatch == null)
				return;
			if (!Player.ArenaMatch.Started)
				return;
			
			Player.ArenaMatch.Quit(Player);
		}
		public static void GiveUpArena(Entities.GameClient Player)
		{
			if (Player.ArenaMatch == null)
				return;
			if (Player.ArenaMatch.Started)
				return;
			
			Player.ArenaMatch.GiveUp(Player);
		}
		public static void QuitWaitArena(Entities.GameClient Player)
		{
			if (Player.Battle != null)
				return;
			if (Player.DynamicMap != null)
				return;
			if (Player.ArenaMatch != null)
				return;
			
			Player.Arena.Status = Enums.ArenaStatus.NotSignedUp;
			using (var wait = Player.Arena.Build())
				Player.Send(wait);
			
			PlayerQueue.Remove(Player);
		}
		private static Entities.GameClient FindMatchUp(Entities.GameClient Player)
		{
			foreach (Entities.GameClient client in PlayerQueue.ToDictionary().Values)
			{
				if (client.EntityUID == Player.EntityUID)
					continue;
				if (client.ArenaMatch != null)
					continue;

				byte MAX_LEVEL = (byte)(client.Level + 25);
				byte MIN_LEVEL = (byte)(Player.Level - 25);
				if (client.Level >= MIN_LEVEL && Player.Level <= MAX_LEVEL)
				{
					ushort PLUS_NUM = client.Equipments.GetNumberOfPlus();
					PLUS_NUM = (ushort)(PLUS_NUM > 2 ? PLUS_NUM / 2 : PLUS_NUM);
					if (PLUS_NUM <= client.Equipments.GetNumberOfPlus())
					{
						return client;
					}
				}
			}
			return null; // no match up ...
		}
	}
}
