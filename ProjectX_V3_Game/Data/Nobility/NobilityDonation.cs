//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// Description of NobilityDonation.
	/// </summary>
	public class NobilityDonation
	{
		public long Donation;
		public string Name;
		public int DatabaseID;
		public int RecordID;
		public Enums.NobilityRank Rank;
		public Enums.NobilityRank OldRank;
		public Entities.GameClient Client;
		public int Ranking;
		
		public void UpdateDonation()
		{
			Database.NobilityDatabase.UpdateNobilityDonation(DatabaseID, "NobilityDonation", Donation);
		}
		
		public void CreateDatabase()
		{
			Database.NobilityDatabase.AddNewNobility(DatabaseID, Name, Donation);
			Database.NobilityDatabase.SetRecordID(Client);
		}
		
		public override string ToString()
		{
			return RecordID.ToString() + " 0 0 " + Name + " " + Donation.ToString() + " " + ((uint)Rank).ToString() + " " + Ranking.ToString();
		}

	}
}
