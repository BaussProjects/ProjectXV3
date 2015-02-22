//Project by BaussHacker aka. L33TS
using System;

namespace ProjectX_V3_Game.Entities.Bosses
{
	/// <summary>
	/// Description of SnowBanshee.
	/// </summary>
	public class SnowBanshee : BossMonster
	{
		public static SnowBanshee Create()
		{
			SnowBanshee frozenBoss = new SnowBanshee();
			frozenBoss.EntityUID = Core.UIDGenerators.GetMonsterUID();
			return frozenBoss;
		}

		private SnowBanshee()
			: base()
		{
			this.Name = "SnowBanshee";
			this.Level = 135;
			this.Mesh = 951;
			this.MinAttack = 8500;
			this.MaxAttack = 12252;
			this.Defense = 9690;
			this.Dexterity = 20;
			this.Dodge = 91;
			this.AttackRange = 5;
			this.ViewRange = 15;
			this.MoveSpeed = 500;
			this.AttackType = 2;
			this.Behaviour = Enums.MonsterBehaviour.Aggresive;
			this.MagicType = 0;
			this.MagicDefense = 2500;
			this.MagicHitRate = 0;
			this.ExtraExperience = 15000;
			this.ExtraDamage = 0;
			this.Action = 0;
			this.MaxHP = 500000;
			this.HP = MaxHP;
			this.MaxMP = 0;
			this.MP = 0;
			this.BossSkillSpeed = 500;
			
			#region IceBerg
			BossSkills.TryAdd(10929, Data.Skills.AdvancedSkills.IceBerg.Create());
			#endregion
			#region SnowQuake
			BossSkills.TryAdd(9990, Data.Skills.AdvancedSkills.SnowQuake.Create());
			#endregion
			#region BansheeSummon
			BossSkills.TryAdd(7685, Data.Skills.AdvancedSkills.BansheeSummon.Create());
			#endregion
			#region BansheeRage
			Data.Skills.AdvancedSkills.BansheeRage rage = Data.Skills.AdvancedSkills.BansheeRage.Create();
			rage.MapSkill = new ProjectX_V3_Game.Data.Skills.MapSkill();
			rage.MapSkill.Shake = true;
			rage.MapSkill.Dark = true;
			rage.MapSkill.DestructionEffect = "ice03";
			rage.MapSkill.Killer = this;
			rage.MapSkill.PercentTageEffect = 33;
			rage.MapSkill.EffectRatio = 2;
			rage.MapSkill.Range = 25;
			BossAlwaysSkills.TryAdd(7487, rage);
			#endregion
		}
		
		public override void ON_TELEPORTED()
		{
			BossAlwaysSkills[7487].MapSkill.map = this.Map;
		}
		
		public override void ON_DEATH(GameClient Killer)
		{
//			if (Killer.Battle is Data.FrozenGrotto)
//			{
//				Data.FrozenGrotto grotto = Killer.Battle as Data.FrozenGrotto;
//				grotto.DefeatedThird = true;
//				grotto.SendTeamMessage("Banshee has been killed, hurray!!!");
//				using (var weather = new Packets.WeatherPacket())
//				{
//					weather.Weather = Enums.Weather.Nothing;
//					
//					foreach (Entities.GameClient member in grotto.Clients.Values)
//					{
//						member.Send(weather);
//					}
//				}
//				
//				Data.ItemInfo item;
//				if (Core.Kernel.ItemInfos.TrySelect(1088000, out item))
//				{
//					for (int i = 0; i < ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(5, 15); i++)
//					{
//						item = item.Copy();
//						item.Drop(Map, X, Y);
//					}
//				}
//				Data.ItemInfo item2;
//				if (Core.Kernel.ItemInfos.TrySelect(1088001, out item2))
//				{
//					for (int i = 0; i < ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(10, 15); i++)
//					{
//						item2 = item2.Copy();
//						item2.Drop(Map, X, Y);
//					}
//				}
//				
//				grotto.Boss4.AbortBoss(true);
//				ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(grotto.Finish, 25000, 0);
//			}
		}
	}
}
