//Project by BaussHacker aka. L33TS

using System;
using System.Runtime.InteropServices;

namespace ProjectX_V3_Lib.Native
{
	/// <summary>
	/// Library Type: Windows API Library
	/// Library Name: msvcrt.dll
	/// Library Location: %systemroot%\system32\msvcrt.dll
	/// More Info: http://pinvoke.net/search.aspx?search=msvcrt&namespace=msvcrt
	/// Download (Only if missing): http://www.dll-files.com/dllindex/dll-files.shtml?msvcrt
	/// </summary>
	public unsafe class Msvcrt
	{
		[DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
		private static extern unsafe void* memcpy(void* dest, void* src, uint size);
		
		[DllImport("msvcrt.dll", EntryPoint = "srand", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
		public static extern void srand(int seed);
		
		[DllImport("msvcrt.dll", EntryPoint = "rand", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
		public static extern short rand();
		
		public static void MemoryCopy(void* src, void* dest, uint size)
		{
			memcpy(dest, src, size);
		}
		
		public static void MemoryCopy(byte* src, byte* dest, uint size)
		{
			memcpy((void*)dest, (void*)src, size);
		}
		
		public static void MemoryCopy(byte[] srcarr, byte[] destarr)
		{
			fixed (byte* dest = destarr, src = srcarr)
				memcpy((void*)dest, (void*)src, (uint)srcarr.Length);
		}
		
		public static void MemoryCopy(byte[] srcarr, byte[] destarr, uint size)
		{
			fixed (byte* dest = destarr, src = srcarr)
				memcpy((void*)dest, (void*)src, (uint)size);
		}
		
		public static void MemoryCopy(byte[] srcarr, byte[] destarr, int size)
		{
			fixed (byte* dest = destarr, src = srcarr)
				memcpy((void*)dest, (void*)src, (uint)size);
		}
		
		public static void MemoryCopy(void* src, void* dest, int size)
		{
			MemoryCopy(src, dest, (uint)size);
		}
		public static void MemoryCopy(byte* src, byte* dest, int size)
		{
			MemoryCopy(src, dest, (uint)size);
		}
	}
}
