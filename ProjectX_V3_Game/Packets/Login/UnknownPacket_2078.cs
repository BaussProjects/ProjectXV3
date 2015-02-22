//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Server -> Client
	/// </summary>
	public class UnknownPacket_2078 : DataPacket
	{
		public UnknownPacket_2078()
			: base(264 + 8, 2078)
		{
			WriteUInt32(0x4e591dba, 4);
		}
	}
}
