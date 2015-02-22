//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.IO;
using System.IO;
using ProjectX_V3_Lib.Extensions;
using ProjectX_V3_Lib.Sql;

namespace ProjectX_V3_Game.Database
{
	/// <summary>
	/// Monster database.
	/// </summary>
	public class MonsterDatabase
	{
		public static bool LoadMonsterInfo()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("\tLoading Monsters...");
			
			using (var mob = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(mob, SqlCommandType.SELECT, false))
				{
					cmd.Finish("DB_MobInfo");
				}
				while (mob.Read())
				{
					int mobid = mob.ReadInt32("MobID");
					Entities.Monster monster = new Entities.Monster();
					monster.Name = mob.ReadString("Name");
					monster.Level = mob.ReadByte("MobLevel");
					monster.Mesh = mob.ReadUInt32("Lookface");
					monster.MinAttack = mob.ReadUInt32("MinAttack");
					monster.MaxAttack = mob.ReadUInt32("MaxAttack");
					monster.Defense = mob.ReadUInt32("Defense");
					monster.Dexterity = mob.ReadByte("Dexterity");
					monster.Dodge = mob.ReadByte("Dodge");
					monster.AttackRange = mob.ReadInt32("AttackRange");
					monster.ViewRange = mob.ReadInt32("ViewRange");
					monster.AttackSpeed = mob.ReadInt32("AttackSpeed");
					monster.MoveSpeed = mob.ReadInt32("MoveSpeed");
					if (monster.MoveSpeed < 100)
						monster.MoveSpeed = 100;
					if (monster.MoveSpeed > 5000)
						monster.MoveSpeed = 5000;
					monster.AttackType = mob.ReadInt32("AttackType");
					monster.Behaviour = (Enums.MonsterBehaviour)Enum.Parse(typeof(Enums.MonsterBehaviour), mob.ReadString("Behaviour"));
					monster.MagicType = mob.ReadInt32("MagicType");
					monster.MagicDefense = mob.ReadInt32("MagicDefense");
					monster.MagicHitRate = mob.ReadInt32("MagicHitRate");
					monster.ExtraExperience = mob.ReadUInt64("ExtraExp");
					monster.ExtraDamage = mob.ReadUInt32("ExtraDamage");
					monster.Boss = (mob.ReadByte("Boss") != 0);
					monster.Action = mob.ReadUInt32("Action");
					monster.MaxHP = mob.ReadInt32("Life");
					
					monster.HP = monster.MaxHP;
					monster.MaxMP = mob.ReadInt32("Mana");
					monster.MP = monster.MaxMP;
					
					if (monster.Boss)
					{
						monster.AttackRange = 20;
					}
					
					string skillstring = mob.ReadString("Skills");
					if (!string.IsNullOrWhiteSpace(skillstring))
					{
						int[] ids = new int[0];
						skillstring.Split(',').ConverToInt32(out ids);
						if (ids[0] != 0)
						{
							foreach (int skillid in ids)
								monster.Skills.Add((ushort)skillid);
						}
					}
					
					if (!Core.Kernel.Monsters.TryAdd(mobid, monster))
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("Failed to load monster. Failed at ID: {0}", mobid);
						Console.ResetColor();
						return false;
					}
				}
			}

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\tLoaded {0} Monsters...", Core.Kernel.Monsters.Count);
			return true;
		}
		
		public static bool LoadMonsterSpawns()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			string wrt = "\tLoading Monster Spawns...";
			Console.WriteLine(wrt);
			int SpawnCount = 0;
			int GuardSpawnCount = 0;
			
			using (var spawn = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(spawn, SqlCommandType.SELECT, false))
				{
					cmd.Finish("DB_MobSpawns");
				}
				while (spawn.Read())
				{
					ushort mapid = spawn.ReadUInt16("MapID");
					
					Maps.Map map;
					if (!Core.Kernel.Maps.TrySelect(mapid, out map))
					{
						return false;
					}

					ushort CenterX = spawn.ReadUInt16("CenterX");
					ushort CenterY = spawn.ReadUInt16("CenterY");
					int Range = spawn.ReadUInt16("Range");
				//	int DropData = spawn.ReadInt32("DropID");
					int Monster = spawn.ReadInt32("MonsterID");
					int MobCount = spawn.ReadInt32("Count");
					
					if (CenterX > 0 && CenterY > 0 && Range > 0 && Monster > 0 && MobCount > 0)
					{
						for (int j = 0; j < MobCount; j++)
						{
							Entities.Monster spawnmob = Core.Kernel.Monsters[Monster].Copy();
							spawnmob.MobID = Monster;
							spawnmob.Direction = (byte)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(0, 8);
							if (!map.EnterMap(spawnmob))
							{
								Console.ForegroundColor = ConsoleColor.Red;
								Console.WriteLine("[MAP] Failed to load spawns. Failed at Map: {0} and MobID: {1}", mapid, Monster);
								Console.ResetColor();
								return false;
							}
							Maps.MapPoint Location = spawnmob.Map.CreateAvailableLocation<Entities.Monster>(CenterX, CenterY, Range);
							if (Location != null)
							{
								spawnmob.X = Location.X;
								spawnmob.Y = Location.Y;
								spawnmob.OriginalRange = Range;
								spawnmob.OriginalX = CenterX;
								spawnmob.OriginalY = CenterY;
								
								spawnmob.DropData = Core.Kernel.DropData[map.MapID].Copy();
								
								if (((byte)spawnmob.Behaviour) < 3 || spawnmob.Behaviour == Enums.MonsterBehaviour.PhysicalGuard) // physical guards should walk around
								{
									#if SPAWN_MOBS
									Threads.MonsterThread.AddToMonsterThread(spawnmob, false);
									SpawnCount++;
									#endif
								}
								else
								{
									Threads.MonsterThread.AddToMonsterThread(spawnmob, true);
									GuardSpawnCount++;
								}
							}
							else
								spawnmob.Map.LeaveMap(spawnmob); // there was no location available
							
							Console.Clear();
							Console.WriteLine(wrt);
							Console.WriteLine("\tLoaded {0} Monster Spawns and {1} Guard Spawns...", SpawnCount, GuardSpawnCount);
						}
					}
				}
			}
			return true;
		}
		
		public static void AddNewMonsterSpawn(ushort mapid, ushort x, ushort y, int monsterid, int count, int drop, int range, bool guard)
		{
			using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.INSERT, false))
				{
					cmd.AddInsertValue("MonsterID", monsterid);
					cmd.AddInsertValue("CenterX", x);
					cmd.AddInsertValue("CenterY", y);
					cmd.AddInsertValue("Drop", drop);
					cmd.AddInsertValue("Count", count);
					if (guard)
						cmd.AddInsertValue("Range", (int)1);
					else
						cmd.AddInsertValue("Range", range);
					cmd.Finish("DB_MobSpawns");
				}
				sql.Execute();
			}
			
			for (int i = 0; i < count; i++)
			{
				Entities.Monster spawnmob = Core.Kernel.Monsters[monsterid].Copy();
				spawnmob.Direction = (byte)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(0, 8);
				if (Core.Kernel.Maps[mapid].EnterMap(spawnmob))
				{
					Maps.MapPoint Location = spawnmob.Map.CreateAvailableLocation<Entities.Monster>(x, y, range);
					if (Location != null)
					{
						spawnmob.X = Location.X;
						spawnmob.Y = Location.Y;
						spawnmob.OriginalRange = range;
						spawnmob.OriginalX = x;
						spawnmob.OriginalY = y;
						
						//if (drop > 0)
						//{
						//	spawnmob.DropData = Core.Kernel.DropData[drop].Copy();
						//}
						
						Threads.MonsterThread.AddToMonsterThread(spawnmob, !(((byte)spawnmob.Behaviour) < 3 || spawnmob.Behaviour == Enums.MonsterBehaviour.PhysicalGuard));
						
						spawnmob.Screen.UpdateScreen(null);
					}
				}
			}
		}
	}
}
