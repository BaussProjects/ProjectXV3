//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Interaction.Battle
{
	/// <summary>
	/// Subtype: ?
	/// </summary>
	public class Ranged
	{
		public static void DecreaseArrows(Entities.GameClient attackerclient, int amount = 1)
		{
			if (amount <= 0)
				amount = 1;
			
			Data.ItemInfo arrow = attackerclient.Equipments[Enums.ItemLocation.WeaponL];
			arrow.CurrentDura -= (short)amount;
			Database.CharacterDatabase.SaveEquipment(attackerclient, arrow, Enums.ItemLocation.WeaponL);
			arrow.SendPacket(attackerclient, 3);
		}
		/// <summary>
		/// Processing the client through ranged attacks.
		/// </summary>
		/// <param name="client">The attacker.</param>
		/// <param name="interact">The interact packet.</param>
		/// <returns>Returns true if the client was handled successfully.</returns>
		public static bool ProcessClient(Entities.GameClient client, InteractionPacket interact, bool MAGIC_PACKET = false, int REQUIRED_ARROWS = 1)
		{
			if (!client.Equipments.Contains(Enums.ItemLocation.WeaponR))
				return false;
			if (!client.Equipments[Enums.ItemLocation.WeaponR].IsBow())
				return false;
			if (!client.Equipments.Contains(Enums.ItemLocation.WeaponL))
				return false;
			if (!client.Equipments[Enums.ItemLocation.WeaponL].IsArrow())
				return false;
			if (client.Equipments[Enums.ItemLocation.WeaponL].CurrentDura < REQUIRED_ARROWS)
			{
				uint arrowtype = client.Equipments[Enums.ItemLocation.WeaponL].ItemID;
				if (client.Inventory.ContainsByID(arrowtype))
				{
					Data.ItemInfo arrow = client.Inventory.SearchForNonEmpty(arrowtype);
					if (arrow != null)
					{
						if (!client.Equipments.Equip(arrow, Enums.ItemLocation.WeaponL, true))
						{
							using (var fmsg = Packets.Message.MessageCore.CreateSystem2(client.Name, Core.MessageConst.AUTO_RELOAD_ARROW_FAIL))
								client.Send(fmsg);
							return false;
						}
					}
					else
						return false;
				}
				else if (!client.Equipments.Unequip(Enums.ItemLocation.WeaponL))
				{
					using (var fmsg = Packets.Message.MessageCore.CreateSystem2(client.Name, Core.MessageConst.EMPTY_ARROWS))
						client.Send(fmsg);
					return false;
				}
				else
					return false; // no more arrows in inventory
			}
			if (client.Battle != null)
			{
				if (!client.Battle.HandleBeginHit_Ranged(client))
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
			if (!Combat.FixTarget(attacker, targetentity))
				return;
			
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
			
			uint damage = Calculations.Battle.GetRangedDamage(attacker, targetentity);
			Combat.ProcessDamage(attacker, targetentity, ref damage);
			interact.Data = damage;
			attacker.Screen.UpdateScreen(interact);
			if (attacker is Entities.GameClient)
			{
				Entities.GameClient attackerclient = attacker as Entities.GameClient;
				if (damage > 0)
				{
					if (!(target is Entities.GameClient))
					{
						uint exp = (uint)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)(damage / 2), (int)damage);
						if (targetentity.Level > (attacker.Level + 10))
							exp *= 2;
						else if (attacker.Level > (targetentity.Level + 10))
							exp = (uint)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(1, (int)targetentity.Level);
						
						if (interact.WeaponTypeRight > 0)
							attackerclient.AddProfExp(500, exp); // bow
					}
				}
				Data.ItemInfo arrow = attackerclient.Equipments[Enums.ItemLocation.WeaponL];
				arrow.CurrentDura--;
				Database.CharacterDatabase.SaveEquipment(attackerclient, arrow, Enums.ItemLocation.WeaponL);
				arrow.SendPacket(attackerclient, 3);
				
				attackerclient.Send(interact);
				
				//interact.Data = 0;
				interact.ActivationType = 0;
				interact.ActivationValue = 0;
				attackerclient.AttackPacket = interact;
				attackerclient.UseAutoAttack(interact);
			}
		}
	}
}
