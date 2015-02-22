//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Auth.Packets
{
	/// <summary>
	/// Server -> Client
	/// </summary>
	public class PasswordSeedPacket : DataPacket
	{
		public PasswordSeedPacket()
			: base(8, PacketType.PasswordSeedPacket)
		{
		}
		
		/// <summary>
		/// Gets or sets the password seed.
		/// </summary>
		public uint Seed
		{
			get { return ReadUInt32(4); }
			set { WriteUInt32(value, 4); }
		}
	}
}
