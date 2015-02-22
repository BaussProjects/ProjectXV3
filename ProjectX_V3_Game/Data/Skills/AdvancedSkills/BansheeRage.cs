//Project by BaussHacker aka. L33TS
using System;

namespace ProjectX_V3_Game.Data.Skills.AdvancedSkills
{
	/// <summary>
	/// Description of BansheeRage.
	/// </summary>
	[Serializable()]
	public class BansheeRage : AdvancedSkill
	{
		private BansheeRage()
		{
			Shake = true;
			Explode = false;
			PlayerExplode = false;
			Dark = true;
			Zoom = false;
			MaxTargets = -1;
			SummonCreatures = false;
			FixCreatureSize = false;
			Creature = null;
			MaxCreatures = -1;
			FixTargets = false;
			MapEffect = "";
			ShowEffectAtPlayers = false;
			ShowEffectAtBoss = false;
			RealSkill = 30011;
			RealSkilllevel = 0;
			PercentTageEffect = -1;
			DamageEffect = 0;
			Freeze = false;
			FreezeTime = 0;
			Paralyzed = false;
			ParalyzeTime = 0;
			SpreadEffect = "";
			EffectPos = null;
			SpreadSkill = false;
			SkillPos = null;
			CoolDown = 30;
		}
		
		static BansheeRage()
		{
			bansheeRage = new BansheeRage();
		}
		
		private static BansheeRage bansheeRage;
		public static BansheeRage Create()
		{
			return (BansheeRage)bansheeRage.Copy();
		}
	}
}
