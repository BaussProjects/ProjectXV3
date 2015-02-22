//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Server -> Client
	/// Client -> Server
	/// </summary>
	public class GroundItemPacket : DataPacket
	{
		public GroundItemPacket()
			: base(32, PacketType.GroundItemPacket)
		{
			Unknown1 = 3;
		}
		public GroundItemPacket(DataPacket inPacket)
			: base(inPacket)
		{
			
		}
		
		public uint UID
		{
			get { return ReadUInt32(4); }
			set { WriteUInt32(value, 4); }
		}
		
		public uint ItemID
		{
			get { return ReadUInt32(8); }
			set { WriteUInt32(value, 8); }
		}
		
		public ushort X
		{
			get { return ReadUInt16(12); }
			set { WriteUInt16(value, 12); }
		}
		
		public ushort Y
		{
			get { return ReadUInt16(14); }
			set { WriteUInt16(value, 14); }
		}
		
		public ushort Unknown1
		{
			get { return ReadUInt16(16); }
			set { WriteUInt16(value, 16); }
		}
		
		public ushort DropType
		{
			get { return ReadUInt16(18); }
			set { WriteUInt16(value, 18); }
		}
		
		public static void Handle(Entities.GameClient client, DataPacket Packet)
		{
			if (client.Booth != null)
				return;
			if (!client.Alive)
				return;
			
			using (var grounditem = new GroundItemPacket(Packet))
			{
				if (grounditem.Unknown1 == 0)
				{
					Data.GroundItem gitem;
					Maps.IMapObject mapo;

					if (client.Map.Items.TryGetValue(grounditem.UID, out mapo))
					{
						if (!mapo.IsInMap(client))
							return;
						
						gitem = (Data.GroundItem)mapo;
						if (!gitem.PlayerDrop)
						{
							if (!(DateTime.Now >= gitem.DropTime.AddMilliseconds(20000)) && gitem.OwnerUID != 0)
							{
								if (gitem.OwnerUID != client.EntityUID)
								{
									using (var fmsg = Packets.Message.MessageCore.CreateSystem2(client.Name, Core.MessageConst.NOT_OWNER_ITEM))
										client.Send(fmsg);
									return;
								}
							}
						}
						if (Core.Screen.GetDistance(client.X, client.Y, gitem.X, gitem.Y) < 2)
						{
							if (client.Map.LeaveMap(gitem))
							{
								switch (gitem.DropType)
								{
									case Enums.DropItemType.Item:
										{
											if (client.Inventory.AddItem(gitem.Item))
											{
												ProjectX_V3_Lib.Threading.DelayedTask.Remove(gitem.TaskID);
												grounditem.DropType = 3;
												client.SendToScreen(grounditem, true);
												grounditem.X = 0;
												grounditem.Y = 0;
												grounditem.DropType = 2;
												client.SendToScreen(grounditem, true);
											}
											break;
										}
									case Enums.DropItemType.Gold:
										{
											client.Money += gitem.Money;
											gitem.Map.LeaveMap(gitem);
											gitem.Screen.ClearScreen();
											break;
										}
								}
							}
						}
					}
				}
			}
		}
	}
}
