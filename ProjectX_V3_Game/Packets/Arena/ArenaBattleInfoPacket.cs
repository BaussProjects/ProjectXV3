//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;
using System.Linq;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Description of ArenaBattleInfoPacket.
	/// </summary>
	public class ArenaBattleInfoPacket : DataPacket
	{
		private int Count;
		
		public ArenaBattleInfoPacket(Data.ArenaMatch[] Matches)
			: base((ushort)((Matches == null ? 24 : 24 + (Matches.Length * 152))), PacketType.ArenaBattleInfoPacket)
		{
			if (Matches != null)
			{
				Count = Matches.Length;
			}
			else
				Count = 0;
			WriteUInt32(0x6, 8);

			if (Matches != null)
			{
				foreach (Data.ArenaMatch match in Matches)
					AppendMatch(match);
			}
		}
		
		public uint Page
		{
			get { return ReadUInt32(4); }
			set { WriteUInt32(value, 4); }
		}
		
		public uint Matches
		{
			get { return ReadUInt32(12); }
			set { WriteUInt32(value, 12); }
		}
		
		public uint PlayersInQueue
		{
			get { return ReadUInt32(16); }
			set { WriteUInt32(value, 16); }
		}
		
		public void SetSize()
		{
			WriteUInt32((uint)(Count - (Page - 1)), 24);
		}
		
		private int InfoOffset = 28;
		public void AppendMatch(Data.ArenaMatch Match)
		{
			WriteUInt32(Match.Player1.EntityUID, InfoOffset);
			InfoOffset += 4;
			WriteUInt32(Match.Player1.Mesh, InfoOffset);
			InfoOffset += 4;
			WriteString(Match.Player1.Name, InfoOffset);
			InfoOffset += 16;
			WriteUInt32(Match.Player1.Level, InfoOffset);
			InfoOffset += 4;
			WriteUInt32((uint)Match.Player1.Class, InfoOffset);
			InfoOffset += 4;
			// unknown ...
			InfoOffset += 4;
			WriteUInt32(Match.Player1.Arena.ArenaRanking, InfoOffset);
			InfoOffset += 4;
			WriteUInt32(Match.Player1.Arena.ArenaPoints, InfoOffset);
			InfoOffset += 4;
			WriteUInt32(Match.Player1.Arena.ArenaWinsToday, InfoOffset);
			InfoOffset += 4;
			WriteUInt32(Match.Player1.Arena.ArenaLossToday, InfoOffset);
			InfoOffset += 4;
			WriteUInt32(Match.Player1.Arena.ArenaHonorPoints, InfoOffset);
			InfoOffset += 4;
			WriteUInt32(Match.Player1.Arena.ArenaHonorPoints, InfoOffset);
			InfoOffset += 4;

			WriteUInt32(Match.Player2.EntityUID, InfoOffset);
			InfoOffset += 4;
			WriteUInt32(Match.Player2.Mesh, InfoOffset);
			InfoOffset += 4;
			WriteString(Match.Player2.Name, InfoOffset);
			InfoOffset += 16;
			WriteUInt32(Match.Player2.Level, InfoOffset);
			InfoOffset += 4;
			WriteUInt32((uint)Match.Player2.Class, InfoOffset);
			InfoOffset += 4;
			// unknown ...
			InfoOffset += 4;
			WriteUInt32(Match.Player2.Arena.ArenaRanking, InfoOffset);
			InfoOffset += 4;
			WriteUInt32(Match.Player2.Arena.ArenaPoints, InfoOffset);
			InfoOffset += 4;
			WriteUInt32(Match.Player2.Arena.ArenaWinsToday, InfoOffset);
			InfoOffset += 4;
			WriteUInt32(Match.Player2.Arena.ArenaLossToday, InfoOffset);
			InfoOffset += 4;
			WriteUInt32(Match.Player2.Arena.ArenaHonorPoints, InfoOffset);
			InfoOffset += 4;
			WriteUInt32(Match.Player2.Arena.ArenaHonorPoints, InfoOffset);
			
			// 76
			InfoOffset += 152;
		}
		/*
            MemoryStream strm = new MemoryStream();
            BinaryWriter wtr = new BinaryWriter(strm);
            wtr.Write((ushort)0); 0
            wtr.Write((ushort)2206); 2
            wtr.Write((uint)page); 4
            wtr.Write((uint)0x6); 8
            wtr.Write((uint)Matches.Count); 12
            wtr.Write((uint)PlayersInWaiting.Count); 16
            wtr.Write((uint)0); 20
            page--;
            wtr.Write((uint)(Matches.Count - page)); 24
            QualifierMatch[] GroupsList = Matches.ToArray();
            for (int count = page; count < page + 6; count++)
            {
                if (count >= Matches.Count)
                    break;

                QualifierMatch entry = GroupsList[count];

                wtr.Write((uint)entry.Player1.UID);
                wtr.Write((uint)entry.Player1.Mesh);
                for (int i = 0; i < 16; i++)
                {
                    if (i < entry.Player1.Name.Length)
                    {
                        wtr.Write((byte)entry.Player1.Name[i]);
                    }
                    else
                        wtr.Write((byte)0);
                }

                wtr.Write((uint)entry.Player1.Level);
                wtr.Write((uint)entry.Player1.Profession);
                wtr.Write((uint)0);
                wtr.Write((uint)entry.Player1.ArenaStats.Rank);
                wtr.Write((uint)entry.Player1.ArenaStats.ArenaPoints);
                wtr.Write((uint)entry.Player1.ArenaStats.WinsToday);
                wtr.Write((uint)entry.Player1.ArenaStats.LossesToday);
                wtr.Write((uint)entry.Player1.ArenaStats.CurrentHonor);
                wtr.Write((uint)entry.Player1.ArenaStats.TotalHonor);

                wtr.Write((uint)entry.Player2.UID);
                wtr.Write((uint)entry.Player2.Mesh);
                for (int i = 0; i < 16; i++)
                {
                    if (i < entry.Player2.Name.Length)
                    {
                        wtr.Write((byte)entry.Player2.Name[i]);
                    }
                    else
                        wtr.Write((byte)0);
                }

                wtr.Write((uint)entry.Player2.Level);
                wtr.Write((uint)entry.Player2.Profession);
                wtr.Write((uint)0);
                wtr.Write((uint)entry.Player2.ArenaStats.Rank);
                wtr.Write((uint)entry.Player2.ArenaStats.ArenaPoints);
                wtr.Write((uint)entry.Player2.ArenaStats.WinsToday);
                wtr.Write((uint)entry.Player2.ArenaStats.LossesToday);
                wtr.Write((uint)entry.Player2.ArenaStats.CurrentHonor);
                wtr.Write((uint)entry.Player2.ArenaStats.TotalHonor);
            }
            int packetlength = (int)strm.Length;
            strm.Position = 0;
            wtr.Write((ushort)packetlength);
            strm.Position = strm.Length;
            wtr.Write(Encoding.ASCII.GetBytes("TQServer"));
            strm.Position = 0;
            byte[] buf = new byte[strm.Length];
            strm.Read(buf, 0, buf.Length);
            wtr.Close();
            strm.Close();*/
		
		public static void Handle(Entities.GameClient client, DataPacket packet)
		{
			//if (Data.ArenaQualifier.MatchQueue.Count > 0)
			//{
			Data.ArenaMatch[] matchesArray = null;
			if (Data.ArenaQualifier.MatchQueue.Count > 0)
			{
				matchesArray = Data.ArenaQualifier.MatchQueue.ToDictionary().Values.ToArray();

				if (matchesArray.Length > 10)
					Array.Resize(ref matchesArray, 10);
			}
			using (var matches = new Packets.ArenaBattleInfoPacket(matchesArray))
			{
				matches.Page = packet.ReadUInt32(4);
				matches.SetSize();
				client.Send(matches);
			}
			//}
		}
	}
}
