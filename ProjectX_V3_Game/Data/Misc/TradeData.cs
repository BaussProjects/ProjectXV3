//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Generic;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// The trade data associated with a client.
	/// </summary>
	public class TradeData
	{
		/// <summary>
		/// The trade partner.
		/// </summary>
		public Entities.GameClient Partner;
		
		/// <summary>
		/// Trade items.
		/// </summary>
		public List<ItemInfo> Items = new List<ItemInfo>();
		
		/// <summary>
		/// Boolean defining whether the trade has been confirmed or not.
		/// </summary>
		public bool Confirmed = false;
		
		/// <summary>
		/// Boolean defining whether the trade has been accepted or not.
		/// </summary>
		public bool Accepted = false;
		
		/// <summary>
		/// Boolean defining whether the trade is in process.
		/// </summary>
		public bool Trading = false;
		
		/// <summary>
		/// Trade money.
		/// </summary>
		public uint Money = 0;
		
		/// <summary>
		/// Trade cps.
		/// </summary>
		public uint CPs = 0;
		
		/// <summary>
		/// Boolean defining whether the trade is in request or not.
		/// </summary>
		public bool Requesting = false;
		
		/// <summary>
		/// Boolean defining whether the trade window is open or not.
		/// </summary>
		public bool WindowOpen = false;
		
		/// <summary>
		/// Gets the partner items.
		/// </summary>
		public List<ItemInfo> PartnerItems
		{
			get { return Partner.Trade.Items; }
		}
		
		/// <summary>
		/// Gets a boolean defining whether the partner has confirmed or not.
		/// </summary>
		public bool PartnerConfirmed
		{
			get { return Partner.Trade.Confirmed; }
		}
		/// <summary>
		/// Gets a boolean defining whether the partner has accepted or not.
		/// </summary>
		public bool PartnerAccepted
		{
			get { return Partner.Trade.Accepted; }
		}
		
		/// <summary>
		/// Gets a boolean defining whether the partner is trading or not.
		/// </summary>
		public bool PartnerTrading
		{
			get { return Partner.Trade.Trading; }
		}
		
		/// <summary>
		/// Gets the partner money.
		/// </summary>
		public uint PartnerMoney
		{
			get { return Partner.Trade.Money; }
		}
		
		/// <summary>
		/// Gets the partner cps.
		/// </summary>
		public uint PartnerCPs
		{
			get { return Partner.Trade.CPs; }
		}
		
		/// <summary>
		/// Gets a boolean defining whether the partner is under request or not.
		/// </summary>
		public bool PartnerRequesting
		{
			get { return Partner.Trade.Requesting; }
		}
		
		/// <summary>
		/// Gets a boolean defining whether the partner has the window open or not.
		/// </summary>
		public bool PartnerWindowOpen
		{
			get { return Partner.Trade.WindowOpen; }
		}
		
		/// <summary>
		/// Begins the chat + resets.
		/// </summary>
		/// <param name="partner">The trade partner.</param>
		public void Begin(Entities.GameClient partner)
		{
			if (Items.Count > 0)
				Items.Clear();
			
			Items = new List<ItemInfo>();
			Money = 0;
			CPs = 0;
			Trading = true;
			Accepted = false;
			Confirmed = false;
			WindowOpen = false;
			Partner = partner;
		}
		
		/// <summary>
		/// Does a full reset of the trade.
		/// </summary>
		public void Reset()
		{
			if (Items.Count > 0)
				Items.Clear();
			
			Items = new List<ItemInfo>();
			Money = 0;
			CPs = 0;
			Trading = false;
			Accepted = false;
			Confirmed = false;
			Requesting = false;
			WindowOpen = false;
			Partner = null;
		}
	}
}
