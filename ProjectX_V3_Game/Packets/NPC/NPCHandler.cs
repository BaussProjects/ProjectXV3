//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Extensions;

namespace ProjectX_V3_Game.Packets.NPC
{
	/// <summary>
	/// The npc handler for scripts.
	/// </summary>
	public class NPCHandler
	{
		#region Dialog
		/// <summary>
		/// Sends the dialog text of an npc.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="text">The text.</param>
		public static void SendDialog(Entities.GameClient client, string text)
		{
			using (var reply = new NPCResponsePacket(new StringPacker(text)))
			{
				reply.Action = Enums.NPCDialogAction.Text;
				reply.Option = 255;
				client.Send(reply);
			}
		}
		
		/// <summary>
		/// Sends a dialog option.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="text">The text of the option.</param>
		/// <param name="option">The dialog option id.</param>
		public static void SendOption(Entities.GameClient client, string text, byte option)
		{
			using (var reply = new NPCResponsePacket(new StringPacker(text)))
			{
				reply.Action = Enums.NPCDialogAction.Link;
				reply.Option = option;
				client.Send(reply);
			}
		}
		
		/// <summary>
		/// Sends a dialog inputbox.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="option">The dialog option id.</param>
		public static void SendInput(Entities.GameClient client, byte option)
		{
			using (var reply = new NPCResponsePacket(new StringPacker()))
			{
				reply.Action = Enums.NPCDialogAction.Edit;
				reply.Data = 16;
				reply.Option = option;
				client.Send(reply);
			}
		}
		
		/// <summary>
		/// Finishes the npc dialog.
		/// </summary>
		/// <param name="client">The client.</param>
		public static void Finish(Entities.GameClient client)
		{
			using (var reply = new NPCResponsePacket(new StringPacker()))
			{
				reply.Action = Enums.NPCDialogAction.Pic;
				reply.Data = client.CurrentNPC.Avatar;
				client.Send(reply);
			}
			using (var reply = new NPCResponsePacket(new StringPacker()))
			{
				reply.Action = Enums.NPCDialogAction.Create;
				reply.Option = 255;
				client.Send(reply);
			}
		}
		#endregion
		
		#region Marriage + Divorce
		public static void Marriage(Entities.GameClient client)
		{
			GeneralDataPacket pack = new GeneralDataPacket();
			pack.Id = client.EntityUID;
			pack.Data1 = 1067;
			pack.Action = Enums.DataAction.PostCmd; //(Enums.DataAction)116;
			pack.Data3Low = client.X;
			pack.Data3High = client.Y;
			client.Send(pack);
		}
		
		public static void Divorce(Entities.GameClient client)
		{
			int spouse = client.SpouseDatabaseUID;
			if (spouse > 0)
			{
				using (var msg =Packets.Message.MessageCore.CreateSystem("ALL",
				                                                         string.Format(Core.MessageConst.DIVORCE, client.Name, client.SpouseName)))
				{
					Packets.Message.MessageCore.SendGlobalMessage(msg);
				}
				
				uint euid = Database.CharacterDatabase.GetSpouseEntityUID(client);
				if (euid > 0)
				{
					Entities.GameClient sclient;
					if (Core.Kernel.Clients.TrySelect(euid, out sclient))
					{
						if (sclient.DatabaseUID == spouse)
						{
							Database.CharacterDatabase.RemoveSpouse(sclient);
							using (var mate = new Packets.StringPacket(new StringPacker("None")))
							{
								mate.Data = sclient.EntityUID;
								mate.Action = Enums.StringAction.Mate;
								sclient.Send(mate);
							}
						}
					}
				}
				using (var mate = new Packets.StringPacket(new StringPacker("None")))
				{
					mate.Data = client.EntityUID;
					mate.Action = Enums.StringAction.Mate;
					client.Send(mate);
				}
				Database.CharacterDatabase.RemoveSpouse(client);
			}
		}
		#endregion
		
		#region Guild
		public static void CreateGuild(Entities.GameClient client, string GuildName)
		{
			GuildName = GuildName.Replace(" ", "~");
			GuildName = GuildName.MakeReadable(true, false, true, true, false);
			GuildName = GuildName.StripSize(16);
			if (Core.Kernel.Guilds.Contains(GuildName))
			{
				SendDialog(client, "This guild exist already!");
				SendOption(client, "I see.", 255);
				Finish(client);
				return;
			}
			if (client.Guild != null)
				return;
			
			Data.Guild.Create(client, GuildName);
		}

		
		#endregion
		
		#region Misc
		//static uint NextWindow = 1;
		public static void OpenWindow(Entities.GameClient client, uint subtype)
		{
			Packets.GeneralDataPacket pack = new Packets.GeneralDataPacket();
			pack.Action = Enums.DataAction.OpenDialog;
			pack.Id = client.EntityUID;
			pack.Data1 = subtype;
			pack.Data2Low = client.X;
			pack.Data2High = client.Y;
			client.Send(pack);
			//NextWindow += 1;
		}
		
		public static void OpenUpgrade(Entities.GameClient client, uint subtype)
		{
			Packets.GeneralDataPacket pack = new Packets.GeneralDataPacket();
			pack.Action = Enums.DataAction.OpenUpgrade;
			pack.Id = client.EntityUID;
			pack.Data1 = subtype;
			pack.Data2Low = client.X;
			pack.Data2High = client.Y;
			client.Send(pack);
		}
		#endregion
	}
}
