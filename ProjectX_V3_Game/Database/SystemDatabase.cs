//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Sql;

namespace ProjectX_V3_Game.Database
{
	/// <summary>
	/// Description of SystemDatabase.
	/// </summary>
	public class SystemDatabase
	{
		public static bool LoadSystemVariables()
		{
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.SELECT, false))
				{
					cmd.Finish("DB_SystemVariables");
				}
				while (sql.Read())
				{
					if (!Core.SystemVariables.Variables.TryAdd(sql.ReadString("SystemVariableName"), sql.ReadString("SystemVariableValue")))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
