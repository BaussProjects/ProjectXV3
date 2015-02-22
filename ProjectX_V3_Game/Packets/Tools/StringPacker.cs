//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Generic;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Packing strings.
	/// </summary>
	public class StringPacker
	{
		/// <summary>
		/// The packet holder.
		/// </summary>
		private DataPacket stringPacket;
		
		/// <summary>
		/// Gets the size of the buffer.
		/// </summary>
		public int Size
		{
			get
			{
				if (stringPacket == null)
					return 1;
				
				return stringPacket.BufferLength;
			}
		}
		
		/// <summary>
		/// Creates a new instance of StringPacker.
		/// </summary>
		/// <param name="strings">The strings to add.</param>
		public StringPacker(params string[] strings)
		{
			int totallength = strings.Length + 1;
			foreach (string str in strings)
				totallength += str.Length;
			stringPacket = new DataPacket(new byte[totallength]);
			stringPacket.WriteByte((byte)strings.Length, 0);
			int nextoffset = 1;
			foreach (string str in strings)
			{
				stringPacket.WriteStringWithLength(str, nextoffset, out nextoffset);
			}
		}
		
		/// <summary>
		/// Creates a new instance of StringPacker.
		/// </summary>
		public StringPacker()
		{
		}
		
		/// <summary>
		/// Adds a string to the packet.
		/// </summary>
		/// <param name="str">The string to add.</param>
		public void AddString(string str)
		{
			DataPacket expanded = new DataPacket(new byte[Size + str.Length + 1]); // creates a new packet
			
			expanded.WriteByte((byte)(stringPacket.ReadByte(0) + 1), 0); // sets a new string packet size
			expanded.WriteBytes(stringPacket.Copy(), 0); // copies all the current strings
			if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
				expanded.WriteByte(0, Size);
			else
				expanded.WriteStringWithLength(str, Size); // writes the new string at the end
			
			stringPacket.Dispose(); // disposes the old packet
			stringPacket = expanded; // sets the old packet to be the new
		}
		
		/// <summary>
		/// Appends the string packer to another packet.
		/// </summary>
		/// <param name="Packet">The packet.</param>
		/// <param name="offset">Append offset.</param>
		public void AppendAndFinish(DataPacket Packet, int offset)
		{
			if (stringPacket == null)
				return;
			
			Packet.WriteBytes(stringPacket.Copy(), offset);
			stringPacket.Dispose();
		}
		
		/// <summary>
		/// Analyzes a stringpacker within a packet.
		/// </summary>
		/// <param name="Packet">The packet.</param>
		/// <param name="offset">The offset.</param>
		/// <returns>Returns the strings.</returns>
		public static string[] Analyze(DataPacket Packet, int offset)
		{
			int count = Packet.ReadByte(offset);
			offset++;
			string[] strings = new string[count];
			
			for (int i = 0; i < count; i++)
			{
				strings[i] = Packet.ReadStringFromLength(offset, out offset);
			}
			
			return strings;
		}
	}
}
