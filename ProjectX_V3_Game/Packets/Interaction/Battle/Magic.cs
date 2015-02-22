//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;

namespace ProjectX_V3_Game.Packets.Interaction.Battle
{
	/// <summary>
	/// Subtype: ?
	/// </summary>
	public class Magic
	{
		/// <summary>
		/// Processing the client through magic usage.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="interact">The interaction packet.</param>
		/// <param name="usespell">The usespell packet.</param>
		/// <param name="spell">The spell.</param>
		/// <returns>Returns true if the client was successfully handled.</returns>
		private static bool ProcessClient(Entities.GameClient client, InteractionPacket interact, UseSpellPacket usespell, out Data.Spell spell)
		{
			spell = null;
			ushort spellID = interact.MagicType;
			if (interact.MagicType >= 3090 && interact.MagicType <= 3306)
				spellID = 3090;

			if (spellID == 6012)
			{
				spell = Core.Kernel.SpellInfos[6010][0];
				interact.X = client.X;
				interact.Y = client.Y;
			}
			else
			{
				byte choselevel = 0;
				if (spellID == interact.MagicType)
					choselevel = (byte)client.SpellData.GetSpell(spellID).Level;
				if (Core.Kernel.SpellInfos[spellID] != null && !Core.Kernel.SpellInfos[spellID].ContainsKey(choselevel))
					choselevel = (byte)(Core.Kernel.SpellInfos[spellID].Count - 1);
				spell = Core.Kernel.SpellInfos[spellID][choselevel];
			}
			if (spell == null)
			{
				return false;
			}
			usespell.SpellID = spell.SpellID;
			usespell.SpellLevel = spell.Level;
			
			if (client.Battle != null)
			{
				if (!client.Battle.HandleBeginHit_Magic(client, usespell))
					return false;
			}
			
			//if (!client.SpellData.ContainsSpell(interact.MagicType))
			//	return false;
			
			return true;
		}
		public static void Handle(Entities.IEntity attacker, InteractionPacket interact)
		{
			if (interact == null)
				return;
			ConcurrentBag<Entities.IEntity> targets = new ConcurrentBag<ProjectX_V3_Game.Entities.IEntity>();
			
			Maps.IMapObject attackermap = attacker as Maps.IMapObject;
			if (!Core.Screen.ValidDistance(attackermap.X, attackermap.Y, interact.X, interact.Y))
			{
				return;
			}
//			if (!attackermap.Screen.ContainsEntity(interact.TargetUID))
//			{
//				return;
//			}
//			Maps.IMapObject target = null;
//			if (!attackermap.Screen.GetEntity(interact.TargetUID, out target))
//			{
//				return;
//			}
//			Entities.IEntity targetentity = target as Entities.IEntity;
//			if (!Core.Screen.ValidDistance(attackermap.X, attackermap.Y, target.X, target.Y))
//			{
//				return;
//			}
			if (!attacker.Alive)
			{
				return;
			}
			
			Maps.IMapObject target = null;
			if (interact.TargetUID > 0)
			{
				if (!attackermap.Screen.GetEntity(interact.TargetUID, out target) && interact.TargetUID != attacker.EntityUID)
				{
					return; // fail here
				}
			}
			if (target != null)
			{
				if (target is Entities.NPC)
					return;
				if (target is Entities.GameClient)
				{
					if (!(target as Entities.GameClient).LoggedIn)
						return;
					
					if (target.Map.MapType == Enums.MapType.NoPK && (attacker is Entities.GameClient))
					{
						if (interact.MagicType != 1045 && interact.MagicType != 1046)
						{
							return;
						}
					}
					if (!(DateTime.Now >= (target as Entities.GameClient).LoginProtection.AddSeconds(10)))
					{
						return;
					}
					
					// check if revive skill here and above as well!
					if (!(DateTime.Now >= (target as Entities.GameClient).ReviveProtection.AddSeconds(5)))
					{
						return;
					}
					
				}
				
				Entities.IEntity targetentity = target as Entities.IEntity;
				if (!Core.Screen.ValidDistance(attackermap.X, attackermap.Y, target.X, target.Y))
				{
					return;
				}
				
				
				if (!Combat.FixTarget(attacker, targetentity))
					return;
				
				if (!targetentity.Alive && interact.MagicType != 1100 && interact.MagicType != 1050)
				{
					return;
				}
			}
			
			UseSpellPacket usespell = new UseSpellPacket();
			usespell.EntityUID = attacker.EntityUID;
			usespell.SpellID = interact.MagicType;
			usespell.SpellX = interact.X;
			usespell.SpellY = interact.Y;
			usespell.SpellLevel = 0;
			
			Data.Spell spell = null;
			if (attacker is Entities.GameClient)
			{
				if (!(attacker as Entities.GameClient).IsAIBot)
				{
					if (!ProcessClient((attacker as Entities.GameClient), interact, usespell, out spell))
						return;
				}
				else
				{
					usespell.SpellLevel = interact.MagicLevel;
					spell = Core.Kernel.SpellInfos[usespell.SpellID][(byte)usespell.SpellLevel];
				}
			}
			else if (attacker is Entities.Monster)
			{
				if (Core.Kernel.SpellInfos.ContainsKey(interact.MagicType))
				{
					byte maxlevel = (byte)Core.Kernel.SpellInfos[interact.MagicType].Keys.Count;
					maxlevel--;
					if (Core.Kernel.SpellInfos[interact.MagicType].ContainsKey(maxlevel))
						spell = Core.Kernel.SpellInfos[interact.MagicType][maxlevel];
				}
			}
			// AI
			// Guards
			// Mobs with skills ??
			if (spell == null)
				return;
			
			if (interact.EntityUID != attacker.EntityUID && target != null)
			{
				usespell.SpellX = target.X;
				usespell.SpellY = target.Y;
			}
			
			if (spell.UseEP > 0)
			{
				if (attacker is Entities.GameClient)
				{
					if (!(attacker as Entities.GameClient).IsAIBot)
					{
						if (spell.ID == 7001)
						{
							if (!(attacker as Entities.GameClient).ContainsFlag1(Enums.Effect1.Riding))
							{
								if ((attacker as Entities.GameClient).Stamina < spell.UseEP)
									return;
							}
						}
						else
							if ((attacker as Entities.GameClient).Stamina < spell.UseEP)
								return;
					}
				}
			}
			
			if (spell.UseMP > 0)
			{
				
				if (attacker is Entities.Monster)
				{
					if (attacker.MP < spell.UseMP && ((byte)(attacker as Entities.Monster).Behaviour) < 3)
						return;
				}
				else if (attacker.MP < spell.UseMP)
					return;
			}
			
			bool success = false;
			uint damage = 0;
			if (attacker.Map == null)
				return;
			
			if (attacker.Map.MapType == Enums.MapType.Tournament)
			{
				if (interact.MagicType != 1045 && interact.MagicType != 1046)
					return;
			}
			
			bool singleSkill = false;
			switch (interact.MagicType)
			{
					#region Line Skills
				case 1045:
				case 1046:
				case 11005:
				case 11000:
					success = Skills.LineSkills.Handle(attacker, targets, interact, usespell, spell, out damage);
					break;
					#endregion
					#region SectorSkills
					
					#region Physical
				case 1250:
				case 5050:
				case 5020:
				case 1300:
					success = Skills.SectoreSkills.HandlePhy(attacker, targets, interact, usespell, spell, out damage);
					break;
					#endregion
					#region Magic
				case 1165:
				case 7014:
					success = Skills.SectoreSkills.HandleMag(attacker, targets, interact, usespell, spell, out damage);
					break;
					#endregion
					
					#endregion
					#region Single
					#region Magic
				case 10310:
				case 1000:
				case 1001:
				case 1002:
				case 1150:
				case 1160:
				case 1180:
					if (target == null)
						return;
					
					singleSkill = true;
					success = Skills.Single.HandleMag(attacker, (target as Entities.IEntity), interact, usespell, spell, out damage);
					break;
					#endregion
					#region Physical
				case 1290:
				case 5030:
				case 5040:
				case 7000:
				case 7010:
				case 7030:
				case 7040:
					if (target == null)
						return;
					
					singleSkill = true;
					success = Skills.Single.HandlePhys(attacker, (target as Entities.IEntity), interact, usespell, spell, out damage);
					break;
					#endregion
					#endregion
					#region Circle
					
					#region Physical
				case 5010:
				case 7020:
				case 1115://herc
					success = Skills.CircleSkills.HandlePhy(attacker, targets, interact, usespell, spell, out damage);
					break;
					#endregion
					#region Magic
				case 1010://lightning tao
				case 1120://fc
				case 1125://volc
				case 3090://pervade
				case 5001://speed
				case 8030://arrows
				case 7013://flame shower
				case 30011://small ice circle
				case 30012://large ice circle
				case 10360:
				case 10361:
				case 10392:
				case 10308:
					success = Skills.CircleSkills.HandleMag(attacker, targets, interact, usespell, spell, out damage);
					break;
					#endregion
					
					#endregion
					#region MountSkill
				case 7001:
					singleSkill = true;
					if (attacker is Entities.GameClient)
					{
						success = Skills.MountSkill.Handle((attacker as Entities.GameClient), usespell);
					}
					else
						success = false;
					break;
					#endregion
					#region Buff
				case 1075:
				case 1085:
				case 1090:
				case 1095:
					if (target == null)
						return;
					singleSkill = true;
					success = Skills.BuffSkills.Handle(attacker, (target as Entities.IEntity), interact, usespell, spell, out damage);
					break;
					#endregion
					#region Revive
				case 1050:
				case 1100:
					if (target == null)
						return;
					singleSkill = true;
					success = Skills.ReviveSkills.Handle(attacker, (target as Entities.IEntity), interact, usespell, spell, out damage);
					break;
					#endregion
					#region Fly
				case 8002:
				case 8003:
					singleSkill = true;
					success = Skills.FlySkill.Handle(attacker, (target as Entities.IEntity), interact, usespell, spell, out damage);
					break;
					#endregion
					#region Scatter
				case 8001:
					success = Skills.ScatterSkill.Handle(attacker, targets, interact, usespell, spell, out damage);
					break;
					#endregion
					#region Cure
					
					#region Self
				case 1190:
				case 1195:
				case 7016:
					if (target == null)
						return;
					singleSkill = true;
					success = Skills.CureSkills.HandleSelf(attacker, (target as Entities.IEntity), interact, usespell, spell, out damage);
					break;
					#endregion
					#region Surroundings
				case 1005:
				case 1055:
				case 1170:
				case 1175:
					if (target == null)
						return;
					singleSkill = true;
					success = Skills.CureSkills.HandleSurrounding(attacker, (target as Entities.IEntity), interact, usespell, spell, out damage);
					break;
					#endregion
					
					#endregion
					#region Archer
				case 10313:
				case 8000:
				case 9991:
				case 7012:
				case 7015:
				case 7017:
				case 1320:
					if (target == null)
						return;
					
					singleSkill = true;
					success = Skills.ArcherSkills.Handle(attacker, (target as Entities.IEntity), interact, usespell, spell, out damage);
					break;
					#endregion
					#region Ninja
					#region Toxic Fog
				case 6001:
					success = Skills.NinjaSkills.HandlePoison(attacker, targets, interact, usespell, spell, out damage);
					break;
					#endregion
					#region TwoFold
				case 6000:
					if (target == null)
						return;
					
					singleSkill = true;
					success = Skills.NinjaSkills.HandleTwoFold(attacker, (target as Entities.IEntity), interact, usespell, spell, out damage);
					break;
					#endregion
					#region PoisonStar
				case 6002:
					if (target == null)
						return;
					
					singleSkill = true;
					success = Skills.NinjaSkills.HandlePoisonStar(attacker, (target as Entities.IEntity), interact, usespell, spell, out damage);
					break;
					#endregion
					#region ArcherBane
				case 6004:
					if (target == null)
						return;
					
					singleSkill = true;
					success = Skills.NinjaSkills.HandleArcherBane(attacker, (target as Entities.IEntity), interact, usespell, spell, out damage);
					break;
					#endregion
					#endregion
					
				default:
					if (attacker is Entities.GameClient)
					{
						Entities.GameClient client = (attacker as Entities.GameClient);
						using (var fmsg = Packets.Message.MessageCore.CreateSystem2(client.Name, Core.MessageConst.INVALID_SKILL))
							client.Send(fmsg);
					}
					return;
			}
			if (success)
			{
				if (spell.UseEP > 0)
				{
					if (attacker is Entities.GameClient)
					{
						if ((attacker as Entities.GameClient).Stamina >= spell.UseEP)
							(attacker as Entities.GameClient).Stamina -= spell.UseEP;
					}
				}
				
				if (spell.UseMP > 0)
				{
					if (attacker.MP >= spell.UseMP)
						attacker.MP -= spell.UseMP;
				}
				
//				if (spell.UseItemNum == 0) // fix
//				{
//					if (attacker is Entities.GameClient)
//					{
//						Entities.GameClient client = attacker as Entities.GameClient;
//						if (!client.Equipments.Contains(Enums.ItemLocation.WeaponL))
//							return;
//						if (!client.Equipments[Enums.ItemLocation.WeaponL].IsArrow())
//							return;
//						if (client.Equipments[Enums.ItemLocation.WeaponL].CurrentDura == 0)
//						{
//							uint arrowtype = client.Equipments[Enums.ItemLocation.WeaponL].ItemID;
//							if (client.Inventory.ContainsByID(arrowtype))
//							{
//								Data.ItemInfo arrow = client.Inventory.SearchForNonEmpty(arrowtype);
//								if (arrow != null)
//								{
//									if (!client.Equipments.Equip(arrow, Enums.ItemLocation.WeaponL, true))
//									{
//										using (var fmsg = Packets.Message.MessageCore.CreateSystem2(client.Name, Core.MessageConst.AUTO_RELOAD_ARROW_FAIL))
//											client.Send(fmsg);
//										return;
//									}
//								}
//								else
//									return;
//							}
//							else if (!client.Equipments.Unequip(Enums.ItemLocation.WeaponL))
//							{
//								using (var fmsg = Packets.Message.MessageCore.CreateSystem2(client.Name, Core.MessageConst.EMPTY_ARROWS))
//									client.Send(fmsg);
//								return;
//							}
//							else
//								return; // no more arrows in inventory
//						}
//					}
//				}
//
				attacker.Screen.UpdateScreen(usespell);
				Entities.IEntity targetentity = target as Entities.IEntity;
				
				if (targetentity != null && singleSkill)
				{
					if (targetentity.EntityUID != attacker.EntityUID)
						Combat.HitDamage(attacker, targetentity, damage);
					else if (targets.Count > 0)
					{
						foreach (Entities.IEntity targete in targets)
							Combat.HitDamage(attacker, targete, damage);
					}
				}
				else if (targets.Count > 0)
				{
					foreach (Entities.IEntity targete in targets)
						Combat.HitDamage(attacker, targete, damage);
				}
				
				if (attacker is Entities.GameClient)
				{
					Entities.GameClient attackerclient = attacker as Entities.GameClient;
					attackerclient.Send(usespell);
					
					if (usespell.SpellID >= 1000 && usespell.SpellID <= 1002)
					{
						//interact.Data = 0;
						interact.ActivationType = 0;
						interact.ActivationValue = 0;
						attackerclient.AttackPacket = interact;
						attackerclient.UseAutoAttack(interact);
					}
				}

			}
		}
	}
}
