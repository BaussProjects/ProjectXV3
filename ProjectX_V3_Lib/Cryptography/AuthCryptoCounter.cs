//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Lib.Cryptography
{
	/// <summary>
	/// The key counter for the auth crypto.
	/// </summary>
	public class AuthCryptoCounter
	{
		/// <summary>
		/// The counter.
		/// </summary>
		private ushort m_Counter;
		
		/// <summary>
		/// Creates a new instance of AuthCryptoCounter.
		/// </summary>
		public AuthCryptoCounter()
		{
			m_Counter = 0;
		}

		/// <summary>
		/// Gets the first key.
		/// </summary>
		public byte Key1
		{
			get { return (byte)(m_Counter & 0xFF); }
		}
		
		/// <summary>
		/// Gets the second key.
		/// </summary>
		public byte Key2
		{
			get { return (byte)(m_Counter >> 8); }
		}

		/// <summary>
		/// Increases the counter by 1.
		/// </summary>
		public void Increase()
		{
			Increase(1);
		}
		
		/// <summary>
		/// Increases the counter by a specific amount.
		/// </summary>
		/// <param name="amount">The amount to increase the counter by.</param>
		public void Increase(ushort amount)
		{
			m_Counter += amount;
		}
	}
}
