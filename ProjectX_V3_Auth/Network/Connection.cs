//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Auth.Network
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
				Client.AuthClient client = new Client.AuthClient(socketClient);
				using (var packet = new Packets.PasswordSeedPacket())
				{
					packet.Seed = 90011337;
					client.Send(packet);
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
