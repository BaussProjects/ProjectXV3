//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Lib.Time
{
	// Thanks to albetros source for this, because it's a nice implementation for manipulating with timeGetTime()
	
	/// <summary>
	/// Represents and instance in system time.
	/// </summary>
	public struct SystemTime
	{
		private readonly uint _time;

		/// <summary>
		/// Initializes a new instance of the <see cref="SystemTime"/> structure to a specified number of milliseconds.
		/// </summary>
		/// <param name="millis">A time expressed in millisecond units.</param>
		public SystemTime(uint millis)
		{
			_time = millis;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SystemTime"/> structure to a specified number of milliseconds.
		/// </summary>
		/// <param name="millis">A time expressed in millisecond units.</param>
		public SystemTime(int millis)
		{
			_time = (uint)millis;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SystemTime"/> structure to a specified number of milliseconds.
		/// </summary>
		/// <param name="millis">A time expressed in millisecond units.</param>
		public SystemTime(long millis)
		{
			_time = (uint)millis;
		}

		private SystemTime Add(int value, int scale)
		{
			return AddMilliseconds(value * scale);
		}

		/// <summary>
		/// Adds the specified number of milliseconds to the value of this instance.
		/// </summary>
		/// <param name="value">A number of milliseconds. The <paramref name="value"/> parameter can be negative or positive.</param>
		/// <returns>A <see cref="SystemTime"/> whose value is the sum of the time represented by this instance and the number of milliseconds represented by <paramref name="value"/>.</returns>
		public SystemTime AddMilliseconds(int value)
		{
			return new SystemTime(_time + value);
		}

		/// <summary>
		/// Adds the specified number of seconds to the value of this instance.
		/// </summary>
		/// <param name="value">A number of seconds. The <paramref name="value"/> parameter can be negative or positive.</param>
		/// <returns>A <see cref="SystemTime"/> whose value is the sum of the time represented by this instance and the number of seconds represented by <paramref name="value"/>.</returns>
		public SystemTime AddSeconds(int value)
		{
			return Add(value, 1000);
		}

		/// <summary>
		/// Adds the specified number of minutes to the value of this instance.
		/// </summary>
		/// <param name="value">A number of minutes. The <paramref name="value"/> parameter can be negative or positive.</param>
		/// <returns>A <see cref="SystemTime"/> whose value is the sum of the time represented by this instance and the number of minutes represented by <paramref name="value"/>.</returns>
		public SystemTime AddMinutes(int value)
		{
			return Add(value, 60000);
		}

		/// <summary>
		/// Adds the specified number of hours to the value of this instance.
		/// </summary>
		/// <param name="value">A number of hours. The <paramref name="value"/> parameter can be negative or positive.</param>
		/// <returns>A <see cref="SystemTime"/> whose value is the sum of the time represented by this instance and the number of hours represented by <paramref name="value"/>.</returns>
		public SystemTime AddHours(int value)
		{
			return Add(value, 3600000);
		}

		public static SystemTime operator +(SystemTime a, SystemTime b)
		{
			return new SystemTime(a._time + b._time);
		}

		public static SystemTime operator -(SystemTime a, SystemTime b)
		{
			return new SystemTime(a._time - b._time);
		}

		public static bool operator ==(SystemTime a, SystemTime b)
		{
			return a._time == b._time;
		}

		public static bool operator !=(SystemTime a, SystemTime b)
		{
			return a._time != b._time;
		}

		public static bool operator <(SystemTime a, SystemTime b)
		{
			return a._time < b._time;
		}

		public static bool operator >(SystemTime a, SystemTime b)
		{
			return a._time > b._time;
		}

		public static bool operator <=(SystemTime a, SystemTime b)
		{
			return a._time <= b._time;
		}

		public static bool operator >=(SystemTime a, SystemTime b)
		{
			return a._time >= b._time;
		}

		public static implicit operator uint(SystemTime time)
		{
			return time._time;
		}
//		public static DateTime UnreliableDateTimeFromTickCount(int tickCount)
//		{
//			DateTime now = DateTime.UtcNow;
//			DateTime boot = now - TimeSpan.FromMilliseconds(Native.Winmm.timeGetTime());
//			return boot + TimeSpan.FromMilliseconds(tickCount);
//		}

		public static implicit operator DateTime(SystemTime time)
		{
			DateTime now = DateTime.UtcNow;
			DateTime boot = now - TimeSpan.FromMilliseconds(time._time);
			DateTime ntime = (boot + TimeSpan.FromMilliseconds(time._time));
			return ntime.AddHours(1);
		}
		public static SystemTime Now
		{
			get
			{
				return Native.Winmm.timeGetTime();
			}
		}

		public bool Equals(SystemTime other)
		{
			return other._time == _time;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (obj.GetType() != typeof(SystemTime)) return false;
			return Equals((SystemTime)obj);
		}

		public override int GetHashCode()
		{
			return _time.GetHashCode();
		}

		public override string ToString()
		{
			return _time.ToString();
		}
	}
}
