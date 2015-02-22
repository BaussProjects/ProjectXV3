//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;

namespace ProjectX_V3_Lib.ThreadSafe
{
	/// <summary>
	/// A concurrent dictionary storing a concurrent dictionary.
	/// </summary>
	public class MultiConcurrentDictionary<TKey1, TKey2, TValue>
	{
		private ConcurrentDictionary<TKey1, ConcurrentDictionary<TKey2, TValue>> dictionary;
		public MultiConcurrentDictionary()
		{
			dictionary = new ConcurrentDictionary<TKey1, ConcurrentDictionary<TKey2, TValue>>();
		}
		
		public ConcurrentDictionary<TKey2, TValue> this[TKey1 key]
		{
			get { return dictionary[key]; }
		}
		
		public bool TryAdd(TKey1 key, ConcurrentDictionary<TKey2, TValue> value)
		{
			return dictionary.TryAdd(key, value);
		}
		public bool TryAdd(TKey1 key)
		{
			return TryAdd(key, new ConcurrentDictionary<TKey2, TValue>());
		}
		public bool TryRemove(TKey1 key, out ConcurrentDictionary<TKey2, TValue> value)
		{
			return dictionary.TryRemove(key, out value);
		}
		public bool TryGetValue(TKey1 key, out ConcurrentDictionary<TKey2, TValue> value)
		{
			return dictionary.TryGetValue(key, out value);
		}
		
		public int Count
		{
			get { return dictionary.Count; }
		}
		
		public bool ContainsKey(TKey1 key1)
		{
			return dictionary.ContainsKey(key1);
		}
	}
}
