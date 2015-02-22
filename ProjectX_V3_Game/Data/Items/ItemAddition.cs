//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// The item additions for + etc.
	/// </summary>
	public class ItemAddition
	{
		/// <summary>
		/// The item ID.
		/// </summary>
		public uint ItemID;
		
		/// <summary>
		/// The plus.
		/// </summary>
		public byte Plus;
		
		/// <summary>
		/// The HP.
		/// </summary>
		public ushort HP;
		
		/// <summary>
		/// The minimum attack.
		/// </summary>
		public uint MinAttack;
		
		/// <summary>
		/// The maximum attack.
		/// </summary>
		public uint MaxAttack;
		
		/// <summary>
		/// The defense.
		/// </summary>
		public ushort Defense;
		
		/// <summary>
		/// The magic attack.
		/// </summary>
		public ushort MagicAttack;
		
		/// <summary>
		/// The magic defense.
		/// </summary>
		public ushort MagicDefense;
		
		/// <summary>
		/// The dexterity.
		/// </summary>
		public ushort Dexterity;
		
		/// <summary>
		/// The dodge.
		/// </summary>
		public byte Dodge;
	}
}
