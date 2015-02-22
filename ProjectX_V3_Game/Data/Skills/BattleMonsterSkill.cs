//Project by BaussHacker aka. L33TS
using System;
using ProjectX_V3_Lib.Extensions;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// Description of BattleMonsterSkill.
	/// </summary>
	[Serializable()]
	public class BattleMonsterSkill
	{
		public Enums.PetType SkillType;
		public byte PP;
		public byte MaxPP;
		public byte StateChance;
		public Enums.PetState State;
		public int Power;
		public string Name;
		
		public bool Attack(Entities.BattlePet opponent, Entities.BattlePet target, bool ForceDamage = false, double FDamage = 0)
		{
			if (ForceDamage)
			{
				if (FDamage < 1)
					FDamage = 1;
				target.HP -= (int)FDamage;
				
				opponent.Battle.UpdateScreen();
				Send(target, (uint)FDamage);
				
				if (target.HP < 0)
				{
					opponent.Battle.Finish(opponent);
					return false;
				}
				return true;
			}
			
			double Evolve = (double)(opponent.EvolveState + 1);
			double SkillPower = (double)this.Power;
			double Power = (double)opponent.Power;
			double Level = (double)opponent.Level;
			double Damage = (Evolve * (Power + (SkillPower + ((SkillPower / 100) * Level))));

			double TEvolve = (double)(target.EvolveState + 1);
			double TPower = (double)target.Power;
			double TLevel = (double)target.Level;
			double Defense = (TEvolve * (TPower + ((TPower / 100) * TLevel)));

			if (State == Enums.PetState.ChanceDoubleDamage ||
			    State == Enums.PetState.TurnAttack ||
			    State == Enums.PetState.TurnDoubleAttack ||
			    State != Enums.PetState.ChanceDoubleDamage &&
			    State != Enums.PetState.TurnAttack &&
			    State != Enums.PetState.TurnDoubleAttack &&
			    State != Enums.PetState.None &&
			    Calculations.BasicCalculations.ChanceSuccess(StateChance))
			{
				switch (State)
				{
					case Enums.PetState.Absorb:
						{
							Defense = 0;
							Damage = ((target.HP / 100) * SkillPower);
							opponent.HP += (int)Damage;
							break;
						}
					case Enums.PetState.ChanceDoubleDamage:
						{
							byte HitCount = 0;
							int Chance = (int)StateChance;
								Damage -= Defense;
								if (Damage < 1)
									Damage = 1;
							while (Calculations.BasicCalculations.ChanceSuccess(Chance) && HitCount < 2)
							{
								target.HP -= (int)Damage;
								
								opponent.Battle.UpdateScreen();
								Send(target, (uint)Damage);
								
								if (target.HP < 0)
								{
									opponent.Battle.Finish(opponent);
									return false;
								}
								HitCount++;
								Chance /= 2;
							}
							return true;
						}
					case Enums.PetState.Sleep:
						{
							if (target.State != Enums.PetState.None)
								return false;
							target.State = State;
							target.AddStatusEffect1(Enums.Effect1.PartiallyInvisible);
							opponent.Battle.UpdateScreen();
							return false;
						}
					case Enums.PetState.Reflect:
						{
							double ReturnDamage = ((Damage / 100) * SkillPower);
							if (!Attack(target, opponent, true, ReturnDamage))
								return false;
							break;
						}
					case Enums.PetState.Confuse:
						{
							if (target.State != Enums.PetState.None)
								return false;
							target.State = State;
							target.AddStatusEffect1(Enums.Effect1.Confused);
							opponent.Battle.UpdateScreen();
							break;
						}
					case Enums.PetState.Freezing:
						{
							if (target.State != Enums.PetState.None)
								return false;
							target.State = State;
							target.AddStatusEffect1(Enums.Effect1.IceBlock);
							opponent.Battle.UpdateScreen();
							break;
						}
					case Enums.PetState.Paralyzis:
						{
							if (target.State != Enums.PetState.None)
								return false;
							target.State = State;
							target.AddStatusEffect1(Enums.Effect1.Dazed);
							opponent.Battle.UpdateScreen();
							break;
						}
					case Enums.PetState.Rest:
						{
							if (opponent.State != Enums.PetState.None)
								return false;
							opponent.State = State;
							opponent.AddStatusEffect1(Enums.Effect1.PartiallyInvisible);
							
							double RestoreHP = ((opponent.MaxHP / 100) * SkillPower);
							opponent.HP += (int)RestoreHP;
							
							opponent.Battle.UpdateScreen();
							return false;
						}
					case Enums.PetState.Restore:
						{
							double RestoreHP = ((opponent.MaxHP / 100) * SkillPower);
							opponent.HP += (int)RestoreHP;
							
							opponent.Battle.UpdateScreen();
							return false;
						}
					case Enums.PetState.TurnAttack:
						{
							byte HitCount = 0;
							int Chance = (int)StateChance;
								Damage -= Defense;
								if (Damage < 1)
									Damage = 1;
							while (Calculations.BasicCalculations.ChanceSuccess(Chance) && HitCount < 5)
							{
								target.HP -= (int)Damage;
								
								opponent.Battle.UpdateScreen();
								Send(target, (uint)Damage);
								
								if (target.HP < 0)
								{
									opponent.Battle.Finish(opponent);
									return false;
								}
								HitCount++;
								Chance /= 2;
							}
							return true;
						}
					case Enums.PetState.TurnDoubleAttack:
						{
							byte HitCount = 0;
							int Chance = (int)StateChance;
							Damage -= Defense;
							if (Damage < 1)
								Damage = 1;
							while (Calculations.BasicCalculations.ChanceSuccess(Chance) && HitCount < 5)
							{
								for (int i = 0; i < 2; i++)
								{
									target.HP -= (int)Damage;
									
									opponent.Battle.UpdateScreen();
									Send(target, (uint)Damage);
									
									if (target.HP < 0)
									{
										opponent.Battle.Finish(opponent);
										return false;
									}
								}
								HitCount++;
								Chance /= 2;
							}
							return true;
						}
				}
			}
			
			Damage -= Defense;
			if (Damage < 1)
				Damage = 1;
			target.HP -= (int)Damage;
			
			opponent.Battle.UpdateScreen();
			Send(target, (uint)Damage);
			
			if (target.HP < 0)
			{
				opponent.Battle.Finish(opponent);
				return false;
			}
			return true;
		}
		
		public void Send(Entities.BattlePet Target, uint Damage)
		{
			
		}
		
		public BattleMonsterSkill Copy()
		{
			return this.DeepClone();
		}
	}
}
