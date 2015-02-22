//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Entities.Creatures
{
	/// <summary>
	/// Description of Demon.
	/// </summary>
	[Serializable()]
	public class IceElf : BossCreature
	{
		private IceElf()
			: base()
		{
			this.Name = "IceElf";
			this.Level = 125;
			this.Mesh = 273;
			this.MinAttack = 4000;
			this.MaxAttack = 4800;
			this.Defense = 5000;
			this.Dexterity = 14;
			this.Dodge = 70;
			this.AttackRange = 3;
			this.ViewRange = 15;
			this.MoveSpeed = 500;
			this.AttackType = 2;
			this.Behaviour = Enums.MonsterBehaviour.Aggresive;
			this.MagicType = 0;
			this.MagicDefense = 500;
			this.MagicHitRate = 0;
			this.ExtraExperience = 10000;
			this.ExtraDamage = 0;
			this.Action = 0;
			this.MaxHP = 25000;
			this.HP = MaxHP;
			this.MaxMP = 0;
			this.MP = 0;
		}
		
		private static IceElf iceelf;
		static IceElf()
		{
			iceelf = new IceElf();
		}
		public static IceElf Create()
		{
			return (IceElf)iceelf.Copy();
		}
	}
}
