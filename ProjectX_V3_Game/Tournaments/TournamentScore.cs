//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Tournaments
{
	/// <summary>
	/// Description of TournamentScore.
	/// </summary>
	public class TournamentScore
	{
		public uint CurrentScore = 0;
		public uint CurrentKills = 0;
		public uint CurrentDeaths = 0;
		public uint CurrentHP = 0;
		
		public void Clear()
		{
			CurrentScore = 0;
			CurrentKills = 0;
			CurrentDeaths = 0;
			CurrentHP = 0;
		}
	}
}
