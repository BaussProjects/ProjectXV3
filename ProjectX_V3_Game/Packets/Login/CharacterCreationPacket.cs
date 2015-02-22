//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;
using ProjectX_V3_Lib.Extensions;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Client -> Server
	/// </summary>
	public class CharacterCreationPacket : DataPacket
	{
		public CharacterCreationPacket(DataPacket inPacket)
			: base(inPacket)
		{
		}
		
		/// <summary>
		/// Gets the name.
		/// </summary>
		public string Name
		{
			get { return ReadString(20, 16); }
		}
		
		/// <summary>
		/// Gets the model.
		/// </summary>
		public ushort Model
		{
			get { return ReadUInt16(72); }
		}
		
		/// <summary>
		/// Gets the job.
		/// </summary>
		public ushort Job
		{
			get { return ReadUInt16(74); }
		}
		
		/// <summary>
		/// Gets the EntityUID.
		/// </summary>
		public uint EntityUID
		{
			get { return ReadUInt32(76); }
		}
		
		/// <summary>
		/// Handles the CharacterCreationPacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="Packet">The packet.</param>
		public static void Handle(Entities.GameClient client, DataPacket Packet)
		{
			try
			{
				using (var create = new CharacterCreationPacket(Packet))
				{
					if (create.ReadByte(4) != 0)
					{
						client.NetworkClient.Disconnect("Character creation disconnect...");
						return;
					}
					if (create.EntityUID != client.EntityUID)
					{
						client.NetworkClient.Disconnect("Invalid EntityUID");
						return;
					}
					if (create.Model != 1003 && create.Model != 1004 && create.Model != 2001 && create.Model != 2002)
					{
						using (var msg = Packets.Message.MessageCore.CreateCharacter(Core.MessageConst.INVALID_MODEL))
							client.Send(msg);
						return;
					}
					if (create.Job != 10 && create.Job != 20 && create.Job != 40 && create.Job != 50 /*&& create.Job != 60*/ && create.Job != 100)
					{
						using (var msg = Packets.Message.MessageCore.CreateCharacter(Core.MessageConst.INVALID_CLASS))
							client.Send(msg);
						return;
					}
					string Name = create.Name.MakeReadable(true, false, true, true, false);
					
					if (string.IsNullOrEmpty(Name) || string.IsNullOrWhiteSpace(Name))
					{
						using (var msg = Packets.Message.MessageCore.CreateCharacter(Core.MessageConst.INVALID_CHARS))
							client.Send(msg);
						return;
					}
					if (Core.Kernel.IsBannedName(Name))
					{
						using (var msg = Packets.Message.MessageCore.CreateCharacter(Core.MessageConst.INVALID_NAME))
							client.Send(msg);
						return;
					}
					if (Database.CharacterDatabase.CharacterExists(Name))
					{
						using (var msg = Packets.Message.MessageCore.CreateCharacter(Core.MessageConst.CHAR_EXIST))
							client.Send(msg);
						return;
					}
					
					ushort model = create.Model;
					if (client.NetworkClient.IsACamel)
					{
						model = (ushort)(model == 2000 ? 1004 : model);
						model = (ushort)(model == 2001 ? 1003 : model);
					}
					
					if (!Database.CharacterDatabase.CreateCharacter(client, Name, (byte)create.Job, model))
					{
						client.NetworkClient.Disconnect("Failed to create the character...");
					}
					else
					{
						using (var msg = Packets.Message.MessageCore.CreateCharacter("ANSWER_OK"))
							client.Send(msg);
						bool newchar;
						if (Database.CharacterDatabase.LoadCharacter(client, out newchar))
						{
							if (!Core.Kernel.GotPermission(client.Permission))
							{
								client.NetworkClient.Disconnect("No permission to join the server.");
								return;
							}
							
							// sometimes this fails... so dc as of now.
							
							// character info
							using (var charinfo = Packets.CharacterInfoPacket.Create(client))
								client.Send(charinfo);
							// datetime
							using (var datetime = new Packets.DatePacket())
								client.Send(datetime);
							client.NetworkClient.Disconnect("Character created...");
						}
						else
							client.NetworkClient.Disconnect("Failed to login...");
					}
				}
			}
			catch (Exception e)
			{
				client.NetworkClient.Disconnect(e.ToString());
			}
		}
	}
}
