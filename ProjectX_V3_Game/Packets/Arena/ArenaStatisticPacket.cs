//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Description of ArenaStatisticPacket.
	/// </summary>
	public class ArenaStatisticPacket : DataPacket
	{
		public ArenaStatisticPacket()
			: base(52, PacketType.ArenaStatisticPacket)
		{
		}
		
		public uint Rank
		{
			get { return ReadUInt32(4); }
			set { WriteUInt32(value, 4); }
		}
		
		public uint Unknown8
		{
			get { return ReadUInt32(8); }
			set { WriteUInt32(value, 8); }
		}
		
		public Enums.ArenaStatus Status
		{
			get { return (Enums.ArenaStatus)ReadUInt32(12); }
			set { WriteUInt32((uint)value, 12); }
		}
		
		public uint WinsTotal
		{
			get { return ReadUInt32(16); }
			set { WriteUInt32(value, 16); }
		}
		
		public uint LossesTotal
		{
			get { return ReadUInt32(20); }
			set { WriteUInt32(value, 20); }
		}
		
		public uint WinsToday
		{
			get { return ReadUInt32(24); }
			set { WriteUInt32(value, 24); }
		}
		
		public uint LossesToday
		{
			get { return ReadUInt32(28); }
			set { WriteUInt32(value, 28); }
		}
		
		public uint TotalHonor
		{
			get { return ReadUInt32(32); }
			set { WriteUInt32(value, 32); }
		}

		public uint CurrentHonor
		{
			get { return ReadUInt32(36); }
			set { WriteUInt32(value, 36); }
		}
		
		public uint ArenaPoints
		{
			get { return ReadUInt32(40); }
			set { WriteUInt32(value, 40); }
		}
		
		public uint WinsSeason
		{
			get { return ReadUInt32(44); }
			set { WriteUInt32(value, 44); }
		}
		
		public uint LossesSeason
		{
			get { return ReadUInt32(48); }
			set { WriteUInt32(value, 48); }
		}
		
		public static void Handle(Entities.GameClient client, DataPacket packet)
		{
			using (var wait = client.Arena.Build())
			{
				client.Send(wait);
			}
		}
	}
}
