//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;
using System.Text;

namespace ProjectX_V3_Game.Core
{
	/// <summary>
	/// Description of SystemVariables.
	/// </summary>
	public class SystemVariables
	{
		public static ConcurrentDictionary<string, string> Variables;
		
		static SystemVariables()
		{
			Variables = new ConcurrentDictionary<string, string>();
		}
		
		public static string ReplaceVariables(string msg)
		{
			string refmsg = msg;
			foreach (string variable in Variables.Keys)
			{
				string variableValue;
				if (Variables.TryGetValue(variable, out variableValue))
				{
					refmsg = refmsg.Replace(string.Format("[[{0}]]", variable), variableValue);
				}
			}
			return refmsg;
		}
	}
}
