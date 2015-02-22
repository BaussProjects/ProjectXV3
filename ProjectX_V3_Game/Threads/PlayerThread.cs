//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Threads
{
	/// <summary>
	/// Handling the player thread. AVOID ANY SLEEPS!
	/// </summary>
	public class PlayerThread
	{
		public static void Handle()
		{
			foreach (Entities.GameClient client in Core.Kernel.Clients.selectorCollection1.Values)
			{
				try
				{
					if (!client.LoggedIn)
						continue;
					if (!client.CanSave)
						continue;
					
					#region Remove PKPoints
					if (client.PKPoints > 0)
					{
						if (DateTime.Now >= client.LastPKRemove.AddMilliseconds(Core.TimeIntervals.PKPRemovalTime))
						{
							client.LastPKRemove = DateTime.Now;
							client.PKPoints -= 1;
							if (client.PKPoints == 0)
							{
								using (var msg = Packets.Message.MessageCore.CreateSystem(client.Name, Core.MessageConst.PK_POINTS_REMOVED))
									client.Send(msg);
							}
						}
					}
					#endregion
					#region Show Hawk Message
					if (client.Booth != null)
					{
						if (!string.IsNullOrWhiteSpace(client.Booth.HawkMessage))
						{
							if (DateTime.Now >= client.ShowHawkMessage.AddMilliseconds(Core.TimeIntervals.ShowHawkMessage))
							{
								using (var msg = Packets.Message.MessageCore.CreateHawk(client.Name, client.Booth.HawkMessage))
								{
									client.SendToScreen(msg, false, false);
								}
							}
						}
					}
					#endregion
					#region GuildWars
					if (client.Map.MapType == Enums.MapType.GuildWars)
					{
						if (client.Guild == null)
							client.Teleport(client.LastMapID, client.LastMapX, client.LastMapY);
					}
					#endregion
					#region Tournament / Raids
					if (client.Map.MapType == Enums.MapType.Tournament || client.Map.MapType == Enums.MapType.Dungeon)
					{
						if (client.Battle == null)
						{
							if (client.Map.MapType == Enums.MapType.Tournament || client.Map.MapType == Enums.MapType.Dungeon)
							{
								client.Teleport(1002, 400, 400);
								client.Teleport(1002, 400, 400); // stupid fix ... but whatever...
							}
						}
					}
					#endregion
					#region Poison
					if (client.ContainsFlag1(Enums.Effect1.Poisoned))
					{
						if (DateTime.Now >= client.LastPoison.AddMilliseconds(3000))
						{
							client.LastPoison = DateTime.Now;
							if (client.PoisonEffect > 0)
							{
								uint damage = (uint)((client.HP / 100) * client.PoisonEffect);
								if (client.HP > damage)
								{
									Packets.Interaction.Battle.Combat.HitDamage(null, client, damage);
									using (var interact = new Packets.InteractionPacket())
									{
										interact.Action = Enums.InteractAction.Attack;
										interact.EntityUID = client.EntityUID;
										interact.TargetUID = client.EntityUID;
										interact.UnPacked = true;
										interact.X = client.X;
										interact.Y = client.Y;
										interact.Data = damage;
										client.SendToScreen(interact, true, false);
									}
								}
								else
									client.RemoveFlag1(Enums.Effect1.Poisoned);
							}
						}
					}
					#endregion
					#region Fun
					#region Super Aids
					if (client.SuperAids)
					{
						if (Calculations.BasicCalculations.ChanceSuccess(5))
						{
							client.SuperAids = false;
							
							using (var msg = Packets.Message.MessageCore.CreateCenter(string.Format(Core.MessageConst.SUPER_AIDS, client.Name)))
							{
								Packets.Message.MessageCore.SendGlobalMessage(msg);
							}
							Packets.Interaction.Battle.Combat.Kill(null, client, (uint)client.HP);
						}
						else
						{
							uint damage = (uint)((client.HP / 100) * 3);
							client.HP -= (int)damage;
							
							Packets.Interaction.Battle.Combat.HitDamage(null, client, damage);
							using (var interact = new Packets.InteractionPacket())
							{
								interact.Action = Enums.InteractAction.Attack;
								interact.EntityUID = client.EntityUID;
								interact.TargetUID = client.EntityUID;
								interact.UnPacked = true;
								interact.X = client.X;
								interact.Y = client.Y;
								interact.Data = damage;
								client.SendToScreen(interact, true, false);
							}
						}
					}
					#endregion
					#endregion
				}
				catch { client.NetworkClient.Disconnect("THREAD_FAIL"); }
			}
		}
	}
}
