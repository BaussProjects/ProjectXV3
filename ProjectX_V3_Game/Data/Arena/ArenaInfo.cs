//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// Description of ArenaInfo.
	/// </summary>
	public class ArenaInfo
	{
		public int DatabaseID = 0;
		
		public uint ArenaTotalWins = 0;
		public uint ArenaWinsToday = 0;
		
		public uint ArenaTotalLoss = 0;
		public uint ArenaLossToday = 0;
		
		public uint ArenaTotal
		{
			get
			{
				int total = (int)ArenaTotalWins;
				total -= (int)ArenaTotalLoss;
				if (total <= 0)
					return 0;
				return (uint)total;
			}
		}
		public uint ArenaPoints = 9001;
		
		public uint ArenaRanking = 0;
		
		private uint arenaHonorPoints = 0;
		public uint ArenaHonorPoints
		{
			get { return arenaHonorPoints; }
			set
			{
				int total = (int)arenaHonorPoints;
				total -= (int)value;
				if (total <= 0)
					arenaHonorPoints = 0;
				else
					arenaHonorPoints = value;
			}
		}
		
		public Enums.ArenaStatus Status = Enums.ArenaStatus.NotSignedUp;
		
		public string Name;
		
		public uint Mesh;
		
		public int ArenaID;
		
		public uint Class;
		
		public uint Level;
		
		public bool IsBot  = false;
		
		public Entities.GameClient Owner;
		public ArenaInfo(Entities.GameClient Owner)
		{
			this.Owner = Owner;
			DatabaseID = Owner.DatabaseUID;
			this.Mesh = Owner.Mesh;
			this.Name = Owner.Name;
			this.Level = (uint)Owner.Level;
			this.Class = (uint)Owner.Class;
			IsBot = Owner.IsAIBot;
		}
		public ArenaInfo()
		{
			
		}
		
		public void Save()
		{
			if (this.IsBot)
				return;
			
			Database.ArenaDatabase.SaveArenaInfo(this);
		}

		public Packets.ArenaStatisticPacket Build()
		{
			var stats = new Packets.ArenaStatisticPacket();
			stats.Rank = ArenaRanking + 1;
			stats.WinsToday = ArenaWinsToday;
			stats.WinsTotal = ArenaTotalWins;
			stats.LossesToday = ArenaLossToday;
			stats.LossesTotal = ArenaTotalLoss;
			stats.ArenaPoints = ArenaPoints;
			stats.CurrentHonor = ArenaHonorPoints;
			stats.TotalHonor = ArenaHonorPoints;
			stats.Status = Status;
			return stats;
		}
	}
}
