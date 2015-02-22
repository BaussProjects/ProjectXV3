//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Threads
{
	/// <summary>
	/// Description of DynamicMapThread.
	/// </summary>
	public class DynamicMapThread
	{
		public static void Handle()
		{
			foreach (Maps.DynamicMap dyn in Core.Kernel.DynamicMaps.selectorCollection1.Values)
			{
				if (!dyn.HasHadPlayers)
				{
					dyn.HasHadPlayers = dyn.HasPlayers(true);
				}
				
				if (!dyn.HasPlayers())
					dyn.Cleanup();
			}
		}
	}
}
