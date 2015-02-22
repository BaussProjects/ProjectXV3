//Project by BaussHacker aka. L33TS

using System;
using System.IO;
using ProjectX_V3_Lib.Sql;

namespace ProjectX_V3_Auth.Database
{
	/// <summary>
	/// The database handler for the auth server.
	/// </summary>
	public class ServerDatabase
	{
		/// <summary>
		/// The location of the database.
		/// </summary>
		public static readonly string DatabaseLocation;
		static ServerDatabase()
		{
			string dbloc = "\\CODB\\auth";

			foreach (DriveInfo drives in DriveInfo.GetDrives())
			{
				if (Directory.Exists(drives.Name + dbloc))
				{
					DatabaseLocation = drives.Name + dbloc;
					return;
				}
			}
			DatabaseLocation = Environment.CurrentDirectory + dbloc;
		}
		
		public static Enums.AccountStatus Authenticate(Client.AuthClient client, string Account, string Password)
		{
			try
			{
				using (var sql = new SqlHandler(Program.Config.ReadString("AuthConnectionString")))
				{
					using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.SELECT, true))
					{
						cmd.AddWhereValue("AccountName", Account);
						cmd.AddWhereValue("AccountPassword", Password);
						
						cmd.Finish("DB_Accounts");
					}
					
					if (!sql.Read())
						return Enums.AccountStatus.Invalid_AccountID_Or_Password;
					client.DatabaseUID = sql.ReadInt32("AccountID");
					client.Server = sql.ReadByte("AccountServer");
					client.Account = Account;
					client.Password = Password;
					using (var sql2 = new SqlHandler(Program.Config.ReadString("AuthConnectionString")))
					{
						using (var cmd2 = new SqlCommandBuilder(sql2, SqlCommandType.UPDATE, true))
						{
							cmd2.AddWhereValue("AccountID", client.DatabaseUID);
							if (sql.ReadBoolean("AccountBanned"))
							{
								DateTime banexpire = sql.ReadDateTime("AccountBanExpire");
								if (DateTime.Now < banexpire)
									return Enums.AccountStatus.Account_Banned;
								
								cmd2.AddUpdateValue("AccountBanned", false);
							}
							
							cmd2.AddUpdateValue("AccountLastLoginIP", client.NetworkClient.IP);
							cmd2.Finish("DB_Accounts");
						}
						sql2.Execute();
					}
				}
				return Enums.AccountStatus.Ready;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				return Enums.AccountStatus.Datebase_Error;
			}
		}
		
		private static bool PlayerExists(int DatabaseID)
		{
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.SELECT, true))
				{
					cmd.AddWhereValue("PlayerID", DatabaseID);
					cmd.Finish("DB_Players");
				}
				return sql.Read();
			}
		}
		public static void UpdateAuthentication(Client.AuthClient client)
		{
			if (PlayerExists(client.DatabaseUID))
			{
				using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
				{
					using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.UPDATE, true))
					{
						cmd.AddWhereValue("PlayerID", client.DatabaseUID);
						cmd.AddUpdateValue("PlayerLastEntityUID", client.EntityUID);
						cmd.AddUpdateValue("PlayerLoginOK", true);
						
						cmd.Finish("DB_Players");
					}
					sql.Execute();
				}
			}
			else
			{
				using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
				{
					using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.INSERT, false))
					{
						cmd.AddInsertValue("PlayerID", client.DatabaseUID);
						cmd.AddInsertValue("PlayerNew", true);
						cmd.AddInsertValue("PlayerAccount", client.Account);
						cmd.AddInsertValue("PlayerLastEntityUID", client.EntityUID);
						cmd.AddInsertValue("PlayerLoginOK", true);
						cmd.AddInsertValue("PlayerServer", client.Server);
						
						cmd.Finish("DB_Players");
					}
					sql.Execute();
				}
			}
		}
		
		/*/// <summary>
		/// Authenticates the client.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="Account">The account.</param>
		/// <param name="Password">The password.</param>
		/// <returns>Returns the status of the client.</returns>
		public static Enums.AccountStatus Authenticate(Client.AuthClient client, string Account, string Password)
		{
			Enums.AccountStatus status = Enums.AccountStatus.Invalid_AccountID_Or_Password;
			IniFile accountfile = new IniFile(DatabaseLocation + "\\accounts\\" + Account + ".ini", "Account");
			if (accountfile.Exists())
			{
				client.Account = Account;
				client.Password = accountfile.ReadSmallString("Password", string.Empty);
				client.DatabaseUID = accountfile.ReadInt32("DatabaseUID", 0);
				
				if (accountfile.ReadBoolean("Banned", false))
				{
					DateTime banend = DateTime.Parse(accountfile.ReadString("BanExpire", "01-01-2001"));
					if (DateTime.Now <= banend)
					{
						return Enums.AccountStatus.Account_Banned;
					}
					else
					{
						accountfile.WriteString("BanExpire", "0");
						accountfile.Write<bool>("Banned", false);
					}
				}
				
				accountfile.WriteString("LastLoginIP", client.NetworkClient.IP);
				
				if (client.Password == Password)
					status = Enums.AccountStatus.Ready;
			}
			return status;
		}*/
	}
}
