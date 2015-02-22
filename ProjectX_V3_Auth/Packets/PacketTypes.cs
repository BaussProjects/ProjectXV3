//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Auth.Packets
{
	/// <summary>
	/// Packet type constants.
	/// </summary>
	public class PacketType
	{
		public const ushort
			PasswordSeedPacket = 1059,
			AuthRequestPacket1 = 1060,
			AuthRequestPacket2 = 1086,
			AuthResponsePacket = 1055;
	}
}
