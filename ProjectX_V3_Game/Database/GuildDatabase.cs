//Project by BaussHacker aka. L33TS

using System;
using System.IO;
using ProjectX_V3_Lib.Sql;
using ProjectX_V3_Lib.Extensions;
using System.Linq;

namespace ProjectX_V3_Game.Database
{
	/// <summary>
	/// Description of GuildDatabase.
	/// </summary>
	public class GuildDatabase
	{
		public static bool LoadGuilds()
		{
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.SELECT, false))
				{
					cmd.Finish("DB_Guilds");
				}
				while (sql.Read())
				{
					Data.Guild guild = new ProjectX_V3_Game.Data.Guild();
					guild.DatabaseID = sql.ReadUInt32("GuildID");
					guild.Name = sql.ReadString("GuildName");
					guild.Bullentin = sql.ReadString("GuildBullentin");
					guild.MoneyDonation = sql.ReadUInt64("GuildFund");
					guild.CPDonation = sql.ReadUInt32("GuildCPsFund");
					guild.Level = sql.ReadByte("GuildLevel");
					guild.Allie1 = sql.ReadString("GuildAllie1");
					guild.Allie2 = sql.ReadString("GuildAllie2");
					guild.Allie3 = sql.ReadString("GuildAllie3");
					guild.Allie4 = sql.ReadString("GuildAllie4");
					guild.Allie5 = sql.ReadString("GuildAllie5");
					guild.Enemy1 = sql.ReadString("GuildEnemy1");
					guild.Enemy2 = sql.ReadString("GuildEnemy2");
					guild.Enemy3 = sql.ReadString("GuildEnemy3");
					guild.Enemy4 = sql.ReadString("GuildEnemy4");
					guild.Enemy5 = sql.ReadString("GuildEnemy5");
					if (!LoadGuildMembers(guild))
					{
						return false;
					}
					if (!Core.Kernel.Guilds.TryAdd(guild.GuildID, guild.Name, guild))
					{
						return false;
					}
					guild.ReorderIndex();
				}
			}
			return true;
		}
		
		private static bool LoadGuildMembers(Data.Guild guild)
		{
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.SELECT, true))
				{
					cmd.AddWhereValue("PlayerGuild", guild.Name);
					cmd.Finish("DB_Players");
				}
				while (sql.Read())
				{
					Data.GuildMember member = new ProjectX_V3_Game.Data.GuildMember();
					member.DatabaseUID = sql.ReadInt32("PlayerID");
					member.JoinDate = Core.Kernel.TimeGet(Enums.TimeType.Day); //DateTime.Now;
					member.Name = sql.ReadString("PlayerName");
					member.Level = sql.ReadByte("PlayerLevel");
					member.MoneyDonation = sql.ReadUInt32("PlayerGuildDonation");
					member.CPDonation = sql.ReadUInt32("PlayerGuildCPDonation");
					member.Rank = (Enums.GuildRank)Enum.Parse(typeof(Enums.GuildRank), sql.ReadString("PlayerGuildRank"));
					if (!guild.Members.TryAdd(member.DatabaseUID, member.Name, member))
						return false;
					
					if (member.Rank == Enums.GuildRank.GuildLeader)
						guild.Leader = member;
					else if (member.Rank == Enums.GuildRank.DeputyLeader)
					{
						if (!guild.DeputyLeaders.TryAdd(member.DatabaseUID, member.Name, member))
							return false;
					}
				}
			}
			return true;
		}
		
		public static bool Create(Entities.GameClient leader, string Name)
		{
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.INSERT, false))
				{
					cmd.AddInsertValue("GuildName", Name);
					cmd.AddInsertValue("GuildBullentin", "");
					cmd.AddInsertValue("GuildFund", (ulong)0);
					cmd.AddInsertValue("GuildCPsFund", (uint)0);
					cmd.AddInsertValue("GuildLevel", (byte)0);
					cmd.AddInsertValue("GuildAllie1", "");
					cmd.AddInsertValue("GuildAllie2", "");
					cmd.AddInsertValue("GuildAllie3", "");
					cmd.AddInsertValue("GuildAllie4", "");
					cmd.AddInsertValue("GuildAllie5", "");
					cmd.AddInsertValue("GuildEnemy1", "");
					cmd.AddInsertValue("GuildEnemy2", "");
					cmd.AddInsertValue("GuildEnemy3", "");
					cmd.AddInsertValue("GuildEnemy4", "");
					cmd.AddInsertValue("GuildEnemy5", "");
					cmd.Finish("DB_Guilds");
				}
				sql.Execute();
			}
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.SELECT, true))
				{
					cmd.AddWhereValue("GuildName", Name);
					cmd.Finish("DB_Guilds");
				}
				if (!sql.Read())
					return false;
				
				Data.Guild newGuild = new ProjectX_V3_Game.Data.Guild();
				newGuild.DatabaseID = sql.ReadUInt32("GuildID");
				newGuild.Name = sql.ReadString("GuildName");
				newGuild.Bullentin = sql.ReadString("GuildBullentin");
				newGuild.MoneyDonation = sql.ReadUInt64("GuildFund");
				newGuild.CPDonation = sql.ReadUInt32("GuildCPsFund");
				newGuild.Level = sql.ReadByte("GuildLevel");
				
				if (Core.Kernel.Guilds.TryAdd(newGuild.GuildID, newGuild.Name, newGuild))
				{
					newGuild.Leader = new Data.GuildMember(leader);
					newGuild.Leader.Rank = Enums.GuildRank.GuildLeader;
					newGuild.Leader.JoinDate = Core.Kernel.TimeGet(Enums.TimeType.Day);
					newGuild.Members.TryAdd(leader.DatabaseUID, leader.Name, newGuild.Leader);
					leader.Guild = newGuild;
					leader.GuildMemberInfo = newGuild.Leader;
					leader.SendGuild();
					Database.CharacterDatabase.Save(leader);
					return true;
				}				
			}
			
			return false;
		}
		public static void DeleteGuild(Data.Guild guild)
		{
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.DELETE, true))
				{
					cmd.AddWhereValue("GuildName", guild.Name);
					cmd.Finish("DB_Guilds");
				}
				sql.Execute();
			}
		}
		
		public static void UpdateGuildInfo(Data.Guild guild, string Column, object Value)
		{
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.UPDATE, true))
				{
					cmd.AddWhereValue("GuildID", guild.DatabaseID);
					cmd.AddUpdateValue(Column, Value);
					cmd.Finish("DB_Guilds");
				}
				sql.Execute();
			}			
		}
	}
}
