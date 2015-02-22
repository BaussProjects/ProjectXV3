//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// Description of BoothItem.
	/// </summary>
	public class BoothItem
	{
		public uint ItemUID;
		public uint Price;
		public bool IsCP;
		public BoothItem(bool cp = false)
		{
			IsCP = cp;
		}
		public ItemInfo GetInfo(Entities.GameClient client)
		{
			if (!client.Inventory.ContainsByUID(ItemUID))
				return null;
			return client.Inventory[ItemUID];
		}
	}
}
