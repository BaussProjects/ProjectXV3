//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Interaction.Battle
{
	/// <summary>
	/// Subtypes: ?
	/// </summary>
	public class Combat
	{
		/// <summary>
		/// Handling the combat of the interact packet.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="interact">The interact packet.</param>
		public static void Handle(Entities.GameClient client, InteractionPacket interact)
		{
			if (interact == null)
				return;
			
			if (!client.Alive)
				return;
			
			if (client.Paralyzed)
				return;
			
			if (!client.CanAttack)
				return;
			
			if (!(DateTime.Now >= client.LoginProtection.AddSeconds(10)))
			{
				using (var fmsg = Packets.Message.MessageCore.CreateSystem2(client.Name, Core.MessageConst.REST))
					client.Send(fmsg);
				return;
			}
			if (!(DateTime.Now >= client.ReviveProtection.AddSeconds(5)))
			{
				using (var fmsg = Packets.Message.MessageCore.CreateSystem2(client.Name, Core.MessageConst.REST))
					client.Send(fmsg);
				return;
			}
			
			if (!(DateTime.Now >= client.LastAttack.AddMilliseconds(Core.TimeIntervals.AttackInterval)) && client.AttackPacket == null)
			{
				using (var fmsg = Packets.Message.MessageCore.CreateSystem2(client.Name, Core.MessageConst.REST))
					client.Send(fmsg);
				return;
			}
			
			if (client.Battle != null)
			{
				if (!client.Battle.HandleBeginAttack(client))
				{
					using (var fmsg = Packets.Message.MessageCore.CreateSystem2(client.Name, Core.MessageConst.REST))
						client.Send(fmsg);
					return;
				}
			}
			client.LastAttack = DateTime.Now;
			//client.AutoAttacking = false;
			switch (interact.Action)
			{
				case Enums.InteractAction.MagicAttack:
					{
						#region TemporaryDecryption
						if (!interact.UnPacked)
						{
							interact.UnPacked = true;
							
							byte[] packet = interact.Copy();
							ushort SkillId = Convert.ToUInt16(((long)packet[24] & 0xFF) | (((long)packet[25] & 0xFF) << 8));
							SkillId ^= (ushort)0x915d;
							SkillId ^= (ushort)client.EntityUID;
							SkillId = (ushort)(SkillId << 0x3 | SkillId >> 0xd);
							SkillId -= 0xeb42;

							uint Target = ((uint)packet[12] & 0xFF) | (((uint)packet[13] & 0xFF) << 8) | (((uint)packet[14] & 0xFF) << 16) | (((uint)packet[15] & 0xFF) << 24);
							Target = ((((Target & 0xffffe000) >> 13) | ((Target & 0x1fff) << 19)) ^ 0x5F2D2463 ^ client.EntityUID) - 0x746F4AE6;

							ushort TargetX = 0;
							ushort TargetY = 0;
							long xx = (packet[16] & 0xFF) | ((packet[17] & 0xFF) << 8);
							long yy = (packet[18] & 0xFF) | ((packet[19] & 0xFF) << 8);
							xx = xx ^ (client.EntityUID & 0xffff) ^ 0x2ed6;
							xx = ((xx << 1) | ((xx & 0x8000) >> 15)) & 0xffff;
							xx |= 0xffff0000;
							xx -= 0xffff22ee;
							yy = yy ^ (client.EntityUID & 0xffff) ^ 0xb99b;
							yy = ((yy << 5) | ((yy & 0xF800) >> 11)) & 0xffff;
							yy |= 0xffff0000;
							yy -= 0xffff8922;
							TargetX = Convert.ToUInt16(xx);
							TargetY = Convert.ToUInt16(yy);
							
							interact.TargetUID = Target;
							interact.MagicType = SkillId;
							interact.X = TargetX;
							interact.Y = TargetY;
						}
						#endregion
						
						if (client.ContainsFlag1(Enums.Effect1.Riding) && interact.MagicType != 7001)
						{
							if (client.Stamina >= 100)
							{
								client.Stamina = 0;
								client.RemoveFlag1(Enums.Effect1.Riding);
							}
							return;
						}
						
						Magic.Handle(client, interact);
						break;
					}
				case Enums.InteractAction.Attack:
					if (client.ContainsFlag1(Enums.Effect1.Riding))
					{
						if (client.Stamina >= 100)
						{
							client.Stamina = 0;
							client.RemoveFlag1(Enums.Effect1.Riding);
						}
						return;
					}
					Physical.Handle(client, interact);
					break;
				case Enums.InteractAction.Shoot:
					if (client.ContainsFlag1(Enums.Effect1.Riding))
					{
						if (client.Stamina >= 100)
						{
							client.Stamina = 0;
							client.RemoveFlag1(Enums.Effect1.Riding);
						}
						return;
					}
					Ranged.Handle(client, interact);
					break;
			}
		}
		
		/// <summary>
		/// Processing damage.
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="attacked">The attacked.</param>
		/// <param name="damage">The damage.</param>
		public static void ProcessDamage(Entities.IEntity attacker, Entities.IEntity attacked, ref uint damage, bool kill_damage = true)
		{
			// Removed dura, bugged atm. the dura works, although client doesn't update proper etc. CBA to fix it as I never intended to use it
			// Although you can go ahead and fix it yourself if you want, I may do it if I ever feel like it
			// The dura lose is commented out below.
			#region ATTACKER : GAMECLIENT
			if (attacker is Entities.GameClient)
			{
				Entities.GameClient attackerclient = attacker as Entities.GameClient;
				attackerclient.LoseAttackDura(damage);
				
				#region ATTACKED : GAMECLIENT
				if (attacked is Entities.GameClient)
				{
					Entities.GameClient attackedclient = attacked as Entities.GameClient;
					// tournament check, damage = 1 + return
					if (attackerclient.Battle != null)
					{
						if (!attackerclient.Battle.HandleAttack(attackerclient, attackedclient, ref damage))
						{
							damage = 0;
							return;
						}
					}
					else
					{
						if (attacked.Map.GotKillCons() && !attackedclient.ContainsFlag1(Enums.Effect1.BlueName) && !attackedclient.ContainsFlag1(Enums.Effect1.RedName) && !attackedclient.ContainsFlag1(Enums.Effect1.BlackName))
						{
							attackerclient.AddStatusEffect1(Enums.Effect1.BlueName, 20000);
						}
						
						attackedclient.LoseDefenseDura(damage);
					}
				}
				#endregion
				#region ATTACKED : MONSTER
				if (attacked is Entities.Monster)
				{
					Entities.Monster attackedmob = attacked as Entities.Monster;
					
					if (((byte)attackedmob.Behaviour) >= 3)
					{
						attackerclient.AddStatusEffect1(Enums.Effect1.BlueName, 20000);
					}
					if (damage > 0)
					{
						ulong exp = (ulong)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)(damage / 2), (int)damage);
						if (attacked.Level > (attacker.Level + 10))
							exp *= 2;
						else if (attacker.Level > (attacked.Level + 10))
							exp = (ulong)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(1, (int)attackedmob.Level);
						attackerclient.AddExp(exp);
					}
				}
				#endregion
				//if (Calculations.Battle.LoseDuraAttak())
				//attackerclient.LoseAttackDura(damage);
			}
			#endregion
			#region ATTACKER : MONSTER
			
			#region ATTACKED : GAMECLIENT
			if (attacked is Entities.GameClient)
			{
				Entities.GameClient attackedclient = attacked as Entities.GameClient;

				attackedclient.LoseDefenseDura(damage);
			}
			#endregion
			
			#endregion
			
			if (kill_damage)
			{
				HitDamage(attacker, attacked, damage);
			}
		}
		
		public static void HitDamage(Entities.IEntity attacker, Entities.IEntity attacked, uint damage)
		{
			attacked.HP -= (int)damage;
			if (attacked.HP <= 0)
			{
				Kill(attacker, attacked, damage);
			}
		}
		
		/// <summary>
		/// Kills an entity.
		/// </summary>
		/// <param name="attacker">The attacker.</param>
		/// <param name="attacked">The attacked.</param>
		public static void Kill(Entities.IEntity attacker, Entities.IEntity attacked, uint damage = 0)
		{
			attacked.HP = 0;
			attacked.Alive = false;
			
			if (attacked is Entities.Monster)
			{
				(attacked as Entities.Monster).Kill(attacker, damage);
				if (attacker is Entities.GameClient)
				{
					if ((attacker as Entities.GameClient).Battle != null)
						(attacker as Entities.GameClient).Battle.KillMob((attacker as Entities.GameClient), attacked.EntityUID);
				}
			}
			
			using (var killpacket = new Packets.InteractionPacket())
			{
				killpacket.Action = Enums.InteractAction.Kill;
				killpacket.TargetUID = attacked.EntityUID;
				killpacket.X = attacked.X;
				killpacket.Y = attacked.Y;
				killpacket.Data = 1;
				
				if (attacker != null)
				{
					killpacket.EntityUID = attacker.EntityUID;

					attacker.Screen.UpdateScreen(killpacket);
					if (attacker is Entities.GameClient)
						(attacker as Entities.GameClient).Send(killpacket);
				}
				else
				{
					killpacket.EntityUID = 0;
					attacked.Screen.UpdateScreen(killpacket);
					if (attacked is Entities.GameClient)
						(attacked as Entities.GameClient).Send(killpacket);
				}
			}
			
			if (attacked is Entities.GameClient)
			{
				Entities.GameClient attackedclient = attacked as Entities.GameClient;
				attackedclient.RemoveFlag1(Enums.Effect1.Fly);
				attackedclient.RemoveFlag1(Enums.Effect1.Invisible);
				
				if (attacker != null)
				{
					if (attacked.EntityUID != attacker.EntityUID)
					{
						if (attacker is Entities.GameClient)
						{
							if ((attacker as Entities.GameClient).Battle != null)
							{
								if (!(attacker as Entities.GameClient).Battle.HandleDeath((attacker as Entities.GameClient), attackedclient))
									return;
							}
							else if (attacked.Map.GotKillCons())
							{
								Entities.GameClient attackerclient = attacker as Entities.GameClient;
								if (attackedclient.Guild != null && attackerclient.Guild != null)
								{
									if (attackerclient.Guild.IsEnemy(attackedclient.Guild.Name))
										attackerclient.PKPoints += 3;
									else
										attackerclient.PKPoints += 10;
								}
								else
									attackerclient.PKPoints += 10;
							}
						}
					}
				}
				attackedclient.AttackPacket = null;
				attackedclient.ReviveTime = DateTime.Now.AddSeconds(20);
				attackedclient.AddStatusEffect1(Enums.Effect1.Dead);
				attackedclient.AddStatusEffect1(Enums.Effect1.Ghost);
				attackedclient.RemoveFlag1(Enums.Effect1.BlueName);
				attackedclient.Stamina = 0;
				
				attackedclient.Transformation = Calculations.BasicCalculations.GetGhostTransform(attackedclient.Model);
			}
			
			if (attacked is Entities.BossCreature)
			{
				Entities.BossCreature creature = attacked as Entities.BossCreature;
				creature.Abort();
			}
			else if (attacked is Entities.BossMonster)
			{
				Entities.BossMonster boss = attacked as Entities.BossMonster;
				boss.AbortBoss(false);
				if (attacker is Entities.GameClient)
					boss.ON_DEATH(attacker as Entities.GameClient);
			}
		}
		
		public static bool FixTarget(Entities.IEntity attacker, Entities.IEntity target)
		{
			if (attacker is Entities.GameClient && target is Entities.GameClient)
			{
				if ((attacker as Entities.GameClient).PKMode == Enums.PKMode.Team)
				{
					if ((attacker as Entities.GameClient).Team != null)
					{
						if ((attacker as Entities.GameClient).Team.Members.ContainsKey(target.EntityUID))
							return false;
					}
					else if ((attacker as Entities.GameClient).Guild != null)
					{
						if ((attacker as Entities.GameClient).Guild.Members.Contains((target as Entities.GameClient).DatabaseUID))
							return false;
						else if ((attacker as Entities.GameClient).Guild != null)
						{
							if ((attacker as Entities.GameClient).Guild.IsAllie((target as Entities.GameClient).Guild.Name))
								return false;
						}
					}
				}
			}
			return true;
		}
	}
}
