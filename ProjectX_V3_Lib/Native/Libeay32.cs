//Project by BaussHacker aka. L33TS

using System;
using System.Runtime.InteropServices;

namespace ProjectX_V3_Lib.Native
{
	/// <summary>
	/// Library Type: Open SSL
	/// Library Name: libeay32.dll
	/// Library Location: NO_DEFAULT_LOCATION
	/// More Info: http://www.dll-files.com/dllindex/dll-files.shtml?libeay32
	/// </summary>
	public class Libeay32
	{
		[DllImport("libeay32.dll", CallingConvention = CallingConvention.Cdecl)]
		public extern static void CAST_set_key(IntPtr _key, int len, byte[] data);

		[DllImport("libeay32.dll", CallingConvention = CallingConvention.Cdecl)]
		public extern static void CAST_ecb_encrypt(byte[] in_, byte[] out_, IntPtr schedule, int enc);

		[DllImport("libeay32.dll", CallingConvention = CallingConvention.Cdecl)]
		public extern static void CAST_cbc_encrypt(byte[] in_, byte[] out_, int length, IntPtr schedule, byte[] ivec, int enc);

		[DllImport("libeay32.dll", CallingConvention = CallingConvention.Cdecl)]
		public extern static void CAST_cfb64_encrypt(byte[] in_, byte[] out_, int length, IntPtr schedule, byte[] ivec, ref int num, int enc);

		[DllImport("libeay32.dll", CallingConvention = CallingConvention.Cdecl)]
		public extern static void CAST_ofb64_encrypt(byte[] in_, byte[] out_, int length, IntPtr schedule, byte[] ivec, out int num);

	}
}
