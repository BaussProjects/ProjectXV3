//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Network
{
	/// <summary>
	/// Handling all network connections.
	/// </summary>
	public partial class NetworkConnections
	{
		/// <summary>
		/// Handling all disconnections.
		/// </summary>
		/// <param name="socketClient">The socket client.</param>
		public static bool Handle_Disconnection(SocketClient socketClient)
		{
			try
			{
				if (socketClient.Owner == null)
					return false;
				
				Entities.GameClient client = socketClient.Owner as Entities.GameClient;
					int removetries = 0;
				if (client.LoggedIn && client.CanSave)
				{
					client.Screen.ClearScreen();
					
					Database.CharacterDatabase.Save(client);
					client.Inventory.SaveInventory();
					client.Equipments.SaveEquipments();
					
					if (client.ArenaMatch != null)
					{
						client.ArenaMatch.Quit(client);
					}
					
					client.LoggedIn = false;
					
					if (client.Booth != null)
					{
						client.Booth.CancelBooth();
					}
					if (client.Guild != null)
					{
						client.Guild = null;
						client.GuildMemberInfo.Client = null;
						client.GuildMemberInfo = null;
					}
					
					if (client.Nobility != null)
					{
						client.Nobility.Client = null;
						client.Nobility = null;
					}
					
					if (!string.IsNullOrEmpty(socketClient.DCReason) && !string.IsNullOrWhiteSpace(socketClient.DCReason))
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("{0} disconnected. Reason: {1}", client.Name, socketClient.DCReason);
						Console.ResetColor();
					}
					
					while (!client.Map.LeaveMap(client) && removetries < 5)
					{
						removetries++;
						System.Threading.Thread.Sleep(100);
					}
				}
				removetries = 0;
				while (!Core.Kernel.Clients.TryRemove(client.EntityUID) && removetries < 5)
				{
					removetries++;
					System.Threading.Thread.Sleep(100);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
			return true;
		}
	}
}
