//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace ProjectX_V3_Lib.ScriptEngine
{
	/// <summary>
	/// A collection of scripts.
	/// </summary>
	public class ScriptCollection
	{
		/// <summary>
		/// The scripts.
		/// </summary>
		private ConcurrentDictionary<uint, MethodInfo> scripts;
		
		/// <summary>
		/// Settings associated with the scripts.
		/// </summary>
		private ScriptSettings Settings;
		
		/// <summary>
		/// Creates a new instance of ScriptCollection.
		/// </summary>
		/// <param name="Settings">The settings associated with the script engine.</param>
		public ScriptCollection(ScriptSettings Settings)
		{
			scripts = new ConcurrentDictionary<uint, MethodInfo>();
			this.Settings = Settings;
		}
		
		/// <summary>
		/// Adds or updates a script.
		/// </summary>
		/// <param name="key">The key of the script.</param>
		/// <param name="script">The script.</param>
		public void AddOrUpdate(uint key, MethodInfo script)
		{
			if (scripts.ContainsKey(key))
			{		
				scripts[key] = script;
				return;
			}
			if (!scripts.TryAdd(key, script))
				throw new Exception("Could not add: " + key);
		}
		
		/// <summary>
		/// Invokes a script.
		/// </summary>
		/// <param name="key">The key of the script.</param>
		/// <param name="parameters">Parameters associated with the script. [null, if no parameters]</param>
		/// <returns>Returns true if the script exists.</returns>
		public bool Invoke(uint key, object[] parameters)
		{
			if (!scripts.ContainsKey(key))
				return false;
			
			MethodInfo script;
			if (!scripts.TryGetValue(key, out script))
				throw new Exception("Could not execute: " + key);
			
			script.Invoke(null, parameters);
			return true;
		}
	}
}
