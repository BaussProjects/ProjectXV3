//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Lib.Network
{
	/// <summary>
	/// A delegate associated with socket-connection events.
	/// </summary>
	public delegate bool ConnectionEvent(SocketClient sClient);
	
	/// <summary>
	/// A delegate associated with socket-buffer events.
	/// </summary>
	public delegate bool BufferEvent(SocketClient sClient, DataPacket Packet);
	
	/// <summary>
	/// Events called through sockets.
	/// </summary>
	public class SocketEvents
	{
		/// <summary>
		/// An event raised when a client is connected.
		/// </summary>
		public ConnectionEvent OnConnection = new ConnectionEvent(empty_conn);
		
		/// <summary>
		/// Empty method for the connection events.
		/// </summary>
		/// <param name="sClient">The socket client.</param>
		/// <returns>Returns true always.</returns>
		static bool empty_conn(SocketClient sClient) { return true; }
		
		/// <summary>
		/// An event raised when a client is disconnecting.
		/// </summary>
		public ConnectionEvent OnDisconnection = new ConnectionEvent(empty_conn);
		
		/// <summary>
		/// An event raised when a client has send data to the server.
		/// </summary>
		public BufferEvent OnReceive = new BufferEvent(empty_buff);
		
		/// <summary>
		/// Empty method for the buffer events.
		/// </summary>
		/// <param name="sClient">The socket client.</param>
		/// <param name="packet">The data packet.</param>
		/// <returns>Returns true always.</returns>
		static bool empty_buff(SocketClient sClient, DataPacket packet) { return true; }
	}
}
