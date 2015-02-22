//Project by BaussHacker aka. L33TS

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;

namespace ProjectX_V3_Lib.IO
{
	/// <summary>
	/// An inifile handler.
	/// </summary>
	public unsafe class IniFile
	{
		#region PInvoke
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		public static extern uint GetPrivateProfileStringW(string Section, string Key, string Default, char* ReturnedString, int Size, string FileName);
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool WritePrivateProfileStringW(string Section, string Key, string Value, string FileName);
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool WritePrivateProfileSectionW(string Section, string String, string FileName);
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		public static extern int GetPrivateProfileSectionNamesW(char* ReturnBuffer, int Size, string FileName);
		#endregion
		#region Parsing
		public static Func<string, int> ToInt32 = new Func<string, int>(int.Parse);
		public static Func<string, uint> ToUInt32 = new Func<string, uint>(uint.Parse);
		public static Func<string, short> ToInt16 = new Func<string, short>(short.Parse);
		public static Func<string, ushort> ToUInt16 = new Func<string, ushort>(ushort.Parse);
		public static Func<string, sbyte> ToInt8 = new Func<string, sbyte>(sbyte.Parse);
		public static Func<string, byte> ToUInt8 = new Func<string, byte>(byte.Parse);
		public static Func<string, bool> ToBool = new Func<string, bool>(bool.Parse);
		public static Func<string, double> ToDouble = new Func<string, double>(double.Parse);
		public static Func<string, long> ToInt64 = new Func<string, long>(long.Parse);
		public static Func<string, ulong> ToUInt64 = new Func<string, ulong>(ulong.Parse);
		public static Func<string, float> ToFloat = new Func<string, float>(float.Parse);
		#endregion
		#region Sizes
		public const int Byte_Size = 6;
		public const int Short_Size = 9;
		public const int Int_Size = 15;
		public const int Long_Size = 22;
		public const int Float_Size = 10;
		public const int Double_Size = 20;
		#endregion
		
		/// <summary>
		/// The name of the inifile.
		/// </summary>
		private string fileName;
		
		/// <summary>
		/// The section of the inifile.
		/// </summary>
		private string fileSection;
		
		/// <summary>
		/// Sets the section of the inifile.
		/// </summary>
		/// <param name="section"></param>
		public void SetSection(string section)
		{
			this.fileSection = section;
		}
		
		/// <summary>
		/// Creates a new inifile.
		/// </summary>
		/// <param name="fileName">The file name.</param>
		public IniFile(string fileName)
		{
			this.fileName = fileName;
		}
		
		/// <summary>
		/// Creates a new inifile.
		/// </summary>
		/// <param name="fileName">The file name.</param>
		/// <param name="fileSection">The section.</param>
		public IniFile(string fileName, string fileSection)
		{
			this.fileName = fileName;
			this.fileSection = fileSection;
		}
		
		/// <summary>
		/// Checks whether the inifile exists or not.
		/// </summary>
		/// <returns>Returns true if the file exists.</returns>
		public bool Exists()
		{
			return File.Exists(fileName);
		}
		
		/// <summary>
		/// Creates the ini file.
		/// </summary>
		public void Create()
		{
			File.WriteAllText(fileName, "[Character]" + Environment.NewLine);
		}
		
		/// <summary>
		/// Reads a string value from the inifile.
		/// </summary>
		/// <param name="Section">The section.</param>
		/// <param name="Key">The key.</param>
		/// <param name="Default">The default value.</param>
		/// <param name="Size">The size.</param>
		/// <returns>Returns the string.</returns>
		private string ReadString(string Section, string Key, string Default, int Size)
		{
			char* lpBuffer = stackalloc char[Size];
			GetPrivateProfileStringW(Section, Key, Default, lpBuffer, Size, fileName);
			return new string(lpBuffer).Trim('\0');
		}
		
		/// <summary>
		/// Reads a generic value from the inifile.
		/// </summary>
		/// <param name="Section">The section.</param>
		/// <param name="Key">The key.</param>
		/// <param name="Default">The default value.</param>
		/// <param name="callback">The callback. {parsing}</param>
		/// <param name="BufferSize">The size.</param>
		/// <returns>Returns the value or default value if fail.</returns>
		private T ReadValue<T>(string Section, string Key, T Default, Func<string, T> callback, int BufferSize)
		{
			try
			{
				return callback.Invoke(ReadString(Section, Key, Default.ToString(), BufferSize));
			}
			catch
			{
				return Default;
			}
		}
		
		/// <summary>
		/// Writes a generic value to the inifile.
		/// </summary>
		/// <param name="Key">The key.</param>
		/// <param name="Value">The value.</param>
		public void Write<T>(string Key, T Value)
		{
			WritePrivateProfileStringW(fileSection, Key, Value.ToString(), fileName);
		}
		
		/// <summary>
		/// Writes a string value to the inifile.
		/// </summary>
		/// <param name="Key">The key.</param>
		/// <param name="Value">The value.</param>
		public void WriteString(string Key, string Value)
		{
			WritePrivateProfileStringW(fileSection, Key, Value, fileName);
		}
		
		/// <summary>
		/// Reads a string value. {max 255 chars}
		/// </summary>
		/// <param name="Key">The key.</param>
		/// <param name="Default">The default value.</param>
		/// <returns>Returns the value.</returns>
		public string ReadString(string Key, string Default)
		{
			return ReadString(fileSection, Key, Default, 255);
		}
		
		/// <summary>
		/// Reads a string value. {max 16 chars}
		/// </summary>
		/// <param name="Key">The key.</param>
		/// <param name="Default">The default value.</param>
		/// <returns>Returns the value.</returns>
		public string ReadSmallString(string Key, string Default)
		{
			return ReadString(fileSection, Key, Default, 16);
		}
		
		/// <summary>
		/// Reads a signed byte value.
		/// </summary>
		/// <param name="Key">The key.</param>
		/// <param name="Default">The default value.</param>
		/// <returns>Returns the value.</returns>
		public sbyte ReadSByte(string Key, sbyte Default)
		{
			return ReadValue<sbyte>(fileSection, Key, Default, ToInt8, Byte_Size);
		}
		/// <summary>
		/// Reads a signed int16 value.
		/// </summary>
		/// <param name="Key">The key.</param>
		/// <param name="Default">The default value.</param>
		/// <returns>Returns the value.</returns>
		public short ReadInt16(string Key, short Default)
		{
			return ReadValue<short>(fileSection, Key, Default, ToInt16, Short_Size);
		}
		
		/// <summary>
		/// Reads a signed int32 value.
		/// </summary>
		/// <param name="Key">The key.</param>
		/// <param name="Default">The default value.</param>
		/// <returns>Returns the value.</returns>
		public int ReadInt32(string Key, int Default)
		{
			return ReadValue<int>(fileSection, Key, Default, ToInt32, Int_Size);
		}
		
		/// <summary>
		/// Reads a signed int64 value.
		/// </summary>
		/// <param name="Key">The key.</param>
		/// <param name="Default">The default value.</param>
		/// <returns>Returns the value.</returns>
		public long ReadInt64(string Key, long Default)
		{
			return ReadValue<long>(fileSection, Key, Default, ToInt64, Long_Size);
		}
		
		/// <summary>
		/// Reads an unsigned byte value.
		/// </summary>
		/// <param name="Key">The key.</param>
		/// <param name="Default">The default value.</param>
		/// <returns>Returns the value.</returns>
		public byte ReadByte(string Key, byte Default)
		{
			return ReadValue<byte>(fileSection, Key, Default, ToUInt8, Byte_Size);
		}
		
		/// <summary>
		/// Reads an unsigned int16 value.
		/// </summary>
		/// <param name="Key">The key.</param>
		/// <param name="Default">The default value.</param>
		/// <returns>Returns the value.</returns>
		public ushort ReadUInt16(string Key, ushort Default)
		{
			return ReadValue<ushort>(fileSection, Key, Default, ToUInt16, Short_Size);
		}
		
		/// <summary>
		/// Reads an unsigned int32 value.
		/// </summary>
		/// <param name="Key">The key.</param>
		/// <param name="Default">The default value.</param>
		/// <returns>Returns the value.</returns>
		public uint ReadUInt32(string Key, uint Default)
		{
			return ReadValue<uint>(fileSection, Key, Default, ToUInt32, Int_Size);
		}
		
		/// <summary>
		/// Reads an unsigned int64 value.
		/// </summary>
		/// <param name="Key">The key.</param>
		/// <param name="Default">The default value.</param>
		/// <returns>Returns the value.</returns>
		public ulong ReadUInt64(string Key, ulong Default)
		{
			return ReadValue<ulong>(fileSection, Key, Default, ToUInt64, Long_Size);
		}
		
		/// <summary>
		/// Reads a single float point.
		/// </summary>
		/// <param name="Key">The key.</param>
		/// <param name="Default">The default value.</param>
		/// <returns>Returns the value.</returns>
		public float ReadSingle(string Key, float Default)
		{
			return ReadValue<float>(fileSection, Key, Default, ToFloat, Float_Size);
		}
		
		/// <summary>
		/// Reads a double float point.
		/// </summary>
		/// <param name="Key">The key.</param>
		/// <param name="Default">The default value.</param>
		/// <returns>Returns the value.</returns>
		public double ReadDouble(string Key, double Default)
		{
			return ReadValue<double>(fileSection, Key, Default, ToDouble, Double_Size);
		}
		
		/// <summary>
		/// Reads a boolean.
		/// </summary>
		/// <param name="Key">The key.</param>
		/// <param name="Default">The default value.</param>
		/// <returns>Returns the value.</returns>
		public bool ReadBoolean(string Key, bool Default)
		{
			return ReadValue<bool>(fileSection, Key, Default, ToBool, Byte_Size);
		}
		
		/// <summary>
		/// Gets all the section names.
		/// </summary>
		/// <param name="BufferSize">The buffer size.</param>
		/// <returns>Returns the section names.</returns>
		public string[] GetSectionNames(int BufferSize)
        {
            char* lpBuffer = stackalloc char[BufferSize];
            int Size = GetPrivateProfileSectionNamesW(lpBuffer, BufferSize, fileName);
            if (Size == 0)
                return new string[0];
            return new string(lpBuffer, 0, Size - 1).Split('\0');
        }
		
		/// <summary>
		/// Deletes a section.
		/// </summary>
		/// <param name="Section">The section to delete.</param>
		public void DeleteSection(string Section)
		{
			WritePrivateProfileSectionW(Section, null, fileName);
		}
		
		/// <summary>
		/// Deletes a specific range of sections.
		/// </summary>
		/// <param name="Exclude">Sections to exclude.</param>
		public void DeleteRange(params string[] Exclude)
		{
			foreach (string s in GetSectionNames(255))
			{
				if (!Exclude.Contains(s))
					DeleteSection(s);
			}
		}
	}
}
