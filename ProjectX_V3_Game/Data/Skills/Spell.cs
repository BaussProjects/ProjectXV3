//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// Magic type spell data.
	/// </summary>
	public class Spell
	{
		public ushort ID;
		public ushort SpellID; // Type
		public string Name;
		public byte Sort;
		public bool Crime;
		public bool Ground;
		public bool Multi;
		public byte Target;
		public byte Level;
		public ushort UseMP;
		public ushort Power;
		public float PowerPercentage;
		public ushort IntoneSpeed;
		public byte Percentage;
		public byte Duration; //StepSecs
		public ushort Range;
		public ushort Distance;
		public ulong Status;
		public uint NeedExp;
		public byte NeedLevel;
		public byte UseXP;
		public ushort WeaponSubtype;
		public byte UseEP;
		public ushort NextMagic;
		public byte UseItem; // 50 = arrow
		public byte UseItemNum;
		public int Sector;
	}
}
