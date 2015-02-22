//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;
using System.Xml;
using System.Text;

namespace ProjectX_V3_Lib.IO
{
	/// <summary>
	/// A xml configuration file.
	/// </summary>
	public class XmlConfig
	{
		/// <summary>
		/// The settings.
		/// </summary>
		private ConcurrentDictionary<string, string> configSettings;
		
		/// <summary>
		/// Creates a new Xml Config.
		/// </summary>
		public XmlConfig()
		{
			configSettings = new ConcurrentDictionary<string, string>();
		}
		
		/// <summary>
		/// Loads a config file into the config settings.
		/// </summary>
		/// <param name="configFile">The config file to load.</param>
		public void LoadConfig(string configFile)
		{
			using (XmlTextReader xmlTextReader = new XmlTextReader(configFile))
			{
				bool enteredConfig = false;
				while (xmlTextReader.Read())
				{
					if (xmlTextReader.NodeType != XmlNodeType.Element)
						continue;
					
					if (enteredConfig)
					{
						string settingName = xmlTextReader.Name;
						string settingValue = xmlTextReader.ReadString();
						if (!configSettings.TryAdd(settingName, settingValue))
							throw new Exception(string.Format("Could not add {0} to the settings.", settingName));
					}
					else if (xmlTextReader.Name == "Config")
						enteredConfig = true;
				}
			}
		}
		
		/// <summary>
		/// Saves the config settings into an xml file.
		/// </summary>
		/// <param name="configFile">The config file to save.</param>
		public void SaveConfig(string configFile)
		{
			StringBuilder xmlBuilder = new StringBuilder();
			xmlBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>")
				.Append(Environment.NewLine).Append(Environment.NewLine)
				.Append("<Config>").Append(Environment.NewLine);
			
			foreach (string settingName in configSettings.Keys)
			{
				xmlBuilder.Append("\t").Append("<").Append(settingName).Append(">")
					.Append(configSettings[settingName])
					.Append("</").Append(settingName).Append(">")
					.Append(Environment.NewLine);
			}
			
			xmlBuilder.Append("</Config>");
			System.IO.File.WriteAllText(configFile, xmlBuilder.ToString());
		}
		/// <summary>
		/// Reads a value from the settings.
		/// </summary>
		/// <param name="settingName">The settings name.</param>
		/// <param name="settingValue">[out] The settings value.</param>
		/// <returns>Returns true if the value was read.</returns>
		private bool Read(string settingName, out string settingValue)
		{
			return configSettings.TryGetValue(settingName, out settingValue);
		}
		
		/// <summary>
		/// Reads a string value from the settings.
		/// </summary>
		/// <param name="settingName">The settings name.</param>
		/// <returns>Returns the value.</returns>
		public string ReadString(string settingName)
		{
			string settingValue;
			if (!Read(settingName, out settingValue))
				throw new Exception("Could not retrieve setting.");
			return settingValue;
		}
		
		/// <summary>
		/// Reads a signed byte value from the settings.
		/// </summary>
		/// <param name="settingName">The settings name.</param>
		/// <returns>Returns the value.</returns>
		public sbyte ReadSByte(string settingName)
		{
			string settingValue;
			if (!Read(settingName, out settingValue))
				throw new Exception("Could not retrieve setting.");
			return sbyte.Parse(settingValue);
		}
		
		/// <summary>
		/// Reads a signed int16 from the settings.
		/// </summary>
		/// <param name="settingName">The settings name.</param>
		/// <returns>Returns the value.</returns>
		public short ReadInt16(string settingName)
		{
			string settingValue;
			if (!Read(settingName, out settingValue))
				throw new Exception("Could not retrieve setting.");
			return short.Parse(settingValue);
		}
		
		/// <summary>
		/// Reads a signed int32 value from the settings.
		/// </summary>
		/// <param name="settingName">The settings name.</param>
		/// <returns>Returns the value.</returns>
		public int ReadInt32(string settingName)
		{
			string settingValue;
			if (!Read(settingName, out settingValue))
				throw new Exception("Could not retrieve setting.");
			return int.Parse(settingValue);
		}
		
		/// <summary>
		/// Reads a signed int64 from the settings.
		/// </summary>
		/// <param name="settingName">The settings name.</param>
		/// <returns>Returns the value.</returns>
		public long ReadInt64(string settingName)
		{
			string settingValue;
			if (!Read(settingName, out settingValue))
				throw new Exception("Could not retrieve setting.");
			return long.Parse(settingValue);
		}
		
		/// <summary>
		/// Reads an unsigned byte value from the settings.
		/// </summary>
		/// <param name="settingName">The settings name.</param>
		/// <returns>Returns the value.</returns>
		public byte ReadByte(string settingName)
		{
			string settingValue;
			if (!Read(settingName, out settingValue))
				throw new Exception("Could not retrieve setting.");
			return byte.Parse(settingValue);
		}
		
		/// <summary>
		/// Reads an unsigned int16 value from the settings.
		/// </summary>
		/// <param name="settingName">The settings name.</param>
		/// <returns>Returns the value.</returns>
		public ushort ReadUInt16(string settingName)
		{
			string settingValue;
			if (!Read(settingName, out settingValue))
				throw new Exception("Could not retrieve setting.");
			return ushort.Parse(settingValue);
		}
		
		/// <summary>
		/// Reads an unsigned int32 value from the settings.
		/// </summary>
		/// <param name="settingName">The settings name.</param>
		/// <returns>Returns the value.</returns>
		public uint ReadUInt32(string settingName)
		{
			string settingValue;
			if (!Read(settingName, out settingValue))
				throw new Exception("Could not retrieve setting.");
			return uint.Parse(settingValue);
		}
		
		/// <summary>
		/// Reads an unsigned int64 value from the settings.
		/// </summary>
		/// <param name="settingName">The settings name.</param>
		/// <returns>Returns the value.</returns>
		public ulong ReadUInt64(string settingName)
		{
			string settingValue;
			if (!Read(settingName, out settingValue))
				throw new Exception("Could not retrieve setting.");
			return ulong.Parse(settingValue);
		}
		
		/// <summary>
		/// Reads a boolean value from the settings.
		/// </summary>
		/// <param name="settingName">The settings name.</param>
		/// <returns>Returns the value.</returns>
		public bool ReadBool(string settingName)
		{
			string settingValue;
			if (!Read(settingName, out settingValue))
				throw new Exception("Could not retrieve setting.");
			return bool.Parse(settingValue.ToLower()); // not sure if FaLsE would work otherwise, cba to test
		}
		
		/// <summary>
		/// Reads a float value from the settings.
		/// </summary>
		/// <param name="settingName">The settings name.</param>
		/// <returns>Returns the value.</returns>
		public float ReadFloat(string settingName)
		{
			string settingValue;
			if (!Read(settingName, out settingValue))
				throw new Exception("Could not retrieve setting.");
			return float.Parse(settingValue.ToLower());
		}
		
		/// <summary>
		/// Reads a double value from the settings.
		/// </summary>
		/// <param name="settingName">The settings name.</param>
		/// <returns>Returns the value.</returns>
		public double ReadDouble(string settingName)
		{
			string settingValue;
			if (!Read(settingName, out settingValue))
				throw new Exception("Could not retrieve setting.");
			return double.Parse(settingValue.ToLower());
		}
		
		/// <summary>
		/// Reads a decimal value from the settings.
		/// </summary>
		/// <param name="settingName">The settings name.</param>
		/// <returns>Returns the value.</returns>
		public decimal ReadDecimal(string settingName)
		{
			string settingValue;
			if (!Read(settingName, out settingValue))
				throw new Exception("Could not retrieve setting.");
			return decimal.Parse(settingValue.ToLower());
		}
		
		/// <summary>
		/// Adds or updates a setting.
		/// </summary>
		/// <param name="settingName">The setting name.</param>
		/// <param name="settingValue">The setting value.</param>
		public void AddOrUpdate(string settingName, string settingValue)
		{
			if (configSettings.ContainsKey(settingName))
				configSettings[settingName] = settingValue;
			else if (!configSettings.TryAdd(settingName, settingValue))
				throw new Exception("Could not add or update a setting.");
		}
	}
}
