//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Description of NobilityPacket.
	/// </summary>
	public class NobilityPacket : DataPacket
	{
		public NobilityPacket(DataPacket inPacket)
			: base(inPacket)
		{
		}
		
		public NobilityPacket()
			: base(80, PacketType.NobilityPacket)
		{
			
		}
		
		public NobilityPacket(ushort Size)
			: base(Size, PacketType.NobilityPacket)
		{
			
		}
		
		public Enums.NobilityAction Action
		{
			get { return (Enums.NobilityAction)ReadUInt32(4); }
			set { WriteUInt32((uint)value, 4); }
		}
		
		public ushort Data1LowA
		{
			get { return ReadUInt16(8); }
			set { WriteUInt16(value, 8); }
		}
		
		public long Data1HighA
		{
			get { return ReadInt64(8); }
			set { WriteInt64(value, 8); }
		}

		public ushort Data2
		{
			get { return ReadUInt16(10); }
			set { WriteUInt16(value, 10); }
		}
		
		public uint Data3
		{
			get { return ReadUInt32(16); }
			set { WriteUInt32(value, 16); }
		}
		
		public uint Data4
		{
			get { return ReadUInt32(20); }
			set { WriteUInt32(value, 20); }
		}
		
		public uint Data5
		{
			get { return ReadUInt32(24); }
			set { WriteUInt32(value, 24); }
		}
		
		private int DataOffset = 32;
		private const int DataSize = 48;
		public void WriteNobilityData(Data.NobilityDonation donation)
		{
			if (donation.Client == null)
				WriteInt32(donation.RecordID, DataOffset);
			else
				WriteUInt32(donation.Client.EntityUID, DataOffset);
			//	Offset 4 = unknown UINT
			if (donation.Client != null)
			{
				WriteUInt32(donation.Client.Mesh, DataOffset + 8);
			}
			else
			{
				WriteUInt32(0, DataOffset + 8);
			}
			WriteString(donation.Name, DataOffset + 12);
			WriteInt64(donation.Donation,  DataOffset + 32);
			WriteUInt32((uint)donation.Rank, DataOffset + 40);
			WriteInt32(donation.Ranking, DataOffset + 44);
			DataOffset += DataSize;
		}
		public static int GetSize(int count)
		{
			return (DataSize * count);
		}
		
		#region Data1

		private long _data;
		
		/// <summary>
		/// Offset [8/0x08]
		/// </summary>
		public long Data1
		{
			get { return _data; }
			set { _data = value; }
		}

		/// <summary>
		/// Offset [8/0x08]
		/// </summary>
		public uint Data1Low
		{
			get { return (uint)_data; }
			set { _data = (Data1High << 32) | value; }
		}

		/// <summary>
		/// Offset [12/0x0c]
		/// </summary>
		public uint Data1High
		{
			get { return (uint)(_data >> 32); }
			set { _data = (((long)value << 32) | Data1Low); }
		}

		/// <summary>
		/// Offset [8/0x08]
		/// </summary>
		public ushort Data1LowLow
		{
			get { return (ushort)Data1Low; }
			set { Data1Low = (uint)((Data1LowHigh << 16) | value); }
		}

		/// <summary>
		/// Offset [10/0x0a]
		/// </summary>
		public ushort Data1LowHigh
		{
			get { return (ushort)(Data1Low >> 16); }
			set { Data1Low = (uint)((value << 16) | Data1LowLow); }
		}

		/// <summary>
		/// Offset [12/0x0c]
		/// </summary>
		public ushort Data1HighLow
		{
			get { return (ushort)Data1High; }
			set { Data1High = (uint)((Data1HighHigh << 16) | value); }
		}

		/// <summary>
		/// Offset [14/0x0e]
		/// </summary>
		public ushort Data1HighHigh
		{
			get { return (ushort)(Data1High >> 16); }
			set { Data1High = (uint)((value << 16) | Data1HighLow); }
		}

		#endregion

		private void WriteData()
		{
			WriteInt64(_data, 8);
		}
		
		public static void Handle(Entities.GameClient client, DataPacket packet)
		{
			using (var nobility = new NobilityPacket(packet))
			{
				switch (nobility.Action)
				{
					case Enums.NobilityAction.Donate:
						{
							if (client.Level < 70)
							{
								return;
							}
							if (nobility.Data3 == 2)
								Data.NobilityBoard.DonateCPs(client, nobility.Data1HighA);
							else if (nobility.Data3 == 0)
								Data.NobilityBoard.Donate(client, nobility.Data1HighA);
							break;
						}
						
					case Enums.NobilityAction.QueryRemainingSilver:
						{
							/*if (client.Nobility == null)
							{
								using (var query = new NobilityPacket())
								{
									query.Data4 = 60;
									query.Data1High = Data.NobilityBoard.RemainingDonation((Enums.NobilityRank)query.Data1Low, client.Nobility.Donation);
									// ...
								}
								return;
							}*/
							break;
						}
					case Enums.NobilityAction.List:
						{
							int PageMax;
							Data.NobilityDonation[] Page = Data.NobilityBoard.GetPage((int)nobility.Data1LowA, out PageMax);
							PageMax = (PageMax > 0 ? (PageMax + 1) : 0);
							if (Page != null)
							{
								if (Page.Length > 0)
								{
									using (var list = new NobilityPacket((ushort)(NobilityPacket.GetSize(Page.Length) + 32)))
									{
										list.Action = Enums.NobilityAction.List;
										list.Data1LowLow = nobility.Data1LowA;
										list.Data1LowHigh = (ushort)PageMax;
										list.Data1HighLow = (ushort)Page.Length;
										list.WriteData();
										foreach (Data.NobilityDonation donation in Page)
										{
											if (donation != null)
											{
												list.WriteNobilityData(donation);
											}
										}
										client.Send(list);
									}
								}
							}
							break;
						}
						
					default:
						Console.WriteLine("Unkown Nobility: {0} from {1}", nobility.Action, client.Name);
						break;
				}
			}
		}
	}
}
