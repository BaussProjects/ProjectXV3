//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Description of WarehousePacket.
	/// </summary>
	public class WarehousePacket : DataPacket
	{
		public WarehousePacket(Data.ItemInfo item)
			: base((ushort)(24 + (72 * 1)), PacketType.WarehousePacket)
		{
			WhType = 20;
			Action = Enums.WarehouseAction.Add;
			//Unknown12 = 60;
			WriteItem(item);
		}
		public WarehousePacket(DataPacket inPacket)
			: base(inPacket)
		{
			
		}
		
		public uint WarehouseID
		{
			get { return ReadUInt32(4); }
			set { WriteUInt32(value, 4); }
		}
		
		public Enums.WarehouseAction Action
		{
			get { return (Enums.WarehouseAction)ReadByte(8); }
			set { WriteByte((byte)value, 8); }
		}
		
		public byte WhType
		{
			get { return ReadByte(9); }
			set { WriteByte(value, 9); }
		}
		
		public uint Unknown12
		{
			get { return ReadUInt32(12); }
			set { WriteUInt32(value, 12); }
		}
		public uint Identifier
		{
			get { return ReadUInt32(16); }
			set { WriteUInt32(value, 16); }
		}
		
		private void WriteItem(Data.ItemInfo item)
		{
			if (item == null)
				return;
			WriteByte(1, 20);
			WriteUInt32(item.UID, 24);
            WriteUInt32(item.ItemID, 28);
            WriteByte((byte)item.Gem1, 33);
            WriteByte((byte)item.Gem2, 34);
            WriteByte(item.Plus, 41);
            WriteByte(item.Bless, 42);
            WriteBool(item.Free, 43);
            WriteUInt16(item.Enchant, 44);
            WriteUInt16(item.RebornEffect, 46);
            WriteBool(item.Locked, 48);
            WriteBool(item.Suspicious, 49);
            WriteByte((byte)item.Color, 51);
            WriteUInt32(item.SocketAndRGB, 52);
            WriteUInt32(item.Composition, 56);
			/*            WriteUInt32(item.UID, 24, buffer);
            WriteUInt32(item.ID, 28, buffer);
            WriteByte((byte)item.SocketOne, 33, buffer);
            WriteByte((byte)item.SocketTwo, 34, buffer);
            WriteByte(item.Plus, 41, buffer);
            WriteByte(item.Bless, 42, buffer);
            WriteByte((byte)(item.Bound == true ? 1 : 0), 43, buffer);
            WriteUInt16(item.Enchant, 44, buffer);
            WriteUInt16((ushort)item.Effect, 46, buffer);
            WriteByte(item.Lock, 48, buffer);
            WriteByte((byte)(item.Suspicious == true ? 1 : 0), 49, buffer);
            WriteByte((byte)item.Color, 51, buffer);
            WriteUInt32(item.SocketProgress, 52, buffer);
            WriteUInt32(item.PlusProgress, 56, buffer);*/
			//int loop = 0;
			//int Offset = 24;
			//foreach (Data.ItemInfo item in items)
			//{
				
				//int Offset = 24 + (loop * 56);
				//WriteUInt32(item.UID, Offset);
				//Offset += 4;
				//WriteUInt32(item.ItemID, Offset);
				/*Offset += 4;
				Offset++;
				WriteByte((byte)item.Gem1, Offset);
				Offset++;
				WriteByte((byte)item.Gem2, Offset);
				Offset++;
				Offset += 6;
				WriteByte(item.Plus, Offset);
				Offset++;
				WriteByte(item.Bless, Offset);
				Offset++;
				WriteBool(item.Free, Offset);
				Offset++;
				WriteByte(item.Enchant, Offset);
				Offset++;
				Offset++;
				Offset += 2;
				WriteBool(item.Suspicious, Offset);
				Offset+= 2;
				WriteBool(item.Locked, Offset);
				Offset++;
				WriteByte((byte)item.Color, Offset);
				Offset++;
				WriteUInt32(item.SocketAndRGB, Offset);
				Offset += 4;*/
			//	loop++;
			//	Offset += 56;
				/*
                        if (loop >= 19)
                            break;
                        int offset = 24 + (loop * 52);
                        *((uint*)(ptr + offset)) = item.UniqueID;
* offset += 4;
                        *((uint*)(ptr + offset)) = item.StaticID; 
* offset += 4;
                        offset++;
                        *((byte*)(ptr + offset)) = item.Gem1;
*  offset++;
                        *((byte*)(ptr + offset)) = item.Gem2;
*  offset++;
                        offset += 6;//unknown info... Dura?                        
                        *((byte*)(ptr + offset)) = item.Plus;
*  offset++;
                        *((byte*)(ptr + offset)) = item.Bless;
*  offset++;
                        free *((byte*)(ptr + offset)) = 1;
                        offset++;
                        *((byte*)(ptr + offset)) = item.Enchant;
*  offset++;
                        offset++;
                        offset += 2;
                        suspecious *((byte*)(ptr + offset)) = 1;
                        offset += 2;
                        locked *((byte*)(ptr + offset)) = 1;
                        offset++;
                        *((byte*)(ptr + offset)) = item.Color;
                        offset++;
                        *((uint*)(ptr + offset)) = item.SocketProgress;
                        offset += 4;
                        loop++;*/
			//}
		}
		
		public static void Handle(Entities.GameClient client, DataPacket packet)
		{
			using (var warehouse = new WarehousePacket(packet))
			{
				Data.Warehouse wh;
				if (client.Warehouses.TryGetValue((ushort)warehouse.WarehouseID, out wh))
				{
					switch (warehouse.Action)
					{
						case Enums.WarehouseAction.Display:
							{
								wh.SendAll();
								break;
							}
						case Enums.WarehouseAction.Add:
							{
								if (wh.FreeSpaces == 20)
									return;
								
								if (client.Inventory.ContainsByUID(warehouse.Identifier))
								{
									Data.ItemInfo item = client.Inventory.RemoveItemByUID(warehouse.Identifier);
									if (item != null)
									{
										wh.AddItem(item);
									}
								}
								break;
							}
						case Enums.WarehouseAction.Remove:
							{
								if (client.Inventory.FreeSpaces == 40)
									return;
								
								if (wh.ContainsByUID(warehouse.Identifier))
								{
									Data.ItemInfo item = wh.RemoveItemByUID(warehouse.Identifier);
									if (item != null)
									{
										client.Inventory.AddItem(item);
										//warehouse.WhType = 10;
										client.Send(warehouse);
									}
								}
								break;
							}
					}
				}
			}
		}
	}
}
