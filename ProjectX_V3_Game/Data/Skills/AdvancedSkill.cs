//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Extensions;
using System.Collections.Concurrent;
using System.Drawing;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// Description of AdvancedSkill.
	/// </summary>
	[Serializable()]
	public class AdvancedSkill
	{
		public AdvancedSkill()
		{
		}
		
		public bool Shake; // if true, the screen will shake
		public bool Explode; // if true, explosions will happen on the screen
		public bool PlayerExplode; // if true, the explosion happens at players
		public bool BossExplode;
		public ushort[] ExplodePos;
		public bool Dark; // if true darkness will happen on the screen
		public bool Zoom; // if true the screen will zoom
		public int MaxTargets; // the max number of targets, -1 = everyone, ex. 1 = single target
		public bool SummonCreatures; // if true, creatures are summoned
		public bool FixCreatureSize; // if true the number of creatures will only match targets, if false it will double
		public Entities.BossCreature Creature; // the creature
		public int MaxCreatures; // the max number of creatures, -1 = infinite
		public bool FixTargets; // if the creatures should use a fix target
		public string MapEffect; // the string effect to show on the map
		public bool ShowEffectAtPlayers; // if true, mapeffect shows at players
		public bool ShowEffectAtBoss; // if true, mapeffect shows at boss
		public int RealSkill; // the real skill to show, -1 for no skills
		public ushort RealSkilllevel; // the level of the skill
		public sbyte PercentTageEffect; // the percentage effect of the skill, ex. 50% for taking 50% HP, -1 for no usage
		public int DamageEffect; // the damage to deal, 0 for no usage
		public bool Freeze; // if true the targets will be freeze (not able to move)
		public int FreezeTime; // MS: how much time targets will be frozen in
		public bool Paralyzed; // if true the targets cannot atack
		public int ParalyzeTime; // MS: how much time targets will be paralyzed in
		public string SpreadEffect; // the effect to spread over an area
		public ushort[][] EffectPos; // the coords for the spread effect
		public bool SpreadSkill; // if true the skill will be available in an area only
		public ushort[][] SkillPos; // the coords for the area
		public int SkillShowTime; // the time for the skill to show in an area
		public int CoolDown; // S: the cooldown for the skill
		private DateTime CoolDownTime = DateTime.Now;
		public Data.Skills.MapSkill MapSkill;
		
		private class AreaSkill
		{
			public Entities.BossMonster boss;
			public AdvancedSkill skill;
		}
		private static ConcurrentDictionary<Core.PortalPoint, AreaSkill> AreaSkills;
		static AdvancedSkill()
		{
			AreaSkills = new ConcurrentDictionary<Core.PortalPoint, AdvancedSkill.AreaSkill>();
		}
		public static void SkillInArea(Entities.GameClient client, ushort x, ushort y)
		{
			Core.PortalPoint p = new Core.PortalPoint(client.Map.MapID, x, y);
			if (AreaSkills.ContainsKey(p))
			{
				AreaSkill areaSkill = AreaSkills[p];
				areaSkill.skill.Use2(areaSkill.boss, client);
			}
		}
		
		public void Use(Entities.BossMonster boss, Entities.GameClient[] Targets)
		{
			if (Targets.Length == 0 && !SpreadSkill || MaxTargets == 0 && !SpreadSkill)
				return;
			if (DateTime.Now < CoolDownTime)
				return;
			CoolDownTime = DateTime.Now.AddSeconds(CoolDown);
			
			if (MapSkill != null)
			{
				MapSkill.ExecuteStart(boss.X, boss.Y);
			}
			
			AreaSkill area = new AreaSkill();
			area.boss = boss;
			area.skill = this;
			
			#region Shake, Dark, Zoom
			if (Shake || Dark || Zoom)
			{
				using (var effect = new Packets.MapEffectPacket())
				{
					effect.Shake = Shake;
					effect.Darkness = Dark;
					effect.Zoom = Zoom;
					effect.AppendFlags();
					
					foreach (Entities.GameClient target in Targets)
					{
						effect.X = target.X;
						effect.Y = target.Y;
						target.Send(effect);
					}
				}
			}
			#endregion
			
			if (MaxTargets == 1)
				Array.Resize(ref Targets, 1);
			else if (MaxTargets > 0 && Targets.Length > MaxTargets)
				Array.Resize(ref Targets, MaxTargets);
			
			
			#region Explode
			// TODO: Explosion effect ..
			if (Explode)
			{
				if (PlayerExplode)
				{
					using (var str = new Packets.StringPacket(new Packets.StringPacker(MapEffect)))
					{
						str.Action = Enums.StringAction.MapEffect;
						foreach (Entities.GameClient target in Targets)
						{
							str.PositionX = target.X;
							str.PositionY = target.Y;

							target.SendToScreen(str, true, false);
						}
					}
				}
				else if (BossExplode)
				{
					using (var str = new Packets.StringPacket(new Packets.StringPacker(MapEffect)))
					{
						str.Action = Enums.StringAction.MapEffect;
						str.PositionX = boss.X;
						str.PositionY = boss.Y;
						foreach (Entities.GameClient target in Targets)
						{
							target.Send(str);
						}
					}
				}
				else
				{
					using (var str = new Packets.StringPacket(new Packets.StringPacker(MapEffect)))
					{
						str.Action = Enums.StringAction.MapEffect;
						str.PositionX = ExplodePos[0];
						str.PositionY = ExplodePos[1];
						foreach (Entities.GameClient target in Targets)
						{
							target.Send(str);
						}
					}
				}
			}
			#endregion

			#region Creatures
			if (SummonCreatures)
			{
				int SpawnSize = (FixCreatureSize ? Targets.Length : (Targets.Length * 2));
				if (MaxCreatures != -1)
				{
					if (SpawnSize > MaxCreatures)
					{
						SpawnSize = MaxCreatures;
					}
				}
				int count = 0;
				for (int i = 0; i < SpawnSize; i++)
				{
					Entities.BossCreature creature = (Entities.BossCreature)Creature.Copy();
					if (!FixTargets)
					{
						Maps.MapPoint Location = boss.Map.CreateAvailableLocation<Entities.BossCreature>(boss.X, boss.Y, 9);
						creature.SetData(boss, null);
						creature.Teleport(Location);
					}
					else
					{
						try
						{
							Entities.GameClient Target = Targets[count];
							if (!Target.Alive)
								continue;
							creature.SetData(boss, Target);
							Maps.MapPoint Location = boss.Map.CreateAvailableLocation<Entities.BossCreature>(Target.X, Target.Y, 9);
							creature.Teleport(Location);
							
							Entities.BossCreature creature2 = (Entities.BossCreature)Creature.Copy();
							creature2.SetData(boss, Target);
							Location = boss.Map.CreateAvailableLocation<Entities.BossCreature>(Target.X, Target.Y, 9);
							creature2.Teleport(Location);
							i++;
							count++;
						}
						catch { }
					}
				}
			}
			#endregion
			
			#region MapEffect
			if (!string.IsNullOrWhiteSpace(MapEffect))
			{
				using (var str = new Packets.StringPacket(new Packets.StringPacker(MapEffect)))
				{
					str.Action = Enums.StringAction.MapEffect;
					str.PositionX = boss.X;
					str.PositionY = boss.Y;
					
					foreach (Entities.GameClient target in Targets)
					{
						if (ShowEffectAtPlayers)
						{
							str.PositionX = target.X;
							str.PositionY = target.Y;
						}
						
						target.SendToScreen(str, true, false);
					}
				}
			}
			#endregion
			
			if (!SpreadSkill)
			{
				#region SkillAnimation + Power
				if (RealSkill != -1)
				{
					var usespell = new Packets.UseSpellPacket();
					
					usespell.EntityUID = boss.EntityUID;
					usespell.SpellID = (ushort)RealSkill;
					usespell.SpellX = boss.X;
					usespell.SpellY = boss.Y;
					usespell.SpellLevel = RealSkilllevel;
					
					foreach (Entities.GameClient target in Targets)
					{
						if (!target.Alive)
							continue;
						if (PercentTageEffect != -1)
						{
							int damage = ((target.HP / 100) * PercentTageEffect);
							if (damage <= 0)
								damage = 1;
							
							usespell.AddTarget(target.EntityUID, (uint)damage);
							target.HP -= damage;
							if (target.HP <= 0)
							{
								Packets.Interaction.Battle.Combat.Kill(boss, target, (uint)damage);
							}
						}
						else if (DamageEffect > 0)
						{
							int damage = ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(DamageEffect / 2, DamageEffect);
							usespell.AddTarget(target.EntityUID, (uint)damage);
							target.HP -= damage;
							if (target.HP <= 0)
							{
								Packets.Interaction.Battle.Combat.Kill(boss, target, (uint)damage);
							}
						}
						else
							usespell.AddTarget(target.EntityUID, 0);
					}
					boss.Screen.UpdateScreen(usespell);
				}
				else
				{
					foreach (Entities.GameClient target in Targets)
					{
						if (!target.Alive)
							continue;
						
						using (var interact = new Packets.InteractionPacket())
						{
							interact.Action = Enums.InteractAction.Attack;
							interact.EntityUID = boss.EntityUID;
							interact.TargetUID = target.EntityUID;
							interact.UnPacked = true;
							interact.X = target.X;
							interact.Y = target.Y;
							if (PercentTageEffect != -1)
							{
								int damage = (target.HP / PercentTageEffect);
								interact.Data = (uint)damage;
								
								target.HP -= damage;
								if (target.HP <= 0)
								{
									Packets.Interaction.Battle.Combat.Kill(boss, target, (uint)damage);
								}
							}
							else if (DamageEffect > 0)
							{
								int damage = ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(DamageEffect / 2, DamageEffect);
								interact.Data = (uint)damage;
								
								target.HP -= damage;
								if (target.HP <= 0)
								{
									Packets.Interaction.Battle.Combat.Kill(boss, target, (uint)damage);
								}
							}
							else
								interact.Data = 0;
							boss.Screen.UpdateScreen(interact);
						}
					}
				}
				#endregion
				
				#region Freeze
				if (Freeze)
				{
					foreach (Entities.GameClient target in Targets)
					{
						if (!target.Alive)
							continue;
						
						target.AddStatusEffect1(Enums.Effect1.IceBlock, FreezeTime);
					}
				}
				#endregion
				
				#region Paralyze
				if (Paralyzed)
				{
					foreach (Entities.GameClient target in Targets)
					{
						if (!target.Alive)
							continue;
						
						target.ParalyzeClient(ParalyzeTime);
					}
				}
				#endregion
			}
			
			#region SpreadEffect
			if (!string.IsNullOrWhiteSpace(SpreadEffect))
			{
				for (int i = 0; i < EffectPos.Length; i++)
				{
					using (var str = new Packets.StringPacket(new Packets.StringPacker(SpreadEffect)))
					{
						str.Action = Enums.StringAction.MapEffect;
						str.PositionX = EffectPos[i][0];
						str.PositionY = EffectPos[i][1];
						foreach (Entities.GameClient target in Targets)
						{
							target.Send(str);
						}
					}
				}
			}
			#endregion
			
			#region SpreadSkill
			if (SpreadSkill)
			{
				for (int i = 0; i < SkillPos.Length; i++)
				{
					Core.PortalPoint p = new Core.PortalPoint(boss.Map.MapID, SkillPos[i][0], SkillPos[i][1]);
					AreaSkills.TryAdd(p, area);
					uint TaskID = 0;
					TaskID = ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(() =>
					                                                                {
					                                                                	Core.PortalPoint p2 = new Core.PortalPoint(boss.Map.MapID, SkillPos[i][0], SkillPos[i][1]);
					                                                                	AreaSkill rArea;
					                                                                	AreaSkills.TryRemove(p2, out rArea);
					                                                                }, SkillShowTime, 0);
				}
			}
			#endregion
		}
		public void Use2(Entities.BossMonster boss, Entities.GameClient target)
		{
			if (!target.Alive)
				return;
			
			#region SkillAnimation + Power
			if (RealSkill != -1)
			{
				var usespell = new Packets.UseSpellPacket();
				
				usespell.EntityUID = boss.EntityUID;
				usespell.SpellID = (ushort)RealSkill;
				usespell.SpellX = boss.X;
				usespell.SpellY = boss.Y;
				usespell.SpellLevel = RealSkilllevel;
				
				
				if (PercentTageEffect != -1)
				{
					int damage = (target.HP / PercentTageEffect);
					usespell.AddTarget(target.EntityUID, (uint)damage);
					target.HP -= damage;
					if (target.HP <= 0)
					{
						Packets.Interaction.Battle.Combat.Kill(boss, target, (uint)damage);
					}
				}
				else if (DamageEffect > 0)
				{
					int damage = ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(DamageEffect / 2, DamageEffect);
					usespell.AddTarget(target.EntityUID, (uint)damage);
					target.HP -= damage;
					if (target.HP <= 0)
					{
						Packets.Interaction.Battle.Combat.Kill(boss, target, (uint)damage);
					}
				}
				else
					usespell.AddTarget(target.EntityUID, 0);

				boss.Screen.UpdateScreen(usespell);
			}
			else
			{
				using (var interact = new Packets.InteractionPacket())
				{
					interact.Action = Enums.InteractAction.Attack;
					interact.EntityUID = boss.EntityUID;
					interact.TargetUID = target.EntityUID;
					interact.UnPacked = true;
					interact.X = target.X;
					interact.Y = target.Y;
					if (PercentTageEffect != -1)
					{
						int damage = (target.HP / PercentTageEffect);
						interact.Data = (uint)damage;
						
						target.HP -= damage;
						if (target.HP <= 0)
						{
							Packets.Interaction.Battle.Combat.Kill(boss, target, (uint)damage);
						}
					}
					else if (DamageEffect > 0)
					{
						int damage = ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(DamageEffect / 2, DamageEffect);
						interact.Data = (uint)damage;
						
						target.HP -= damage;
						if (target.HP <= 0)
						{
							Packets.Interaction.Battle.Combat.Kill(boss, target, (uint)damage);
						}
					}
					else
						interact.Data = 0;
					boss.Screen.UpdateScreen(interact);
				}
			}
			#endregion
			
			#region Freeze
			if (Freeze)
			{
				target.AddStatusEffect1(Enums.Effect1.IceBlock, FreezeTime);
			}
			#endregion
			
			#region Paralyze
			if (Paralyzed)
			{
				target.ParalyzeClient(ParalyzeTime);
			}
			#endregion
		}
		public AdvancedSkill Copy()
		{
			return this.DeepClone();
		}
	}
}
