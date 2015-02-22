//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Server -> Client
	/// </summary>
	public class StringPacket : DataPacket
	{
		public StringPacket(StringPacker strings)
			: base((ushort)(9 + strings.Size), PacketType.StringPacket)
		{
			strings.AppendAndFinish(this, 9);
		}
		
		public StringPacket(DataPacket inPacket)
			: base(inPacket)
		{
			Strings = StringPacker.Analyze(inPacket, 9);
		}
		public readonly string[] Strings;
		public uint Data
		{
			get { return ReadUInt32(4); }
			set { WriteUInt32(value, 4); }
		}

		public uint TargetId
		{
			get { return Data; }
			set { Data = value; }
		}

		public ushort PositionX
		{
			get { return (ushort)Data; }
			set { Data = (uint)((PositionY << 16) | value); }
		}

		public ushort PositionY
		{
			get { return (ushort)(Data >> 16); }
			set { Data = (uint)((value << 16) | PositionX); }
		}
		
		public Enums.StringAction Action
		{
			get { return (Enums.StringAction)ReadByte(8); }
			set { WriteByte((byte)value, 8); }
		}
		
		public static void Handle(Entities.GameClient client, DataPacket packet)
		{
			using (var strings = new StringPacket(packet))
			{
				switch (strings.Action)
				{
					case Enums.StringAction.WhisperWindowInfo:
						Packets.Strings.WhisperWindowInfo.Handle(client, strings);
						break;
					case Enums.StringAction.QueryMate:
						Packets.Strings.QueryMate.Handle(client, strings);
						break;
						
					default:
						Console.WriteLine("Unknown StringPacket {0} from {1}", strings.Action, client.Name);
						break;
				}
			}
		}
	}
}
