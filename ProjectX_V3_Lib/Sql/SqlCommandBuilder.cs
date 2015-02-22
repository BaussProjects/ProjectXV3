//Project by BaussHacker aka. L33TS

using System;
using System.Text;
using System.Collections.Concurrent;

namespace ProjectX_V3_Lib.Sql
{
	/// <summary>
	/// Description of SqlCommandBuilder.
	/// </summary>
	public class SqlCommandBuilder : IDisposable
	{
		private string CommandString;
		private bool UseWhere;
		
		private ConcurrentDictionary<string, object> InsertValues;
		private ConcurrentDictionary<string, object> UpdateValues;
		private ConcurrentDictionary<string, object> WhereValues;
		
		private SqlHandler Handler;
		
		private SqlCommandType CommandType;
		
		public SqlCommandBuilder(SqlHandler sqlHandler, SqlCommandType commandType, bool UseWhere)
		{
			if (commandType == SqlCommandType.WHERE_HANDLE)
				throw new Exception("This command type is only allowed by the core handler.");
			
			this.CommandType = commandType;
			InsertValues = new ConcurrentDictionary<string, object>();
			UpdateValues = new ConcurrentDictionary<string, object>();
			WhereValues = new ConcurrentDictionary<string, object>();
			
			this.Handler = sqlHandler;
			this.UseWhere = UseWhere;
			
			int WhereFormatCount = 1;
			
			switch (commandType)
			{
				case SqlCommandType.SELECT:
					CommandString = "SELECT * FROM {0}";
					goto case SqlCommandType.WHERE_HANDLE;
					
				case SqlCommandType.INSERT:
					CommandString = "INSERT INTO {0} ({1}) VALUES ({2})";
					break;
					
				case SqlCommandType.UPDATE:
					CommandString = "UPDATE {0} SET {1}";
					WhereFormatCount = 2;
					goto case SqlCommandType.WHERE_HANDLE;
					
				case SqlCommandType.DELETE:
					CommandString = "DELETE {0}";
					goto case SqlCommandType.WHERE_HANDLE;
					
				case SqlCommandType.WHERE_HANDLE:
					if (this.UseWhere)
						CommandString = string.Format("{0} WHERE {{{1}}}", CommandString, WhereFormatCount);
					break;
			}
		}
		
		public bool AddInsertValue(string name, object value)
		{
			return InsertValues.TryAdd(name, value);
		}
		public bool AddUpdateValue(string name, object value)
		{
			return UpdateValues.TryAdd(name, value);
		}
		public bool AddWhereValue(string name, object value)
		{
			return WhereValues.TryAdd(name, value);
		}
		
		public void Finish(string Table)
		{
			switch (this.CommandType)
			{
				case SqlCommandType.INSERT:
					{
						StringBuilder BuilderValuesA = new StringBuilder();
						StringBuilder BuilderValuesB = new StringBuilder();
						foreach (string Name in InsertValues.Keys)
						{
							BuilderValuesA.Append(Name).Append(", ");
							BuilderValuesB.Append("@").Append(Name).Append(", ");
							object Value;
							if (!InsertValues.TryGetValue(Name, out Value))
								throw new Exception("[INSERT] Failed to get value.");
							Handler.AddParameter("@" + Name, Value);
						}
						BuilderValuesA.Length -= 2;
						BuilderValuesB.Length -= 2;
						
						Handler.SetCommand(string.Format(CommandString, Table, BuilderValuesA.ToString(), BuilderValuesB.ToString()));
						BuilderValuesA.Clear();
						BuilderValuesB.Clear();
						break;
					}
					
				case SqlCommandType.UPDATE:
					{
						StringBuilder BuilderValues = new StringBuilder();
						foreach (string Name in UpdateValues.Keys)
						{
							BuilderValues.Append(Name).Append(" = @").Append(Name).Append(", ");
							object Value;
							if (!UpdateValues.TryGetValue(Name, out Value))
								throw new Exception("[UPDATE] Failed to get value.");
							Handler.AddParameter("@" + Name, Value);
						}
						BuilderValues.Length -= 2;
						
						if (UseWhere)
						{
							StringBuilder BuilderWhereValues = new StringBuilder();
							foreach (string Name in WhereValues.Keys)
							{
								BuilderWhereValues.Append(Name).Append(" = @").Append(Name).Append(" AND ");
								object Value;
								if (!WhereValues.TryGetValue(Name, out Value))
									throw new Exception("[UPDATE] Failed to get value.");
								Handler.AddParameter("@" + Name, Value);
							}
							BuilderWhereValues.Length -= 5;
							
							Handler.SetCommand(string.Format(CommandString, Table, BuilderValues.ToString(), BuilderWhereValues.ToString()));
							
							BuilderValues.Clear();
							BuilderWhereValues.Clear();
						}
						else
							Handler.SetCommand(string.Format(CommandString, Table, BuilderValues.ToString()));
						break;
					}
				case SqlCommandType.SELECT:
					{
						if (UseWhere)
						{
							StringBuilder BuilderWhereValues = new StringBuilder();
							foreach (string Name in WhereValues.Keys)
							{
								BuilderWhereValues.Append(Name).Append(" = @").Append(Name).Append(" AND ");
								object Value;
								if (!WhereValues.TryGetValue(Name, out Value))
									throw new Exception("[UPDATE] Failed to get value.");
								Handler.AddParameter("@" + Name, Value);
							}
							BuilderWhereValues.Length -= 5;
							
							Handler.SetCommand(string.Format(CommandString, Table, BuilderWhereValues.ToString()));
							BuilderWhereValues.Clear();
						}
						else
							Handler.SetCommand(string.Format(CommandString, Table));
						break;
					}
					
				case SqlCommandType.DELETE:
					{
						if (UseWhere)
						{
							StringBuilder BuilderWhereValues = new StringBuilder();
							foreach (string Name in WhereValues.Keys)
							{
								BuilderWhereValues.Append(Name).Append(" = @").Append(Name).Append(" AND ");
								object Value;
								if (!WhereValues.TryGetValue(Name, out Value))
									throw new Exception("[UPDATE] Failed to get value.");
								Handler.AddParameter("@" + Name, Value);
							}
							BuilderWhereValues.Length -= 5;
							
							Handler.SetCommand(string.Format(CommandString, Table, BuilderWhereValues.ToString()));
							BuilderWhereValues.Clear();
						}
						else
							Handler.SetCommand(string.Format(CommandString, Table));
						break;
					}
			}
		}
		
		/// <summary>
		/// Disposing the commandbuilder.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		/// <summary>
		/// Disposing the commandbuilder.
		/// </summary>
		/// <param name="disposing">Set to true if disposing.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				InsertValues.Clear();
				UpdateValues.Clear();
				WhereValues.Clear();
				CommandString = null;
			}
			// unmanaged clear here...
		}
	}
}
