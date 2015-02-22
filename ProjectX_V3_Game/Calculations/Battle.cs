//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Calculations
{
	/// <summary>
	/// Calculations used for battle / combat.
	/// </summary>
	public class Battle
	{
		/// <summary>
		/// Returns a boolean defining whether the attack is a success. [Unused atm.]
		/// </summary>
		/// <param name="Percentage">The percentage.</param>
		/// <returns>Returns true if the attack is a success.</returns>
		public static bool AttackSuccess(double Percentage)
		{
			return false;
		}
		
		/// <summary>
		/// Getting a boolean defining whether or not defense dura can be lost.
		/// </summary>
		/// <returns>Returns true if the chance is success.</returns>
		public static bool LoseDuraDefense()
		{
			return false;
			//return BasicCalculations.ChanceSuccess(2); // 10 % chance
		}
		
		/// <summary>
		/// Getting a boolean defining whether or not attack dura can be lost.
		/// </summary>
		/// <returns>Returns true if the chance is success.</returns>
		public static bool LoseDuraAttack()
		{
			return false;
			//return BasicCalculations.ChanceSuccess(1); // 3 % chance
		}
		
		#region Physical
		/// <summary>
		/// Gets the physical damage.
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="attacked">The attacked.</param>
		/// <returns>Returns the damage.</returns>
		public static uint GetPhysicalDamage(Entities.IEntity attacker, Entities.IEntity attacked)
		{
			double damage = 0;
			if (attacker is Entities.GameClient)
			{
				if (attacked is Entities.GameClient)
					damage = GetPhysicalDamage_PVP((attacker as Entities.GameClient), (attacked as Entities.GameClient));
				else if (attacked is Entities.Monster)
					damage = GetPhysicalDamage_PVM((attacker as Entities.GameClient), (attacked as Entities.Monster));
			}
			if (attacker is Entities.Monster)
			{
				if (attacked is Entities.GameClient)
					damage = GetPhysicalDamage_MVP((attacker as Entities.Monster), (attacked as Entities.GameClient));
				else if (attacked is Entities.Monster)
					damage = GetPhysicalDamage_MVM((attacker as Entities.Monster), (attacked as Entities.Monster));
			}
			if (damage > 1)
			{
				if (attacker.Level > (attacked.Level + 10))
					damage *= 1.25;
				else if ((attacker.Level + 10) < attacked.Level)
					damage = (damage * 0.75);
			}
			damage = (damage >= 1 ? damage : 1);
			if (damage > 1)
			{
				damage += ((damage / 2) * attacker.Reborns);
			}
			return (uint)damage;
		}
		
		/// <summary>
		/// Gets the physical damage (PVP).
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="attacked">The attacked.</param>
		/// <returns>Returns the damage.</returns>
		private static double GetPhysicalDamage_PVP(Entities.GameClient attacker, Entities.GameClient attacked)
		{
			double damage = ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(attacker.MinAttack, attacker.MaxAttack);
			damage *= 1 + attacker.DragonGemPercentage;
			damage -= attacked.Defense;
			
			if (attacked.ContainsFlag1(Enums.Effect1.Shield))
				damage *= 0.5;
			if (attacker.ContainsFlag1(Enums.Effect1.Stig))
				damage *= 1.75;
			
			double damage_perc = damage;
			damage_perc = (damage / 100) * attacked.TortoiseGemPercentage;
			damage -= damage_perc;
			damage_perc = (damage / 100) * attacked.Bless;
			damage -= damage_perc;
			
			return damage;
		}
		
		/// <summary>
		/// Gets the physical damage (PVM).
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="attacked">The attacked.</param>
		/// <returns>Returns the damage.</returns>
		private static double GetPhysicalDamage_PVM(Entities.GameClient attacker, Entities.Monster attacked)
		{
			double damage = ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(attacker.MinAttack, attacker.MaxAttack);
			damage *= 1 + attacker.DragonGemPercentage;
			damage -= attacked.Defense;
			
			if (attacker.ContainsFlag1(Enums.Effect1.Stig))
				damage *= 1.75;
			//double damage_perc = damage;
			//damage_perc = (damage / 100) * attacked.TortoiseGemPercentage;
			//damage -= damage_perc;
			//damage_perc = (damage / 100) * attacked.Bless;
			//damage -= damage_perc;
			
			return damage;
		}
		
		/// <summary>
		/// Gets the physical damage (MVP).
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="attacked">The attacked.</param>
		/// <returns>Returns the damage.</returns>
		private static double GetPhysicalDamage_MVP(Entities.Monster attacker, Entities.GameClient attacked)
		{
			double damage = ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)attacker.MinAttack, (int)attacker.MaxAttack);
			//damage *= 1 + attacker.DragonGemPercentage;
			damage -= attacked.Defense;
			
			if (attacked.ContainsFlag1(Enums.Effect1.Shield))
				damage *= 0.5;
			
			double damage_perc = damage;
			damage_perc = (damage / 100) * attacked.TortoiseGemPercentage;
			damage -= damage_perc;
			damage_perc = (damage / 100) * attacked.Bless;
			damage -= damage_perc;
			
			return damage;
		}
		
		/// <summary>
		/// Gets the physical damage (MVM).
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="attacked">The attacked.</param>
		/// <returns>Returns the damage.</returns>
		private static double GetPhysicalDamage_MVM(Entities.Monster attacker, Entities.Monster attacked)
		{
			double damage = ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)attacker.MinAttack, (int)attacker.MaxAttack);
			//damage *= 1 + attacker.DragonGemPercentage;
			damage -= attacked.Defense;
			//double damage_perc = damage;
			//damage_perc = (damage / 100) * attacked.TortoiseGemPercentage;
			//damage -= damage_perc;
			//damage_perc = (damage / 100) * attacked.Bless;
			//damage -= damage_perc;
			
			return damage;
		}
		#endregion
		#region Ranged
		/// <summary>
		/// Gets the ranged damage.
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="attacked">The attacked.</param>
		/// <returns>Returns the damage.</returns>
		public static uint GetRangedDamage(Entities.IEntity attacker, Entities.IEntity attacked)
		{
			double damage = 0;
			if (attacker is Entities.GameClient)
			{
				if (attacked is Entities.GameClient)
					damage = GetRangedDamage_PVP((attacker as Entities.GameClient), (attacked as Entities.GameClient));
				if (attacked is Entities.Monster)
					damage = GetRangedDamage_PVM((attacker as Entities.GameClient), (attacked as Entities.Monster));
			}
			if (damage > 1)
			{
				if (attacker.Level > (attacked.Level + 10))
					damage *= 1.25;
				else if ((attacker.Level + 10) < attacked.Level)
					damage = (damage * 0.75);
			}
			damage = (damage >= 1 ? damage : 1);
			if (damage > 1)
			{
				damage += ((damage / 2) * attacker.Reborns);
			}
			return (uint)damage;
		}
		
		/// <summary>
		/// Gets the ranged damage. (PVP)
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="attacked">The attacked.</param>
		/// <returns>Returns the damage.</returns>
		private static double GetRangedDamage_PVP(Entities.GameClient attacker, Entities.GameClient attacked)
		{
			double damage = ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(attacker.MinAttack, attacker.MaxAttack);
			damage *= 1 - (attacked.Dodge * 0.01);
			damage *= 0.45;
			
			if (attacker.ContainsFlag1(Enums.Effect1.Stig))
				damage *= 1.75;
			
			damage *= 1 + attacker.DragonGemPercentage;
			
			double damage_perc = damage;
			damage_perc = (damage / 100) * attacked.TortoiseGemPercentage;
			damage -= damage_perc;
			damage_perc = (damage / 100) * attacked.Bless;
			damage -= damage_perc;
			
			return damage;
		}
		
		/// <summary>
		/// Gets the ranged damage. (PVP)
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="attacked">The attacked.</param>
		/// <returns>Returns the damage.</returns>
		private static double GetRangedDamage_PVM(Entities.GameClient attacker, Entities.Monster attacked)
		{
			double damage = ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(attacker.MinAttack, attacker.MaxAttack);
			damage *= 1 - (attacked.Dodge * 0.01);
			damage *= 0.45;
			
			if (attacker.ContainsFlag1(Enums.Effect1.Stig))
				damage *= 1.75;
			
			damage *= 1 + attacker.DragonGemPercentage;
			//double damage_perc = damage;
			//damage_perc = (damage / 100) * attacked.TortoiseGemPercentage;
			//damage -= damage_perc;
			//damage_perc = (damage / 100) * attacked.Bless;
			//damage -= damage_perc;
			
			return damage;
		}
		#endregion
		#region Magic
		/// <summary>
		/// Gets the magic damage.
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="attacked">The attacked.</param>
		/// <param name="spell">The spell.</param>
		/// <returns>Returns the damage.</returns>
		public static uint GetMagicDamage(Entities.IEntity attacker, Entities.IEntity attacked, Data.Spell spell)
		{
			double damage = 0;
			if (attacker is Entities.GameClient)
			{
				if (attacked is Entities.GameClient)
					damage = GetMagicDamage_PVP((attacker as Entities.GameClient), (attacked as Entities.GameClient), spell);
				else if (attacked is Entities.Monster)
					damage = GetMagicDamage_PVM((attacker as Entities.GameClient), (attacked as Entities.Monster), spell);
			}
			if (attacker is Entities.Monster)
			{
				if (attacked is Entities.GameClient)
					damage = GetMagicDamage_MVP((attacker as Entities.Monster), (attacked as Entities.GameClient), spell);
				else if (attacked is Entities.Monster)
					damage = GetMagicDamage_MVM((attacker as Entities.Monster), (attacked as Entities.Monster), spell);
			}
			if (damage > 1)
			{
				if (attacker.Level > (attacked.Level + 10))
					damage *= 1.25;
				else if ((attacker.Level + 10) < attacked.Level)
					damage = (damage * 0.75);
			}
			damage = (damage >= 1 ? damage : 1);
			if (damage > 1)
			{
				damage += ((damage / 2) * attacker.Reborns);
			}
			return (uint)damage;
		}
		
		/// <summary>
		/// Gets the magic damage. (PVP)
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="attacked">The attacked.</param>
		/// <param name="spell">The spell.</param>
		/// <returns>Returns the damage.</returns>
		private static double GetMagicDamage_PVP(Entities.GameClient attacker, Entities.GameClient attacked, Data.Spell spell)
		{
			double damage = (double)attacker.MagicAttack;
			damage -= attacked.MagicDefense;
			damage *= 0.65;
			damage += spell.Power;
			damage *= 1 + attacker.PhoenixGemPercentage;
			//	damage -= attacked.Defense;
			double damage_perc = damage;
			damage_perc = (damage / 100) * attacked.TortoiseGemPercentage;
			damage -= damage_perc;
			damage_perc = (damage / 100) * attacked.Bless;
			damage -= damage_perc;
			
			return damage;
		}
		
		/// <summary>
		/// Gets the magic damage. (PVM)
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="attacked">The attacked.</param>
		/// <param name="spell">The spell.</param>
		/// <returns>Returns the damage.</returns>
		private static double GetMagicDamage_PVM(Entities.GameClient attacker, Entities.Monster attacked, Data.Spell spell)
		{
			double damage = (double)attacker.MagicAttack;
			damage -= attacked.MagicDefense;
			damage *= 0.65;
			damage += spell.Power;
			damage *= 1 + attacker.PhoenixGemPercentage;
			//	damage -= attacked.Defense;
			//	double damage_perc = damage;
			//	damage_perc = (damage / 100) * attacked.TortoiseGemPercentage;
			//	damage -= damage_perc;
			//	damage_perc = (damage / 100) * attacked.Bless;
			//	damage -= damage_perc;
			
			return damage;
		}
		
		/// <summary>
		/// Gets the magic damage. (MVP)
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="attacked">The attacked.</param>
		/// <param name="spell">The spell.</param>
		/// <returns>Returns the damage.</returns>
		private static double GetMagicDamage_MVP(Entities.Monster attacker, Entities.GameClient attacked, Data.Spell spell)
		{
			double damage = (double)attacker.MaxAttack;
			damage -= attacked.MagicDefense;
			damage *= 0.65;
			damage += spell.Power;
			//damage *= 1 + attacker.PhoenixGemPercentage;
			//	damage -= attacked.Defense;
			//	double damage_perc = damage;
			//	damage_perc = (damage / 100) * attacked.TortoiseGemPercentage;
			//	damage -= damage_perc;
			//	damage_perc = (damage / 100) * attacked.Bless;
			//	damage -= damage_perc;
			
			return damage;
		}
		
		/// <summary>
		/// Gets the magic damage. (MVM)
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="attacked">The attacked.</param>
		/// <param name="spell">The spell.</param>
		/// <returns>Returns the damage.</returns>
		private static double GetMagicDamage_MVM(Entities.Monster attacker, Entities.Monster attacked, Data.Spell spell)
		{
			double damage = attacker.MaxAttack;
			damage -= attacked.MagicDefense;
			damage *= 0.65;
			damage += spell.Power;
			//	damage *= 1 + attacker.PhoenixGemPercentage;
			//	damage -= attacked.Defense;
			//	double damage_perc = damage;
			//	damage_perc = (damage / 100) * attacked.TortoiseGemPercentage;
			//	damage -= damage_perc;
			//	damage_perc = (damage / 100) * attacked.Bless;
			//	damage -= damage_perc;
			
			return damage;
		}
		#endregion
		#region PhysicalMagic
		/// <summary>
		/// Gets the physical magic damage.
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="attacked">The attacked.</param>
		/// <param name="spell">The spell.</param>
		/// <returns>Returns the damage.</returns>
		public static uint GetPhysicalMagicDamage(Entities.IEntity attacker, Entities.IEntity attacked, Data.Spell spell)
		{
			double damage = 0;
			if (attacker is Entities.GameClient)
			{
				if (attacked is Entities.GameClient)
					damage = GetPhysicalMagicDamage_PVP((attacker as Entities.GameClient), (attacked as Entities.GameClient), spell);
				else if (attacked is Entities.Monster)
					damage = GetPhysicalMagicDamage_PVM((attacker as Entities.GameClient), (attacked as Entities.Monster), spell);
			}
			if (attacker is Entities.Monster)
			{
				if (attacked is Entities.GameClient)
					damage = GetPhysicalMagicDamage_MVP((attacker as Entities.Monster), (attacked as Entities.GameClient), spell);
				else if (attacked is Entities.Monster)
					damage = GetPhysicalMagicDamage_MVM((attacker as Entities.Monster), (attacked as Entities.Monster), spell);
			}
			if (damage > 1)
			{
				if (attacker.Level > (attacked.Level + 10))
					damage *= 1.25;
				else if ((attacker.Level + 10) < attacked.Level)
					damage = (damage * 0.75);
			}
			damage = (damage >= 1 ? damage : 1);
			if (damage > 1)
			{
				damage += ((damage / 2) * attacker.Reborns);
			}
			return (uint)damage;
		}
		
		/// <summary>
		/// Gets the magic damage. (PVP)
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="attacked">The attacked.</param>
		/// <param name="spell">The spell.</param>
		/// <returns>Returns the damage.</returns>
		private static double GetPhysicalMagicDamage_PVP(Entities.GameClient attacker, Entities.GameClient attacked, Data.Spell spell)
		{
			double damage = (double) ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(attacker.MinAttack, attacker.MaxAttack * 2);
			
			damage -= attacked.Defense;
			damage *= 0.65;
			// penetration extra damage
			if (spell.SpellID == 1290 && damage > 0)
			{
				double hunperc = (double)((damage / 100) * 26.6);
				damage += (hunperc * spell.Level);
			}
			//	damage += spell.Power;
			damage *= 1 + attacker.DragonGemPercentage;
			
			if (attacked.ContainsFlag1(Enums.Effect1.Shield))
				damage *= 0.5;
			
			//	damage -= attacked.Defense;
			double damage_perc = damage;
			damage_perc = (damage / 100) * attacked.TortoiseGemPercentage;
			damage -= damage_perc;
			damage_perc = (damage / 100) * attacked.Bless;
			damage -= damage_perc;
			
			return damage;
		}
		
		/// <summary>
		/// Gets the physical magic damage. (PVM)
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="attacked">The attacked.</param>
		/// <param name="spell">The spell.</param>
		/// <returns>Returns the damage.</returns>
		private static double GetPhysicalMagicDamage_PVM(Entities.GameClient attacker, Entities.Monster attacked, Data.Spell spell)
		{
			double damage = (double) ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(attacker.MinAttack, attacker.MaxAttack * 2);
			
			damage -= attacked.Defense;
			damage *= 0.65;
			// penetration extra damage
			if (spell.SpellID == 1290 && damage > 0)
			{
				double hunperc = (double)((damage / 100) * 26.6);
				damage += (hunperc * spell.Level);
			}
			//damage += spell.Power;
			damage *= 1 + attacker.DragonGemPercentage;
			//	damage -= attacked.Defense;
			//	double damage_perc = damage;
			//	damage_perc = (damage / 100) * attacked.TortoiseGemPercentage;
			//	damage -= damage_perc;
			//	damage_perc = (damage / 100) * attacked.Bless;
			//	damage -= damage_perc;
			
			return damage;
		}
		
		/// <summary>
		/// Gets the physical magic damage. (MVP)
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="attacked">The attacked.</param>
		/// <param name="spell">The spell.</param>
		/// <returns>Returns the damage.</returns>
		private static double GetPhysicalMagicDamage_MVP(Entities.Monster attacker, Entities.GameClient attacked, Data.Spell spell)
		{
			double damage = (double) ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)attacker.MaxAttack, (int)attacker.MaxAttack * 2);
			
			damage -= attacked.Defense;
			damage *= 0.65;
			// penetration extra damage
			if (spell.SpellID == 1290 && damage > 0)
			{
				double hunperc = (double)((damage / 100) * 26.6);
				damage += (hunperc * spell.Level);
			}
			
			if (attacked.ContainsFlag1(Enums.Effect1.Shield))
				damage *= 0.5;
			//	damage += spell.Power;
			//damage *= 1 + attacker.PhoenixGemPercentage;
			//	damage -= attacked.Defense;
			//	double damage_perc = damage;
			//	damage_perc = (damage / 100) * attacked.TortoiseGemPercentage;
			//	damage -= damage_perc;
			//	damage_perc = (damage / 100) * attacked.Bless;
			//	damage -= damage_perc;
			
			return damage;
		}
		
		/// <summary>
		/// Gets the physical magic damage. (MVM)
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="attacked">The attacked.</param>
		/// <param name="spell">The spell.</param>
		/// <returns>Returns the damage.</returns>
		private static double GetPhysicalMagicDamage_MVM(Entities.Monster attacker, Entities.Monster attacked, Data.Spell spell)
		{
			double damage = (double) ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)attacker.MaxAttack, (int)attacker.MaxAttack * 2);
			
			damage -= attacked.Defense;
			damage *= 0.65;
			// penetration extra damage
			if (spell.SpellID == 1290 && damage > 0)
			{
				double hunperc = (double)((damage / 100) * 26.6);
				damage += (hunperc * spell.Level);
			}
			//damage += spell.Power;
			//	damage *= 1 + attacker.PhoenixGemPercentage;
			//	damage -= attacked.Defense;
			//	double damage_perc = damage;
			//	damage_perc = (damage / 100) * attacked.TortoiseGemPercentage;
			//	damage -= damage_perc;
			//	damage_perc = (damage / 100) * attacked.Bless;
			//	damage -= damage_perc;
			
			return damage;
		}
		#endregion
	}
}
