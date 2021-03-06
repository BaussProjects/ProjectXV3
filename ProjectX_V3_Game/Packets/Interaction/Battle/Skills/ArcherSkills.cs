﻿//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Interaction.Battle.Skills
{
	/// <summary>
	/// Description of ArcherSkills.
	/// </summary>
	public class ArcherSkills
	{
		/// <summary>
		/// Handles the single attack skills.
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="target">The target.</param>
		/// <param name="interaction">The interaction packet.</param>
		/// <param name="usespell">The usespell packet.</param>
		/// <param name="spell">The spell.</param>
		/// <param name="damage">The damage.</param>
		/// <returns>Returns true if the skill was used successfully.</returns>
		public static bool Handle(Entities.IEntity attacker, Entities.IEntity target, InteractionPacket interaction, UseSpellPacket usespell, Data.Spell spell, out uint damage)
		{
			damage = 0;
			if (attacker is Entities.GameClient)
			{
				if (!Ranged.ProcessClient(attacker as Entities.GameClient, interaction, true))
				{
					return false;
				}
			}
			if (attacker.EntityUID == target.EntityUID)
				return false;
			if (target is Entities.NPC)
				return false;
			
			if (target is Entities.BossMonster)
			{
				if (!(target as Entities.BossMonster).CanBeAttacked)
					return false;
			}
			
			if (target is Entities.Monster)
			{
				if (((byte)(target as Entities.Monster).Behaviour) >= 3)
					return false;
			}
			
			if (target is Entities.GameClient)
			{
				if (!(target as Entities.GameClient).LoggedIn)
					return false;
				
				if (target.Map.MapType == Enums.MapType.NoPK && (attacker is Entities.GameClient))
					return false;
				
				if (!(DateTime.Now >= (target as Entities.GameClient).LoginProtection.AddSeconds(10)))
				{
					return false;
				}
				
				if (!(DateTime.Now >= (target as Entities.GameClient).ReviveProtection.AddSeconds(5)))
				{
					return false;
				}
				
				if (!Combat.FixTarget(attacker, target))
					return false;
			}
			
			if (attacker is Entities.GameClient)
			{
				Ranged.DecreaseArrows(attacker as Entities.GameClient, 1);
			}
			damage = Calculations.Battle.GetRangedDamage(attacker, target);
			damage = (uint)(damage * 1.5);
			Combat.ProcessDamage(attacker, target, ref damage, false);
			if (damage > 0)
			{
				if (!(target is Entities.GameClient))
				{
					uint exp = (uint)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)(damage / 2), (int)damage);
					if (target.Level > (attacker.Level + 10))
						exp *= 2;
					if ((attacker is Entities.GameClient))
						(attacker as Entities.GameClient).AddSpellExp(spell.SpellID, exp);
				}
				usespell.AddTarget(target.EntityUID, damage);
			}
			return true;
		}
	}
}
