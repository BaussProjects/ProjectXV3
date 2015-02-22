//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;
using System.Linq;

namespace ProjectX_V3_Game.Entities
{
	/// <summary>
	/// Description of SubClasses.
	/// </summary>
	public class SubClasses
	{
		private ConcurrentDictionary<Enums.SubClasses, SubClass> SubClassCollection;
		public SubClasses()
		{
			SubClassCollection = new ConcurrentDictionary<ProjectX_V3_Game.Enums.SubClasses, SubClass>();
		}
		public int Count
		{
			get { return SubClassCollection.Count; }
		}
		public SubClass GetSubClass(Enums.SubClasses subclass)
		{
			return SubClassCollection[subclass];
		}
		public void AddSubClass(SubClass subclass)
		{
			SubClassCollection.TryAdd(subclass.ID, subclass);
		}
		public SubClass[] GetAll()
		{
			return SubClassCollection.Values.ToArray();
		}
	}
}
