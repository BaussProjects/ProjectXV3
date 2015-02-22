//Project by BaussHacker aka. L33TS
using System;

namespace ProjectX_V3_Game.Data.Skills.AdvancedSkills
{
	/// <summary>
	/// Description of IceBerg.
	/// </summary>
	[Serializable()]
	public class IceBerg : AdvancedSkill
	{
		private IceBerg()
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
			Freeze = true;
			FreezeTime = 5000;
			Paralyzed = false;
			ParalyzeTime = 0;
			SpreadEffect = "";
			EffectPos = null;
			SpreadSkill = false;
			SkillPos = null;
			CoolDown = 30;
		}
		
		static IceBerg()
		{
			iceBerg = new IceBerg();
		}
		
		private static IceBerg iceBerg;
		public static IceBerg Create()
		{
			return (IceBerg)iceBerg.Copy();
		}
	}
}
