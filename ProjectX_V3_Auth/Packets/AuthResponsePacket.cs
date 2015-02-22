//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Auth.Packets
{
	/// <summary>
	/// Server -> Client
	/// </summary>
	public class AuthResponsePacket : DataPacket
	{
		public AuthResponsePacket()
			: base(52, PacketType.AuthResponsePacket)
		{
		}
		
		/// <summary>
		/// Gets or sets the EntityUID. This is the UID that the client will go by.
		/// </summary>
		public uint EntityUID
		{
			get { return ReadUInt32(4); }
			set { WriteUInt32(value, 4); }
		}
		
		/// <summary>
		/// Gets or sets the account's authentication status.
		/// </summary>
		public Enums.AccountStatus AccountStatus
		{
			get { return (Enums.AccountStatus)ReadUInt32(8); }
			set { WriteUInt32((uint)value, 8); }
		}
		
		/// <summary>
		/// Gets or sets the port.
		/// </summary>
		public uint Port
		{
			get { return ReadUInt32(12); }
			set { WriteUInt32(value, 12); }
		}
		
		/// <summary>
		/// Gets or sets the login hash.
		/// </summary>
		public uint Hash
		{
			get { return ReadUInt32(16); }
			set { WriteUInt32(value, 16); }
		}
		
		/// <summary>
		/// Gets or sets the IPAddress.
		/// </summary>
		public string IPAddress
		{
			get { return ReadString(20, 16); }
			set { WriteString(value, 20); }
		}
	}
}
