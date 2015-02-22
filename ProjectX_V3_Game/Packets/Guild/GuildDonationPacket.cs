//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Description of GuildDonationPacket.
	/// </summary>
	public class GuildDonationPacket : DataPacket
	{
		public GuildDonationPacket()
			: base(52, PacketType.GuildDonationPacket)
		{
		}
		
		public Enums.GuildDonationFlags DonationFlag
		{
			get { return (Enums.GuildDonationFlags)ReadUInt32(4); }
			set { WriteUInt32((uint)value, 4); }
		}
		
		public uint Money
		{
			get { return ReadUInt32(8); }
			set { WriteUInt32(value, 8); }
		}
		
		public uint CPs
		{
			get { return ReadUInt32(12); }
			set { WriteUInt32(value, 12); }
		}
		
		public static void Handle(Entities.GameClient client, DataPacket packet)
		{
			if (client.Guild == null)
				return;
			
			using (var donation = new GuildDonationPacket())
			{
				donation.DonationFlag = Enums.GuildDonationFlags.AllDonations;
				donation.Money = client.GuildMemberInfo.MoneyDonation;
				donation.CPs = client.GuildMemberInfo.CPDonation;
				client.Send(donation);
			}
		}
		
		/**((GuildDonationFlags*)(ptr + 4)) = packet.UpdateFlags;
                *((int*)(ptr + 8)) = packet.Money;
                *((int*)(ptr + 12)) = packet.EMoney;
                *((int*)(ptr + 16)) = packet.Guide;
                *((int*)(ptr + 20)) = packet.Pk;
                *((int*)(ptr + 24)) = packet.Arsenal;
                *((uint*)(ptr + 28)) = packet.Rose;
                *((uint*)(ptr + 32)) = packet.Orchid;
                *((uint*)(ptr + 36)) = packet.Lily;
                *((uint*)(ptr + 40)) = packet.Tulip;
                *((uint*)(ptr + 44)) = packet.Exploits;
                *((uint*)(ptr + 48)) = packet.Unknown48;*/
	}
}
