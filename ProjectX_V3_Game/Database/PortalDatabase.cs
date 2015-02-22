//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Sql;

namespace ProjectX_V3_Game.Database
{
	/// <summary>
	/// Description of PortalDatabase.
	/// </summary>
	public class PortalDatabase
	{
		public static bool LoadPortals()
		{
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.SELECT, false))
				{
					cmd.Finish("DB_Portals");
				}
				while (sql.Read())
				{
					if (!Core.Kernel.Portals.TryAdd(
						new Core.PortalPoint(sql.ReadUInt16("StartMap"), sql.ReadUInt16("StartX"), sql.ReadUInt16("StartY")),
						new Core.PortalPoint(sql.ReadUInt16("EndMap"), sql.ReadUInt16("EndX"), sql.ReadUInt16("EndY"))))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
