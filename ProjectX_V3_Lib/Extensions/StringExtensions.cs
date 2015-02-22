//Project by BaussHacker aka. L33TS

using System;
using System.Text;
using System.Linq;

namespace ProjectX_V3_Lib.Extensions
{
	/// <summary>
	/// Extensions for System.String
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Gets the bytes whom the string represents.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns>Returns the bytes.</returns>
		public static byte[] GetBytes(this string str)
		{
			// I've experienced problems with Encoding.ASCII.GetBytes in the past, so I want to avoid that.
			byte[] b = new byte[str.Length];
			for (int i = 0; i < str.Length; i++)
				b[i] = (byte)str[i];
			return b;
		}
		
		const string abc = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		const string num = "1234567890";
		const string regchars = ".:,;-_|?+*^()[]{}!#$£~\\/";
		
		/// <summary>
		/// Makes the string readable with A-Z and spaces only.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns>Returns the new readable string.</returns>
		public static string MakeReadable(this string str)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char _char in str.ToCharArray())
			{
				if (abc.Contains(_char.ToString().ToUpper()) ||
				    _char == ' ')
					stringBuilder.Append(_char);
			}
			return stringBuilder.ToString();
		}
		
		/// <summary>
		/// Makes a string readable with specific crits.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="AZ">True if keep A-Z.</param>
		/// <param name="Space">True if keep spaces.</param>
		/// <param name="Num">True if keep numbers.</param>
		/// <param name="RegChars">True if keep regular characters: .:,;-_|?+*^()[]{}!#$£~\/</param>
		/// <param name="Tab">True if keep tab.</param>
		/// <returns>Returns the new readable string.</returns>
		public static string MakeReadable(this string str, bool AZ, bool Space, bool Num, bool RegChars, bool Tab)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char _char in str.ToCharArray())
			{
				if (abc.Contains(_char.ToString().ToUpper()) && AZ)
					stringBuilder.Append(_char);
				else if (_char == ' ' && Space)
					stringBuilder.Append(_char);
				else if (num.Contains(_char) && Num)
					stringBuilder.Append(_char);
				else if (regchars.Contains(_char) && RegChars)
					stringBuilder.Append(_char);
				else if (_char == '\t' && Tab)
					stringBuilder.Append(_char);
			}
			return stringBuilder.ToString();
		}
		
		/// <summary>
		/// Converts a string array into a 32bit integer array.
		/// </summary>
		/// <param name="strarr">The string array.</param>
		/// <param name="intarr">The int array.</param>
		public static void ConverToInt32(this string[] strarr, out int[] intarr)
		{
			intarr = new int[strarr.Length];
			for (int i = 0; i < strarr.Length; i++)
				intarr[i] = int.Parse(strarr[i]);
		}
		
		public static int Append(this string[] strarr, string append)
		{
			string[] narr = new string[strarr.Length + 1];
			System.Buffer.BlockCopy(strarr, 0, narr, 0, strarr.Length);
			narr[narr.Length - 1] = append;
			strarr = narr;
			return (narr.Length - 1);
		}
		
		public static string StripSize(this string str, int size)
		{
			if (str.Length <= size)
				return str;
			
			StringBuilder sb = new StringBuilder();
			sb.Append(str);
			sb.Length = size;
			string nstr = sb.ToString();
			return nstr;
		}
		
		public static byte ToByte(this string str)
		{
			byte value;
			byte.TryParse(str, out value);
			return value;
		}
		public static ushort ToUInt16(this string str)
		{
			ushort value;
			ushort.TryParse(str, out value);
			return value;
		}
		public static uint ToUInt32(this string str)
		{
			uint value;
			uint.TryParse(str, out value);
			return value;
		}
		public static ulong ToUInt64(this string str)
		{
			ulong value;
			ulong.TryParse(str, out value);
			return value;
		}
		
		public static sbyte ToSByte(this string str)
		{
			sbyte value;
			sbyte.TryParse(str, out value);
			return value;
		}
		public static short ToInt16(this string str)
		{
			short value;
			short.TryParse(str, out value);
			return value;
		}
		public static int ToInt32(this string str)
		{
			int value;
			int.TryParse(str, out value);
			return value;
		}
		public static long ToInt64(this string str)
		{
			long value;
			long.TryParse(str, out value);
			return value;
		}
		
		public static bool IsNumericString(this string str)
		{
			foreach (char _char in str.ToCharArray())
			{
				if (!char.IsNumber(_char))
					return false;
			}
			return true;
		}
	}
}
