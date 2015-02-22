//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;
using System.Linq;

namespace ProjectX_V3_Lib.ThreadSafe
{
	/// <summary>
	/// Description of ConcurrentArrayList.
	/// </summary>
	[Serializable()]
	public class ConcurrentArrayList<T>
	{
		private ConcurrentDictionary<int, T> ListItems;

		public ConcurrentDictionary<int, T> ToDictionary()
		{
			return ListItems;
		}
		
		public void Clear()
		{
			ListItems.Clear();
		}
		private object SyncRoot;
		private int IndexUID = 0;
		
		public void ResetIndex()
		{
			lock (SyncRoot)
			{
				T[] Items = ListItems.Values.ToArray();
				foreach (T item in Items)
					ListItems.TryAdd(ListItems.Count, item);
				IndexUID = ListItems.Count;
			}
		}
		public ConcurrentArrayList()
		{
			System.Threading.Interlocked.CompareExchange(ref SyncRoot, new object(), null);
			ListItems = new ConcurrentDictionary<int, T>();
		}
		
		public ConcurrentArrayList(int concurrencylevel, int capacity)
		{
			System.Threading.Interlocked.CompareExchange(ref SyncRoot, new object(), null);
			ListItems = new ConcurrentDictionary<int, T>(concurrencylevel, capacity);
		}
		
		public void Add(T item)
		{
			foreach (T tItem in ListItems.Values)
			{
				object tObj = tItem;
				object tObjc = item;
				
				if (tObj == tObjc)
					return;
			}
			lock (SyncRoot)
			{
				ListItems.TryAdd(IndexUID, item);
				IndexUID++;
			}
		}
		public int[] GetIndexes()
		{
			int Index = 0;
			int RealIndex = 0;
			lock (SyncRoot)
			{
				Index = IndexUID;
			}
			for (int i = 0; i < Index; i++)
			{
				if (ListItems.ContainsKey(i))
				{
					RealIndex++;
				}
			}
			int[] Return = new int[RealIndex];
			for (int i = 0; i < Index; i++)
			{
				if (ListItems.ContainsKey(i))
				{
					Return[i] = i;
				}
			}
			return Return;
		}
		public void Remove(T item)
		{
			int Index = 0;
			
			lock (SyncRoot)
			{
				Index = IndexUID;
			}
			for (int i = 0; i < Index; i++)
			{
				if (ListItems.ContainsKey(i))
				{
					object tObj = ListItems[i];
					object tObjc = item;
					
					if (tObj == tObjc)
					{
						T rItem;
						ListItems.TryRemove(i, out rItem);
						break;
					}
				}
			}
		}
		
		public int FindIndex(T item)
		{
			int Index = 0;
			
			lock (SyncRoot)
			{
				Index = IndexUID;
			}
			for (int i = 0; i < Index; i++)
			{
				if (ListItems.ContainsKey(i))
				{
					object tObj = ListItems[i];
					object tObjc = item;
					
					if (tObj == tObjc)
					{
						return i;
					}
				}
			}
			return -1;
		}
		public T this[int index]
		{
			get { return ListItems[index]; }
		}
		
		public T Pull(int index)
		{
			T item;
			if (!ListItems.TryGetValue(index, out item))
				return default(T);
			return item;
		}
		
		public bool Contains(T item)
		{
			return (FindIndex(item) != -1);
		}
		
		public int Count
		{
			get { return ListItems.Count; }
		}
	}
}
