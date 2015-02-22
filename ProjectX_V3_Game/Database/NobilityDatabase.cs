//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Sql;

namespace ProjectX_V3_Game.Database
{
	/// <summary>
	/// Description of NobilityDatabase.
	/// </summary>
	public class NobilityDatabase
	{
		public static bool LoadNobilityBoard()
		{
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.SELECT, false))
				{
					cmd.Finish("DB_NobilityBoard");
				}
				while (sql.Read())
				{
					Data.NobilityDonation donation = new ProjectX_V3_Game.Data.NobilityDonation();
					donation.DatabaseID = sql.ReadInt32("PlayerID");
					donation.Name = sql.ReadString("PlayerName");
					donation.Donation = sql.ReadInt64("NobilityDonation");
					donation.Rank = Enums.NobilityRank.Serf;
					donation.OldRank = donation.Rank;
					donation.RecordID = sql.ReadInt32("NobilityID");
					if (!Data.NobilityBoard.AddNobility(donation))
					{
						return false;
					}
				}
			}
			Data.NobilityBoard.GetTop50();
			return true;
		}
		
		public static void SetRecordID(Entities.GameClient client)
		{
			if (client.Nobility == null)
				return;
			
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.SELECT, true))
				{
					cmd.AddWhereValue("PlayerID", client.DatabaseUID);
					cmd.Finish("DB_NobilityBoard");
				}
				if (sql.Read())
				{
					client.Nobility.RecordID = sql.ReadInt32("NobilityID");
				}
			}
		}
		
		public static void UpdateNobilityDonation(int Identifier, string Column, object Value)
		{
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.UPDATE, true))
				{
					cmd.AddWhereValue("PlayerID", Identifier);
					cmd.AddUpdateValue(Column, Value);
					cmd.Finish("DB_NobilityBoard");
				}
				sql.Execute();
			}
		}
		
		public static void AddNewNobility(int Identifier, string Name, long Donation)
		{
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.INSERT, false))
				{
					cmd.AddInsertValue("PlayerID", Identifier);
					cmd.AddInsertValue("PlayerName", Name);
					cmd.AddInsertValue("NobilityDonation", Donation);
					cmd.Finish("DB_NobilityBoard");
				}
				sql.Execute();
			}
		}
	}
}
