//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Sql;

namespace ProjectX_V3_Game.Entities
{
	/// <summary>
	/// Description of AIBot.
	/// </summary>
	public class AIBot
	{
		private int JumpSpeed = 0;
		private int ShootChance = 0;
		private int Accuracy = 0;
		
		public void SetLevel(Enums.BotLevel Level)
		{
			switch (Level)
			{
				case Enums.BotLevel.Noob:
					JumpSpeed = 3000;
					ShootChance = 10;
					Accuracy = 5;
					break;
					
				case Enums.BotLevel.Easy:
					JumpSpeed = 1500;
					ShootChance = 25;
					Accuracy = 10;
					break;
					
				case Enums.BotLevel.Normal:
					JumpSpeed = 1250;
					ShootChance = 33;
					Accuracy = 20;
					break;
					
				case Enums.BotLevel.Medium:
					JumpSpeed = 1000;
					ShootChance = 50;
					Accuracy = 33;
					break;
					
				case Enums.BotLevel.Hard:
					JumpSpeed = 1000;
					ShootChance = 75;
					Accuracy = 50;
					break;
					
				case Enums.BotLevel.Insane:
					JumpSpeed = 1000;
					ShootChance = 90;
					Accuracy = 80;
					break;
			}
		}
		private Entities.GameClient Original;
		private Entities.IEntity Target;
		public AIBot()
		{
			Original = new GameClient();
			SetLevel(Enums.BotLevel.Normal);
		}
		
		private ushort MaxX = 0, MaxY = 0, MinX = 0, MinY = 0;
		public void SetMinMaxLoc(ushort maxX, ushort maxY, ushort minX, ushort minY)
		{
			MaxX = maxX;
			MaxY = maxY;
			MinX = minX;
			MinY = minY;
		}
		
		public void LoadBot(Enums.BotType BotType, int botid, Maps.MapPoint location, Entities.GameClient Opponent)
		{
			switch (BotType)
			{
					#region afk bot
				case Enums.BotType.AFKBot:
					{
						using (var sql = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
						{
							using (var cmd = new SqlCommandBuilder(sql, SqlCommandType.SELECT, true))
							{
								cmd.AddWhereValue("BotID", botid);
								cmd.Finish("DB_Bots");
							}

							if (!sql.Read())
								return;
							
							Original.Name = Core.NameGenerator.GetName();
							if (string.IsNullOrEmpty(Original.Name) || string.IsNullOrWhiteSpace(Original.Name))
								return;
							
							Original.Avatar = sql.ReadUInt16("BotAvatar");
							Original.Model = sql.ReadUInt16("BotModel");
							Original.HairStyle = sql.ReadUInt16("BotHairStyle");
							Original.Transformation = sql.ReadUInt16("BotTransformation");
							Original.Strength = sql.ReadUInt16("BotStrength");
							Original.Agility = sql.ReadUInt16("BotAgility");
							Original.Vitality = sql.ReadUInt16("BotVitality");
							Original.Spirit = sql.ReadUInt16("BotSpirit");
							Original.PKPoints = sql.ReadInt16("BotPKPoints");
							Original.Level = sql.ReadByte("BotLevel");
							Original.Class = (Enums.Class)Enum.Parse(typeof(Enums.Class), sql.ReadString("BotClass"));
							Original.PlayerTitle = (Enums.PlayerTitle)Enum.Parse(typeof(Enums.PlayerTitle), sql.ReadString("BotTitle"));
							Original.Reborns = sql.ReadByte("BotReborns");

							Maps.Map map = location.Map;
							Original.Map = map;
							Original.LastMapID = Original.Map.MapID;
							Original.X = location.X;
							Original.Y = location.Y;
							Original.LastMapX = location.X;
							Original.LastMapY = location.Y;
							Original.LastX = location.X;
							Original.LastY = location.Y;
							
							Original.Action = Enums.ActionType.Sit;
							Original.Direction = (byte)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(8);
							uint entityuid = (uint)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(700000000, 999999999);
							Original.EntityUID = entityuid;
							
							if (!Original.Map.EnterMap(Original))
								return;
							
							uint WeaponR = sql.ReadUInt32("BotWeaponR");
							if (WeaponR > 0)
							{
								Data.ItemInfo item = Core.Kernel.ItemInfos[WeaponR].Copy();
								Original.Equipments.Equip(item, Enums.ItemLocation.WeaponR, false, false);
							}
							uint WeaponL = sql.ReadUInt32("BotWeaponL");
							if (WeaponL > 0)
							{
								Data.ItemInfo item = Core.Kernel.ItemInfos[WeaponL].Copy();
								Original.Equipments.Equip(item, Enums.ItemLocation.WeaponL, false, false);
							}
							uint Armor = sql.ReadUInt32("BotArmor");
							if (Armor > 0)
							{
								Data.ItemInfo item = Core.Kernel.ItemInfos[Armor].Copy();
								Original.Equipments.Equip(item, Enums.ItemLocation.Armor, false, false);
							}
							uint Head = sql.ReadUInt32("BotHead");
							if (Head > 0)
							{
								Data.ItemInfo item = Core.Kernel.ItemInfos[Head].Copy();
								Original.Equipments.Equip(item, Enums.ItemLocation.Head, false, false);
							}
							bool UseSteed = sql.ReadBoolean("BotSteed");
							if (UseSteed)
							{
								uint Steed = sql.ReadUInt32("BotSteedColor");
								Data.ItemInfo item = Core.Kernel.ItemInfos[300000].Copy();
								item.SocketAndRGB = Steed;
								Original.Equipments.Equip(item, Enums.ItemLocation.Steed, false, false);
								
								uint MountArmor = sql.ReadUInt32("BotMountArmor");
								if (MountArmor > 0)
								{
									Data.ItemInfo item2 = Core.Kernel.ItemInfos[MountArmor].Copy();
									Original.Equipments.Equip(item2, Enums.ItemLocation.SteedArmor, false, false);
								}
								
								Original.Action = Enums.ActionType.None;
								Original.AddStatusEffect1(Enums.Effect1.Riding);
							}
							uint Garment = sql.ReadUInt32("BotGarment");
							if (Garment > 0)
							{
								Data.ItemInfo item = Core.Kernel.ItemInfos[Garment].Copy();
								Original.Equipments.Equip(item, Enums.ItemLocation.Garment, false, false);
							}
							Original.LoggedIn = true;
						}
						break;
					}
					#endregion*
					#region duel bot
				case Enums.BotType.DuelBot:
					{
						if (Opponent == null)
							return;
						
						Original.Name = Core.NameGenerator.GetName();
						if (string.IsNullOrEmpty(Original.Name) || string.IsNullOrWhiteSpace(Original.Name))
							return;
						
						Original.Avatar = Opponent.Avatar;
						Original.Model = Opponent.Model;
						Original.HairStyle = Opponent.HairStyle;
						Original.Transformation = 0;
						Original.Strength = Opponent.Strength;
						Original.Agility = Opponent.Agility;
						Original.Vitality = Opponent.Vitality;
						Original.Spirit = Opponent.Spirit;
						Original.PKPoints = Opponent.PKPoints;
						Original.Level = Opponent.Level;
						Original.Class = Opponent.Class;
						Original.PlayerTitle = Opponent.PlayerTitle;
						Original.Reborns = Opponent.Reborns;

						Maps.Map map = Opponent.Map;
						Original.Map = map;
						Original.LastMapID = Original.Map.MapID;
						Original.X = Opponent.X;
						Original.Y = Opponent.Y;
						Original.LastMapX = Opponent.X;
						Original.LastMapY = Opponent.Y;
						Original.LastX = Opponent.X;
						Original.LastY = Opponent.Y;
						
						Original.Action = Enums.ActionType.None;
						Original.Direction = (byte)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(8);
						uint entityuid = (uint)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(700000000, 999999999);
						Original.EntityUID = entityuid;
						
						if (!Original.Map.EnterMap(Original))
							return;
						
						Original.Equipments.ForceEquipments(Opponent.Equipments);
						
						Original.BaseEntity.CalculateBaseStats();
						Original.HP = Original.MaxHP;
						Original.MP = Original.MaxMP;
						
						/*
							uint WeaponR = sql.ReadUInt32("BotWeaponR");
							if (WeaponR > 0)
							{
								Data.ItemInfo item = Core.Kernel.ItemInfos[WeaponR].Copy();
								Original.Equipments.Equip(item, Enums.ItemLocation.WeaponR, false, false);
							}
							uint WeaponL = sql.ReadUInt32("BotWeaponL");
							if (WeaponL > 0)
							{
								Data.ItemInfo item = Core.Kernel.ItemInfos[WeaponL].Copy();
								Original.Equipments.Equip(item, Enums.ItemLocation.WeaponL, false, false);
							}
							uint Armor = sql.ReadUInt32("BotArmor");
							if (Armor > 0)
							{
								Data.ItemInfo item = Core.Kernel.ItemInfos[Armor].Copy();
								Original.Equipments.Equip(item, Enums.ItemLocation.Armor, false, false);
							}
							uint Head = sql.ReadUInt32("BotHead");
							if (Head > 0)
							{
								Data.ItemInfo item = Core.Kernel.ItemInfos[Head].Copy();
								Original.Equipments.Equip(item, Enums.ItemLocation.Head, false, false);
							}
							bool UseSteed = sql.ReadBoolean("BotSteed");
							if (UseSteed)
							{
								uint Steed = sql.ReadUInt32("BotSteedColor");
								Data.ItemInfo item = Core.Kernel.ItemInfos[300000].Copy();
								item.SocketAndRGB = Steed;
								Original.Equipments.Equip(item, Enums.ItemLocation.Steed, false, false);
								
								uint MountArmor = sql.ReadUInt32("BotMountArmor");
								if (MountArmor > 0)
								{
									Data.ItemInfo item2 = Core.Kernel.ItemInfos[MountArmor].Copy();
									Original.Equipments.Equip(item2, Enums.ItemLocation.SteedArmor, false, false);
								}
								
								Original.Action = Enums.ActionType.None;
								Original.AddStatusEffect1(Enums.Effect1.Riding);
							}
							uint Garment = sql.ReadUInt32("BotGarment");
							if (Garment > 0)
							{
								Data.ItemInfo item = Core.Kernel.ItemInfos[Garment].Copy();
								Original.Equipments.Equip(item, Enums.ItemLocation.Garment, false, false);
							}*/
						Original.LoggedIn = true;
						break;
					}
					#endregion*
			}
		}
		
		#region Jump Bot
		public DateTime LastBotJump = DateTime.Now;
		public void HandleJump()
		{
			if (DateTime.Now >= LastBotJump.AddMilliseconds(JumpSpeed))
			{
				LastBotJump = DateTime.Now;
				Jump_Action();
			}
		}
		public void BeginJumpBot(Entities.GameClient target)
		{
			Threads.BotThread.Bots.TryAdd(Original.EntityUID, this);
			
			Target = target;
		}
		
		public void StopJumpBot()
		{
			AIBot rBot;
			Threads.BotThread.Bots.TryRemove(Original.EntityUID, out rBot);
		}
		
		private void Jump_Action()
		{
			if (Original.WasInArena)
			{
				Original.WasInArena = false;
				Target = null;
				Dispose();
				return;
			}
			if (Target == null)
				return;
			if (!Original.Alive)
			{
				Original.Revive();
				return;
			}
			bool CanFB = true;
			//if (Calculations.BasicCalculations.ChanceSuccess(95))
			//{
			if (Core.Screen.GetDistance(Original.X, Original.Y, Target.X, Target.Y) > 15)
			{
				Enums.ConquerAngle angle = Core.Screen.GetFacing(Core.Screen.GetAngle(Original.X, Original.Y, Target.X, Target.Y));
				ushort size = (ushort)Core.Screen.GetDistance(Target.X, Target.Y, Original.X, Original.Y);
				size /= 3;
				CanFB = false;
				Jump(size, angle);
			}
			else
			{
				ushort size = (ushort)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(10);
				Enums.ConquerAngle angle = (Enums.ConquerAngle)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(8);
				Jump(size, angle);
			}
			/*}
			else
			{
				Packets.GeneralDataPacket general = new ProjectX_V3_Game.Packets.GeneralDataPacket(Enums.DataAction.ChangeAction);
				general.Id = Original.EntityUID;
				general.Data1 = (uint)Enums.ActionType.Sit;
				general.Timestamp = ProjectX_V3_Lib.Native.Winmm.timeGetTime();
				Original.SendToScreen(general, false, false);
			}*/
			if (Original.ArenaMatch != null)
			{
				if (!Original.ArenaMatch.MatchStarted)
					CanFB = false;
			}
			if (Calculations.BasicCalculations.ChanceSuccess(ShootChance) && CanFB)
			{
				if (Core.Screen.GetDistance(Original.X, Original.Y, Target.X, Target.Y) <= 10)
				{
					#region fb / ss
					Shoot(Accuracy);
					#endregion
				}
			}
		}
		
		public void JumpOver(ushort x, ushort y)
		{
			Enums.ConquerAngle angle = Core.Screen.GetFacing(Core.Screen.GetAngle(Original.X, Original.Y, Target.X, Target.Y));
			ushort size = (ushort)Core.Screen.GetDistance(x, y, Original.X, Original.Y);
			size *= 2;
			if (size > 18)
				size = 18;
			Jump(size, angle);
		}
		public void Jump(ushort size, Enums.ConquerAngle angle)
		{
			Maps.MapPoint point = Packets.MovementPacket.CreateDirectionPoint(Original.X, Original.Y, (byte)angle);
			if (!Original.Map.ValidCoord(point.X, point.Y))
				return;
			
			for (ushort i = size; i > 0; i--)
			{
				Maps.MapPoint npoint = Packets.MovementPacket.CreateDirectionPoint(point.X, point.Y, (byte)angle);
				npoint = new ProjectX_V3_Game.Maps.MapPoint(0, npoint.X, npoint.Y);

				if (Original.Map.ValidCoord(npoint.X, npoint.Y))
					point = npoint;
				else
					break;
			}
			Jump(point.X, point.Y);
		}
		
		private void Jump(ushort x, ushort y)
		{
			if (!Original.Map.ValidCoord(x, y))
				return;
			if (MaxX > 0)
			{
				if (x > MaxX)
					return;
				if (y > MaxY)
					return;
				if (x < MinX)
					return;
				if (y < MinY)
					return;
			}
			Packets.GeneralDataPacket general = new ProjectX_V3_Game.Packets.GeneralDataPacket(Enums.DataAction.Jump);
			general.Id = Original.EntityUID;
			general.Data1Low = x;
			general.Data1High = y;
			general.Data5 = uint.MaxValue;
			general.Data3Low = Original.X;
			general.Data3High = Original.Y;
			general.Data4 = (uint)Original.Map.MapID;
			general.Timestamp = ProjectX_V3_Lib.Native.Winmm.timeGetTime();
			Original.SendToScreen(general, false, false);
			
			Original.X = x;
			Original.Y = y;
		}
		#endregion
		
		public void Shoot(int accu)
		{
			using (var interact = new Packets.InteractionPacket())
			{
				interact.Action = Enums.InteractAction.MagicAttack;
				interact.MagicType = 1045;
				interact.EntityUID = Original.EntityUID;
				interact.TargetUID = Target.EntityUID;
				interact.UnPacked = true;
				interact.MagicLevel = 4;
				if (Calculations.BasicCalculations.ChanceSuccess(accu))
				{
					interact.X = Target.X;
					interact.Y = Target.Y;
				}
				else
				{
					interact.X = (ushort)(Target.X + 1);
					interact.Y = (ushort)(Target.Y + 1);
				}
				Packets.Interaction.Battle.Magic.Handle(Original, interact);
			}
		}
		
		public void Dispose()
		{
			StopJumpBot();
			
			if (Original == null)
				return;
			if (Original.Map == null)
				return;
			Original.Map.LeaveMap(Original);
			Original.Screen.UpdateScreen(null, false);
			
			Core.NameGenerator.DeleteName(Original.Name);
			Original = null;
		}
		
		public void BeginDuel(Entities.GameClient Opponent)
		{
			Original.ArenaMatch = Data.ArenaMatch.CreateMatch(Original, Opponent);
			Opponent.ArenaMatch = Original.ArenaMatch;
			
			Original.ArenaMatch.AcceptedFight(Original);
			Original.ArenaMatch.AcceptedFight(Opponent);
			
			Original.ArenaMatch.BeginMatch();
		}
		
		private ushort DestinationX;
		private ushort DestinationY;
		private Threads.ActionThread.ThreadAction CurrentJumpAction;
		
		public void JumpToDestination(ushort X, ushort Y)
		{
			DestinationX = X;
			DestinationY = Y;
			CurrentJumpAction = Threads.ActionThread.AddAction(() => {
			                                                   	Jump();
			                                                   }, 500);
		}
		public void Jump()
		{
			if (Core.Screen.GetDistance(Original.X, Original.Y, DestinationX, DestinationY) <= 5)
			{
				Threads.ActionThread.Actions.TryRemove(CurrentJumpAction.ActionID, out CurrentJumpAction);
				CurrentJumpAction = null;
				return;
			}
			
			Enums.ConquerAngle angle = Core.Screen.GetFacing(Core.Screen.GetAngle(Original.X, Original.Y, DestinationX, DestinationY));
			Maps.MapPoint point = Packets.MovementPacket.CreateDirectionPoint(Original.X, Original.Y, (byte)angle);
			if (!Original.Map.ValidCoord(point.X, point.Y))
				return;
			const ushort size = 14;
			
			for (ushort i = size; i > 0; i--)
			{
				Maps.MapPoint npoint = Packets.MovementPacket.CreateDirectionPoint(point.X, point.Y, (byte)angle);
				npoint = new ProjectX_V3_Game.Maps.MapPoint(0, npoint.X, npoint.Y);

				if (Original.Map.ValidCoord(npoint.X, npoint.Y))
					point = npoint;
				else
					break;
			}
			ushort x = point.X;
			ushort y = point.Y;
			
			Packets.GeneralDataPacket general = new ProjectX_V3_Game.Packets.GeneralDataPacket(Enums.DataAction.Jump);
			general.Id = Original.EntityUID;
			general.Data1Low = x;
			general.Data1High = y;
			general.Data5 = uint.MaxValue;
			general.Data3Low = Original.X;
			general.Data3High = Original.Y;
			general.Data4 = (uint)Original.Map.MapID;
			general.Timestamp = ProjectX_V3_Lib.Native.Winmm.timeGetTime();
			Original.SendToScreen(general, false, false);
			
			Original.X = x;
			Original.Y = y;
		}
		
		public void Teleport(ushort MapID, ushort X, ushort Y)
		{
			if (MapID != Original.Map.MapID)
			{
				Maps.Map map;
				if (!Core.Kernel.Maps.TrySelect(MapID, out map))
				{
					return;
				}
				Original.Map = map;
				if (Original.Map != null)
					Original.LastMapID = Original.Map.MapID;
				
				if (!Original.Map.EnterMap(Original))
					return;
			}
			Maps.MapPoint location = new Maps.MapPoint(MapID, X, Y);
			
			Original.X = location.X;
			Original.Y = location.Y;
			Original.LastMapX = location.X;
			Original.LastMapY = location.Y;
			Original.LastX = location.X;
			Original.LastY = location.Y;
		}
	}
}
