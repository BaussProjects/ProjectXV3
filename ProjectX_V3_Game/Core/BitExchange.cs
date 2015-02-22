//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Core
{
	/// <summary>
	/// Bit exchanger.
	/// </summary>
	public class BitExchange
	{
		public static byte ExchangeBits(byte data, int bits)
		{
			return (byte)((data << bits) | (data >> bits));
		}

		public static uint ExchangeShortBits(uint data, int bits)
		{
			data &= 0xffff;
			return ((data >> bits) | (data << (16 - bits))) & 0xffff;
		}

		public static uint ExchangeLongBits(uint data, int bits)
		{
			return (data >> bits) | (data << (32 - bits));
		}
	}
}
