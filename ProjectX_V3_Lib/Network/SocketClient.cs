//Project by BaussHacker aka. L33TS

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ProjectX_V3_Lib.Network
{
	/// <summary>
	/// A socket client.
	/// </summary>
	public class SocketClient
	{
		/// <summary>
		/// The socket associated with the client.
		/// </summary>
		private Socket clientSocket;
		
		/// <summary>
		/// The socket events associated with the server.
		/// </summary>
		private SocketEvents socketEvents;
		
		/// <summary>
		/// The buffer holding all bytes received.
		/// </summary>
		private byte[] dataHolder;
		
		/// <summary>
		/// An object which is the owner of the client.
		/// </summary>
		public object Owner;
		
		public Cryptography.ConquerCrypto Crypto;
		
		/// <summary>
		/// Checks if the client is a camel.
		/// </summary>
		public bool IsACamel
		{
			get { return IP.StartsWith("41."); }
		}
		
		/// <summary>
		/// Gets the IPAddress associated with the client.
		/// </summary>
		public IPAddress Address
		{
			get
			{
				return (clientSocket.RemoteEndPoint as IPEndPoint).Address;
			}
		}

		/// <summary>
		/// Gets the IPAddress as a string.
		/// </summary>
		public string IP
		{
			get
			{
				return Address.ToString();
			}
		}
		
		/// <summary>
		/// Creates a new socket client.
		/// </summary>
		/// <param name="acceptedsocket">The socket associated with it.</param>
		/// <param name="socketEvents">The socket events associated with the server.</param>
		public SocketClient(Socket acceptedsocket, SocketEvents socketEvents)
		{
			clientSocket = acceptedsocket;
			//clientSocket.SendBufferSize = 1024;
			this.socketEvents = socketEvents;
			System.Threading.Interlocked.CompareExchange(ref send_lock, new object(), null);
		}
		
		private bool fakeconnection = false;
		public SocketClient()
		{
			fakeconnection = true;
		}
		
		/// <summary>
		/// Begins to receive data.
		/// </summary>
		internal void Receive()
		{
			if (!continue_rec)
				return;
			
			try
			{
				if (!Connected)
				{
					Disconnect("Not connected.");
					return;
				}
				
				dataHolder = new byte[65535];
				clientSocket.BeginReceive(dataHolder, 0, dataHolder.Length, SocketFlags.None, new AsyncCallback(Receive_Callback), null);
			}
			catch (Exception e)
			{
				Disconnect("Failed beginreceive. Exception: " + e.ToString());
			}
		}
		
		/// <summary>
		/// The callback from Receive().
		/// </summary>
		/// <param name="asyncResult">The async result.</param>
		private void Receive_Callback(IAsyncResult asyncResult)
		{
			try
			{
				if (!clientSocket.Connected)
				{
					Disconnect("Not connected.");
					return;
				}
				
				if (!continue_rec)
				{
					Disconnect("Not allowed to receive.");
					return;
				}
				
				SocketError err;
				int rSize = clientSocket.EndReceive(asyncResult, out err);
				
				if (err != SocketError.Success)
				{
					Disconnect("Failed receive. (99% regular DC) Socket Error: " + err.ToString());
					return;
				}
				
				if (rSize < 4)
				{
					Disconnect("Invalid Packet Header. (99% regular DC) Size: " + rSize);
					return;
				}
				
				byte[] rBuffer = new byte[rSize];
				System.Buffer.BlockCopy(dataHolder, 0, rBuffer, 0, rSize);
				
				using (var receiveData = new DataPacket(rBuffer))
				{
					if (Crypto != null)
					{
						lock (Crypto)
						{
							receiveData.Decrypt(Crypto);
							
							if (Crypto is Cryptography.GameCrypto)
							{
								Cryptography.GameCrypto crypto = (Crypto as Cryptography.GameCrypto);
								if (!crypto.Exchanged)
								{
									if (Cryptography.GameCrypto.HandleExchange(crypto, this, receiveData))
										Receive();
									return;
								}
							}
						}
					}
					if (receiveData.MatchingSize())
					{
						if (!alreadyDisconnected)
						{
							byte[] rcopy = receiveData.Copy();
							byte[] rpacket = new byte[receiveData.PacketSize];
							
							System.Buffer.BlockCopy(rcopy, 0, rpacket, 0, rpacket.Length);
							using (var packet = new DataPacket(rpacket))
							{
								continue_rec = socketEvents.OnReceive.Invoke(this, packet);
							}
							rcopy = null;
						}
					}
					else
						Split(receiveData.Copy());
				}
			}
			catch (Exception e)
			{
				Disconnect(e.ToString());
			}
			Receive();
		}
		
		private bool continue_rec = true;
		
		/// <summary>
		/// Temp. packet splitter similar to impulse (taken from albetros though)
		/// Will fix the old packet splitter later, but it caused some issues as of now...
		/// </summary>
		/// <param name="packet"></param>
		private void Split(byte[] packet)
		{
			ushort Length = BitConverter.ToUInt16(packet, 0);
			if ((Length + 8) == packet.Length)
			{
				byte[] rpacket = new byte[packet.Length - 8];
				System.Buffer.BlockCopy(packet, 0, rpacket, 0, rpacket.Length);
				using (var rPacket = new DataPacket(rpacket))
				{
					continue_rec = socketEvents.OnReceive.Invoke(this, rPacket);
				}
				return;
			}
			else if ((Length + 8) > packet.Length)
			{
				// Console.WriteLine("BIG: " + Length);
				return;
			}
			else
			{
				byte[] Packet = new byte[Length];
				Buffer.BlockCopy(packet, 0, Packet, 0, Length);
				byte[] _buffer = new byte[(packet.Length - (Length + 8))];
				Buffer.BlockCopy(packet, (Length + 8), _buffer, 0, (packet.Length - (Length + 8)));
				packet = _buffer;
				using (var rPacket = new DataPacket(Packet))
				{
					continue_rec = socketEvents.OnReceive.Invoke(this, rPacket);
				}
				Split(packet);
			}
		}
		
		private object send_lock;
		
		/// <summary>
		/// Sends data to the client.
		/// </summary>
		/// <param name="buffer">The buffer to send.</param>
		public void Send(DataPacket buffer)
		{
			//lock (lockme)
			//{
			//bool PacketSend = false;

			//try
			//{
			if (buffer.BufferLength > 65535)
			{
				Disconnect("Too big packet...");
				return;
			}
			
			byte[] Buffer = buffer.Copy();
			
			lock (send_lock)
			{
				if (Crypto != null)
				{
					//lock (Crypto)
					Crypto.Encrypt(Buffer);
				}
				
				
				try
				{
					// (PacketSend = Monitor.TryEnter(this, 50))
					//{
					if (clientSocket.Connected)
					{
						clientSocket.Send(Buffer);
						//clientSocket.BeginSend(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(Send_Callback), Buffer);
					}
					else
						Disconnect("Not Connected.");
				}
				catch (SocketException se)
				{
					Disconnect(string.Format("Failed to send packet... Error: {0}", se.SocketErrorCode.ToString()));
				}
				//
				//}
				//finally
				//{
				//	if (!PacketSend)
				//		Disconnect("Lag."); // lag
				//	else
				//		Monitor.Exit(this);
				//}
			}
		}

		/// <summary>
		/// The callback from Send().
		/// </summary>
		/// <param name="asyncResult">The async result.</param>
		private void Send_Callback(IAsyncResult asyncResult)
		{
			try
			{
				int send = clientSocket.EndSend(asyncResult);
				if (send < 4)
					Disconnect("Did not send proper packet header.");
				
				if (asyncResult.AsyncState != null && send > 0)
				{
					Array.Clear(((byte[])asyncResult.AsyncState), 0, send);
				}
			}
			catch
			{
			}
		}
		
		/// <summary>
		/// The disconnect reason.
		/// </summary>
		private string dcReason;
		
		/// <summary>
		/// Gets the disconnect reason.
		/// </summary>
		public string DCReason
		{
			get { return dcReason; }
		}
		
		public bool Connected
		{
			get
			{
				if (fakeconnection)
					return true;
				if (clientSocket == null)
					return false;
				return clientSocket.Connected;
			}
		}
		
		/// <summary>
		/// A boolean indicating whether the client has already disconnected or not.
		/// </summary>
		private bool alreadyDisconnected = false;
		
		/// <summary>
		/// Gets a boolean defining whether the client is already disconnected or not.
		/// </summary>
		public bool AlreadyDisconnected
		{
			get { return alreadyDisconnected; }
		}
		
		/// <summary>
		/// Disconnecting the client.
		/// </summary>
		/// <param name="reason">The reason for the disconnect.</param>
		public void Disconnect(string reason)
		{
			if (alreadyDisconnected)
				return;
			alreadyDisconnected = true;
			continue_rec = false;
			dcReason = reason;
			
			try
			{
				clientSocket.Disconnect(false);
				clientSocket.Shutdown(SocketShutdown.Both);
			}
			catch { }

			socketEvents.OnDisconnection.Invoke(this);
		}
		
		/// <summary>
		/// Disconnecting the client.
		/// </summary>
		public void Disconnect()
		{
			Disconnect(string.Empty);
		}
		
		/// <summary>
		/// Connects the socket to an endpoint.
		/// </summary>
		/// <param name="ip">The ip.</param>
		/// <param name="port">The port.</param>
		public void Connect(string ip, int port)
		{
			clientSocket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
			Receive();
		}
	}
}
