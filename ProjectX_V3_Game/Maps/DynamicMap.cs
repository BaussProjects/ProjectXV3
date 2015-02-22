//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Maps
{
	/// <summary>
	/// A dynamic map.
	/// </summary>
	[Serializable()]
	public class DynamicMap : Map
	{
		/// <summary>
		/// Creates a new instance of DynamicMap.
		/// </summary>
		/// <param name="dynamicid">The dynamic map id.</param>
		/// <param name="original">The map to inherit.</param>
		public DynamicMap(uint dynamicid, Map original)
			: base(60000, string.Empty)
		{
			this.dynamicid = dynamicid;
			this.DMapInfo = original.DMapInfo;
			this.InheritanceMap = original.InheritanceMap;
			if (original.RevivePoint != null)
				this.RevivePoint = original.RevivePoint;
		}
		
		/// <summary>
		/// The dynamic mapid holder.
		/// </summary>
		private uint dynamicid;
		
		/// <summary>
		/// Gets the dynamic map id.
		/// </summary>
		public uint DynamicID
		{
			get { return dynamicid; }
		}
		
		public bool HasHadPlayers = false;
		
		public bool HasPlayers(bool force_check = false)
		{
			if (!HasHadPlayers && !force_check)
				return true;
			
			foreach (IMapObject MapObject in MapObjects.Values)
			{
				if (MapObject is Entities.GameClient)
				{
					if (!(MapObject as Entities.GameClient).IsAIBot)
						return true;
				}
			}
			return false;
		}
		/// <summary>
		/// Cleans the dynamic map up for players. [Teleports them to lastmap]
		/// </summary>
		public void Cleanup()
		{
			foreach (IMapObject MapObject in MapObjects.Values)
			{
				try
				{
					MapObject.DynamicMap = null;
					if (MapObject is Entities.GameClient)
					{
						if (!(MapObject as Entities.GameClient).LeaveDynamicMap())
							(MapObject as Entities.GameClient).NetworkClient.Disconnect("Failed to teleport out of dynamic map.");
					}
				}
				catch { }
			}

			MapObjects.Clear();
			Core.Kernel.DynamicMaps.TryRemove(DynamicID);
		}
	}
}
