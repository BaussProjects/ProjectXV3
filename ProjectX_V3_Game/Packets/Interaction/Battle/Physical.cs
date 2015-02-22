//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Interaction.Battle
{
	/// <summary>
	/// Subtype: ?
	/// </summary>
	public class Physical
	{
		/// <summary>
		/// Processing the client through physical attacks.
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="interact">The interact packet.</param>
		/// <returns>Returns true if the client was handled successfully.</returns>
		private static bool ProcessClient(Entities.GameClient attacker, InteractionPacket interact)
		{
			bool used = false;
			
			if (attacker.Equipments.Contains(Enums.ItemLocation.WeaponR))
			{
				ushort subtype = (ushort)(attacker.Equipments[Enums.ItemLocation.WeaponR].ItemID / 1000);
				interact.WeaponTypeRight = subtype;
				
				if (subtype == 421)
					subtype--;
				if (Core.Kernel.WeaponSpells.ContainsKey(subtype))
					subtype = Core.Kernel.WeaponSpells[subtype];
				if (attacker.SpellData.ContainsSpell(subtype) && attacker.Battle == null)
				{
					// use weapon spell ex. phoenix
					if (Core.Kernel.SpellInfos.ContainsKey(subtype))
					{
						Data.Spell spell = Core.Kernel.SpellInfos[subtype][(byte)attacker.SpellData.GetSpell(subtype).Level];
						if (spell != null)
						{
							if (Calculations.BasicCalculations.ChanceSuccess(spell.Percentage))
							{
								interact.MagicType = spell.SpellID;
								interact.MagicLevel = spell.Level;
								interact.Action = Enums.InteractAction.MagicAttack;
								Magic.Handle(attacker, interact);
								used = false;
								return false;
							}
						}
					}
				}
			}
			if (attacker.Equipments.Contains(Enums.ItemLocation.WeaponL) && !used)
			{
				ushort subtype = (ushort)(attacker.Equipments[Enums.ItemLocation.WeaponL].ItemID / 1000);
				interact.WeaponTypeLeft = subtype;
				if (subtype == 421)
					subtype--;
				if (Core.Kernel.WeaponSpells.ContainsKey(subtype))
					subtype = Core.Kernel.WeaponSpells[subtype];
				if (attacker.SpellData.ContainsSpell(subtype) && attacker.Battle == null)
				{
					// use weapon spell ex. phoenix
					if (Core.Kernel.SpellInfos.ContainsKey(subtype))
					{
						Data.Spell spell = Core.Kernel.SpellInfos[subtype][(byte)attacker.SpellData.GetSpell(subtype).Level];
						if (spell != null)
						{
							if (Calculations.BasicCalculations.ChanceSuccess(spell.Percentage))
							{
								interact.MagicType = spell.SpellID;
								interact.MagicLevel = spell.Level;
								interact.Action = Enums.InteractAction.MagicAttack;
								Magic.Handle(attacker, interact);
								used = false;
								return false;
							}
						}
					}
				}
			}
			
			if (attacker.Battle != null)
			{
				if (!attacker.Battle.HandleBeginHit_Physical(attacker))
					return false;
			}
			
			return true;
		}
		public static void Handle(Entities.IEntity attacker, InteractionPacket interact)
		{
			if (interact == null)
				return;
			
			Maps.IMapObject attackermap = attacker as Maps.IMapObject;
			if (!attackermap.Screen.ContainsEntity(interact.TargetUID))
			{
				return;
			}
			Maps.IMapObject target = null;
			if (!attackermap.Screen.GetEntity(interact.TargetUID, out target))
			{
				return;
			}
			if (target is Entities.NPC)
				return;
			if (target is Entities.GameClient)
			{
				if (!(target as Entities.GameClient).LoggedIn)
					return;
				
				if ((target as Entities.GameClient).ContainsFlag1(Enums.Effect1.Fly))
					return;
				
				if (target.Map.MapType == Enums.MapType.NoPK && (attacker is Entities.GameClient))
				{
					return;
				}
				
				if (!(DateTime.Now >= (target as Entities.GameClient).LoginProtection.AddSeconds(10)))
				{
					return;
				}
				
				
				if (!(DateTime.Now >= (target as Entities.GameClient).ReviveProtection.AddSeconds(5)))
				{
					return;
				}
			}
			
			Entities.IEntity targetentity = target as Entities.IEntity;
			if (targetentity is Entities.BossMonster)
			{
				if (!(targetentity as Entities.BossMonster).CanBeAttacked)
					return;
			}
			if (!Core.Screen.ValidDistance(attackermap.X, attackermap.Y, target.X, target.Y))
			{
				return;
			}
			if (!attacker.Alive)
			{
				return;
			}
			if (!targetentity.Alive)
			{
				return;
			}
			if (attacker is Entities.GameClient)
			{
				if (!ProcessClient((attacker as Entities.GameClient), interact))
					return;
			}
			
			uint damage = Calculations.Battle.GetPhysicalDamage(attacker, targetentity);
			Combat.ProcessDamage(attacker, targetentity, ref damage);
			if (damage > 0)
			{
				if (attacker is Entities.GameClient && !(target is Entities.GameClient))
				{
					Entities.GameClient client = attacker as Entities.GameClient;
					
					uint exp = (uint)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)(damage / 2), (int)damage);
					if (targetentity.Level > (attacker.Level + 10))
						exp *= 2;
					else if (attacker.Level > (targetentity.Level + 10))
						exp = (uint)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(1, (int)targetentity.Level);
					
					if (interact.WeaponTypeRight > 0)
						client.AddProfExp(interact.WeaponTypeRight, exp);
					if (interact.WeaponTypeLeft > 0)
						client.AddProfExp(interact.WeaponTypeLeft, exp);
				}
			}
			interact.Data = damage;
			attacker.Screen.UpdateScreen(interact);
			if (attacker is Entities.GameClient)
				(attacker as Entities.GameClient).Send(interact);
		}
	}
}
