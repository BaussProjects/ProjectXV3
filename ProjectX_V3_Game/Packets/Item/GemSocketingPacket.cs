//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Description of GemSocketingPacket.
	/// </summary>
	public class GemSocketingPacket : DataPacket
	{
		public GemSocketingPacket(DataPacket inPacket)
			: base(inPacket)
		{
		}
		
		public uint ItemUID
		{
			get { return ReadUInt32(8); }
			set { WriteUInt32(value, 8); }
		}
		
		public uint GemUID
		{
			get { return ReadUInt32(12); }
			set { WriteUInt32(value, 12); }
		}
		
		public byte Socket
		{
			get { return ReadByte(16); }
			set { WriteByte(value, 16); }
		}
		
		public bool RemoveGem
		{
			get { return ReadBool(18); }
			set { WriteBool(value, 18); }
		}
		
		public static void Handle(Entities.GameClient client, DataPacket packet)
		{
			if (client.Booth != null)
				return;
			using (var socket = new GemSocketingPacket(packet))
			{
				if (!client.Inventory.ContainsByUID(socket.ItemUID) &&
				    !client.Inventory.ContainsByUID(socket.GemUID))
				{
					return;
				}
				
				Data.ItemInfo SocketItem = client.Inventory.GetItemByUID(socket.ItemUID);
				if (SocketItem.CurrentDura < SocketItem.MaxDura)
					return;
				if (!socket.RemoveGem)
				{
					if (SocketItem.Gem1 != Enums.SocketGem.EmptySocket && socket.Socket == 1)
					{
						return;
					}
					else if (SocketItem.Gem2 != Enums.SocketGem.EmptySocket)
					{
						return;
					}
					
					Data.ItemInfo Gem = client.Inventory.GetItemByUID(socket.GemUID);
					if (Gem == null || SocketItem == null)
					{
						return;
					}
					if (SocketItem.IsGarment() || SocketItem.IsArrow() || SocketItem.IsBottle() ||
					    SocketItem.IsSteed() || SocketItem.IsMisc() || SocketItem.IsFan() || SocketItem.IsTower())
					{
						return;
					}
					
					Enums.SocketGem gem = (Enums.SocketGem)(Gem.ItemID % 100);
					
					if (gem != Enums.SocketGem.NormalThunderGem &&
					    gem != Enums.SocketGem.RefinedThunderGem &&
					    gem != Enums.SocketGem.SuperThunderGem &&
					    gem != Enums.SocketGem.NormalGloryGem &&
					    gem != Enums.SocketGem.RefinedGloryGem &&
					    gem != Enums.SocketGem.SuperGloryGem)
					{
						if (socket.Socket == 1)
						{
							SocketItem.Gem1 = gem;
						}
						else
						{
							SocketItem.Gem2 = gem;
						}
						
						Database.CharacterDatabase.SaveInventory(client, SocketItem, client.Inventory.GetPositionFromItemUID(SocketItem.UID));
						client.Inventory.RemoveItemByUID(Gem.UID);
						SocketItem.SendPacket(client, 3);
					}
				}
				else
				{
					if (SocketItem.Gem1 == Enums.SocketGem.EmptySocket && socket.Socket == 1
					    || SocketItem.Gem1 == Enums.SocketGem.NoSocket && socket.Socket == 1)
					{
						return;
					}
					else if (SocketItem.Gem2 == Enums.SocketGem.EmptySocket && socket.Socket != 1
					         ||SocketItem.Gem2 == Enums.SocketGem.NoSocket && socket.Socket != 1)
					{
						return;
					}
					
					if (socket.Socket == 1)
					{
						SocketItem.Gem1 = Enums.SocketGem.EmptySocket;
					}
					else
					{
						SocketItem.Gem2 = Enums.SocketGem.EmptySocket;
					}
					
					Database.CharacterDatabase.SaveInventory(client, SocketItem, client.Inventory.GetPositionFromItemUID(SocketItem.UID));
					SocketItem.SendPacket(client, 3);
				}
			}
		}
	}
}
