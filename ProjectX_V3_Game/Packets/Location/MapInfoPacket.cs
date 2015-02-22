//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Server -> Client.
	/// </summary>
	public class MapInfoPacket : DataPacket
	{
		public MapInfoPacket()
			: base(20, PacketType.MapInfoPacket)
		{
		}
		
		/// <summary>
		/// Gets or sets the mapid.
		/// </summary>
		public uint MapID
		{
			get { return ReadUInt32(4); }
			set { WriteUInt32(value, 4); }
		}
		
		/// <summary>
		/// Gets or sets the doc id.
		/// </summary>
		public uint DocID
		{
			get { return ReadUInt32(8); }
			set { WriteUInt32(value, 8); }
		}
		
		/// <summary>
		/// Gets or sets the flags.
		/// </summary>
		private ulong Flags
		{
			get { return ReadUInt64(12); }
			set { WriteUInt64(value, 12); }
		}
		
		/// <summary>
		/// The flag holder.
		/// </summary>
		private Enums.MapTypeFlags mapFlag;
		
		/// <summary>
		/// Adds a flag to the map info packet.
		/// </summary>
		/// <param name="Flag">The flag to add.</param>
		public void AddFlag(Enums.MapTypeFlags Flag)
		{
			mapFlag |= Flag;
		}
		
		/// <summary>
		/// Finishes the mapinfo packet with the added flags.
		/// </summary>
		public void Finish()
		{
			Flags = (ulong)mapFlag;
		}
	}
}
