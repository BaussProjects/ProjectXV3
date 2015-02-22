//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Data.Skills.AdvancedSkills
{
	/// <summary>
	/// Description of EarthQuake.
	/// </summary>
	[Serializable()]
	public class SnowQuake : AdvancedSkill
	{
		private SnowQuake()
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
			RealSkill = 5010;
			RealSkilllevel = 9;
			PercentTageEffect = 25;
			DamageEffect = 0;
			Freeze = false;
			FreezeTime = 0;
			Paralyzed = false;
			ParalyzeTime = 0;
			SpreadEffect = "";
			EffectPos = null;
			SpreadSkill = false;
			SkillPos = null;
			CoolDown = 41;
		}
		
		static SnowQuake()
		{
			snowQuake = new SnowQuake();
		}
		
		private static SnowQuake snowQuake;
		public static SnowQuake Create()
		{
			return (SnowQuake)snowQuake.Copy();
		}
	}
}
