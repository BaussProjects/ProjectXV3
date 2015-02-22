//Project by BaussHacker aka. L33TS

using System;
using System.Runtime.InteropServices;

namespace ProjectX_V3_Lib.Native
{
	/// <summary>
	/// Library Type: Windows API Library
	/// Library Name: kernel32.dll
	/// Library Location: %systemroot%\system32\kernel32.dll
	/// More Info: http://pinvoke.net/search.aspx?search=kernel32&namespace=kernel32
	/// Download (Only if missing): http://www.dll-files.com/dllindex/dll-files.shtml?kernel32
	/// </summary>
	public class Kernel32
	{
		public delegate bool ConsoleEventHandler(CtrlType sig);
		
		public enum CtrlType
		{
			CTRL_C_EVENT = 0,
			CTRL_BREAK_EVENT = 1,
			CTRL_CLOSE_EVENT = 2,
			CTRL_LOGOFF_EVENT = 5,
			CTRL_SHUTDOWN_EVENT = 6
		}
		
		[DllImport("kernel32.dll")]
        public static extern bool SetConsoleCtrlHandler(ConsoleEventHandler handler, bool add);
	}
}
