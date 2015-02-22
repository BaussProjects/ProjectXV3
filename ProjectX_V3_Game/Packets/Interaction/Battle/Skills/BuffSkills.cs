//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;

namespace ProjectX_V3_Game.Packets.Interaction.Battle.Skills
{
	/// <summary>
	/// Description of BuffSkills.
	/// </summary>
	public class BuffSkills
	{
		public static bool Handle(Entities.IEntity attacker, Entities.IEntity target, InteractionPacket interaction, UseSpellPacket usespell, Data.Spell spell, out uint damage)
		{
			damage = 0;
			if (!attacker.Alive)
				return false;
			if (target == null)
				return false;
			if (!target.Alive)
				return false;
			if (!(target is Entities.GameClient))
				return false;
			if (attacker is Entities.GameClient)
			{
				Entities.GameClient client = (attacker as Entities.GameClient);
				if (!(DateTime.Now >= client.LastSmallLongSkill.AddMilliseconds(Core.TimeIntervals.SmallLongSkillInterval)) && client.AttackPacket == null)
				{
					using (var fmsg = Packets.Message.MessageCore.CreateSystem2(client.Name, Core.MessageConst.REST))
						client.Send(fmsg);
					return false;
				}
				client.LastSmallLongSkill = DateTime.Now;
			}
			
			Entities.GameClient TargetClient = (target as Entities.GameClient);
			if (!TargetClient.LoggedIn)
				return false;
			
			switch (spell.SpellID)
			{
				case 1075:
					TargetClient.AddStatusEffect1(Enums.Effect1.PartiallyInvisible, spell.Duration * 1000);
					break;
				case 1085:
					TargetClient.AddStatusEffect1(Enums.Effect1.StarOfAccuracy, spell.Duration * 1000);
					break;
				case 1090:
					TargetClient.AddStatusEffect1(Enums.Effect1.Shield, spell.Duration * 1000);
					break;
				case 1095:
					TargetClient.AddStatusEffect1(Enums.Effect1.Stig, spell.Duration * 1000);
					break;
					
				default:
					return false;
			}
			byte level_target = (byte)(TargetClient.Level > 50 ? 50 : TargetClient.Level);
			uint exp = (uint)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)level_target, (int)level_target * 2);

			if ((attacker is Entities.GameClient))
				(attacker as Entities.GameClient).AddSpellExp(spell.SpellID, exp);
			
			usespell.AddTarget(TargetClient.EntityUID, damage);
			
			return true;
		}
	}
}
