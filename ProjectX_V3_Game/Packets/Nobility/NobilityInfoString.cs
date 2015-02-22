//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Nobility
{
	/// <summary>
	/// Description of NobilityInfoString.
	/// </summary>
	public class NobilityInfoString
	{
		public uint EntityUID;
		public long Donation;
		public Enums.NobilityRank Rank;
		public int Ranking;
		
		public override string ToString()
        {
            // return string.Format("{0} {1} {2} {3} {4} {5} {6}", UserId, IsOnline, Lookface, Name, Donation, Nobility, Rank);
            return string.Format("{0} {1} {2} {3}", EntityUID, Donation, (uint)Rank, Ranking);
        }
	}
}
