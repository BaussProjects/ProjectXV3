//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Description of SteedVigorPacket.
	/// </summary>
	public class SteedVigorPacket : DataPacket
	{
		public SteedVigorPacket()
			: base(36, PacketType.DateTimeVigorPacket)
		{
		}
		
		public uint Type
		{
			get { return ReadUInt32(4); }
			set { WriteUInt32(value, 4); }
		}
		
		public uint Amount
		{
			get { return ReadUInt32(8); }
			set { WriteUInt32(value, 8); }
		}
		/*var buffer = new byte[36 + 8];
            fixed (byte* ptr = buffer)
            {
                PacketBuilder.AppendHeader(ptr, buffer.Length, 1033);
                *((uint*)(ptr + 4)) = packet.Type;
                *((uint*)(ptr + 8)) = packet.Amount;
                *((uint*)(ptr + 12)) = packet.Unknown1;
                *((uint*)(ptr + 16)) = packet.Unknown2;
                *((uint*)(ptr + 20)) = packet.Unknown3;
                *((uint*)(ptr + 24)) = packet.Unknown4;
                *((uint*)(ptr + 28)) = packet.Unknown5;
                *((uint*)(ptr + 32)) = packet.Unknown6;
                *((uint*)(ptr + 36)) = packet.Unknown7;

            }*/
	}
}
