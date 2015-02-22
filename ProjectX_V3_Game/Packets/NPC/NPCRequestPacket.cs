//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Client -> Server
	/// </summary>
	public class NPCRequestPacket : DataPacket
	{
		public NPCRequestPacket(DataPacket inPacket)
			: base(inPacket)
		{
		}
		
		public uint NPCID
		{
			get { return ReadUInt32(4); }
		}
		public byte Option
		{
			get { return ReadByte(10); }
		}
		public static void Handle(Entities.GameClient client, DataPacket inPacket)
		{
			if (client.Trade.Trading)
				return;
			if (!client.Alive)
				return;
			
			using (var npcrequest = new NPCRequestPacket(inPacket))
			{
				try
				{
					if (npcrequest.Option == 255)
					{
						client.CurrentNPC = null;
						return;
					}
					
					if (Core.Kernel.Shops.ContainsKey(npcrequest.NPCID))
					{
						if (!Core.Kernel.Shops[npcrequest.NPCID].AssociatedNPC.IsInMap(client))
						{
							return;
						}
						
						Packets.GeneralDataPacket pack = new Packets.GeneralDataPacket();
						pack.Action = Enums.DataAction.OpenUpgrade;
						pack.Id = client.EntityUID;
						pack.Data1 = 32;
						pack.Data3Low = client.X;
						pack.Data3High = client.Y;
						pack.Timestamp = npcrequest.NPCID;
						client.Send(pack);
					}
					else
					{
						Entities.NPC npc = Core.Kernel.NPCs[npcrequest.NPCID];
						if (!npc.IsInMap(client))
						{
							return;
						}
						
						if (Core.Screen.GetDistance(client.X, client.Y, npc.X, npc.Y) >= Core.NumericConst.MaxNPCDistance && npc.NPCType != Enums.NPCType.Distance)
						{
							using (var fmsg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.TOO_FAR_NPC))
								client.Send(fmsg);
							return;
						}
						client.CurrentNPC = npc;
						
						client.CurrentNPC.CallDialog(client, npcrequest.Option);
					}
				}
				catch
				{
					using (var fmsg = Packets.Message.MessageCore.CreateSystem(client.Name, string.Format("NPCID: {0}", npcrequest.NPCID)))
						client.Send(fmsg);
				}
			}
		}
	}
}
