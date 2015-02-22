//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Client -> Server
	/// Server -> Client
	/// </summary>
	public class TradePacket : DataPacket
	{
		public TradePacket()
			: base(20, PacketType.TradePacket)
		{
			
		}
		
		public TradePacket(DataPacket inPacket)
			: base(inPacket)
		{
			
		}
		
		public uint TargetUID
		{
			get { return ReadUInt32(4); }
			set { WriteUInt32(value, 4); }
		}
		
		public Enums.TradeType TradeType
		{
			get { return (Enums.TradeType)ReadUInt32(8); }
			set { WriteUInt32((uint)value, 8); }
		}
		
		public static void Handle(Entities.GameClient client, DataPacket packet)
		{
			if (!client.Alive)
				return;
			
			using (var trade = new TradePacket(packet))
			{
				switch (trade.TradeType)
				{
						#region Request : 1
					case Enums.TradeType.Request:
						Trade.Request.Handle(client, trade);
						break;
						#endregion
						#region Close : 2
					case Enums.TradeType.Close:
						Trade.Close.Handle(client, trade);
						break;
						#endregion
						#region TimeOut : 17
					case Enums.TradeType.TimeOut:
						Trade.TimeOut.Handle(client, trade);
						break;
						#endregion
						#region AddItem : 6
					case Enums.TradeType.AddItem:
						Trade.AddItem.Handle(client, trade);
						break;
						#endregion
						#region SetMoney : 7
					case Enums.TradeType.SetMoney:
						Trade.SetMoney.Handle(client, trade);
						break;
						#endregion
						#region SetConquerPoints : 12
					case Enums.TradeType.SetConquerPoints:
						Trade.SetConquerPoints.Handle(client, trade);
						break;
						#endregion
						#region Accept : 10
					case Enums.TradeType.Accept:
						Trade.Accept.Handle(client, trade);
						break;
						#endregion
						
						#region default
					default:
						Console.WriteLine("Unknown Trade Packet Sub-type: {0}", trade.ReadUInt32(8));
						break;
						#endregion
				}
			}
		}
	}
}
