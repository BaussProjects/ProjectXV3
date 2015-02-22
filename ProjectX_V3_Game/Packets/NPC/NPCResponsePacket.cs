//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Server -> Client
	/// Client -> Server
	/// </summary>
	public class NPCResponsePacket : DataPacket
	{
		public NPCResponsePacket(StringPacker strings)
			: base((ushort)(12 + strings.Size), PacketType.NPCResponsePacket)
		{
			strings.AppendAndFinish(this, 12);
		}
		public NPCResponsePacket(DataPacket inPacket)
			: base(inPacket)
		{
			string[] stringdata = StringPacker.Analyze(this, 12);
			if (stringdata.Length > 0)
				InputData = stringdata[0];
			else
				InputData = string.Empty;
		}
		
		public readonly string InputData;
		
		public uint TaskID
		{
			get { return ReadUInt32(4); }
			set { WriteUInt32(value, 4); }
		}

		public ushort TaskX
		{
			get { return (ushort)TaskID; }
			set { TaskID = (uint)((TaskY << 16) | value); }
		}

		public ushort TaskY
		{
			get { return (ushort)(TaskID >> 16); }
			set { TaskID = (uint)((value << 16) | TaskX); }
		}
		
		public byte Data
		{
			get { return ReadByte(8); }
			set { WriteByte(value, 8); }
		}
		
		public byte Option
		{
			get { return ReadByte(10); }
			set { WriteByte(value, 10); }
		}
		
		public Enums.NPCDialogAction Action
		{
			get { return (Enums.NPCDialogAction)ReadByte(11); }
			set { WriteByte((byte)value, 11); }
		}
		
		public static void Handle(Entities.GameClient client, DataPacket packet)
		{
			if (client.Trade.Trading)
				return;
			if (!client.Alive)
				return;
			
			using (var resp = new NPCResponsePacket(packet))
			{
				switch (resp.Action)
				{
					case Enums.NPCDialogAction.Popup:
					case Enums.NPCDialogAction.Answer:
						{
							if (client.CurrentNPC == null)
							{
								return;
							}
							if (resp.Option == 255)
							{
								return;
							}
							
							if (Core.Screen.GetDistance(client.X, client.Y, client.CurrentNPC.X, client.CurrentNPC.Y) >= Core.NumericConst.MaxNPCDistance
							    && client.CurrentNPC.NPCType != Enums.NPCType.Distance)
							{
								using (var fmsg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.TOO_FAR_NPC))
									client.Send(fmsg);
								return;
							}
							
							client.NPCInput = resp.InputData;
							client.CurrentNPC.CallDialog(client, resp.Option);
							break;
						}
				}
			}
		}
	}
}
