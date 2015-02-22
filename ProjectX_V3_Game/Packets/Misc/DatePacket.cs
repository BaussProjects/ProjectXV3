//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Server -> Client
	/// </summary>
	public class DatePacket : DataPacket
	{
		public DatePacket()
			: base(36, PacketType.DateTimeVigorPacket)
		{
			DateTime time = DateTime.Now;
			WriteInt32(time.Year - 1900, 8);
			WriteInt32(time.Month - 1, 12);
			WriteInt32(time.DayOfYear - 1, 16);
			WriteInt32(time.Day, 20);
			WriteInt32(time.Hour, 24);
			WriteInt32(time.Minute, 28);
			WriteInt32(time.Second, 32);
		}
		
		/*                *((int*) (ptr + 4)) = packet.Unknown;
                *((int*) (ptr + 8)) = packet.Year;
                *((int*) (ptr + 12)) = packet.Month;
                *((int*) (ptr + 16)) = packet.DayOfYear;
                *((int*) (ptr + 20)) = packet.Day;
                *((int*) (ptr + 24)) = packet.Hour;
                *((int*) (ptr + 28)) = packet.Minute;
                *((int*) (ptr + 32)) = packet.Second;*/
	}
}
