//Project by BaussHacker aka. L33TS

using System;
using System.Data.SqlClient;

namespace ProjectX_V3_Lib.Sql
{
	/// <summary>
	/// Handler for sql.
	/// </summary>
	public class SqlHandler : IDisposable
	{
		/// <summary>
		/// The sql connection.
		/// </summary>
		private SqlConnection Connection;
		/// <summary>
		/// The sql command.
		/// </summary>
		private SqlCommand Command;
		/// <summary>
		/// The sql data reader.
		/// </summary>
		private SqlDataReader Reader;

		/// <summary>
		/// Creates a new instance of SqlHandler.
		/// </summary>
		/// <param name="connectionString">The connectionstring.</param>
		/// <param name="commandstring">The commandstring.</param>
		public SqlHandler(string connectionstring, string commandstring)
		{
			Connection = new SqlConnection(connectionstring);
			Connection.Open();
			
			Command = new SqlCommand(commandstring, Connection);
		}
		
		/// <summary>
		/// Creates a new instance of SqlHandler.
		/// </summary>
		/// <param name="connectionString">The connectionstring.</param>
		public SqlHandler(string connectionString)
		{
			Connection = new SqlConnection(connectionString);
			Connection.Open();
			
			Command = new SqlCommand("", Connection);
		}
		
		/// <summary>
		/// Adds a parameter to the handler.
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="value">The value.</param>
		public void AddParameter(string name, object value)
		{
			object realval = value;
			if (value.GetType() == typeof(byte))
				realval = (short)(byte)value;
			else if (value.GetType() == typeof(ushort))
				realval = (int)(ushort)value;
			else if (value.GetType() == typeof(uint))
				realval = (int)(uint)value;
			else if (value.GetType() == typeof(ulong))
				realval = (long)(ulong)value;
			
			Command.Parameters.AddWithValue(name, realval);
		}
		
		/// <summary>
		/// Will create the datareader, if it's not created.
		/// Will read data from the current row.
		/// </summary>
		/// <returns>Returns true if there is any rows.</returns>
		public bool Read()
		{
			if (Reader == null)
				Reader = Command.ExecuteReader();
			return Reader.Read();
		}
		
		/// <summary>
		/// Reads a value from the current row.
		/// </summary>
		/// <param name="name">The column name.</param>
		/// <returns>Returns the value.</returns>
		public sbyte ReadSByte(string name)
		{
			return (sbyte)Reader[name];
		}
		
		/// <summary>
		/// Reads a value from the current row.
		/// </summary>
		/// <param name="name">The column name.</param>
		/// <returns>Returns the value.</returns>
		public short ReadInt16(string name)
		{
			return (short)Reader[name];
		}
		
		/// <summary>
		/// Reads a value from the current row.
		/// </summary>
		/// <param name="name">The column name.</param>
		/// <returns>Returns the value.</returns>
		public int ReadInt32(string name)
		{
			return (int)Reader[name];
		}
		
		/// <summary>
		/// Reads a value from the current row.
		/// </summary>
		/// <param name="name">The column name.</param>
		/// <returns>Returns the value.</returns>
		public long ReadInt64(string name)
		{
			return (long)Reader[name];
		}
		
		/// <summary>
		/// Reads a value from the current row.
		/// </summary>
		/// <param name="name">The column name.</param>
		/// <returns>Returns the value.</returns>
		public byte ReadByte(string name)
		{
			return (byte)(short)Reader[name];
		}
		
		/// <summary>
		/// Reads a value from the current row.
		/// </summary>
		/// <param name="name">The column name.</param>
		/// <returns>Returns the value.</returns>
		public ushort ReadUInt16(string name)
		{
			return (ushort)(int)Reader[name];
		}
		
		/// <summary>
		/// Reads a value from the current row.
		/// </summary>
		/// <param name="name">The column name.</param>
		/// <returns>Returns the value.</returns>
		public uint ReadUInt32(string name)
		{
			return (uint)(int)Reader[name];
		}
		
		/// <summary>
		/// Reads a value from the current row.
		/// </summary>
		/// <param name="name">The column name.</param>
		/// <returns>Returns the value.</returns>
		public ulong ReadUInt64(string name)
		{
			return (ulong)(long)Reader[name];
		}
		
		/// <summary>
		/// Reads a value from the current row.
		/// </summary>
		/// <param name="name">The column name.</param>
		/// <returns>Returns the value.</returns>
		public string ReadString(string name)
		{
			return Reader[name].ToString();
		}
		
		/// <summary>
		/// Reads a value from the current row.
		/// </summary>
		/// <param name="name">The column name.</param>
		/// <returns>Returns the value.</returns>
		public DateTime ReadDateTime(string name)
		{
			return (DateTime)Reader[name];
		}
		
		/// <summary>
		/// Reads a value from the current row.
		/// </summary>
		/// <param name="name">The column name.</param>
		/// <returns>Returns the value.</returns>
		public bool ReadBoolean(string name)
		{
			return (bool)Reader[name];
		}
		
		/// <summary>
		/// Reads a value from the current row.
		/// </summary>
		/// <param name="name">The column name.</param>
		/// <returns>Returns the value.</returns>
		public object ReadObject(string name)
		{
			return Reader[name];
		}
		
		/// <summary>
		/// Executes a nonquery command.
		/// </summary>
		/// <returns>Returns the amount of rows affected.</returns>
		public int Execute()
		{
			return Command.ExecuteNonQuery();
		}
		
		/// <summary>
		/// Will close the current connection and forward it to a new connection.
		/// </summary>
		/// <param name="connectionString">The connectionstring.</param>
		/// <param name="commandstring">The commandstring.</param>
		public void Forward(string connectionstring, string commandstring)
		{
			Dispose(true);
			
			Connection = new SqlConnection(connectionstring);
			Connection.Open();
			
			Command = new SqlCommand(commandstring, Connection);
		}
		
		/// <summary>
		/// Sets a new commandstring.
		/// </summary>
		/// <param name="commandstring">The commandstring.</param>
		public void SetCommand(string commandstring)
		{
			Command.CommandText = commandstring;
		}
		
		/// <summary>
		/// Disposing the handler.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		/// <summary>
		/// Disposing the handler.
		/// </summary>
		/// <param name="disposing">Set to true if disposing.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (Reader != null)
					Reader.Dispose();
				if (Command != null)
					Command.Dispose();
				if (Connection != null)
					Connection.Close();
			}
			// unmanaged clear here...
		}
	}
}
