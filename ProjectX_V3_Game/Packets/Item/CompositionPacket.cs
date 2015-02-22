//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Description of CompositionPacket.
	/// </summary>
	public class CompositionPacket : DataPacket
	{
		public CompositionPacket(DataPacket inPacket)
			: base(inPacket)
		{
		}
		
		public Enums.CompositionType CompositionType
		{
			get { return (Enums.CompositionType)ReadByte(4); }
			set { WriteByte((byte)value, 4); }
		}
		
		public uint MainItem
		{
			get { return ReadUInt32(8); }
			set { WriteUInt32(value, 8); }
		}
		
		public uint MinorItem
		{
			get { return ReadUInt32(12); }
			set { WriteUInt32(value, 12); }
		}
		
		public static void Handle(Entities.GameClient client, DataPacket packet)
		{
			using (var composition = new CompositionPacket(packet))
			{
				if (!client.Inventory.ContainsByUID(composition.MainItem))
				{
					using (var msg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.ITEM_NOT_FOUND))
						client.Send(msg);
					return;
				}
				if (!client.Inventory.ContainsByUID(composition.MinorItem))
				{
					using (var msg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.ITEM_NOT_FOUND))
						client.Send(msg);
					return;
				}
				Data.ItemInfo MainItem = client.Inventory.GetItemByUID(composition.MainItem);
				if (MainItem.IsGarment() || MainItem.IsArrow() || MainItem.IsBottle() || MainItem.IsMisc() || MainItem.IsMountArmor())
				{
					using (var msg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.ITEM_INVALID_UPGRADE))
						client.Send(msg);
					return;
				}
				
				if (MainItem.CurrentDura < MainItem.MaxDura)
				{
					using (var msg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.ITEM_LOW_DURA))
						client.Send(msg);
					return;
				}
				Data.ItemInfo MinorItem = client.Inventory.GetItemByUID(composition.MinorItem);
				if (MainItem.Plus >= Core.NumericConst.MaxPlus)
				{
					using (var msg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.ITEM_MAX_PLUS))
						client.Send(msg);
					return;
				}
				if (MinorItem.Plus == 0)
				{
					using (var msg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.ITEM_NO_PLUS))
						client.Send(msg);
					return;
				}
				
				if (MinorItem.Plus < MainItem.Plus)
				{
					using (var msg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.ITEM_LOW_PLUS))
						client.Send(msg);
					return;
				}
				
				//ushort[] CompositionPoints = new ushort[12] { 20, 20, 80, 240, 720, 2160, 6480, 19440, 58320, 2700, 5500, 9000 };
				
				//ushort AddPoints = (CompositionPoints[MainItem.Plus] / 2);
				
				switch (composition.CompositionType)
				{
					case Enums.CompositionType.SteedComposition:
						{
							int color1 = (int)MainItem.SocketAndRGB;
							int color2 = (int)MinorItem.SocketAndRGB;
							int B1 = color1 & 0xFF;
							int B2 = color2 & 0xFF;
							int G1 = (color1 >> 8) & 0xFF;
							int G2 = (color2 >> 8) & 0xFF;
							int R1 = (color1 >> 16) & 0xFF;
							int R2 = (color2 >> 16) & 0xFF;
							int newB = (int)Math.Floor(0.9 * B1) + (int)Math.Floor(0.1 * B2);
							int newG = (int)Math.Floor(0.9 * G1) + (int)Math.Floor(0.1 * G2);
							int newR = (int)Math.Floor(0.9 * R1) + (int)Math.Floor(0.1 * R2);
							uint NewColor = (uint)(newB | (newG << 8) | (newR << 16));
							if (NewColor == MainItem.SocketAndRGB)
								return;

							MainItem.SocketAndRGB = NewColor;

							goto case Enums.CompositionType.BonusCompositionA;
						}
					case Enums.CompositionType.BonusCompositionA:
					case Enums.CompositionType.BonusCompositionB:
						{
							//uint CompositionPoints = Calculations.BasicCalculations.CompositionPoints(MinorItem.Plus);
							client.Inventory.RemoveItemByUID(MinorItem.UID);
							
							if (MainItem.Composition > 0)
							{
								MainItem.Plus++;
								MainItem.Composition = 0;
							}
							else
							{
								MainItem.Composition = 1;
							}					
							
							Database.CharacterDatabase.SaveInventory(client, MainItem, client.Inventory.GetPositionFromItemUID(MainItem.UID));
							MainItem.SendPacket(client, 3);
							break;
						}
				}
			}
		}
	}
}
