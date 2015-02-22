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
		/// Handling all packets received.
		/// </summary>
		/// <param name="socketClient">The socket client.</param>
		/// <param name="Packet">The packet.</param>
		public static bool Handle_Receive(SocketClient socketClient, DataPacket Packet)
		{
			try
			{
				Client.AuthClient client = socketClient.Owner as Client.AuthClient;

				switch (Packet.PacketID)
				{
					case Packets.PacketType.AuthRequestPacket1:
					case Packets.PacketType.AuthRequestPacket2:
						Packets.AuthRequestPacket.Handle(client, Packet);
						break;
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
