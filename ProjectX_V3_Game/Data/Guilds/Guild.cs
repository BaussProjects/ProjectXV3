//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;
using ProjectX_V3_Lib.ThreadSafe;
using ProjectX_V3_Lib.Extensions;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// Description of Guild.
	/// </summary>
	public class Guild
	{
		public Guild()
		{
			DeputyLeaders = new Selector<int, string, GuildMember>();
			Members = new Selector<int, string, GuildMember>();
			GuildID = Core.UIDGenerators.GetGuildUID();
		}
		
		public uint DatabaseID; // db only ...
		public uint GuildID;
		public string Bullentin;
		public string Name;
		public byte Level;
		public ulong MoneyDonation;
		public uint CPDonation;
		public GuildMember Leader;
		public Selector<int, string, GuildMember> DeputyLeaders;
		public Selector<int, string, GuildMember> Members;
		public string Allie1;
		public string Allie2;
		public string Allie3;
		public string Allie4;
		public string Allie5;
		public string Enemy1;
		public string Enemy2;
		public string Enemy3;
		public string Enemy4;
		public string Enemy5;
		
		public string StringInfo
		{
			get
			{
				return string.Format("{0} {1} {2} {3}", Name, Leader.Name, Level, Members.Count);
			}
		}
		
		public void ReorderIndex()
		{
			Leader.Index = 0;
			int CurrentIndex = 1;
			foreach (GuildMember deputy in DeputyLeaders.selectorCollection1.Values)
			{
				deputy.Index = CurrentIndex;
				CurrentIndex++;
			}
			foreach (GuildMember member in Members.selectorCollection1.Values)
			{
				if (member.Rank == Enums.GuildRank.Member)
				{
					member.Index = CurrentIndex;
					CurrentIndex++;
				}
			}
		}
		
		public GuildMember[] SelectFromIndex(int from_index)
		{
			ConcurrentBag<GuildMember> members = new ConcurrentBag<GuildMember>();
			foreach (GuildMember member in Members.selectorCollection1.Values)
			{
				if (member.Index >= from_index && member.Index < (from_index + 12))
					members.Add(member);
			}
			return members.ToArray();
		}
		
		public void SetBullentin(Entities.GameClient client, string bullentin)
		{
			this.Bullentin = bullentin.MakeReadable(true, true, true, false, false);
			
			Database.GuildDatabase.UpdateGuildInfo(this, "GuildBullentin", this.Bullentin);
			foreach (GuildMember member in Members.selectorCollection1.Values)
			{
				if (member.Online)
				{
					using (var announce = new Packets.GuildPacket(new Packets.StringPacker(this.Bullentin)))
					{
						announce.Data = Core.Kernel.TimeGet(Enums.TimeType.Day);
						announce.Action = Enums.GuildAction.SetAnnounce;
						member.Client.Send(announce);
					}
				}
			}
			
			SendMessage(string.Format(Core.MessageConst.BULLENTIN_UPDATE, client.Name));
		}
		
		public static bool Create(Entities.GameClient leader, string Name)
		{
			if (Database.GuildDatabase.Create(leader, Name))
			{
				leader.SendGuild();
				leader.Screen.FullUpdate();
				leader.GuildMemberInfo.Index = 0;
				
				using (var msg =Packets.Message.MessageCore.CreateSystem("ALL",
				                                                         string.Format(Core.MessageConst.NEW_GUILD, Name, leader.Name)))
				{
					Packets.Message.MessageCore.SendGlobalMessage(msg);
				}
				return true;
			}
			return false;
		}
		
		public void Disban()
		{
			if (Core.Kernel.Guilds.TryRemove(this.GuildID))
			{
				
				SendMessage(Core.MessageConst.GUILD_DISBAN);
				foreach (GuildMember member in Members.selectorCollection1.Values)
				{
					RemoveMember(member, false);
				}
				Members.Clear();
				DeputyLeaders.Clear();
				
				Guild allie;
				if (Core.Kernel.Guilds.TrySelect(Allie1, out allie))
					allie.UnprepareAllie(Name);
				if (Core.Kernel.Guilds.TrySelect(Allie2, out allie))
					allie.UnprepareAllie(Name);
				if (Core.Kernel.Guilds.TrySelect(Allie3, out allie))
					allie.UnprepareAllie(Name);
				if (Core.Kernel.Guilds.TrySelect(Allie4, out allie))
					allie.UnprepareAllie(Name);
				if (Core.Kernel.Guilds.TrySelect(Allie5, out allie))
					allie.UnprepareAllie(Name);
				
				foreach (Guild enemy in Core.Kernel.Guilds.selectorCollection1.Values)
				{
					if (enemy.IsEnemy(Name))
						enemy.UnprepareEnemy(Name);
				}
				
				Database.GuildDatabase.DeleteGuild(this);
			}
		}
		
		public void AddMember(Entities.GameClient client)
		{
			if (Members.Contains(client.DatabaseUID))
				client.GuildMemberInfo = Members[client.DatabaseUID];
			else
			{
				client.GuildMemberInfo = new GuildMember(client);
				if (Members.TryAdd(client.DatabaseUID, client.Name, client.GuildMemberInfo))
				{
					client.Guild = this;
					
					SendMessage(string.Format(Core.MessageConst.NEW_MEMBER, client.Name));
				}
				else
					client.GuildMemberInfo = null;
				
				Database.CharacterDatabase.Save(client);
			}
			
			ReorderIndex();
			client.SendGuild();
			client.Screen.FullUpdate();
		}
		
		public void RemoveMember(Data.GuildMember member, bool order = true)
		{
			if (Members.Contains(member.DatabaseUID))
			{
				if (Members.TryRemove(member.DatabaseUID))
				{
					RemoveDeputyLeader(member);
					
					if (member.Client != null)
					{
						member.Client.GuildMemberInfo = null;
						member.Client.Guild = null;
						
						Database.CharacterDatabase.Save(member.Client);
					}
					else
						Database.CharacterDatabase.RemoveGuild(member.DatabaseUID);
					
					SendMessage(string.Format(Core.MessageConst.LEAVE_GUILD, member.Name));
				}
			}
			
			if (member.Client != null)
			{
				member.Client.SendGuild();
				member.Client.Screen.FullUpdate();
			}
			
			if (order)
				ReorderIndex();
		}
		
		public void AddDeputyLeader(Data.GuildMember member)
		{
			if (DeputyLeaders.TryAdd(member.DatabaseUID, member.Name, member))
			{
				member.Rank = Enums.GuildRank.DeputyLeader;
				if (member.Client != null)
					Database.CharacterDatabase.Save(member.Client);
				else
					Database.CharacterDatabase.SaveGuildRank(member.DatabaseUID, member.Rank);
				
				SendMessage(string.Format(Core.MessageConst.NEW_DEPUTY_LEADER, member.Name));
			}
			
			if (member.Client != null)
			{
				member.Client.SendGuild();
				member.Client.Screen.FullUpdate();
			}
			
			ReorderIndex();
		}
		
		public void RemoveDeputyLeader(Data.GuildMember member)
		{
			if (DeputyLeaders.Contains(member.DatabaseUID))
			{
				if (DeputyLeaders.TryRemove(member.DatabaseUID))
				{
					member.Rank = Enums.GuildRank.Member;
					
					if (member.Client != null)
						Database.CharacterDatabase.Save(member.Client);
					else
						Database.CharacterDatabase.SaveGuildRank(member.DatabaseUID, member.Rank);
					
					SendMessage(string.Format(Core.MessageConst.DEPUTY_REMOVED, member.Name));
				}
			}
			
			if (member.Client != null)
			{
				member.Client.SendGuild();
				member.Client.Screen.FullUpdate();
			}
			
			ReorderIndex();
		}
		
		public void DonateMoney(Entities.GameClient client, uint Amount)
		{
			if (client.Money >= Amount)
			{
				client.Money -= Amount;
				MoneyDonation += Amount;
				client.GuildMemberInfo.MoneyDonation += Amount;
				Database.GuildDatabase.UpdateGuildInfo(this, "GuildFund", MoneyDonation);
				Database.CharacterDatabase.Save(client);
				client.SendGuild();
				
				SendMessage(string.Format(Core.MessageConst.GUILD_DONATE, client.Name, Amount, "Silvers"));
			}
		}
		
		public void DonateCPs(Entities.GameClient client, uint Amount)
		{
			if (client.CPs >= Amount)
			{
				client.CPs -= Amount;
				CPDonation += Amount;
				client.GuildMemberInfo.CPDonation += Amount;
				Database.GuildDatabase.UpdateGuildInfo(this, "GuildCPsFund", CPDonation);
				Database.CharacterDatabase.Save(client);
				client.SendGuild();
				
				SendMessage(string.Format(Core.MessageConst.GUILD_DONATE, client.Name, Amount, "CPs"));
			}
		}
		
		public void SendMessage(string Msg)
		{
			foreach (GuildMember member in Members.selectorCollection1.Values)
			{
				if (member.Online)
				{
					using (var msg = Packets.Message.MessageCore.Create(Enums.ChatType.Guild, System.Drawing.Color.Indigo, "SYSTEM", "ALL", Msg))
						member.Client.Send(msg);
				}
			}
		}
		
		public void BroadcastMessage(Packets.MessagePacket msg)
		{
			foreach (GuildMember member in Members.selectorCollection1.Values)
			{
				if (member.Online)
				{
					member.Client.Send(msg);
				}
			}
		}
		
		public void KickMember(string Name)
		{
			if (Members.Contains(Name))
			{
				GuildMember member;
				if (Members.TrySelect(Name, out member))
					RemoveMember(member);
			}
		}
		
		private string AddAllieName = string.Empty;
		
		public bool CanAddAllie()
		{
			bool CanAdd = false;
			if (string.IsNullOrWhiteSpace(Allie1))
				CanAdd = true;
			if (string.IsNullOrWhiteSpace(Allie2))
				CanAdd = true;
			if (string.IsNullOrWhiteSpace(Allie3))
				CanAdd = true;
			if (string.IsNullOrWhiteSpace(Allie4))
				CanAdd = true;
			if (string.IsNullOrWhiteSpace(Allie5))
				CanAdd = true;
			return CanAdd;
		}
		private void PrepareAllie(string GuildName)
		{
			bool AddedAllie = true;
			if (string.IsNullOrWhiteSpace(Allie1))
			{
				Allie1 = GuildName;
				Database.GuildDatabase.UpdateGuildInfo(this, "GuildAllie1", Allie1);
			}
			
			else if (string.IsNullOrWhiteSpace(Allie2))
			{
				Allie2 = GuildName;
				Database.GuildDatabase.UpdateGuildInfo(this, "GuildAllie2", Allie2);
			}
			
			else if (string.IsNullOrWhiteSpace(Allie3))
			{
				Allie3 = GuildName;
				Database.GuildDatabase.UpdateGuildInfo(this, "GuildAllie3", Allie3);
			}
			
			else if (string.IsNullOrWhiteSpace(Allie4))
			{
				Allie4 = GuildName;
				Database.GuildDatabase.UpdateGuildInfo(this, "GuildAllie4", Allie4);
			}
			
			else if (string.IsNullOrWhiteSpace(Allie5))
			{
				Allie5 = GuildName;
				Database.GuildDatabase.UpdateGuildInfo(this, "GuildAllie5", Allie5);
			}
			else
				AddedAllie = false;
			
			if (AddedAllie)
			{
				Data.Guild allie;
				if (Core.Kernel.Guilds.TrySelect(GuildName, out allie))
				{
					using (var guildpack = new Packets.GuildPacket(new Packets.StringPacker()))
					{
						guildpack.Data = allie.GuildID;
						guildpack.Action = Enums.GuildAction.SetAlly;
						foreach (GuildMember member in Members.selectorCollection1.Values)
						{
							if (member.Online)
								member.Client.Send(guildpack);
						}
					}
				}
			}
		}
		private void UnprepareAllie(string GuildName)
		{
			bool RemovedAllie = true;
			
			if (Allie1 == GuildName)
			{
				Allie1 = string.Empty;
				Database.GuildDatabase.UpdateGuildInfo(this, "GuildAllie1", string.Empty);
			}
			else if (Allie2 == GuildName)
			{
				Allie2 = string.Empty;
				Database.GuildDatabase.UpdateGuildInfo(this, "GuildAllie2", string.Empty);
			}
			else if (Allie3 == GuildName)
			{
				Allie3 = string.Empty;
				Database.GuildDatabase.UpdateGuildInfo(this, "GuildAllie3", string.Empty);
			}
			else if (Allie4 == GuildName)
			{
				Allie4 = string.Empty;
				Database.GuildDatabase.UpdateGuildInfo(this, "GuildAllie4", string.Empty);
			}
			else if (Allie5 == GuildName)
			{
				Allie5 = string.Empty;
				Database.GuildDatabase.UpdateGuildInfo(this, "GuildAllie5", string.Empty);
			}
			else
				RemovedAllie = false;
			
			if (RemovedAllie)
			{
				Data.Guild allie;
				if (Core.Kernel.Guilds.TrySelect(GuildName, out allie))
				{
					using (var guildpack = new Packets.GuildPacket(new Packets.StringPacker()))
					{
						guildpack.Data = allie.GuildID;
						guildpack.Action = Enums.GuildAction.ClearAlly;
						foreach (GuildMember member in Members.selectorCollection1.Values)
						{
							if (member.Online)
								member.Client.Send(guildpack);
						}
					}
				}
			}
		}
		
		public bool IsAllie(string GuildName)
		{
			if (Allie1 == GuildName)
				return true;
			if (Allie2 == GuildName)
				return true;
			if (Allie3 == GuildName)
				return true;
			if (Allie4 == GuildName)
				return true;
			if (Allie5 == GuildName)
				return true;
			return false;
		}
		public void AddAllie(string GuildName)
		{
			Guild allie;
			if (Core.Kernel.Guilds.TrySelect(GuildName, out allie))
			{
				if (allie.AddAllieName == Name)
				{
					PrepareAllie(GuildName);
					allie.PrepareAllie(Name);
					
					SendMessage(string.Format(Core.MessageConst.NEW_ALLIE, GuildName));
					allie.SendMessage(string.Format(Core.MessageConst.NEW_ALLIE, Name));
				}
				else
				{
					this.AddAllieName = GuildName;
				}
			}
		}
		
		public void RemoveAllie(string GuildName)
		{
			Guild allie;
			if (Core.Kernel.Guilds.TrySelect(GuildName, out allie))
			{
				UnprepareAllie(GuildName);
				allie.UnprepareAllie(Name);
				
				SendMessage(string.Format(Core.MessageConst.REMOVE_ALLIE, GuildName));
				allie.SendMessage(string.Format(Core.MessageConst.REMOVE_ALLIE, Name));
			}
		}
		
		public Guild[] GetAllies()
		{
			ConcurrentBag<Guild> allies = new ConcurrentBag<Guild>();
			if (!string.IsNullOrWhiteSpace(Allie1))
			{
				Guild allie;
				if (Core.Kernel.Guilds.TrySelect(Allie1, out allie))
					allies.Add(allie);
			}
			if (!string.IsNullOrWhiteSpace(Allie2))
			{
				Guild allie;
				if (Core.Kernel.Guilds.TrySelect(Allie2, out allie))
					allies.Add(allie);
			}
			if (!string.IsNullOrWhiteSpace(Allie3))
			{
				Guild allie;
				if (Core.Kernel.Guilds.TrySelect(Allie3, out allie))
					allies.Add(allie);
			}
			if (!string.IsNullOrWhiteSpace(Allie4))
			{
				Guild allie;
				if (Core.Kernel.Guilds.TrySelect(Allie4, out allie))
					allies.Add(allie);
			}
			if (!string.IsNullOrWhiteSpace(Allie5))
			{
				Guild allie;
				if (Core.Kernel.Guilds.TrySelect(Allie5, out allie))
					allies.Add(allie);
			}
			return allies.ToArray();
		}
		
		public bool CanAddEnemy()
		{
			bool CanAdd = false;
			if (string.IsNullOrWhiteSpace(Enemy1))
				CanAdd = true;
			if (string.IsNullOrWhiteSpace(Enemy2))
				CanAdd = true;
			if (string.IsNullOrWhiteSpace(Enemy3))
				CanAdd = true;
			if (string.IsNullOrWhiteSpace(Enemy4))
				CanAdd = true;
			if (string.IsNullOrWhiteSpace(Enemy5))
				CanAdd = true;
			return CanAdd;
		}
		private void PrepareEnemy(string GuildName)
		{
			bool AddedEnemy = true;
			if (string.IsNullOrWhiteSpace(Enemy1))
			{
				Enemy1 = GuildName;
				Database.GuildDatabase.UpdateGuildInfo(this, "GuildEnemy1", Enemy1);
			}
			
			else if (string.IsNullOrWhiteSpace(Enemy2))
			{
				Enemy2 = GuildName;
				Database.GuildDatabase.UpdateGuildInfo(this, "GuildEnemy2", Enemy2);
			}
			
			else if (string.IsNullOrWhiteSpace(Enemy3))
			{
				Enemy3 = GuildName;
				Database.GuildDatabase.UpdateGuildInfo(this, "GuildEnemy3", Enemy3);
			}
			
			else if (string.IsNullOrWhiteSpace(Enemy4))
			{
				Enemy4 = GuildName;
				Database.GuildDatabase.UpdateGuildInfo(this, "GuildEnemy4", Enemy4);
			}
			
			else if (string.IsNullOrWhiteSpace(Enemy5))
			{
				Enemy5 = GuildName;
				Database.GuildDatabase.UpdateGuildInfo(this, "GuildEnemy5", Enemy5);
			}
			else
				AddedEnemy = false;
			
			if (AddedEnemy)
			{
				Data.Guild enemy;
				if (Core.Kernel.Guilds.TrySelect(GuildName, out enemy))
				{
					using (var guildpack = new Packets.GuildPacket(new Packets.StringPacker()))
					{
						guildpack.Data = enemy.GuildID;
						guildpack.Action = Enums.GuildAction.SetEnemy;
						foreach (GuildMember member in Members.selectorCollection1.Values)
						{
							if (member.Online)
								member.Client.Send(guildpack);
						}
					}
				}
			}
		}
		private void UnprepareEnemy(string GuildName)
		{
			bool RemovedEnemy = true;
			
			if (Enemy1 == GuildName)
			{
				Enemy1 = string.Empty;
				Database.GuildDatabase.UpdateGuildInfo(this, "GuildEnemy1", string.Empty);
			}
			else if (Enemy2 == GuildName)
			{
				Enemy2 = string.Empty;
				Database.GuildDatabase.UpdateGuildInfo(this, "GuildEnemy2", string.Empty);
			}
			else if (Enemy3 == GuildName)
			{
				Enemy3 = string.Empty;
				Database.GuildDatabase.UpdateGuildInfo(this, "GuildEnemy3", string.Empty);
			}
			else if (Enemy4 == GuildName)
			{
				Enemy4 = string.Empty;
				Database.GuildDatabase.UpdateGuildInfo(this, "GuildEnemy4", string.Empty);
			}
			else if (Enemy5 == GuildName)
			{
				Enemy5 = string.Empty;
				Database.GuildDatabase.UpdateGuildInfo(this, "GuildEnemy5", string.Empty);
			}
			else
				RemovedEnemy = false;
			
			if (RemovedEnemy)
			{
				Data.Guild enemy;
				if (Core.Kernel.Guilds.TrySelect(GuildName, out enemy))
				{
					using (var guildpack = new Packets.GuildPacket(new Packets.StringPacker()))
					{
						guildpack.Data = enemy.GuildID;
						guildpack.Action = Enums.GuildAction.ClearEnemy;
						foreach (GuildMember member in Members.selectorCollection1.Values)
						{
							if (member.Online)
								member.Client.Send(guildpack);
						}
					}
				}
			}
		}
		
		public bool IsEnemy(string GuildName)
		{
			if (Enemy1 == GuildName)
				return true;
			if (Enemy2 == GuildName)
				return true;
			if (Enemy3 == GuildName)
				return true;
			if (Enemy4 == GuildName)
				return true;
			if (Enemy5 == GuildName)
				return true;
			return false;
		}
		public void AddEnemy(string GuildName)
		{
			AddAllieName = "";
			
			if (Core.Kernel.Guilds.Contains(GuildName))
			{
				PrepareEnemy(GuildName);
				SendMessage(string.Format(Core.MessageConst.NEW_ENEMY, GuildName));
			}
		}
		
		public void RemoveEnemy(string GuildName)
		{
			AddAllieName = "";
			UnprepareEnemy(GuildName);
			SendMessage(string.Format(Core.MessageConst.REMOVE_ENEMY, GuildName));
		}
		
		public Guild[] GetEnemies()
		{
			ConcurrentBag<Guild> enemies = new ConcurrentBag<Guild>();
			if (!string.IsNullOrWhiteSpace(Enemy1))
			{
				Guild enemy;
				if (Core.Kernel.Guilds.TrySelect(Enemy1, out enemy))
					enemies.Add(enemy);
			}
			if (!string.IsNullOrWhiteSpace(Enemy2))
			{
				Guild enemy;
				if (Core.Kernel.Guilds.TrySelect(Enemy2, out enemy))
					enemies.Add(enemy);
			}
			if (!string.IsNullOrWhiteSpace(Enemy3))
			{
				Guild enemy;
				if (Core.Kernel.Guilds.TrySelect(Enemy3, out enemy))
					enemies.Add(enemy);
			}
			if (!string.IsNullOrWhiteSpace(Enemy4))
			{
				Guild enemy;
				if (Core.Kernel.Guilds.TrySelect(Enemy4, out enemy))
					enemies.Add(enemy);
			}
			if (!string.IsNullOrWhiteSpace(Enemy5))
			{
				Guild enemy;
				if (Core.Kernel.Guilds.TrySelect(Enemy5, out enemy))
					enemies.Add(enemy);
			}
			return enemies.ToArray();
		}
	}
}
