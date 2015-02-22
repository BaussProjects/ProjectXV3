//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;
using System.Collections.Concurrent;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Description of GuildMemberListPacket.
	/// </summary>
	public class GuildMemberListPacket : DataPacket
	{
		public class GuildMemberInfo
		{
			public string Name;
			public uint Unknown16;
			public uint Unknown20;
			public uint Level;
			public Enums.GuildRank Rank;
			public ushort Unknown30;
			public uint Unknown32;
			public int Donation;
			public bool IsOnline;
			public uint Unknown44;
			
			public static GuildMemberInfo Create(Data.GuildMember member)
			{
				GuildMemberInfo info = new GuildMemberListPacket.GuildMemberInfo();
				info.Name = member.Name;
				info.Level = member.Level;
				info.Rank = member.Rank;
				info.Donation = (int)member.MoneyDonation;
				info.IsOnline = member.Online;
				return info;
			}
			
			public void Append(DataPacket buffer, int offset, out int nextoffset)
			{
				nextoffset = offset;
				buffer.WriteString(Name, offset);
				buffer.WriteUInt32(Unknown16, offset + 16);
				buffer.WriteUInt32(Unknown20, offset + 20);
				buffer.WriteUInt32(Level, offset + 24);
				buffer.WriteUInt16((ushort)Rank, offset + 28);
				buffer.WriteUInt16(Unknown30, offset + 30);
				buffer.WriteUInt32(Unknown32, offset + 32);
				buffer.WriteInt32(Donation, offset + 36);
				buffer.WriteBool(IsOnline, offset + 40);
				buffer.WriteUInt32(Unknown44, offset + 44);
				nextoffset += 48;
			}
		}
		public GuildMemberListPacket(DataPacket inPacket)
			: base(inPacket)
		{
		}
		public GuildMemberListPacket(int Amount)
			: base((ushort)(20 + Amount * 48), PacketType.GuildMemberListPacket)
		{
			MemberList = new ConcurrentBag<GuildMemberListPacket.GuildMemberInfo>();
			WriteInt32(Amount, 12);
			this.Amount = Amount;
		}
		public uint Unknown4;
        public int StartIndex
        {
        	get { return ReadInt32(8); }
        	set { WriteInt32(value, 8); }
        }
        public int Amount;
        private ConcurrentBag<GuildMemberInfo> MemberList;
        public uint UnknownEnd;
        
        public void AddInfo(GuildMemberInfo info)
        {
        	if ((MemberList.Count + 1) > Amount)
        		return;
        	
        	MemberList.Add(info);
        }
        
		public static void Handle(Entities.GameClient client, DataPacket packet)
		{
			if (client.Guild == null)
				return;
			
			using (var info = new GuildMemberListPacket(packet))
			{
				Data.GuildMember[] members = client.Guild.SelectFromIndex(info.StartIndex);
				using (var sinfo = new GuildMemberListPacket(members.Length))
				{
					foreach (Data.GuildMember member in members)
						sinfo.AddInfo(GuildMemberInfo.Create(member));
					int offset = 16;
					foreach (GuildMemberInfo minfo in sinfo.MemberList)
					{
						minfo.Append(sinfo, offset, out offset);
					}
					client.Send(sinfo);
				}
			}
		}
	}
}
