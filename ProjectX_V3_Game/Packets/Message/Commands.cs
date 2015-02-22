//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Extensions;

namespace ProjectX_V3_Game.Packets.Message
{
	/// <summary>
	/// Handles all player commands.
	/// </summary>
	public class Commands
	{
		public static void Handle(Entities.GameClient client, string FullCommand, string[] Command)
		{
			if ((byte)client.Permission > 2)
				return;
			Command[0] = Command[0].Substring(1).ToLower();
			
			byte permission = (byte)client.Permission;
			
			#region Normal
			if (permission <= 3)
			{
				switch (Command[0])
				{
						#region dc
					case "dc":
						Handle_DC(client, FullCommand, Command);
						return;
						#endregion
						#region item
					case "item":
						Handle_Item(client, FullCommand, Command);
						return;
						#endregion
						#region Money
					case "money":
						Handle_Money(client, FullCommand, Command);
						return;
						#endregion
						#region Cps
					case "cps":
						Handle_CPs(client, FullCommand, Command);
						return;
						#endregion
						#region mm
					case "mm":
						Handle_MM(client, FullCommand, Command);
						return;
						#endregion
						#region Invalid
					default:
						{
							using (var msg = Message.MessageCore.CreateSystem(client.Name,
							                                                  string.Format(Core.MessageConst.UNKNOWN_COMMAND, Command[0])))
							{
								client.Send(msg);
							}
							break;
						}
						#endregion
				}
			}
			#endregion
			#region Mod
			if (permission <= 2)
			{
				switch (Command[0])
				{
						#region Invalid
					default:
						{
							using (var msg = Message.MessageCore.CreateSystem(client.Name,
							                                                  string.Format(Core.MessageConst.UNKNOWN_COMMAND, Command[0])))
							{
								client.Send(msg);
							}
							break;
						}
						#endregion
				}
			}
			#endregion
			#region GM
			if (permission <= 1)
			{
				switch (Command[0])
				{
						#region Invalid
					default:
						{
							using (var msg = Message.MessageCore.CreateSystem(client.Name,
							                                                  string.Format(Core.MessageConst.UNKNOWN_COMMAND, Command[0])))
							{
								client.Send(msg);
							}
							break;
						}
						#endregion
				}
			}
			#endregion
			#region PM
			if (permission <= 0)
			{
				switch (Command[0])
				{
						#region Invalid
					default:
						{
							using (var msg = Message.MessageCore.CreateSystem(client.Name,
							                                                  string.Format(Core.MessageConst.UNKNOWN_COMMAND, Command[0])))
							{
								client.Send(msg);
							}
							break;
						}
						#endregion
				}
			}
			#endregion
		}
		
		#region /dc | @dc
		/// <summary>
		/// Disconnects the client.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="FullCommand"></param>
		/// <param name="Command"></param>
		public static void Handle_DC(Entities.GameClient client, string FullCommand, string[] Command)
		{
			client.NetworkClient.Disconnect("Command");
		}
		#endregion
		#region /item Name + - HP gem1 gem2 | @item Name + - HP gem1 gem2
		/// <summary>
		/// Gives an item to yourself.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="FullCommand"></param>
		/// <param name="Command"></param>
		public static void Handle_Item(Entities.GameClient client, string FullCommand, string[] Command)
		{
			if (client.Inventory.Count >= 40)
				return;
			
			string Name = Command[1];
			Data.ItemInfo item;
			if (Core.Kernel.ItemInfos.TrySelect(Name, out item))
			{
				if (!item.IsMisc())
				{
					if (item.Quality != 9)
					{
						byte q = item.Quality;
						uint id = item.ItemID;
						
						while (q != 9)
						{
							id++;
							q++;
						}
						
						Data.ItemInfo Titem;
						if (Core.Kernel.ItemInfos.TrySelect(id, out Titem))
						{
							item = Titem;
						}
					}
					
					byte Plus = Command[2].ToByte();
					if (Plus > 9)
					{
						return;
					}
					item.Plus = Plus;
					byte Bless = Command[3].ToByte();
					if (Bless > 7)
					{
						return;
					}
					item.Bless = Bless;
					byte Enchant = Command[4].ToByte();
					if (Command[5].IsNumericString())
					{
						byte Gem1ID = Command[5].ToByte();
						Enums.SocketGem Gem1 = (Enums.SocketGem)Gem1ID;
						item.Gem1 = Gem1;
					}
					else
					{
						Enums.SocketGem Gem1;
						if (Enum.TryParse<Enums.SocketGem>(Command[5], out Gem1))
						{
							item.Gem1 = Gem1;
						}
					}
					if (Command[6].IsNumericString())
					{
						byte Gem2ID = Command[6].ToByte();
						Enums.SocketGem Gem2 = (Enums.SocketGem)Gem2ID;
						item.Gem2 = Gem2;
					}
					else
					{
						Enums.SocketGem Gem2;
						if (Enum.TryParse<Enums.SocketGem>(Command[6], out Gem2))
						{
							item.Gem2 = Gem2;
						}
					}
				}
				
				client.Inventory.AddItem(item);
				client.Inventory.SendItemToClient(item);
			}
			else
			{
				// invalid item ...
			}
		}
		#endregion
		#region /money amount | @money amount
		/// <summary>
		/// Gives them bitches some cash.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="FullCommand"></param>
		/// <param name="Command"></param>
		public static void Handle_Money(Entities.GameClient client, string FullCommand, string[] Command)
		{
			uint amount;
			if (!uint.TryParse(Command[1], out amount))
			{
				return;
			}
			client.Money = amount;
		}
		#endregion
		#region /cps amount | @cps amount
		/// <summary>
		/// Slut cash.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="FullCommand"></param>
		/// <param name="Command"></param>
		public static void Handle_CPs(Entities.GameClient client, string FullCommand, string[] Command)
		{
			uint amount;
			if (!uint.TryParse(Command[1], out amount))
			{
				return;
			}
			client.CPs = amount;
		}
		#endregion
		#region /mm map x y | @mm map x y
		/// <summary>
		/// Niggas be running from police.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="FullCommand"></param>
		/// <param name="Command"></param>
		public static void Handle_MM(Entities.GameClient client, string FullCommand, string[] Command)
		{
			ushort map;
			if (!ushort.TryParse(Command[1], out map))
			{
				return;
			}
			ushort x;
			if (!ushort.TryParse(Command[2], out x))
			{
				return;
			}
			ushort y;
			if (!ushort.TryParse(Command[3], out y))
			{
				return;
			}
			client.Teleport(map, x, y);
		}
		#endregion
	}
}
