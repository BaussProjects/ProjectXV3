//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;
using ProjectX_V3_Lib.Extensions;

namespace ProjectX_V3_Game.Network
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
				Entities.GameClient client = socketClient.Owner as Entities.GameClient;

				#region AntiBooth
				if (client.Booth != null)
				{
					switch (Packet.PacketID)
					{
						case Packets.PacketType.AuthMessagePacket:
						case Packets.PacketType.CharacterCreationPacket:
						case Packets.PacketType.MovementPacket:
						case Packets.PacketType.NPCRequestPacket:
						case Packets.PacketType.NPCResponsePacket:
						case Packets.PacketType.GroundItemPacket:
						case Packets.PacketType.InteractionPacket:
						case Packets.PacketType.GemSocketingPacket:
						case Packets.PacketType.WarehousePacket:
						case Packets.PacketType.CompositionPacket:
						case Packets.PacketType.ArenaActionPacket:
							return true;
					}
				}
				#endregion
				
				switch (Packet.PacketID)
				{
						#region AuthMessagePacket : 1052
					case Packets.PacketType.AuthMessagePacket:
						return Packets.AuthMessagePacket.Handle(client, Packet);
						#endregion
						#region CharacterCreationPacket : 1001
					case Packets.PacketType.CharacterCreationPacket:
						Packets.CharacterCreationPacket.Handle(client, Packet);
						break;
						#endregion
						#region GeneralDataPacket : 10010
					case Packets.PacketType.GeneralDataPacket:
						Packets.GeneralDataPacket.Handle(client, Packet);
						break;
						#endregion
						#region MessagePacket : 1004
					case Packets.PacketType.MessagePacket:
						Packets.MessagePacket.Handle(client, Packet);
						break;
						#endregion
						#region ItemPacket : 1009
					case Packets.PacketType.ItemPacket:
						Packets.ItemPacket.Handle(client, Packet);
						break;
						#endregion
						#region MovementPacket : 10005
					case Packets.PacketType.MovementPacket:
						Packets.MovementPacket.Handle(client, Packet);
						break;
						#endregion
						#region NPCRequestPacket : 2031
					case Packets.PacketType.NPCRequestPacket:
						Packets.NPCRequestPacket.Handle(client, Packet);
						break;
						#endregion
						#region NPCResponsePacket 2032
					case Packets.PacketType.NPCResponsePacket:
						Packets.NPCResponsePacket.Handle(client, Packet);
						break;
						#endregion
						#region GroundItemPacket : 1101
					case Packets.PacketType.GroundItemPacket:
						Packets.GroundItemPacket.Handle(client, Packet);
						break;
						#endregion
						#region TradePacket : 1056
					case Packets.PacketType.TradePacket:
						Packets.TradePacket.Handle(client, Packet);
						break;
						#endregion
						#region InteractionPacket : 1022
					case Packets.PacketType.InteractionPacket:
						Packets.InteractionPacket.Handle(client, Packet);
						break;
						#endregion
						#region CharacterStatsPacket : 1040
					case Packets.PacketType.CharacterStatsPacket:
						Packets.CharacterStatsPacket.Handle(client, Packet);
						break;
						#endregion
						#region Guild 1107
					case Packets.PacketType.GuildPacket:
						Packets.GuildPacket.Handle(client, Packet);
						break;
						#endregion
						#region GuildMemberListPacket : 2102
					case Packets.PacketType.GuildMemberListPacket:
						Packets.GuildMemberListPacket.Handle(client, Packet);
						break;
						#endregion
						#region GuildDonationPacket : 1058
					case Packets.PacketType.GuildDonationPacket:
						Packets.GuildDonationPacket.Handle(client, Packet);
						break;
						#endregion
						#region StringPacket : 1015
					case Packets.PacketType.StringPacket:
						Packets.StringPacket.Handle(client, Packet);
						break;
						#endregion
						#region BroadcastPacket : 2050
					case Packets.PacketType.BroadcastPacket:
						Packets.BroadcastPacket.Handle(client, Packet);
						break;
						#endregion
						#region NobilityPacket : 2064
					case Packets.PacketType.NobilityPacket:
						Packets.NobilityPacket.Handle(client, Packet);
						break;
						#endregion
						#region SubClassPacket : 2320
					case Packets.PacketType.SubClassPacket:
						Packets.SubClassPacket.Handle(client, Packet);
						break;
						#endregion
						#region TeamActionPacket : 1023
					case Packets.PacketType.TeamActionPacket:
						Packets.TeamActionPacket.Handle(client, Packet);
						break;
						#endregion
						#region GemSocketingPacket : 1023
					case Packets.PacketType.GemSocketingPacket:
						Packets.GemSocketingPacket.Handle(client, Packet);
						break;
						#endregion
						#region WarehousePacket : 1102
					case Packets.PacketType.WarehousePacket:
						Packets.WarehousePacket.Handle(client, Packet);
						break;
						#endregion
						#region CompositionPacket : 2036
					case Packets.PacketType.CompositionPacket:
						Packets.CompositionPacket.Handle(client, Packet);
						break;
						#endregion
						#region ArenaActionPacket : 2205
					case Packets.PacketType.ArenaActionPacket:
						Packets.ArenaActionPacket.Handle(client, Packet);
						break;
						#endregion
						#region ArenaPlayersPacket : 2208
					case Packets.PacketType.ArenaPlayersPacket:
						Packets.ArenaPlayersPacket.Handle(client, Packet);
						break;
						#endregion
						#region ArenaStatisticPacket : 2209
					case Packets.PacketType.ArenaStatisticPacket:
						Packets.ArenaStatisticPacket.Handle(client, Packet);
						break;
						#endregion
						#region ArenaBattleInfoPacket : 2206
					case Packets.PacketType.ArenaBattleInfoPacket:
						Packets.ArenaBattleInfoPacket.Handle(client, Packet);
						break;
						#endregion
						#region ArenaWatchPacket : 2211
					case Packets.PacketType.ArenaWatchPacket:
						Packets.ArenaWatchPacket.Handle(client, Packet);
						break;
						#endregion
						
						#region Unknown Packets
						// TODO: Log all unknown packets
						
						#region 1151
					case 1151:
						// This packet is send from the client to the server during login, not sure if send at other times.
						break;
						#endregion
						#region 1037
					case 1037:
						// This packet is send over time ... cba to analyze it as I could not find any info on it, so probs not important.
						break;
						#endregion
						
						#endregion
						
						#region Unhandled PAckets
						// all packets here is known by type, but not handled yet...
						
						#region QuestPacket : 1134
					case 1134:
						break;
						#endregion
						
						#endregion
						
						#region default
					default:
						{
							if (client.LoggedIn)
								Console.WriteLine("Unknown PacketID: {0} PacketSize: {1} User: {2}", Packet.PacketID, Packet.PacketSize, client.Name);
							break;
						}
						#endregion
				}
			}
			catch (Exception e)
			{
				string eString = e.ToString();
				if (eString.Contains("get_PacketID()"))
				{
					socketClient.Disconnect("Failed to get PacketID... Pointer = null.");
					return false;
				}
				Console.WriteLine("[Major Exception]");
				Console.WriteLine(eString);
			}
			return true;
		}
	}
}
