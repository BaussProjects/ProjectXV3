//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Lib.Cryptography
{
	/// <summary>
	/// The different types of algorithms for the blowfish crypto.
	/// </summary>
	internal enum BlowfishAlgorithm
	{
		/// <summary>
		/// Electronic codebook.
		/// </summary>
		ECB,
		
		/// <summary>
		/// Cipher-block chaining.
		/// </summary>
		CBC,
		
		/// <summary>
		/// Cipher feedback.
		/// </summary>
		CFB64,
		
		/// <summary>
		/// Output feedback.
		/// </summary>
		OFB64,
	};
}
