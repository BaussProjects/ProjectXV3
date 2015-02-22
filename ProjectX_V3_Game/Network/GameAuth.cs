//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;
using System.Collections.Concurrent;

namespace ProjectX_V3_Game.Network
{
	/// <summary>
	/// Handling the game auth server.
	/// </summary>
	public class GameAuth
	{
		/// <summary>
		/// A collection of UID's.
		/// </summary>
		private static ProjectX_V3_Lib.ThreadSafe.Selector<uint, int, DateTime> UIDCollection = new ProjectX_V3_Lib.ThreadSafe.Selector<uint, int, DateTime>();
		
		/// <summary>
		/// Gets a database UID based on an entity UID.
		/// </summary>
		/// <param name="EntityUID">The entity UID.</param>
		/// <param name="DatabaseUID">[out] The database UID.</param>
		/// <returns>Returns true if the id was retrieved.</returns>
		public static bool GetDatabaseUID(uint EntityUID, out int DatabaseUID)
		{
			return UIDCollection.TryGetSecondKey(EntityUID, out DatabaseUID);
		}
		
		/// <summary>
		/// Removes an entry from the UIDCollection.
		/// </summary>
		/// <param name="EntityUID">The entity UID.</param>
		public static void Remove(uint EntityUID)
		{
			UIDCollection.TryRemove(EntityUID);
		}
		
		/// <summary>
		/// Starts the game auth server.
		/// </summary>
		public static void Start()
		{
			SocketEvents socketEvents = new SocketEvents();
			socketEvents.OnReceive = new BufferEvent(HandlePacket);
			SocketServer gameauth = new SocketServer(socketEvents);
			gameauth.Start(Program.Config.ReadString("IPAddress"), Program.Config.ReadInt32("AuthPort"));
			
			// thread to clean for login : CHECK if UIDCollection contains the entity id at login (before adding to kernel.clients and before ANSWER_OK)
			new System.Threading.Thread(() =>
			                            {
			                            	while (true)
			                            	{
			                            		try
			                            		{
			                            			int failed;
			                            			UIDCollection.TryForeachAction((key1, time) =>
			                            			                               {
			                            			                               	try
			                            			                               	{
			                            			                               		if (DateTime.Now >= time)
			                            			                               		{
			                            			                               			int tries = 0;
			                            			                               			while (!UIDCollection.TryRemove(key1) && tries <= 3)
			                            			                               			{
			                            			                               				tries++;
			                            			                               				System.Threading.Thread.Sleep(20);
			                            			                               			}
			                            			                               		}
			                            			                               	}
			                            			                               	catch { }
			                            			                               }, out failed);
			                            			if (failed > 0)
			                            				Console.WriteLine("{0} failed to be removed from the login queue.", failed);
			                            		}
			                            		catch { }
			                            		System.Threading.Thread.Sleep(25000);
			                            	}
			                            }).Start();
		}
		
		/// <summary>
		/// Handles all packets received by the game auth server.
		/// </summary>
		/// <param name="client">The socket client.</param>
		/// <param name="Packet">The packet.</param>
		/// <returns>ALWAYS returns false.</returns>
		private static bool HandlePacket(SocketClient client, DataPacket Packet)
		{
			if (Packet.PacketID == 9001)
			{
				if (Packet.ReadString(4, 16) == Program.Config.ReadString("ServerPassword"))
				{
					string Account = Packet.ReadString(20, 16);
					int DatabaseUID = Packet.ReadInt32(36);
					if (DatabaseUID == 0)
						return false;
					uint EntityUID = Packet.ReadUInt32(40);
					if (EntityUID == 0)
						return false;
					
					if (Core.Kernel.Clients.Contains(EntityUID))
					{
						UIDCollection.TryRemove(EntityUID);
						return false;
					}
					
					if (UIDCollection.Contains(EntityUID))
					{
						int secondkey;
						if (UIDCollection.TryGetSecondKey(EntityUID, out secondkey))
						{
							if (DatabaseUID == secondkey)
								UIDCollection.TryRemove(EntityUID);
						}
						return false;
					}
					
					if (UIDCollection.Contains(DatabaseUID))
					{
						uint firstkey;
						if (UIDCollection.TryGetFirstKey(DatabaseUID, out firstkey))
						{
							UIDCollection.TryRemove(DatabaseUID);
						}
					}
					
					UIDCollection.TryAdd(EntityUID, DatabaseUID, DateTime.Now.AddMinutes(10));
					//	return false;
				}
			}
			return false;
		}
	}
}
