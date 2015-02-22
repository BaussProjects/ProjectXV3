//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Description of ArenaActionPacket.
	/// </summary>
	public class ArenaActionPacket : DataPacket
	{
		public const uint ArenaIconOn = 0,
		ArenaIconOff = 1,
		StartCountDown = 2,
		OpponentGaveUp = 4,
		Match = 6,
		YouAreKicked = 7,
		StartTheFight = 8,
		Dialog = 9;
		
		public const uint Lose = 0,
		Win = 1,
		MatchOff = 0,
		MatchOn = 5;
		
		public ArenaActionPacket()
			: base(56, PacketType.ArenaActionPacket)
		{
		}
		
		public ArenaActionPacket(DataPacket inPacket)
			: base(inPacket)
		{
		}
		
		public uint DialogID
		{
			get { return ReadUInt32(4); }
			set { WriteUInt32(value, 4); }
		}
		
		public uint OptionID
		{
			get { return ReadUInt32(8); }
			set { WriteUInt32(value, 8); }
		}
		
		public uint EntityUID
		{
			get { return ReadUInt32(12); }
			set { WriteUInt32(value, 12); }
		}
		
		public string Name
		{
			get { return ReadString(16, 16); }
			set { WriteString(value, 16); }
		}
		
		public uint Rank
		{
			get { return ReadUInt32(32); }
			set { WriteUInt32(value, 32); }
		}
		
		public uint Class
		{
			get { return ReadUInt32(36); }
			set { WriteUInt32(value, 36); }
		}
		
		public uint Unknown40
		{
			get { return ReadUInt32(40); }
			set { WriteUInt32(value, 40); }
		}
		
		public uint ArenaPoints
		{
			get { return ReadUInt32(44); }
			set { WriteUInt32(value, 44); }
		}
		
		public uint Level
		{
			get { return ReadUInt32(48); }
			set { WriteUInt32(value, 48); }
		}
		
		public static void Handle(Entities.GameClient client, DataPacket packet)
		{
			using (var action = new ArenaActionPacket(packet))
			{
				switch (action.DialogID)
				{
					case 0:
						{
							Data.ArenaQualifier.JoinArena(client);
							client.Send(action);
							break;
						}
					case 1:
						{
							Data.ArenaQualifier.QuitWaitArena(client);
							client.Send(action);
							break;
						}
					case 3:
						{
							if (action.OptionID == 1)
							{
								Data.ArenaQualifier.AcceptArena(client);
							}
							else if (action.OptionID == 2)
							{
								Data.ArenaQualifier.GiveUpArena(client);
							}
							break;
						}
					case 4:
						{
							Data.ArenaQualifier.QuitArena(client);
							break;
						}
					case 10:
					case 11:
						{
							if (action.OptionID == 0)
							{
								Data.ArenaQualifier.JoinArena(client);
							}
							break;
						}
					default:
						Console.WriteLine("ARENA PACKET: {0} FROM {1}", action.DialogID, client.Name);
						break;
				}
			}
		}
	}
}
