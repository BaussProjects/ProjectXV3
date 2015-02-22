//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Lib.ThreadSafe
{
	/// <summary>
	/// A thread-safe random generator.
	/// </summary>
	public class RandomGenerator : Random
	{
		/// <summary>
		/// The random generator.
		/// </summary>
		private static readonly RandomGenerator randomGenerator = new RandomGenerator();
		
		/// <summary>
		/// Gets the random generator.
		/// </summary>
		public static RandomGenerator Generator
		{
			get { return randomGenerator; }
		}
		
		/// <summary>
		/// Creates a new instance of RandomGenerator.
		/// </summary>
		private RandomGenerator()
		{
			System.Threading.Interlocked.CompareExchange(ref _syncRoot, new object(), null);
		}
		
		/// <summary>
		/// The synchronization root.
		/// </summary>
		private object _syncRoot;
		
		/// <summary>
		/// Gets the synchronization root.
		/// </summary>
		internal object SyncRoot
		{
			get { return _syncRoot; }
		}

		/// <summary>
		/// Gets a random generated number.
		/// </summary>
		/// <returns>Returns the random generated number.</returns>
		public override int Next()
		{
			lock (SyncRoot)
				return base.Next();
		}
		
		/// <summary>
		/// Gets a random generated number.
		/// </summary>
		/// <param name="maxVal">The max value of the random generated number.</param>
		/// <returns>Returns the random generated number.</returns>
		public override int Next(int maxVal)
		{
			lock (SyncRoot)
				return base.Next(maxVal);
		}
		
		/// <summary>
		/// Gets a random generated number.
		/// </summary>
		/// <param name="minVal">The max value of the random generated number.</param>
		/// <param name="maxVal">The min value of the random generated number.</param>
		/// <returns>Returns the random generated number.</returns>
		public override int Next(int minVal, int maxVal)
		{
			lock (SyncRoot)
				return base.Next(minVal, maxVal);
		}
		
		public override void NextBytes(byte[] buffer)
		{
			lock (SyncRoot)
				base.NextBytes(buffer);
		}
		
		public object NextEnum(Type EnumType)
		{
			Array array = Enum.GetValues(EnumType);
			return array.GetValue(Next(0, array.Length));
		}
	}
}
