//Project by BaussHacker aka. L33TS

using System;
using System.Net;
using System.Net.Sockets;

namespace ProjectX_V3_Lib.Network
{
	/// <summary>
	/// Description of SocketServer.
	/// </summary>
	public class SocketServer
	{
		/// <summary>
		/// The socket associated with the server.
		/// </summary>
		private Socket serverSocket;
		
		/// <summary>
		/// The socket events associated with the server.
		/// </summary>
		private SocketEvents socketEvents;
		
		/// <summary>
		/// Creates a new socket server.
		/// </summary>
		/// <param name="socketEvents">The events associated with the server. Put null for nothing.</param>
		public SocketServer(SocketEvents socketEvents)
		{
			if (socketEvents == null)
				this.socketEvents = new SocketEvents();
			else
				this.socketEvents = socketEvents;
			
			serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}
		
		/// <summary>
		/// Starts the server.
		/// </summary>
		/// <param name="ip">The IP of the server.</param>
		/// <param name="port">The port of the server.</param>
		public void Start(string ip, int port)
		{
			serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
			serverSocket.Listen(500);
			Accept();
		}
		
		/// <summary>
		/// Starts to accept connections.
		/// </summary>
		private void Accept()
		{
			serverSocket.BeginAccept(new AsyncCallback(Accept_Callback), null);
		}
		
		/// <summary>
		/// The callback from Accept()
		/// </summary>
		/// <param name="asyncResult">The async result.</param>
		private void Accept_Callback(IAsyncResult asyncResult)
		{
			try
			{
				Socket accepted = serverSocket.EndAccept(asyncResult);
				if (accepted.Connected)
				{
					SocketClient sClient = new SocketClient(accepted, socketEvents);
					if (socketEvents.OnConnection.Invoke(sClient))
						sClient.Receive();
				}
			}
			catch { }
			Accept();
		}
	}
}
