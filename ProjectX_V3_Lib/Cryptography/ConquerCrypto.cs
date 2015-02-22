//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Lib.Cryptography
{
	/// <summary>
	/// The base class for a crypto.
	/// </summary>
	public abstract class ConquerCrypto
	{
		/// <summary>
		///  Override this.
		/// </summary>
		/// <param name="buffer">buffer</param>
		public abstract void Encrypt(byte[] buffer);
		
		/// <summary>
		/// Override this.
		/// </summary>
		/// <param name="buffer">buffer</param>
		public abstract void Decrypt(byte[] buffer);
	}
}
