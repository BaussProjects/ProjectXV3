//Project by BaussHacker aka. L33TS

using System;
using System.Text;
using ProjectX_V3_Lib.Extensions;

namespace ProjectX_V3_Lib.Network
{
	/// <summary>
	/// A data packet.
	/// </summary>
	public unsafe class DataPacket : IDisposable
	{
		/// <summary>
		/// Creates a new data packet.
		/// </summary>
		/// <param name="inPacket">The incoming data packet.</param>
		public DataPacket(DataPacket inPacket)
		{
			_buffer = new byte[inPacket._buffer.Length];
			inPacket._buffer.memcpy(_buffer);
		}
		
		/// <summary>
		/// Creates a new data packet.
		/// </summary>
		/// <param name="inBuffer">The incoming buffer.</param>
		public DataPacket(byte[] inBuffer)
		{
			_buffer = new byte[inBuffer.Length];
			inBuffer.memcpy(_buffer);
			//Array.Clear(inBuffer, 0, inBuffer.Length);
			//inBuffer = null;
		}
		
		/// <summary>
		/// Creates a new data packet.
		/// </summary>
		/// <param name="packetSize">The size.</param>
		/// <param name="packetID">The ID.</param>
		public DataPacket(ushort packetSize, ushort packetID)
		{
			_buffer = new byte[packetSize];
			WriteUInt16(packetSize, 0);
			WriteUInt16(packetID, 2);
		}
		
		/// <summary>
		/// Destr. for DataPacket.
		/// </summary>
		~DataPacket()
		{
			//Dispose(false);
		}
		
		/// <summary>
		/// The buffer associated with the packet.
		/// </summary>
		private byte[] _buffer;
		
		/// <summary>
		/// Gets the buffer associated with the packet.
		/// </summary>
		protected byte[] DataBuffer
		{
			get
			{
				return _buffer;
			}
		}
		
		/// <summary>
		/// Gets the pointer associated with the packet.
		/// </summary>
		public byte* dataPointer
		{
			get
			{
				fixed (byte* pointer = _buffer)
					return pointer;
			}
		}
		
		/// <summary>
		/// Gets the written size of the packet.
		/// </summary>
		public ushort PacketSize
		{
			get { return ReadUInt16(0); }
		}
		
		/// <summary>
		/// Gets the ID of the packet.
		/// </summary>
		public ushort PacketID
		{
			get { return ReadUInt16(2); }
		}
		
		/// <summary>
		/// Gets the buffer size.
		/// </summary>
		public int BufferLength
		{
			get { return _buffer.Length; }
		}
		
		/// <summary>
		/// Checks whether the actual size matches the written size.
		/// </summary>
		/// <returns>True if the size matches.</returns>
		public bool MatchingSize()
		{
			return (PacketSize == _buffer.Length);
		}
		
		/// <summary>
		/// Writes a signed 8bit value.
		/// </summary>
		/// <param name="value">The value-</param>
		/// <param name="offset">The offset.</param>
		public void WriteSByte(sbyte value, int offset)
		{
			(*(sbyte*)(dataPointer + offset)) = value;
		}
		/// <summary>
		/// Writes a signed 16bit value.
		/// </summary>
		/// <param name="value">The value-</param>
		/// <param name="offset">The offset.</param>
		public void WriteInt16(short value, int offset)
		{
			(*(short*)(dataPointer + offset)) = value;
		}
		/// <summary>
		/// Writes a signed 32bit value.
		/// </summary>
		/// <param name="value">The value-</param>
		/// <param name="offset">The offset.</param>
		public void WriteInt32(int value, int offset)
		{
			(*(int*)(dataPointer + offset)) = value;
		}
		/// <summary>
		/// Writes a signed 64bit value.
		/// </summary>
		/// <param name="value">The value-</param>
		/// <param name="offset">The offset.</param>
		public void WriteInt64(long value, int offset)
		{
			(*(long*)(dataPointer + offset)) = value;
		}
		/// <summary>
		/// Writes an unsigned 8bit value.
		/// </summary>
		/// <param name="value">The value-</param>
		/// <param name="offset">The offset.</param>
		public void WriteByte(byte value, int offset)
		{
			(*(byte*)(dataPointer + offset)) = value;
		}
		/// <summary>
		/// Writes an unsigned 16bit value.
		/// </summary>
		/// <param name="value">The value-</param>
		/// <param name="offset">The offset.</param>
		public void WriteUInt16(ushort value, int offset)
		{
			(*(ushort*)(dataPointer + offset)) = value;
		}
		/// <summary>
		/// Writes an unsigned 32bit value.
		/// </summary>
		/// <param name="value">The value-</param>
		/// <param name="offset">The offset.</param>
		public void WriteUInt32(uint value, int offset)
		{
			(*(uint*)(dataPointer + offset)) = value;
		}
		/// <summary>
		/// Writes an unsigned 64bit value.
		/// </summary>
		/// <param name="value">The value-</param>
		/// <param name="offset">The offset.</param>
		public void WriteUInt64(ulong value, int offset)
		{
			(*(ulong*)(dataPointer + offset)) = value;
		}
		/// <summary>
		/// Reads a signed 8bit value.
		/// </summary>
		/// <param name="offset">The offset.</param>
		/// <returns>Returns the value.</returns>
		public sbyte ReadSByte(int offset)
		{
			return (*(sbyte*)(dataPointer + offset));
		}
		/// <summary>
		/// Reads a signed 16bit value.
		/// </summary>
		/// <param name="offset">The offset.</param>
		/// <returns>Returns the value.</returns>
		public short ReadInt16(int offset)
		{
			return (*(short*)(dataPointer + offset));
		}
		/// <summary>
		/// Reads a signed 32bit value.
		/// </summary>
		/// <param name="offset">The offset.</param>
		/// <returns>Returns the value.</returns>
		public int ReadInt32(int offset)
		{
			return (*(int*)(dataPointer + offset));
		}
		/// <summary>
		/// Reads a signed 64bit value.
		/// </summary>
		/// <param name="offset">The offset.</param>
		/// <returns>Returns the value.</returns>
		public long ReadInt64(int offset)
		{
			return (*(long*)(dataPointer + offset));
		}
		/// <summary>
		/// Reads an unsigned 8bit value.
		/// </summary>
		/// <param name="offset">The offset.</param>
		/// <returns>Returns the value.</returns>
		public byte ReadByte(int offset)
		{
			return (*(byte*)(dataPointer + offset));
		}
		
		/// <summary>
		/// Reads an array of bytes.
		/// </summary>
		/// <param name="offset">The offset.</param>
		/// <param name="len">The length.</param>
		/// <returns>Returns the array of bytes.</returns>
		public byte[] ReadBytes(int offset, int len)
		{
			byte[] ret = new byte[len];
			for (int i = 0; i < len; i++)
				ret[i] = ReadByte(offset + i);
			return ret;
		}
		
		public void WriteBytes(byte[] bytes, int offset)
		{
			for (int i = 0; i < bytes.Length; i++)
				WriteByte(bytes[i], (offset + i));
		}
		
		/// <summary>
		/// Reads an unsigned 16bit value.
		/// </summary>
		/// <param name="offset">The offset.</param>
		/// <returns>Returns the value.</returns>
		public ushort ReadUInt16(int offset)
		{
			return (*(ushort*)(dataPointer + offset));
		}
		/// <summary>
		/// Reads an unsigned 32bit value.
		/// </summary>
		/// <param name="offset">The offset.</param>
		/// <returns>Returns the value.</returns>
		public uint ReadUInt32(int offset)
		{
			return (*(uint*)(dataPointer + offset));
		}
		/// <summary>
		/// Reads an unsigned 64bit value.
		/// </summary>
		/// <param name="offset">The offset.</param>
		/// <returns>Returns the value.</returns>
		public ulong ReadUInt64(int offset)
		{
			return (*(ulong*)(dataPointer + offset));
		}
		
		/// <summary>
		/// Writes a string value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="offset">The offset.</param>
		public void WriteString(string value, int offset)
		{
			foreach (byte b in value.GetBytes())
				WriteByte((byte)b, offset++);
		}
		
		/// <summary>
		/// Writes a string with length.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="offset">The offset.</param>
		public void WriteStringWithLength(string value, int offset)
		{
			WriteByte((byte)value.Length, offset);
			offset++;
			WriteString(value, offset);
		}
		
				/// <summary>
		/// Writes a string with length.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="nextoffset">The next offset.</param>
		public void WriteStringWithLength(string value, int offset, out int nextoffset)
		{
			WriteByte((byte)value.Length, offset);
			offset++;
			WriteString(value, offset);
			nextoffset = (offset + value.Length);
		}
		
		/// <summary>
		/// Reads a string value from length.
		/// </summary>
		/// <param name="offset">The offset.</param>
		/// <param name="length">The length.</param>
		/// <returns>Returns the value.</returns>
		public string ReadString(int offset, int length)
		{
			StringBuilder stringBuilder = new StringBuilder();

			for (int i = 0; i < length; i++)
			{
				byte _byte = ReadByte(offset + i);
				if (_byte > 0)
					stringBuilder.Append((char)_byte);
			}

			return stringBuilder.ToString();
		}
		/// <summary>
		/// Reads a string value.
		/// </summary>
		/// <param name="offset">The offset.</param>
		/// <returns>Returns the value.</returns>
		public string ReadStringFromLength(int offset)
		{
			byte Length = ReadByte(offset);
			offset++;
			return ReadString(offset, Length);
		}
		
				/// <summary>
		/// Reads a string value.
		/// </summary>
		/// <param name="offset">The offset.</param>
		/// <param name="nextoffset">The next offset.</param>
		/// <returns>Returns the value.</returns>
		public string ReadStringFromLength(int offset, out int nextoffset)
		{
			byte Length = ReadByte(offset);
			offset++;
			nextoffset = (Length + offset);
			return ReadString(offset, Length);
		}
	
		/// <summary>
		/// Writes a boolean value.
		/// </summary>
		/// <param name="value">The value-</param>
		/// <param name="offset">The offset.</param>
		public void WriteBool(bool value, int offset)
		{
			WriteByte((byte)(value ? 1 : 0), offset);
		}
		
		/// <summary>
		/// Reads a boolean value.
		/// </summary>
		/// <param name="offset">The offset.</param>
		/// <returns>Returns the value.</returns>
		public bool ReadBool(int offset)
		{
			return (ReadByte(offset) > 0);
		}
		
		/// <summary>
		/// Disposing the packet.
		/// </summary>
		public void Dispose()
		{
			//Dispose(true);
			//GC.SuppressFinalize(this);
		}
		
		/// <summary>
		/// Disposing the packet.
		/// </summary>
		/// <param name="disposing">Set to true if disposing.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				//Array.Clear(_buffer, 0, _buffer.Length);
				//_buffer = null;
			}
			// unmanaged clear here...
		}
		
		/// <summary>
		/// Copies the buffer to a new buffer.
		/// </summary>
		/// <returns>Returns the new buffer.</returns>
		public byte[] Copy()
		{
			byte[] bCopy = new byte[_buffer.Length];
			_buffer.memcpy(bCopy);
			return bCopy;
		}
		
		public void Encrypt(Cryptography.ConquerCrypto Crypto)
		{
			Crypto.Encrypt(_buffer);
		}
		
		public void Decrypt(Cryptography.ConquerCrypto Crypto)
		{
			Crypto.Decrypt(_buffer);
		}
		
		public string GetDump()
		{
			StringBuilder hexBuilder = new StringBuilder();
			hexBuilder.AppendFormat("Type: {0}", PacketID).Append(Environment.NewLine);
			hexBuilder.AppendFormat("Virtual Size: {0}", PacketSize).Append(Environment.NewLine);
			hexBuilder.AppendFormat("Buffer Size: {0}", BufferLength).Append(Environment.NewLine);
			foreach (byte b in _buffer)
				hexBuilder.Append(b.ToString("X2").ToUpper()).Append(" ");
			hexBuilder.Length -= 1;
			hexBuilder.Append(Environment.NewLine);
			hexBuilder.Append(_buffer.GetString());
			return hexBuilder.ToString();
		}
	}
}
