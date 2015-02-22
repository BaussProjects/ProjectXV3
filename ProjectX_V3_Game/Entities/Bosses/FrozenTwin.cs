//Project by BaussHacker aka. L33TS
using System;

namespace ProjectX_V3_Game.Entities.Bosses
{
	/// <summary>
	/// Description of FrozenTwin.
	/// </summary>
	public class FrozenTwin : BossMonster
	{
		public static FrozenTwin Create(string Name, FrozenTwin Controller)
		{
			FrozenTwin frozenBoss = new FrozenTwin(Controller == null);
			if (Controller != null)
			{
				frozenBoss.ControllerTwin = Controller;
				Controller.ControllerTwin = frozenBoss;
			}
			frozenBoss.Name += Name;
			frozenBoss.EntityUID = Core.UIDGenerators.GetMonsterUID();
			return frozenBoss;
		}

		private FrozenTwin ControllerTwin;
		
		private FrozenTwin(bool CanFreeze)
			: base()
		{
			this.Name = "Twin";
			this.Level = 130;
			this.Mesh = 153;
			this.MinAttack = 5600;
			this.MaxAttack = 7000;
			this.Defense = 8500;
			this.Dexterity = 20;
			this.Dodge = 79;
			this.AttackRange = 5;
			this.ViewRange = 15;
			this.MoveSpeed = 500;
			this.AttackType = 2;
			this.Behaviour = Enums.MonsterBehaviour.Aggresive;
			this.MagicType = 0;
			this.MagicDefense = 2300;
			this.MagicHitRate = 0;
			this.ExtraExperience = 15000;
			this.ExtraDamage = 0;
			this.Action = 0;
			this.MaxHP = 150000;
			this.HP = MaxHP;
			this.MaxMP = 0;
			this.MP = 0;
			this.BossSkillSpeed = 500;
			
			if (CanFreeze)
			{
				#region IceBerg
				BossSkills.TryAdd(10929, Data.Skills.AdvancedSkills.IceBerg.Create());
				#endregion
			}
			else
			{
				#region SnowQuake
				BossSkills.TryAdd(9990, Data.Skills.AdvancedSkills.SnowQuake.Create());
				#endregion
			}
		}
		
		public override void ON_TELEPORTED()
		{
			
		}
		
		public override void ON_DEATH(GameClient Killer)
		{
//			if (Killer.Battle is Data.FrozenGrotto)
//			{
//				if (!ControllerTwin.Alive)
//				{
//					Data.FrozenGrotto grotto = Killer.Battle as Data.FrozenGrotto;
//					grotto.DefeatedSecond = true;
//					grotto.SendTeamMessage("The twins has been killed. There is still 1 remaining.");
//					grotto.UpdateScore();
//					grotto.inBossArea = false;
//					
//					using (var weather = new Packets.WeatherPacket())
//					{
//						weather.Weather = Enums.Weather.Nothing;
//						
//						foreach (Entities.GameClient member in grotto.Clients.Values)
//						{
//							member.Send(weather);
//						}
//					}
//					
//					Data.ItemInfo item;
//					if (Core.Kernel.ItemInfos.TrySelect(1088000, out item))
//					{
//						for (int i = 0; i < ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(2, 8); i++)
//						{
//							item = item.Copy();
//							item.Drop(Map, X, Y);
//						}
//					}
//					Data.ItemInfo item2;
//					if (Core.Kernel.ItemInfos.TrySelect(1088001, out item2))
//					{
//						for (int i = 0; i < ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(5, 12); i++)
//						{
//							item2 = item2.Copy();
//							item2.Drop(Map, X, Y);
//						}
//					}
//					
//					grotto.Boss2.AbortBoss(true);
//					grotto.Boss3.AbortBoss(true);
//					grotto.Boss2 = null;
//					grotto.Boss3 = null;
//				}
//			}
		}
	}
}
