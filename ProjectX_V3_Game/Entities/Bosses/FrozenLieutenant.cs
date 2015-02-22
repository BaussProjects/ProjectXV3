//Project by BaussHacker aka. L33TS
using System;

namespace ProjectX_V3_Game.Entities.Bosses
{
	/// <summary>
	/// Description of FrozenLieutenant.
	/// </summary>
	public class FrozenLieutenant : BossMonster
	{ //329

		public static FrozenLieutenant Create()
		{
			FrozenLieutenant frozenBoss = new FrozenLieutenant();
			frozenBoss.EntityUID = Core.UIDGenerators.GetMonsterUID();
			return frozenBoss;
		}

		private FrozenLieutenant()
			: base()
		{
			this.Name = "FrozenLieutenant";
			this.Level = 127;
			this.Mesh = 329;
			this.MinAttack = 4800;
			this.MaxAttack = 5600;
			this.Defense = 6400;
			this.Dexterity = 20;
			this.Dodge = 72;
			this.AttackRange = 5;
			this.ViewRange = 15;
			this.MoveSpeed = 500;
			this.AttackType = 2;
			this.Behaviour = Enums.MonsterBehaviour.Aggresive;
			this.MagicType = 0;
			this.MagicDefense = 1800;
			this.MagicHitRate = 0;
			this.ExtraExperience = 15000;
			this.ExtraDamage = 0;
			this.Action = 0;
			this.MaxHP = 100000;
			this.HP = MaxHP;
			this.MaxMP = 0;
			this.MP = 0;
			this.BossSkillSpeed = 500;
			
			#region IceBerg
			BossSkills.TryAdd(10929, Data.Skills.AdvancedSkills.IceBerg.Create());
			#endregion
		}
		
		public override void ON_TELEPORTED()
		{
			
		}
		
		public override void ON_DEATH(GameClient Killer)
		{
//			if (Killer.Battle is Data.FrozenGrotto)
//			{
//				Data.FrozenGrotto grotto = Killer.Battle as Data.FrozenGrotto;
//				grotto.DefeatedFirst = true;
//				grotto.SendTeamMessage("The first boss has been killed. There is still 2 remaining.");
//				grotto.UpdateScore();
//				grotto.inBossArea = false;
//				
//				Data.ItemInfo item;
//				if (Core.Kernel.ItemInfos.TrySelect(1088000, out item))
//				{
//					for (int i = 0; i < ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(1, 4); i++)
//					{
//						item = item.Copy();
//						item.Drop(Map, X, Y);
//					}
//				}
//				Data.ItemInfo item2;
//				if (Core.Kernel.ItemInfos.TrySelect(1088001, out item2))
//				{
//					for (int i = 0; i < ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(3, 7); i++)
//					{
//						item2 = item2.Copy();
//						item2.Drop(Map, X, Y);
//					}
//				}
//				
//				grotto.Boss1.AbortBoss(true);
//				grotto.Boss1 = null;
//			}
		}
	}
}
