//Project by BaussHacker aka. L33TS

using System;
using System.Runtime.InteropServices;

namespace ProjectX_V3_Lib.Native
{
	/// <summary>
	/// Library Type: Windows API Library
	/// Library Name: winmm.dll
	/// Library Location: %systemroot%\system32\winmm.dll
	/// More Info: http://pinvoke.net/search.aspx?search=winmm&namespace=winmm
	/// Download (Only if missing): http://www.dll-files.com/dllindex/dll-files.shtml?winmm
	/// </summary>
	public class Winmm
	{
		[DllImport("winmm.dll")]
		public static extern Time.SystemTime timeGetTime();
	}
}
