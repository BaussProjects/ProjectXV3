//Project by BaussHacker aka. L33TS
using System;

namespace ProjectX_V3_Game.Data.Skills.AdvancedSkills
{
	/// <summary>
	/// Description of BansheeSummon.
	/// </summary>
	[Serializable()]
	public class BansheeSummon : AdvancedSkill
	{
		public BansheeSummon()
		{
			Shake = false;
			Explode = false;
			PlayerExplode = false;
			Dark = true;
			Zoom = false;
			MaxTargets = -1;
			SummonCreatures = true;
			FixCreatureSize = true;
			Creature = Entities.Creatures.IceElf.Create();
			MaxCreatures = -1;
			FixTargets = false;
			MapEffect = "";
			ShowEffectAtPlayers = false;
			ShowEffectAtBoss = false;
			RealSkill = -1;
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
			CoolDown = 90;
		}
		
		static BansheeSummon()
		{
			bansheeSummon = new BansheeSummon();
		}
		
		private static BansheeSummon bansheeSummon;
		public static BansheeSummon Create()
		{
			return (BansheeSummon)bansheeSummon.Copy();
		}
	}
}
