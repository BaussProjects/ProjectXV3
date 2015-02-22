//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Sql;

namespace ProjectX_V3_Game.Database
{
	/// <summary>
	/// Description of ArenaDatabase.
	/// </summary>
	public class ArenaDatabase
	{
		public static bool LoadArenaQualifiers()
		{
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.SELECT, false))
				{
					cmd.Finish("DB_ArenaQualifier");
				}
				while (sql.Read())
				{
					Data.ArenaInfo arena = new ProjectX_V3_Game.Data.ArenaInfo();
					arena.ArenaID = sql.ReadInt32("ArenaID");
					arena.DatabaseID = sql.ReadInt32("PlayerID");
					arena.Level = sql.ReadUInt32("ArenaLevel");
					arena.Class = sql.ReadUInt32("ArenaClass");
					arena.Name = sql.ReadString("PlayerName");
					arena.Mesh = sql.ReadUInt32("Mesh");
					arena.ArenaTotalWins = sql.ReadUInt32("TotalWins");
					arena.ArenaWinsToday = sql.ReadUInt32("TotalWinsToday");
					arena.ArenaTotalLoss = sql.ReadUInt32("TotalLoss");
					arena.ArenaLossToday = sql.ReadUInt32("TotalLossToday");										
					arena.ArenaPoints = sql.ReadUInt32("ArenaPoints");
					arena.ArenaHonorPoints = sql.ReadUInt32("HonorPoints");	
					
					DateTime dailyupdate = sql.ReadDateTime("TodayUpdate");
					
					if (DateTime.Now >= dailyupdate.AddHours(24))
					{
						UpdateArenaInfo(arena.DatabaseID, "TodayUpdate", DateTime.Now);
						arena.ArenaLossToday = 0;
						arena.ArenaWinsToday = 0;
						arena.Save();
					}
					if (!Data.ArenaQualifier.AddArenaInfo(arena))
					{
						return false;
					}
				}
			}
			Data.ArenaQualifier.GetTop10();
			Data.ArenaQualifier.GetTop10();
			return true;
		}
		
		public static void UpdateArenaInfo(int Identifier, string Column, object Value)
		{
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.UPDATE, true))
				{
					cmd.AddWhereValue("PlayerID", Identifier);
					cmd.AddUpdateValue(Column, Value);
					cmd.Finish("DB_ArenaQualifier");
				}
				sql.Execute();
			}
		}
		
		public static void AddNewArenaInfo(Data.ArenaInfo arena)
		{
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.INSERT, false))
				{
					cmd.AddInsertValue("PlayerID", arena.DatabaseID);
					cmd.AddInsertValue("PlayerName", arena.Name);
					cmd.AddInsertValue("ArenaLevel", arena.Level);
					cmd.AddInsertValue("ArenaClass", arena.Class);
					cmd.AddInsertValue("Mesh", arena.Mesh);
					cmd.AddInsertValue("HonorPoints", arena.DatabaseID);
					cmd.AddInsertValue("ArenaPoints", arena.ArenaPoints);
					cmd.AddInsertValue("TotalWins", arena.ArenaTotalWins);
					cmd.AddInsertValue("TotalWinsToday", arena.ArenaWinsToday);
					cmd.AddInsertValue("TotalLoss", arena.ArenaTotalLoss);
					cmd.AddInsertValue("TotalLossToday", arena.ArenaLossToday);
					cmd.AddInsertValue("TodayUpdate", DateTime.Now);
					cmd.Finish("DB_ArenaQualifier");
				}
				sql.Execute();
			}
			
			SetRecordID(arena);
		}
		
		public static void SetRecordID(Data.ArenaInfo arena)
		{
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.SELECT, true))
				{
					cmd.AddWhereValue("PlayerID", arena.DatabaseID);
					cmd.Finish("DB_ArenaQualifier");
				}
				if (sql.Read())
				{
					arena.ArenaID = sql.ReadInt32("ArenaID");
				}
			}
		}
		
		public static void SaveArenaInfo(Data.ArenaInfo arena)
		{
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.UPDATE, true))
				{
					cmd.AddWhereValue("PlayerID", arena.DatabaseID);
					cmd.AddUpdateValue("PlayerName", arena.Name);
					cmd.AddUpdateValue("ArenaLevel", arena.Level);
					cmd.AddUpdateValue("ArenaClass", arena.Class);
					cmd.AddUpdateValue("Mesh", arena.Mesh);
					cmd.AddUpdateValue("HonorPoints", arena.DatabaseID);
					cmd.AddUpdateValue("ArenaPoints", arena.ArenaPoints);
					cmd.AddUpdateValue("TotalWins", arena.ArenaTotalWins);
					cmd.AddUpdateValue("TotalWinsToday", arena.ArenaWinsToday);
					cmd.AddUpdateValue("TotalLoss", arena.ArenaTotalLoss);
					cmd.AddUpdateValue("TotalLossToday", arena.ArenaLossToday);
					cmd.Finish("DB_ArenaQualifier");
				}
				sql.Execute();
			}
		}
	}
}
