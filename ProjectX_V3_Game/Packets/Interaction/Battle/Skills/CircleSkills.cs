//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;

namespace ProjectX_V3_Game.Packets.Interaction.Battle.Skills
{
	/// <summary>
	/// Description of CircleSkills.
	/// </summary>
	public class CircleSkills
	{
		/// <summary>
		/// Handles the circle skills.
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="targets">The targets. [not set yet]</param>
		/// <param name="interaction">The interaction packet.</param>
		/// <param name="usespell">The usespell packet.</param>
		/// <param name="spell">The spell.</param>
		/// <param name="damage">The damage.</param>
		/// <returns>Returns true if the skill was used successfully.</returns>
		public static bool HandlePhy(Entities.IEntity attacker, ConcurrentBag<Entities.IEntity> targets, InteractionPacket interaction, UseSpellPacket usespell, Data.Spell spell, out uint damage)
		{
			damage = 0;
			bool self = (interaction.TargetUID == attacker.EntityUID);
			if (self)
				return false;
			if (attacker is Entities.GameClient)
			{
				Entities.GameClient client = (attacker as Entities.GameClient);
				if (!(DateTime.Now >= client.LastLongSkill.AddMilliseconds(Core.TimeIntervals.LongSkillInterval)) && client.AttackPacket == null)
				{
					using (var fmsg = Packets.Message.MessageCore.CreateSystem2(client.Name, Core.MessageConst.REST))
						client.Send(fmsg);
					return false;
				}
				client.LastLongSkill = DateTime.Now;
			}
			
			/*Calculations.InLineAlgorithm ila = new ProjectX_V3_Game.Calculations.InLineAlgorithm(
				attacker.X, interaction.X, attacker.Y, interaction.Y,
				(byte)spell.Range, Calculations.InLineAlgorithm.Algorithm.DDA);*/
			
			//	Calculations.Sector sector = new ProjectX_V3_Game.Calculations.Sector(attacker.X, attacker.Y, interaction.X, interaction.Y);
			//	sector.Arrange(spell.Sector, spell.Range);
			
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
				
				if (Core.Screen.GetDistance(obj.X, obj.Y, attacker.X, attacker.Y) >= 8)
					continue;
				
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
				
				//if (!sector.Inside(obj.X, obj.Y))
				//	continue;
				
				Entities.IEntity targetentity = obj as Entities.IEntity;
				damage = Calculations.Battle.GetPhysicalDamage(attacker, targetentity);
				Combat.ProcessDamage(attacker, targetentity, ref damage, false);
				if (damage > 0)
				{
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
		/// Handles the circle skills.
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="targets">The targets. [not set yet]</param>
		/// <param name="interaction">The interaction packet.</param>
		/// <param name="usespell">The usespell packet.</param>
		/// <param name="spell">The spell.</param>
		/// <param name="damage">The damage.</param>
		/// <returns>Returns true if the skill was used successfully.</returns>
		public static bool HandleMag(Entities.IEntity attacker, ConcurrentBag<Entities.IEntity> targets, InteractionPacket interaction, UseSpellPacket usespell, Data.Spell spell, out uint damage)
		{
			damage = 0;

			if (attacker is Entities.GameClient)
			{
				if (spell.SpellID == 8030 || spell.SpellID == 10308 || spell.SpellID == 7013 || spell.SpellID > 10360)
				{
					if (!Ranged.ProcessClient(attacker as Entities.GameClient, interaction, true, 3))
					{
						return false;
					}
				}
			}
			
			/*Calculations.InLineAlgorithm ila = new ProjectX_V3_Game.Calculations.InLineAlgorithm(
				attacker.X, interaction.X, attacker.Y, interaction.Y,
				(byte)spell.Range, Calculations.InLineAlgorithm.Algorithm.DDA);*/
			
			//	Calculations.Sector sector = new ProjectX_V3_Game.Calculations.Sector(attacker.X, attacker.Y, interaction.X, interaction.Y);
			//	sector.Arrange(spell.Sector, spell.Range);
			
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
				
				if (!Core.Screen.ValidDistance(obj.X, obj.Y, attacker.X, attacker.Y))
					continue;
				
				if (obj is Entities.GameClient)
				{
					if (!(obj as Entities.GameClient).LoggedIn)
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
					if ((obj as Entities.GameClient).ContainsFlag1(Enums.Effect1.Fly))
						continue;
					
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
				
				//if (!sector.Inside(obj.X, obj.Y))
				//	continue;
				
				Entities.IEntity targetentity = obj as Entities.IEntity;
				if (spell.SpellID == 8030 || spell.SpellID == 10308 || spell.SpellID == 7013 || spell.SpellID > 10360)
					damage = Calculations.Battle.GetRangedDamage(attacker, targetentity);
				else
					damage = Calculations.Battle.GetMagicDamage(attacker, targetentity, spell);
				Combat.ProcessDamage(attacker, targetentity, ref damage, false);
				if (damage > 0)
				{
					if (!(targetentity is Entities.GameClient))
					{
						uint exp = (uint)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)(damage / 2), (int)damage);
						if (targetentity.Level > (attacker.Level + 10))
							exp *= 2;
						else if (attacker.Level > (targetentity.Level + 10))
							exp = (uint)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(1, (int)targetentity.Level);
						if ((attacker is Entities.GameClient))
							(attacker as Entities.GameClient).AddSpellExp(spell.SpellID, exp);
					}
					usespell.AddTarget(targetentity.EntityUID, damage);
					targets.Add(targetentity);
				}
				count++;
			}
			if (spell.SpellID == 8030 || spell.SpellID == 10308 || spell.SpellID == 7013 || spell.SpellID > 10360)
			{
				if (attacker is Entities.GameClient)
				{
					Ranged.DecreaseArrows(attacker as Entities.GameClient, 3);
				}
			}
			if (attacker is Entities.GameClient && bluename)
				(attacker as Entities.GameClient).AddStatusEffect1(Enums.Effect1.BlueName, 10000);
			return true;
		}
	}
}
