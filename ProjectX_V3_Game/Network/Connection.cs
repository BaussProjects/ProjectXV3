//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Network
{
	/// <summary>
	/// Handling all network connections.
	/// </summary>
	public partial class NetworkConnections
	{
		/// <summary>
		/// Handling all connections.
		/// </summary>
		/// <param name="socketClient">The socket client.</param>
		public static bool Handle_Connection(SocketClient socketClient)
		{
			try
			{
				Entities.GameClient client = new ProjectX_V3_Game.Entities.GameClient(socketClient);
				
				using (var dhkeypacket = (client.NetworkClient.Crypto as ProjectX_V3_Lib.Cryptography.GameCrypto).GetExchangePacket())
				{
					client.NetworkClient.Send(dhkeypacket);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
			return true;
		}
	}
}
