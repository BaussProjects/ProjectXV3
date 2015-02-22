//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Auth.Client
{
	/// <summary>
	/// The auth client.
	/// </summary>
	public class AuthClient
	{
		/// <summary>
		/// The socket client associated to the auth client.
		/// </summary>
		private SocketClient socketClient;
		
		/// <summary>
		/// Gets the network client.
		/// </summary>
		public SocketClient NetworkClient
		{
			get { return socketClient; }
		}
		
		/// <summary>
		/// Creates a new instnace of AuthClient.
		/// </summary>
		/// <param name="socketClient">The socket client associated to the auth client.</param>
		public AuthClient(SocketClient socketClient)
		{
			crypto = new ProjectX_V3_Lib.Cryptography.AuthCrypto();
			
			socketClient.Owner = this;
			socketClient.Crypto = crypto;
			this.socketClient = socketClient;
		}
		
		/// <summary>
		/// The crypto used at the auth state.
		/// </summary>
		private ProjectX_V3_Lib.Cryptography.AuthCrypto crypto;
		
		/// <summary>
		/// Gets the auth crypto.
		/// </summary>
		public ProjectX_V3_Lib.Cryptography.AuthCrypto Crypto
		{
			get { return crypto; }
		}
		
		/// <summary>
		/// The server.
		/// </summary>
		public byte Server;
		
		/// <summary>
		/// The account.
		/// </summary>
		public string Account;
		
		/// <summary>
		/// The password.
		/// </summary>
		public string Password;
		
		/// <summary>
		/// The database uid.
		/// </summary>
		public int DatabaseUID;
		
		/// <summary>
		/// Entity uid.
		/// </summary>
		public uint EntityUID;
		
		/// <summary>
		/// Sends a packet to the client.
		/// </summary>
		/// <param name="Packet">The packet to send.</param>
		public void Send(DataPacket Packet)
		{
			NetworkClient.Send(Packet);
		}
	}
}
