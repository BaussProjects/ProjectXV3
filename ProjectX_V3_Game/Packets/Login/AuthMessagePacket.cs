//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Client -> Server
	/// </summary>
	public class AuthMessagePacket : DataPacket
	{
		public AuthMessagePacket(DataPacket inPacket)
			: base(inPacket)
		{
		}
		
		/// <summary>
		/// Gets the EntityUID.
		/// </summary>
		public uint EntityUID
		{
			get { return ReadUInt32(4); }
		}
		
		/// <summary>
		/// Gets the key.
		/// </summary>
		public uint Key
		{
			get { return ReadUInt32(8); }
		}
		
		/// <summary>
		/// Handles the AuthMessagePacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="Packet">The packet.</param>
		/// <returns>Returns true if the authentication was a success.</returns>
		public static bool Handle(Entities.GameClient client, DataPacket Packet)
		{
			if (!Program.AllowConnections)
			{
				using (var msg = Packets.Message.MessageCore.CreateLogin(Core.MessageConst.SERVER_CONNECTION_OFF))
					client.Send(msg);
				System.Threading.Thread.Sleep(5000);
				client.NetworkClient.Disconnect("Connections disabled...");
				return false;
			}
			try
			{
				using (var authmsg = new AuthMessagePacket(Packet))
				{
					// disconnecting on anything invalid, because it's already handled at the auth server ...
					
					if (authmsg.Key != 2)
					{
						// invalid username, password and/or banned etc.
						client.NetworkClient.Disconnect();
						return false;
					}
					
					int DatabaseUID;
					if (!Network.GameAuth.GetDatabaseUID(authmsg.EntityUID, out DatabaseUID))
					{
						// not even authenticated ...
						client.NetworkClient.Disconnect();
						return false;
					}
					
					if (Core.Kernel.Clients.Contains(authmsg.EntityUID))
					{
						// already an existing client using the entity uid ...
						
						Entities.GameClient secondclient;
						if (Core.Kernel.Clients.TrySelect(authmsg.EntityUID, out secondclient))
						{
							if (secondclient.DatabaseUID == DatabaseUID)
							{
								// same client...
								secondclient.NetworkClient.Disconnect();
							}
						}
						client.NetworkClient.Disconnect();
						return false;
					}
					
					client.EntityUID = authmsg.EntityUID;
					client.DatabaseUID = DatabaseUID;
					
					Network.GameAuth.Remove(authmsg.EntityUID);
					
					bool match = false;
					int failed;
					if (!Core.Kernel.Clients.TryForeachAction((uid, _client) =>
					                                          {
					                                          	if (_client.DatabaseUID == client.DatabaseUID)
					                                          	{
					                                          		match = true;
					                                          		_client.NetworkClient.Disconnect("Duplicate login.");
					                                          	}
					                                          }, out failed))
					{
						client.NetworkClient.Disconnect();
						return false;
					}
					if (match)
					{
						client.NetworkClient.Disconnect();
						return false;
					}
					
					using (var unknown1 = new Packets.UnknownPacket_2079())
						client.Send(unknown1);
					using (var unknown2 = new Packets.UnknownPacket_2078())
						client.Send(unknown2);

					bool newchar;
					if (ProjectX_V3_Game.Database.CharacterDatabase.LoadCharacter(client, out newchar))
					{
						if (!Core.Kernel.GotPermission(client.Permission))
						{
							client.NetworkClient.Disconnect("No permission to join the server.");
							return false;
						}
						using (var msg = Packets.Message.MessageCore.CreateLogin(Core.MessageConst.ANSWER_OK))
							client.Send(msg);

						// character info
						using (var charinfo = Packets.CharacterInfoPacket.Create(client))
							client.Send(charinfo);
						using (var datetime = new Packets.DatePacket())
							client.Send(datetime);
						
						
						if (!Core.Kernel.Clients.TryAdd(client.EntityUID, client.Name, client))
						{
							client.NetworkClient.Disconnect("Could not add the client to the collection.");
							return false;
						}
					}
					else if (newchar)
					{
						using (var msg = Packets.Message.MessageCore.CreateLogin(Core.MessageConst.NEW_ROLE))
							client.Send(msg);
					}
					else
					{
						using (var msg = Packets.Message.MessageCore.CreateLogin(Core.MessageConst.ANSWER_NO))
							client.Send(msg);
					}
				}
				
				return true;
			}
			catch (Exception e)
			{
				client.NetworkClient.Disconnect(e.ToString());
				return false;
			}
		}
	}
}
