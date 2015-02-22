//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;

namespace ProjectX_V3_Game.Packets.Interaction.Battle.Skills
{
	/// <summary>
	/// Description of NinjaSkills.
	/// </summary>
	public class NinjaSkills
	{
		/// <summary>
		/// Handles the poison skill.
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="targets">The targets. [not set yet]</param>
		/// <param name="interaction">The interaction packet.</param>
		/// <param name="usespell">The usespell packet.</param>
		/// <param name="spell">The spell.</param>
		/// <param name="damage">The damage.</param>
		/// <returns>Returns true if the skill was used successfully.</returns>
		public static bool HandlePoison(Entities.IEntity attacker, ConcurrentBag<Entities.IEntity> targets, InteractionPacket interaction, UseSpellPacket usespell, Data.Spell spell, out uint damage)
		{
			damage = 0;
			bool self = (interaction.TargetUID == attacker.EntityUID);
			if (self)
				return false;
			
			if (Core.Screen.GetDistance(attacker.X, attacker.Y, interaction.X, interaction.Y) >= 9)
				return false;
			
			/*Calculations.InLineAlgorithm ila = new ProjectX_V3_Game.Calculations.InLineAlgorithm(
				attacker.X, interaction.X, attacker.Y, interaction.Y,
				(byte)spell.Range, Calculations.InLineAlgorithm.Algorithm.DDA);*/
			
			Calculations.Sector sector = new ProjectX_V3_Game.Calculations.Sector(interaction.X, interaction.Y, interaction.X, interaction.Y);
			sector.Arrange(spell.Sector, spell.Range);
			
			byte count = 0;
			bool bluename = false;
			foreach (Maps.IMapObject obj in attacker.Screen.MapObjects.Values)
			{
				if (count > 28)
					return true;
				
				if(!(obj as Entities.IEntity).Alive)
					continue;
				
				if (obj is Entities.NPC)
					continue;
				
				if (obj.EntityUID == attacker.EntityUID)
					continue;
				
				if (obj is Entities.BossMonster)
				{
					if (!(obj as Entities.BossMonster).CanBeAttacked)
						continue;
				}
				
				if (obj is Entities.Monster)
				{
					if (((byte)(obj as Entities.Monster).Behaviour) >= 3)
						continue;
				}
				
				if (obj is Entities.GameClient)
				{
					if (!(obj as Entities.GameClient).LoggedIn)
						continue;
					
					if ((obj as Entities.GameClient).ContainsFlag1(Enums.Effect1.Fly))
						continue;
					
					if (obj.Map.MapType == Enums.MapType.NoPK && (attacker is Entities.GameClient))
						continue;
					
					if (attacker is Entities.GameClient)
					{
						if ((attacker as Entities.GameClient).PKMode != Enums.PKMode.PK)
							continue;
						
						if (!Combat.FixTarget(attacker, obj as Entities.IEntity))
							continue;
					}
					
					if (!(DateTime.Now >= (obj as Entities.GameClient).LoginProtection.AddSeconds(10)))
					{
						continue;
					}
					
					if (!(DateTime.Now >= (obj as Entities.GameClient).ReviveProtection.AddSeconds(5)))
					{
						continue;
					}
					
					if (!Combat.FixTarget(attacker, (obj as Entities.GameClient)))
						continue;
					
					if ((obj as Entities.GameClient).ContainsFlag1(Enums.Effect1.BlueName))
						bluename = true;
				}
				
				//if (!ila.InLine(obj.X, obj.Y))
				//	continue;
				
				if (!sector.Inside(obj.X, obj.Y))
					continue;
				
				Entities.IEntity targetentity = obj as Entities.IEntity;
				if (targetentity is Entities.Monster)
				{
					if ((targetentity as Entities.Monster).ContainsFlag1(Enums.Effect1.Poisoned))
						continue;
				}
				else if (targetentity is Entities.GameClient)
				{
					if ((targetentity as Entities.GameClient).ContainsFlag1(Enums.Effect1.Poisoned))
						continue;
				}
				
				damage = (uint)(targetentity.HP / 10);//Calculations.Battle.GetPhysicalDamage(attacker, targetentity);
				Combat.ProcessDamage(attacker, targetentity, ref damage, false);
				if (damage > 0)
				{
					if (targetentity is Entities.Monster)
					{
						if ((targetentity as Entities.Monster).ContainsFlag1(Enums.Effect1.Poisoned))
							continue;
						
						(targetentity as Entities.Monster).PoisonEffect = (ushort)(spell.Power - 30000);
						(targetentity as Entities.Monster).AddStatusEffect1(Enums.Effect1.Poisoned, 60000);
					}
					else if (targetentity is Entities.GameClient)
					{
						(targetentity as Entities.GameClient).PoisonEffect = (ushort)(spell.Power - 30000);
						(targetentity as Entities.GameClient).AddStatusEffect1(Enums.Effect1.Poisoned, 60000);
					}
					if (!(targetentity is Entities.GameClient))
					{
						uint exp = (uint)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)(damage / 2), (int)damage);
						if (targetentity.Level > (attacker.Level + 10))
							exp *= 2;
						if ((attacker is Entities.GameClient))
							(attacker as Entities.GameClient).AddSpellExp(spell.SpellID, exp);
					}
					usespell.AddTarget(targetentity.EntityUID, damage);
					targets.Add(targetentity);
				}
				count++;
			}
			if (attacker is Entities.GameClient && bluename)
				(attacker as Entities.GameClient).AddStatusEffect1(Enums.Effect1.BlueName, 10000);
			return true;
		}

		/// <summary>
		/// Handles the twofold attack skills.
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="target">The target.</param>
		/// <param name="interaction">The interaction packet.</param>
		/// <param name="usespell">The usespell packet.</param>
		/// <param name="spell">The spell.</param>
		/// <param name="damage">The damage.</param>
		/// <returns>Returns true if the skill was used successfully.</returns>
		public static bool HandleTwoFold(Entities.IEntity attacker, Entities.IEntity target, InteractionPacket interaction, UseSpellPacket usespell, Data.Spell spell, out uint damage)
		{
			damage = 0;
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
			
			if (Core.Screen.GetDistance(target.X, target.Y, attacker.X, attacker.Y) > spell.Distance)
				return false;
			
			
			
			if (target is Entities.GameClient)
			{
				if (!(target as Entities.GameClient).LoggedIn)
					return false;
				if ((target as Entities.GameClient).ContainsFlag1(Enums.Effect1.Fly))
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
			
			damage = Calculations.Battle.GetPhysicalMagicDamage(attacker, target, spell);
			damage = (uint)((damage / 100) * (spell.Power - 30000));
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
		
		/// <summary>
		/// Handles the poisonstar attack skills.
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="target">The target.</param>
		/// <param name="interaction">The interaction packet.</param>
		/// <param name="usespell">The usespell packet.</param>
		/// <param name="spell">The spell.</param>
		/// <param name="damage">The damage.</param>
		/// <returns>Returns true if the skill was used successfully.</returns>
		public static bool HandlePoisonStar(Entities.IEntity attacker, Entities.IEntity target, InteractionPacket interaction, UseSpellPacket usespell, Data.Spell spell, out uint damage)
		{
			damage = 0;
			if (attacker.EntityUID == target.EntityUID)
				return false;
			if (!(target is Entities.GameClient))
				return false;
			if ((target as Entities.GameClient).ContainsFlag1(Enums.Effect1.Fly))
				return false;
			
			if (Core.Screen.GetDistance(target.X, target.Y, attacker.X, attacker.Y) > spell.Distance)
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
			
			(target as Entities.GameClient).AddStatusEffect1(Enums.Effect1.NoPotion, (5000 * (spell.Level + 1)));
			
			uint exp = (uint)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)target.Level, (int)target.Level * 2);
			if ((attacker is Entities.GameClient))
				(attacker as Entities.GameClient).AddSpellExp(spell.SpellID, exp);
			
			usespell.AddTarget(target.EntityUID, 0);
			
			return true;
		}
		
		/// <summary>
		/// Handles the archerbane attack skills.
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="target">The target.</param>
		/// <param name="interaction">The interaction packet.</param>
		/// <param name="usespell">The usespell packet.</param>
		/// <param name="spell">The spell.</param>
		/// <param name="damage">The damage.</param>
		/// <returns>Returns true if the skill was used successfully.</returns>
		public static bool HandleArcherBane(Entities.IEntity attacker, Entities.IEntity target, InteractionPacket interaction, UseSpellPacket usespell, Data.Spell spell, out uint damage)
		{
			damage = 0;
			if (attacker.EntityUID == target.EntityUID)
				return false;
			if (!(target is Entities.GameClient))
				return false;
			if (!(target as Entities.GameClient).ContainsFlag1(Enums.Effect1.Fly))
				return false;					
			
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
			
			(target as Entities.GameClient).RemoveFlag1(Enums.Effect1.Fly);
			
			//damage = Calculations.Battle.GetPhysicalMagicDamage(attacker, target, spell);
			damage = (uint)(target.HP / 10);
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
			}
				usespell.AddTarget(target.EntityUID, damage);
			return true;
		}
		
	}
}
