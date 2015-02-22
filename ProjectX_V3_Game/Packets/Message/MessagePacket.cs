//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Client -> Server
	/// Server -> Client
	/// </summary>
	public class MessagePacket : DataPacket
	{
		public MessagePacket(DataPacket inPacket)
			: base(inPacket)
		{
			string[] strings = StringPacker.Analyze(inPacket, 24);
			From = strings[0];
			To = strings[1];
			Unknown = strings[2];
			Message = strings[3];
		}
		
		public MessagePacket(StringPacker Message)
			: base((ushort)(24 + Message.Size), PacketType.MessagePacket)
		{
			Message.AppendAndFinish(this, 24);
			Time = 0;
		}
		
		/// <summary>
		/// The from name.
		/// </summary>
		public readonly string From;
		
		/// <summary>
		/// The to name.
		/// </summary>
		public readonly string To;
		
		public readonly string Unknown;
		
		/// <summary>
		/// The message.
		/// </summary>
		public readonly string Message;
		
		/// <summary>
		/// Gets or sets the color of the message.
		/// </summary>
		public System.Drawing.Color Color
		{
			get { return (System.Drawing.Color.FromArgb((int)ReadUInt32(4))); }
			set { WriteUInt32((uint)value.ToArgb(), 4); }
		}
		
		/// <summary>
		/// Gets or sets the chat type of the message.
		/// </summary>
		public Enums.ChatType ChatType
		{
			get { return (Enums.ChatType)ReadUInt32(8); }
			set { WriteUInt32((uint)value, 8); }
		}
		
		/// <summary>
		/// Gets or sets the time stamp.
		/// </summary>
		public uint Time
		{
			get { return ReadUInt32(12); }
			set { WriteUInt32(value, 12); }
		}
		
		/// <summary>
		/// Gets or sets the to mesh.
		/// </summary>
		public uint ToMesh
		{
			get { return ReadUInt32(16); }
			set { WriteUInt32(value, 16); }
		}
		
		/// <summary>
		/// Gets or sets the from mesh.
		/// </summary>
		public uint FromMesh
		{
			get { return ReadUInt32(20); }
			set { WriteUInt32(value, 20); }
		}
		
		/// <summary>
		/// Handles the message packet.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="packet">The packet.</param>
		public static void Handle(Entities.GameClient client, DataPacket packet)
		{
			using (var message = new MessagePacket(packet))
			{
				if (message.Message.StartsWith("/") && !message.Message.StartsWith("//")  || message.Message.StartsWith("@") && !message.Message.StartsWith("@@"))
				{
					Packets.Message.Commands.Handle(client, message.Message, message.Message.Split(' '));
				}
				else
				{
					if (message.ChatType != Enums.ChatType.Team && !client.Alive &&
					    message.ChatType != Enums.ChatType.Ghost)
						return;
					
					if (message.From != client.Name)
					{
						client.NetworkClient.Disconnect("INVALID_CHAT_NAME");
						return;
					}
					
					switch (message.ChatType)
					{
							#region Whisper
						case Enums.ChatType.Whisper:
							{
								if (message.To == client.Name)
									return;
								
								Entities.GameClient toclient;
								if (Core.Kernel.Clients.TrySelect(message.To, out toclient))
								{
									message.FromMesh = client.Mesh;
									message.ToMesh = toclient.Mesh;
									toclient.Send(message);
								}
								else
								{
									if (Database.CharacterDatabase.CharacterExists(message.To))//(System.IO.File.Exists(Database.ServerDatabase.DatabaseLocation + "\\UsedNames\\" + message.To + ".nm"))
									{
										using (var fmsg = Packets.Message.MessageCore.CreateSystem(
											client.Name, string.Format(Core.MessageConst.PLAYER_OFFLINE_WHISPER, message.To)))
											client.Send(fmsg);
										
										ProjectX_V3_Lib.IO.IniFile whisper = new ProjectX_V3_Lib.IO.IniFile(
											Database.ServerDatabase.DatabaseLocation + "\\MessageQuery\\" + message.To + ".ini",
											"Whisper");
										int count = whisper.ReadInt32("Count", 0);
										count++;
										whisper.Write<int>("Count", count);
										whisper.SetSection(count.ToString());
										whisper.WriteString("From", client.Name);
										whisper.WriteString("Message", message.Message);
										whisper.Write<uint>("Mesh", client.Mesh);
									}
									else
									{
										using (var fmsg = Packets.Message.MessageCore.CreateSystem(
											client.Name, string.Format(Core.MessageConst.PLAYER_OFFLINE_WHISPER2, message.To)))
											client.Send(fmsg);
									}
								}
								break;
							}
							#endregion
							#region Talk + Ghost
						case Enums.ChatType.Talk:
						case Enums.ChatType.Ghost:
							{
								client.SendToScreen(message, false, message.ChatType == Enums.ChatType.Ghost);
								break;
							}
							#endregion
							#region World
						case Enums.ChatType.World:
							{
								if (client.Level < 70)
								{
									using (var fmsg = Packets.Message.MessageCore.CreateSystem(
										client.Name, Core.MessageConst.WORLD_CHAT_NO_PERMISSION))
										client.Send(fmsg);
									return;
								}
								int required = 60000;
								if (client.Level >= 100)
									required = 45000;
								if (client.Level >= 110)
									required = 30000;
								if (client.Level >= 120)
									required = 15000;
								if (DateTime.Now >= client.WorldChatSend.AddMilliseconds(required))
								{
									client.WorldChatSend = DateTime.Now;
									foreach (Entities.GameClient sclient in Core.Kernel.Clients.selectorCollection1.Values)
									{
										if (sclient.EntityUID != client.EntityUID)
											sclient.Send(message);
									}
								}
								else
								{
									using (var fmsg = Packets.Message.MessageCore.CreateSystem(
										client.Name, string.Format(Core.MessageConst.WORLD_CHAT_WAIT, required)))
										client.Send(fmsg);
								}
								break;
							}
							#endregion
							#region Guild
						case Enums.ChatType.Guild:
							{
								if (client.Guild != null)
								{
									client.Guild.BroadcastMessage(message);
								}
								break;
							}
							#endregion
							#region Hawk
						case Enums.ChatType.HawkMessage:
							{
								if (client.Booth == null)
									return;
								client.Booth.HawkMessage = message.Message;
								break;
							}
							#endregion
							#region Team
						case Enums.ChatType.Team:
							{
								if (client.TournamentTeam != null)
								{
									foreach (Entities.GameClient teamMember in client.TournamentTeam.TeamMembers.ToDictionary().Values)
									{
										teamMember.Send(message);
									}
								}
								else if (client.Team != null)
								{
									foreach (Entities.GameClient teamMember in client.Team.Members.Values)
									{
										teamMember.Send(message);
									}
								}
								break;
							}
							#endregion
					}
				}
			}
		}
	}
}
