//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Game.Packets;

namespace ProjectX_V3_Game.Core
{
	/// <summary>
	/// Description of LoginHandler.
	/// </summary>
	public class LoginHandler
	{
		public static void Handle(Entities.GameClient client)
		{
			// Send vip, merchant, quiz points and heavenbless etc.
			
			client.BaseEntity.SetBaseStats();
			client.BaseEntity.CalculateBaseStats();
			
			client.Stamina = 50;
			client.PKPoints = client.PKPoints; // update status effect
			client.BoundCPs = client.BoundCPs; // for some reason this has to be done here ...
			
			using (ProjectX_V3_Lib.Sql.SqlHandler sql = Database.CharacterDatabase.OpenRead(client, "DB_Players"))
				client.StatusFlag2 = sql.ReadUInt64("PlayerStatusFlag");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(Core.MessageConst.LOGIN_MSG, client.Name, client.Account, client.EntityUID, client.DatabaseUID);
			Console.ResetColor();
			
			MessagePacket bc = Threads.BroadcastThread.GetLastBroadcast();
			if (bc != null)
			{
				client.Send(bc);
				bc.Dispose();
			}
			Database.CharacterDatabase.LoadMessageQuery(client);
			
			Data.NobilityBoard.SetNobility(client);
			client.SendNobility();
			
			client.SendSubClasses2();
			
			Packets.UpdatePacket update = Packets.UpdatePacket.Create(client.EntityUID);
			update.AddUpdate(Enums.UpdateType.VIPLevel, 6);
			update.AddUpdate(Enums.UpdateType.Merchant, 255);
			client.Send(update);
			
			Data.ArenaQualifier.SetArenaInfo(client);
			if (client.Arena == null)
			{
				client.Arena = new ProjectX_V3_Game.Data.ArenaInfo(client);
				client.Arena.ArenaHonorPoints = 1000;
				client.Arena.Level = (uint)client.Level;
				client.Arena.Class = (uint)client.Class;
				Database.ArenaDatabase.AddNewArenaInfo(client.Arena);
				if (!Data.ArenaQualifier.AddArenaInfo(client.Arena))
				{
					client.NetworkClient.Disconnect("FAILED_TO_MAKE_ARENA_INFO");
					return;
				}
			}
			
			client.CanSave = true;
			using (var motd = Packets.Message.MessageCore.CreateTalk(
				Core.SystemVariables.Variables["ServerName"], client.Name,
				Core.SystemVariables.ReplaceVariables(Program.Config.ReadString("MOTD"))))
				client.Send(motd);			
		}
	}
}
