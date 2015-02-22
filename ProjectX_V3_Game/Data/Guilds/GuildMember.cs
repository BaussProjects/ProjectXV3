//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// Description of GuildMember.
	/// </summary>
	public class GuildMember
	{
		public GuildMember()
		{
			Rank = Enums.GuildRank.Member;
		}
		
		public GuildMember(Entities.GameClient client)
		{
			Rank = Enums.GuildRank.Member;
			DatabaseUID = client.DatabaseUID;
			Client = client;
			Name = client.Name;
		}
		
		public int DatabaseUID;
		public int Index;
		public uint JoinDate;
		public string Name;
		public byte Level;
		
		public uint MoneyDonation;
		public uint CPDonation;
		
		public Entities.GameClient Client;
		
		public bool Online
		{
			get
			{
				if (Client == null)
					return false;
				return Client.LoggedIn;
			}
		}
		
		public Enums.GuildRank Rank;
	}
}
