//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Server -> Client
	/// </summary>
	public class UnknownPacket_2079 : DataPacket
	{
		public UnknownPacket_2079()
			: base(8 + 8, 2079)
		{
			WriteUInt32(0, 4);
		}
	}
}
