//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// A shop of items and other stuff.
	/// </summary>
	public class Shop
	{
		/// <summary>
		/// Creates a new instance of Shop.
		/// </summary>
		/// <param name="npcid">The npcid associated with the shop. This is also the shop id.</param>
		/// <param name="amount">The amount of items in the shop.</param>
		/// <param name="shoptype">The type of shop.</param>
		public Shop(uint npcid, int amount, Enums.ShopType shoptype)
		{
			ShopItems = new uint[amount];
			ShopType = shoptype;
			this.npcid = npcid;
		}
		
		/// <summary>
		/// The npc id holder.
		/// </summary>
		private uint npcid;
		
		/// <summary>
		/// Gets the shop id.
		/// </summary>
		public uint ShopID
		{
			get { return npcid; }
		}
		
		/// <summary>
		/// Gets the npc associated with the shop.
		/// </summary>
		public Entities.NPC AssociatedNPC
		{
			get { return Core.Kernel.NPCs[npcid]; }
		}
		
		/// <summary>
		/// Gets the shop items.
		/// </summary>
		public readonly uint[] ShopItems;
		
		/// <summary>
		/// Gets the shoptype.
		/// </summary>
		public readonly Enums.ShopType ShopType;
	}
}
