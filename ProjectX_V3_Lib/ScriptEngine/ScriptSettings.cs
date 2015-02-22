//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;

namespace ProjectX_V3_Lib.ScriptEngine
{
	/// <summary>
	/// Settings for the script engine.
	/// </summary>
	[Serializable()]
	public class ScriptSettings
	{
		/// <summary>
		/// The primary language of the script engine.
		/// </summary>
		public ScriptLanguage Language = ScriptLanguage.CSharp;
		
		/// <summary>
		/// The full path of the scripts location.
		/// </summary>
		public string ScriptLocation = Environment.CurrentDirectory + "\\scripts";
		
		/// <summary>
		/// The .NET Framework to compile with.
		/// </summary>
		public string Framework = "v4.0";
		
		/// <summary>
		/// The types to compile.
		/// </summary>
		internal ConcurrentDictionary<int, Type> types = new ConcurrentDictionary<int, Type>();
		
		/// <summary>
		/// Adds a type to the script engine.
		/// </summary>
		/// <param name="type">The type to add.</param>
		public void AddScriptType(Type type)
		{
			if (!types.TryAdd(types.Count, type))
				throw new Exception("Could not add type.");
		}
		
		/// <summary>
		/// The namespaces to compile.
		/// </summary>
		internal ConcurrentDictionary<int, string> _namespaces = new ConcurrentDictionary<int, string>();
		
		/// <summary>
		/// Adds a namespace to the script engine.
		/// </summary>
		/// <param name="_namespace">The namespace to add.</param>
		public void AddNamespace(string _namespace)
		{
			if (!_namespaces.TryAdd(_namespaces.Count, _namespace))
				throw new Exception("Could not add type.");
		}
	}
}
