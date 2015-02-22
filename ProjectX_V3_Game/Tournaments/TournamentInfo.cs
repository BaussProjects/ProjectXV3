//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Tournaments
{
	/// <summary>
	/// Description of TournamentInfo.
	/// </summary>
	public class TournamentInfo
	{
		public Entities.GameClient Owner;
		public TournamentInfo(Entities.GameClient Owner)
		{
			this.Owner = Owner;
		}
		private uint totalKills = 0;
		public uint TotalKills
		{
			get { return totalKills; }
			set
			{
				totalKills = value;
				//Data.NobilityBoard.AddPVP(Owner, Ratio);
				Save();
			}
		}
		private uint totalDeaths = 0;
		public uint TotalDeaths
		{
			get { return totalDeaths; }
			set
			{
				totalDeaths = value;
				Save();
			}
		}
		public uint Ratio
		{
			get
			{
				int total = (int)TotalKills;
				total -= (int)TotalDeaths;
				if (total <= 0)
					return 0;
				return (uint)total;
			}
		}
		public uint TournamentPoints
		{
			get
			{
				return Owner.BoundCPs;
			}
			set
			{
				Owner.BoundCPs = value;
			}
		}
		
		public void Save()
		{
			Database.CharacterDatabase.SaveTourny(Owner);
		}
	}
}
