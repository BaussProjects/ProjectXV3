//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;
using System.Linq;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// Description of NobilityBoard.
	/// </summary>
	public class NobilityBoard
	{
		private static ConcurrentDictionary<int, NobilityDonation> Donations;
		static NobilityBoard()
		{
			Donations = new ConcurrentDictionary<int, NobilityDonation>();
		}
		
		public static bool AddNobility(NobilityDonation donation)
		{
			return Donations.TryAdd(donation.DatabaseID, donation);
		}
		
		public static void SetNobility(Entities.GameClient client)
		{
			if (Donations.ContainsKey(client.DatabaseUID) )
			{
				NobilityDonation donation;
				if (Donations.TryGetValue(client.DatabaseUID, out donation))
				{
					client.Nobility = donation;
					donation.Client = client;
				}
			}
		}
		
		public static long[] GetGoals()
		{
			System.Collections.Generic.KeyValuePair<int, NobilityDonation>[]
				OrderDonations = Donations.OrderBy(don => don.Value.Donation).ToArray();
			
			Array.Reverse(OrderDonations);
			
			long KingDonation = 0;
			long PrinceDonation = 0;
			long DukeDonation = 0;
			
			for (int i = 0; i < OrderDonations.Length; i++)
			{
				if (i < 3)
				{
					KingDonation = OrderDonations[i].Value.Donation;
					PrinceDonation = KingDonation - 1;
					DukeDonation = PrinceDonation - 1;
				}
				else if (i < 15)
				{
					PrinceDonation = OrderDonations[i].Value.Donation;
					DukeDonation = PrinceDonation - 1;
				}
				else if (i < 50)
					DukeDonation = OrderDonations[i].Value.Donation;
			}
			
			return new long[] { KingDonation, PrinceDonation, DukeDonation };
		}
		
		public static NobilityDonation[] GetTop50()
		{
			System.Collections.Generic.KeyValuePair<int, NobilityDonation>[]
				OrderDonations = Donations.OrderBy(don => don.Value.Donation).ToArray();
			
			Array.Reverse(OrderDonations);
			
			NobilityDonation[] TopDonation = new NobilityDonation[50];
			for (int i = 0; i < OrderDonations.Length; i++)
			{
				OrderDonations[i].Value.OldRank = OrderDonations[i].Value.Rank;
				
				if (i < 3)
				{
					TopDonation[i] = OrderDonations[i].Value;
					TopDonation[i].Rank = Enums.NobilityRank.King;
				}
				else if (i < 15)
				{
					TopDonation[i] = OrderDonations[i].Value;
					TopDonation[i].Rank = Enums.NobilityRank.Prince;
				}
				else if (i < 50)
				{
					TopDonation[i] = OrderDonations[i].Value;
					TopDonation[i].Rank = Enums.NobilityRank.Duke;
				}
				else
				{
					if (OrderDonations[i].Value.Donation >= 200000000)
							OrderDonations[i].Value.Rank = Enums.NobilityRank.Earl;
						else if (OrderDonations[i].Value.Donation >= 100000000)
							OrderDonations[i].Value.Rank = Enums.NobilityRank.Baron;
						else if (OrderDonations[i].Value.Donation >= 30000000)
							OrderDonations[i].Value.Rank = Enums.NobilityRank.Knight;
				}
				OrderDonations[i].Value.Ranking = i;
				
				if (OrderDonations[i].Value.Client != null && OrderDonations[i].Value.OldRank != OrderDonations[i].Value.Rank)
				{
					switch (OrderDonations[i].Value.Rank)
					{
						case Enums.NobilityRank.King:
							{
								if (OrderDonations[i].Value.Client.Sex == Enums.Sex.Male)
								{
									using (var msg = Packets.Message.MessageCore.CreateCenter(
										string.Format(Core.MessageConst.NEW_KING, OrderDonations[i].Value.Client.Name)))
									{
										Packets.Message.MessageCore.SendGlobalMessage(msg);
									}
								}
								else
								{
									using (var msg = Packets.Message.MessageCore.CreateCenter(
										string.Format(Core.MessageConst.NEW_QUEEN, OrderDonations[i].Value.Client.Name)))
									{
										Packets.Message.MessageCore.SendGlobalMessage(msg);
									}
								}
								break;
							}
						case Enums.NobilityRank.Prince:
							{
								if (OrderDonations[i].Value.Client.Sex == Enums.Sex.Male)
								{
									using (var msg = Packets.Message.MessageCore.CreateCenter(
										string.Format(Core.MessageConst.NEW_PRINCE, OrderDonations[i].Value.Client.Name)))
									{
										Packets.Message.MessageCore.SendGlobalMessage(msg);
									}
								}
								else
								{
									using (var msg = Packets.Message.MessageCore.CreateCenter(
										string.Format(Core.MessageConst.NEW_PRINCESS, OrderDonations[i].Value.Client.Name)))
									{
										Packets.Message.MessageCore.SendGlobalMessage(msg);
									}
								}
								break;
							}
					}
					OrderDonations[i].Value.Client.SendNobility();
				}
			}
			return TopDonation;
		}
		public static NobilityDonation[] GetTop3()
		{
			NobilityDonation[] TopDonations = new NobilityDonation[3];
			System.Buffer.BlockCopy(GetTop50(), 0, TopDonations, 0, 3);
			return TopDonations;
		}
		public static NobilityDonation[] GetPage(int page, out int PageMax)
		{
			ConcurrentDictionary<int, NobilityDonation> Donations = NobilityBoard.Donations;
			
			PageMax = 0;
			if (Donations.Count == 0)
				return null;
			if (page > 5 || page < 0)
				return null;
			
			PageMax = (Donations.Count / 10);
			if (page <= PageMax)
			{
				NobilityDonation[] Page = new NobilityDonation[10];
				NobilityDonation[] TopDonations = GetTop50();
				int counter = 0;
				int to_counter = ((page + 1) * 10);
				int from_counter = (to_counter - 10);
				for (int i = from_counter; i < to_counter; i++)
				{
					if (TopDonations[i] != null)
					{
						Page[counter] = TopDonations[i];
					}
					counter++;
				}
				int realsize = 0;
				foreach (var n in Page)
				{
					if (n != null)
						realsize++;
				}
				Array.Resize(ref Page, realsize);
				return Page;
			}
			return null;
		}
		
		public static void Donate(Entities.GameClient client, long Amount)
		{
			if (client.Money < Amount)
				return;
			if (Amount < 3000000)
				return;
			if (Amount > 999999999)
				return;
			
			if (Donations.ContainsKey(client.DatabaseUID))
			{
				NobilityDonation donation;
				if (Donations.TryGetValue(client.DatabaseUID, out donation))
				{
					if (donation.Donation >= Core.NumericConst.MaxNobilityDonation)
						return;
					
					client.Money -= (uint)Amount;
					donation.Donation = Math.Min(Core.NumericConst.MaxNobilityDonation, (long)(donation.Donation + Amount));
					GetTop50();
					
					client.SendNobility();
					
					donation.UpdateDonation();
				}
			}
			else
			{
				NobilityDonation donation = new NobilityDonation();
				donation.Rank = Enums.NobilityRank.Serf;
				donation.Donation = Amount;
				if (Donations.TryAdd(client.DatabaseUID, donation))
				{
					client.Money -= (uint)Amount;
					donation.DatabaseID = client.DatabaseUID;
					donation.Name = client.Name;
					donation.Client = client;
					client.Nobility = donation;
					GetTop50();
					
					client.SendNobility();
					
					donation.CreateDatabase();
					donation.UpdateDonation();
				}
			}
		}
		public static void DonateCPs(Entities.GameClient client, long Amount)
		{
			uint cps = (uint)(Amount / 50000);
			if (client.CPs < cps)
				return;
			if (Amount < 3000000)
				return;
			if (Amount > 999999999)
				return;
			
			if (Donations.ContainsKey(client.DatabaseUID))
			{
				NobilityDonation donation;
				if (Donations.TryGetValue(client.DatabaseUID, out donation))
				{
					if (donation.Donation >= Core.NumericConst.MaxNobilityDonation)
						return;
					client.CPs -= cps;
					donation.Donation = Math.Min(Core.NumericConst.MaxNobilityDonation, (long)(donation.Donation + Amount));
					GetTop50();
					
					client.SendNobility();
					
					donation.UpdateDonation();
				}
			}
			else
			{
				client.CPs -= cps;
				
				NobilityDonation donation = new NobilityDonation();
				donation.Rank = Enums.NobilityRank.Serf;
				donation.Donation = Amount;
				if (Donations.TryAdd(client.DatabaseUID, donation))
				{
					donation.DatabaseID = client.DatabaseUID;
					donation.Name = client.Name;
					donation.Client = client;
					client.Nobility = donation;
					GetTop50();
					
					client.SendNobility();
					
					donation.CreateDatabase();
					donation.UpdateDonation();
				}
			}
		}
		
		public static long RemainingDonation(Enums.NobilityRank Rank, long Donation)
		{
			long Goal = 0;
			long[] Goals = GetGoals();
			switch (Rank)
			{
				case Enums.NobilityRank.King:
					Goal = Goals[0];
					break;
				case Enums.NobilityRank.Prince:
					Goal = Goals[1];
					break;
				case Enums.NobilityRank.Duke:
					Goal = Goals[2];
					break;
				case Enums.NobilityRank.Earl:
					Goal = 200000000;
					break;
				case Enums.NobilityRank.Baron:
					Goal = 100000000;
					break;
				case Enums.NobilityRank.Knight:
					Goal = 30000000;
					break;
			}
			return Goal - Donation;
		}
	}
}
