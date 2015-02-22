//Project by BaussHacker aka. L33TS

using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ProjectX_V3_Lib.Extensions
{
	/// <summary>
	/// Extensions for System.Object
	/// </summary>
	public static class ObjectExtensions
	{
		/// <summary>
		/// Gets the md5 string associated with the type.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns>Returns the md5 string.</returns>
		public static string GetMd5String(this object obj)
		{
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			byte[] md5bytes = md5.ComputeHash(obj.ToString().GetBytes());
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte md5byte in md5bytes)
				stringBuilder.Append(md5byte.ToString("X2"));
			return stringBuilder.ToString();
		}
		
		/// <summary>
		/// Gets the hex string associated with the type.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns>Returns the hex string.</returns>
		public static string GetHexString(this object obj)
		{
			byte[] bytes = obj.ToString().GetBytes();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte _byte in bytes)
				stringBuilder.Append(_byte.ToString("X2"));
			return stringBuilder.ToString();
		}
		
		/// <summary>
		/// Gets the hex string associated with the type.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="seperator">The seperator between each value.</param>
		/// <returns>Returns the hex string.</returns>
		public static string GetHexString(this object obj, char seperator)
		{
			byte[] bytes = obj.ToString().GetBytes();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte _byte in bytes)
				stringBuilder.Append(_byte.ToString("X2")).Append(seperator);
			return stringBuilder.ToString();
		}
		
		/// <summary>
		/// Performas a deep copy for an object.
		/// </summary>
		/// <param name="a">The object to copy.</param>
		/// <returns>Returns the new copy.</returns>
		public static T DeepClone<T>(this T a)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(stream, a);
				stream.Position = 0;
				return (T) formatter.Deserialize(stream);
			}
		}
	}
}
