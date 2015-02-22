//Project by BaussHacker aka. L33TS

using System;
using System.Text;

namespace ProjectX_V3_Lib.Extensions
{
	/// <summary>
	/// Extensions for System.Byte.
	/// </summary>
	public static class ByteExtensions
	{
		/// <summary>
		/// Copies the data of the byte array into a new byte array.
		/// </summary>
		/// <param name="bytearray">The byte array.</param>
		/// <param name="dest">The destination array.</param>
		public static void memcpy(this byte[] bytearray, byte[] dest)
		{
			Native.Msvcrt.MemoryCopy(bytearray, dest);
		}
		
		/// <summary>
		/// Copies the data of the byte array into a new byte array.
		/// </summary>
		/// <param name="bytearray">The byte array.</param>
		/// <param name="dest">The destination array.</param>
		/// <param name="size">The size to copy.</param>
		public static void memcpy(this byte[] bytearray, byte[] dest, uint size)
		{
			Native.Msvcrt.MemoryCopy(bytearray, dest, size);
		}
		
		/// <summary>
		/// Copies the data of the byte array into a new byte array.
		/// </summary>
		/// <param name="bytearray">The byte array.</param>
		/// <param name="dest">The destination array.</param>
		/// <param name="size">The size to copy.</param>
		public static void memcpy(this byte[] bytearray, byte[] dest, int size)
		{
			Native.Msvcrt.MemoryCopy(bytearray, dest, size);
		}
		
		public static string GetString(this byte[] bytearr)
		{
			// I've experienced problems with Encoding.ASCII.GetBytes in the past, so I want to avoid that.
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in bytearr)
				stringBuilder.Append((char)b);
			return stringBuilder.ToString();
		}
	}
}
