//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Sql;

namespace ProjectX_V3_Game.Database
{
	/// <summary>
	/// Description of BotDatabase.
	/// </summary>
	public class BotDatabase
	{
		public static void LoadBots()
		{
			#if SPAWN_BOTS
			
			#region AfkBots
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.SELECT, false))
				{
					cmd.Finish("DB_BotInfo");
				}
				while (sql.Read())
				{				
					Entities.AIBot bot = new ProjectX_V3_Game.Entities.AIBot();
					bot.LoadBot(Enums.BotType.AFKBot,
					            sql.ReadInt32("BotRefID"),
					            new Maps.MapPoint(
					            	sql.ReadUInt16("BotMapID"),
					            	sql.ReadUInt16("BotX"),
					            	sql.ReadUInt16("BotY")),
					            null);
				}
			}
			#endregion
			
			#endif
		}
	}
}
