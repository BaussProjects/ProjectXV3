//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.GeneralData
{
	/// <summary>
	/// Subtype: 114
	/// </summary>
	public class GetSurroundings
	{
		/// <summary>
		/// Handling the GetSurroundings action from GeneralDataPacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="General">The GeneralDataPacket.</param>
		public static void Handle(Entities.GameClient client, GeneralDataPacket General)
		{
			if (client.LoggedIn)
			{
				if (client.Booth != null)
					client.Booth.CancelBooth();
				client.Screen.ClearScreen();
				client.Screen.UpdateScreen(null);
				
				if (client.WasInArena)
				{
					client.WasInArena = false;
					if (client.LostArena)
						Data.ArenaMatch.SendLose(client);
					else
						Data.ArenaMatch.SendWin(client);
				}
				
				Threads.WeatherThread.ShowWeather(client);
			}
		}
	}
}
