//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// Description of Booth.
	/// </summary>
	public class Booth
	{
		public Booth()
		{
			BoothItems = new ConcurrentDictionary<uint, BoothItem>();
		}
		
		public ConcurrentDictionary<uint, BoothItem> BoothItems;
		
		public Entities.NPC Carpet;
		public Entities.GameClient ShopOwner;
		
		public string HawkMessage = "";
		
		public void CreateBooth(Packets.GeneralDataPacket data)
		{
			foreach (Maps.IMapObject mapcarpet in ShopOwner.Screen.MapObjects.Values)
			{
				if (!(mapcarpet is Entities.NPC))
					return;
				Entities.NPC carpet = (mapcarpet as Entities.NPC);
				if (carpet.NPCType == Enums.NPCType.ShopFlag)
				{
					if (!carpet.IsTakenBooth)
					{
						if (carpet.X == (ShopOwner.X - 2) && carpet.Y == ShopOwner.Y)
						{
							Carpet = carpet;
							Carpet.IsTakenBooth = true;
							break;
						}
					}
				}
			}
			if (Carpet == null) // could not find a carpet ...
			{
				ShopOwner.Booth = null;
				ShopOwner = null;
				return;
			}
			Carpet.StartVending(ShopOwner.Name, this);
			Carpet.Screen.UpdateScreen(null);
			data.Data1 = Carpet.EntityUID;
			data.Id = ShopOwner.EntityUID;
			ShopOwner.Send(data);
			ShopOwner.Screen.FullUpdate();
		}
		public void CancelBooth()
		{
			BoothItems.Clear();
			
			Carpet.StopVending();
			Carpet.Name = "";
			Carpet.Screen.FullUpdate();
			Carpet = null;
			
			ShopOwner.Booth = null;
			ShopOwner.Screen.FullUpdate();
			ShopOwner = null;
		}
		
		public static Booth FindBooth(uint itemUID)
		{
			foreach (Entities.GameClient client in Core.Kernel.Clients.selectorCollection1.Values)
			{
				if (client.Booth == null)
					continue;
				if (client.Booth.ShopOwner == null)
					continue;
				
				if (client.Booth.BoothItems.ContainsKey(itemUID))
				{
					return client.Booth;
				}
			}
			return null;
		}
		
		public static Booth FindBoothFromNPC(uint EntityUID)
		{
			foreach (Entities.NPC npc in Core.Kernel.NPCs.Values)
			{
				if (!npc.IsTakenBooth)
					continue;
				
				if (npc.Booth == null)
					continue;
				
				if (npc.Booth.ShopOwner == null)
					continue;
				
				if (npc.EntityUID == EntityUID)
					return npc.Booth;
			}
			return null;
		}
	}
}
