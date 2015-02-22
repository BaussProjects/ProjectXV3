//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;

namespace ProjectX_V3_Lib.ThreadSafe
{
	/// <summary>
	/// A concurrent dictionary using 2 keys. Only one key is required to get the value.
	/// The Selector class consists of 4 concurrent dictionaries.
	/// 2 for usage of key1 -> value, key2 -> value
	/// 2 for usage for key1 -> key2, key2 -> key1
	/// </summary>
	public class Selector<TKey1, TKey2, TValue>
	{
		/// <summary>
		/// The concurrent dictionary for TKey1.
		/// </summary>
		public ConcurrentDictionary<TKey1, TValue> selectorCollection1;
		
		/// <summary>
		/// The concurrent dictionary for TKey2.
		/// </summary>
		public ConcurrentDictionary<TKey2, TValue> selectorCollection2;
		
		/// <summary>
		/// The keymatching dictionary #1. TKey1 -> TKey2
		/// </summary>
		private ConcurrentDictionary<TKey1, TKey2> keymatcher1;
		
		/// <summary>
		/// The keymatching dictionary #2. TKey2 -> TKey1
		/// </summary>
		private ConcurrentDictionary<TKey2, TKey1> keymatcher2;

		/// <summary>
		/// Creates a new instance of Selector.
		/// </summary>
		public Selector()
		{
			selectorCollection1 = new ConcurrentDictionary<TKey1, TValue>();
			selectorCollection2 = new ConcurrentDictionary<TKey2, TValue>();

			keymatcher1 = new ConcurrentDictionary<TKey1, TKey2>();
			keymatcher2 = new ConcurrentDictionary<TKey2, TKey1>();
		}
		
		/// <summary>
		/// Gets the total amount of entries.
		/// </summary>
		public int Count
		{
			get { return selectorCollection1.Count; }
		}

		/// <summary>
		/// Tries to select a value using the first key.
		/// </summary>
		/// <param name="key">The first key.</param>
		/// <param name="value">The value.</param>
		/// <returns>eturns true if the value was selected.</returns>
		public bool TrySelect(TKey1 key, out TValue value)
		{
			return selectorCollection1.TryGetValue(key, out value);
		}

		/// <summary>
		/// Tries to select a value using the second key.
		/// </summary>
		/// <param name="key">The second key.</param>
		/// <param name="value">The value.</param>
		/// <returns>Returns true if the value was selected.</returns>
		public bool TrySelect(TKey2 key, out TValue value)
		{
			return selectorCollection2.TryGetValue(key, out value);
		}
		
		public bool TryGetFirstKey(TKey2 key2, out TKey1 key1)
		{
			return keymatcher2.TryGetValue(key2, out key1);
		}
		
		public bool TryGetSecondKey(TKey1 key1, out TKey2 key2)
		{
			return keymatcher1.TryGetValue(key1, out key2);
		}

		/// <summary>
		/// Tries to add a value to the selector.
		/// </summary>
		/// <param name="key1">The first key.</param>
		/// <param name="key2">The second key.</param>
		/// <param name="value">The value.</param>
		/// <returns>Returns true if the value was added.</returns>
		public bool TryAdd(TKey1 key1, TKey2 key2, TValue value)
		{
			return (selectorCollection1.TryAdd(key1, value) &&
			        selectorCollection2.TryAdd(key2, value) &&
			        keymatcher1.TryAdd(key1, key2) &&
			        keymatcher2.TryAdd(key2, key1));
		}

		/// <summary>
		/// Tries to add a value to the selector. Allowing fail on key2.
		/// </summary>
		/// <param name="key1">The first key.</param>
		/// <param name="key2">The second key.</param>
		/// <param name="value">The value.</param>
		/// <returns>Returns true if the value was added.</returns>
		public bool TryAddAndDismiss(TKey1 key1, TKey2 key2, TValue value)
		{
			selectorCollection2.TryAdd(key2, value);
			keymatcher2.TryAdd(key2, key1);
			
			return (selectorCollection1.TryAdd(key1, value) &&
			        keymatcher1.TryAdd(key1, key2));
		}
		
		/// <summary>
		/// Tries to remove a value using the first key.
		/// </summary>
		/// <param name="key1">The first key.</param>
		/// <returns>Returns true if the value was removed.</returns>
		public bool TryRemove(TKey1 key1)
		{
			TKey2 key2;
			if (!keymatcher1.TryGetValue(key1, out key2))
				return false;
			TValue rval;
			return (selectorCollection1.TryRemove(key1, out rval) &&
			        selectorCollection2.TryRemove(key2, out rval) &&
			        keymatcher1.TryRemove(key1, out key2) &&
			        keymatcher2.TryRemove(key2, out key1));
		}

		/// <summary>
		/// Tries to remove a value using the second key.
		/// </summary>
		/// <param name="key2">The second key.</param>
		/// <returns>Returns true if the value was removed.</returns>
		public bool TryRemove(TKey2 key2)
		{
			TKey1 key1;
			if (!keymatcher2.TryGetValue(key2, out key1))
				return false;
			TValue rval;
			return (selectorCollection1.TryRemove(key1, out rval) &&
			        selectorCollection2.TryRemove(key2, out rval) &&
			        keymatcher1.TryRemove(key1, out key2) &&
			        keymatcher2.TryRemove(key2, out key1));
		}

		/// <summary>
		/// Checks if the first key exists.
		/// </summary>
		/// <param name="key1">The first key.</param>
		/// <returns>Returns true if the key exists.</returns>
		public bool Contains(TKey1 key1)
		{
			return keymatcher1.ContainsKey(key1);
		}

		/// <summary>
		/// Checks if the second key exists.
		/// </summary>
		/// <param name="key2">The second key.</param>
		/// <returns>Returns true if the key exists.</returns>
		public bool Contains(TKey2 key2)
		{
			return keymatcher2.ContainsKey(key2);
		}

		/// <summary>
		/// Checks if a value exists.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>Returns true if the value exists.</returns>
		public bool Contains(TValue value)
		{
			return selectorCollection1.Values.Contains(value);
		}
		
		/// <summary>
		/// Clears the whole collection.
		/// </summary>
		public void Clear()
		{
			selectorCollection1.Clear();
			selectorCollection2.Clear();
			keymatcher1.Clear();
			keymatcher2.Clear();
		}
		
		/// <summary>
		/// Tries to copy the selector into a new selector.
		/// </summary>
		/// <param name="newSelector">The new selector.</param>
		/// <returns>Returns true if the selector was copied.</returns>
		public bool TryCopy(out Selector<TKey1, TKey2, TValue> newSelector)
		{
			bool success = true;
			newSelector = new Selector<TKey1, TKey2, TValue>();
			foreach (System.Collections.Generic.KeyValuePair<TKey1, TKey2> keys in keymatcher1)
			{
				TValue value;
				if (!selectorCollection1.TryGetValue(keys.Key, out value))
					success = false;
				
				if (!newSelector.TryAdd(keys.Key, keys.Value, value))
					success = false;
			}
			newSelector.Clear();
			return success;
		}
		
		/// <summary>
		/// Tries to perform a foreach action on every value in the selector.
		/// </summary>
		/// <param name="ItemAction">The action to perform.</param>
		/// <param name="failed">How many values the action failed to be performed on.</param>
		/// <returns>Returns true if all actions were invoked proper.</returns>
		public bool TryForeachAction(Action<TKey1, TValue> ItemAction, out int failed)
		{
			failed = 0;
			foreach (TKey1 keys in keymatcher1.Keys)
			{
				TValue value;
				if (selectorCollection1.TryGetValue(keys, out value))
				{
					try
					{
						ItemAction.Invoke(keys, value);
					}
					catch
					{
						failed++;
					}
				}
				else
					failed++;
			}
			return (failed == 0);
		}
		
		public TValue this[TKey1 key1]
		{
			get { return selectorCollection1[key1]; }
		}
	}
}
