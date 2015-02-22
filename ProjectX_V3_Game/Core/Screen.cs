//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;
using ProjectX_V3_Lib.Network;
using System.Diagnostics;

namespace ProjectX_V3_Game.Core
{
	/// <summary>
	/// A screen class. TODO: Rewrite this to make it even faster and with better readability ;)
	/// </summary>
	[Serializable()]
	public class Screen
	{
		/// <summary>
		/// The collection of screen objects.
		/// </summary>
		public ConcurrentDictionary<uint, Maps.IMapObject> MapObjects;
		
		/// <summary>
		/// The collection of screen objects.
		/// </summary>
		public ConcurrentDictionary<uint, Maps.IMapObject> Items;
		
		/// <summary>
		/// The screen owner.
		/// </summary>
		private Maps.IMapObject Owner;
		
		/// <summary>
		/// Creates a new screen.
		/// </summary>
		/// <param name="owner">The owner of the screen.</param>
		public Screen(Maps.IMapObject owner)
		{
			Owner = owner;
			MapObjects = new ConcurrentDictionary<uint, ProjectX_V3_Game.Maps.IMapObject>();
			Items = new ConcurrentDictionary<uint, ProjectX_V3_Game.Maps.IMapObject>();
		}
		
		public void FullUpdate()
		{
			ClearScreen();
			UpdateScreen(null);
			foreach (Maps.IMapObject MapObject in MapObjects.Values)
			{
				if (MapObject is Entities.GameClient)
				{
					MapObject.Screen.UpdateScreen(null);
				}
			}
		}
		/// <summary>
		/// Clears the screen.
		/// </summary>
		public void ClearScreen()
		{
			foreach (Maps.IMapObject MapObject in MapObjects.Values)
			{
				try
				{
					MapObject.Screen.UpdateScreen(null);
					if (Owner is Data.GroundItem && MapObject is Entities.GameClient)
					{
						using (var itemspawn = (Owner as Data.GroundItem).CreateItemSpawnPacket(2))
						{
							itemspawn.X = 0;
							itemspawn.Y = 0;
							(MapObject as Entities.GameClient).Send(itemspawn);
						}
					}
					else
					{
						if (Owner is Entities.GameClient)
						{
							(Owner as Entities.GameClient).Send(Packets.GeneralDataPacket.Create(MapObject.EntityUID, Enums.DataAction.RemoveEntity, 0, 0, 0, 0, 0, 0));
						}
						if (MapObject is Entities.GameClient)
						{
							(MapObject as Entities.GameClient).Send(Packets.GeneralDataPacket.Create(Owner.EntityUID, Enums.DataAction.RemoveEntity, 0, 0, 0, 0, 0, 0));
						}
					}
					Remove(MapObject);
				}
				catch { }
			}
			foreach (Maps.IMapObject MapObject in Items.Values)
			{
				try
				{
					MapObject.Screen.UpdateScreen(null);
					if (Owner is Entities.GameClient)
					{
						using (var itemspawn = (MapObject as Data.GroundItem).CreateItemSpawnPacket(2))
						{
							itemspawn.X = 0;
							itemspawn.Y = 0;
							(Owner as Entities.GameClient).Send(itemspawn);
						}
					}
					Remove(MapObject);
				}
				catch { }
			}
			MapObjects.Clear();
			Items.Clear();
		}
		
		#region New
		/*
 * 		/// <summary>
		/// Adds a map object to the screen.
		/// </summary>
		/// <param name="MapObject">The map object to add.</param>
		public void AddToScreen(Maps.IMapObject MapObject)
		{
			if (MapObject is Data.GroundItem)
			{
				if (Items.ContainsKey(MapObject.EntityUID))
					return;
				Items.TryAdd(MapObject.EntityUID, MapObject);
			}
			else
			{
				if (MapObjects.ContainsKey(MapObject.EntityUID))
					return;
				MapObjects.TryAdd(MapObject.EntityUID, MapObject);
				
				if (MapObject is Entities.GameClient)
				{
					ProjectX_V3_Lib.Network.DataPacket spawn;
					if (Owner is Data.GroundItem)
						spawn = (Owner as Data.GroundItem).CreateItemSpawnPacket(1);
					else if (Owner is Entities.NPC)
						spawn = (Owner as Entities.NPC).CreateNPCSpawnPacket();
					else
						spawn = Owner.CreateSpawnPacket(); // Player + Monster + AI
					(MapObject as Entities.GameClient).Send(spawn);
				}
			}
		}
		
		public void UpdateScreen(DataPacket Packet, bool deadonly = false)
		{
			if (!Owner.CanUpdateSpawn)
				return;
			
			foreach (Maps.IMapObject MapObject in Owner.Map.MapObjects.Values)
			{
				if (MapObject == Owner)
					continue;
				
				if (ValidDistance(MapObject.X, MapObject.Y, Owner.X, Owner.Y))
				{
					bool CanSpawnTo = true;
					bool CanSpawnFrom = true;
					#region Owner is GameClient
					if (Owner is Entities.GameClient)
					{
						Entities.GameClient OwnerClient = Owner as Entities.GameClient;
						#region Arena Watch
						if (!OwnerClient.IsAIBot)
						{
							if (MapObject is Entities.GameClient)
							{
								Entities.GameClient ObjectClient = MapObject as Entities.GameClient;
								if (!ObjectClient.IsAIBot)
								{
									if (OwnerClient.ArenaMatch != null)
									{
										if (!OwnerClient.ArenaMatch.SpawnInArena(ObjectClient))
											CanSpawnTo = false;
									}
									else if (ObjectClient.ArenaMatch != null)
									{
										if (!ObjectClient.ArenaMatch.SpawnInArena(OwnerClient))
											CanSpawnFrom = false;
									}
								}
							}
						}
						#endregion
					}
					#endregion
					#region Owner is Monster
					if (Owner is Entities.Monster)
					{
						Entities.Monster OwnerMonster = Owner as Entities.Monster;
					}
					#endregion
					#region Owner is NPC
					if (Owner is Entities.NPC)
					{
						Entities.NPC OwnerNPC = Owner as Entities.NPC;
					}
					#endregion
					
					if (CanSpawnFrom)
					{
						AddToScreen(MapObject);
						MapObject.Screen.AddToScreen(Owner);
					}
					else
					{
						Remove(MapObject);
						MapObject.Screen.Remove(Owner);
					}
				}
				else
				{
					Remove(MapObject);
					MapObject.Screen.Remove(Owner);
				}
			}
			
			UpdateScreenItems();
		}
		
				/// <summary>
		/// Removes a mapobject from the screen.
		/// </summary>
		/// <param name="MapObject">The mapobject to remove.</param>
		public void Remove(Maps.IMapObject MapObject)
		{
			Maps.IMapObject rObject;
			if (MapObject is Data.GroundItem)
				Items.TryRemove(MapObject.EntityUID, out rObject);
			else if (MapObjects.TryRemove(MapObject.EntityUID, out rObject))
			{
				if (Owner is Entities.GameClient)
				{
					(Owner as Entities.GameClient).Send(Packets.GeneralDataPacket.Create(MapObject.EntityUID, Enums.DataAction.RemoveEntity, 0, 0, 0, 0, 0, 0));
				}
			}
			
			if (Owner is Data.GroundItem)
				MapObject.Screen.Items.TryRemove(Owner.EntityUID, out rObject);
		} */
		#endregion
		
		#region old
		public void AddToScreen(Maps.IMapObject MapObject)
		{
			if (MapObject is Data.GroundItem)
			{
				if (Items.ContainsKey(MapObject.EntityUID))
					return;
				Items.TryAdd(MapObject.EntityUID, MapObject);
			}
			else
			{
				if (MapObjects.ContainsKey(MapObject.EntityUID))
					return;
				MapObjects.TryAdd(MapObject.EntityUID, MapObject);
			}
		}
		
		/// <summary>
		/// Removes a mapobject from the screen.
		/// </summary>
		/// <param name="MapObject">The mapobject to remove.</param>
		public void Remove(Maps.IMapObject MapObject)
		{
			Maps.IMapObject rObject;
			if (MapObject is Data.GroundItem)
				Items.TryRemove(MapObject.EntityUID, out rObject);
			else
				MapObjects.TryRemove(MapObject.EntityUID, out rObject);
			
			if (Owner is Data.GroundItem)
				MapObject.Screen.Items.TryRemove(Owner.EntityUID, out rObject);
			else
				MapObject.Screen.MapObjects.TryRemove(Owner.EntityUID, out rObject);
		}
		
		/// <summary>
		/// Updates the screen.
		/// </summary>
		/// <param name="Packet">Set to null if sending no packets to the screen.</param>
		public void UpdateScreen(DataPacket Packet, bool deadonly = false)
		{
			if (!Owner.CanUpdateSpawn)
				return;

			foreach (Maps.IMapObject MapObject in MapObjects.Values)
			{
				try
				{
					if (MapObject == Owner)
						continue;
					if (!MapObject.IsInMap(Owner))
					{
						Remove(MapObject);
						continue;
					}
					if (ValidDistance(Owner.X, Owner.Y, MapObject.X, MapObject.Y) && MapObject.CanUpdateSpawn)
					{
						if (MapObject is Entities.Monster)
						{
							if (!(MapObject as Entities.Monster).Alive && (DateTime.Now >= (MapObject as Entities.Monster).DieTime.AddMilliseconds(5000)))
							{
								if (Owner is Entities.GameClient)
								{
									(Owner as Entities.GameClient).Send(Packets.GeneralDataPacket.Create(MapObject.EntityUID, Enums.DataAction.RemoveEntity, 0, 0, 0, 0, 0, 0));
								}
								
								Remove(MapObject);
								
								continue;
							}
						}
						
						if (Packet != null)
						{
							if (MapObject is Entities.GameClient)
							{
								Entities.GameClient client = MapObject as Entities.GameClient;
								if (client != null)
								{
									if (!client.IsAIBot) // if the person in screen is not a bot
									{
										if (client.ArenaMatch != null) // if the person in screen has an arena match
										{
											if ((Owner as Entities.GameClient).ArenaMatch == null) // the the owner does not have an arena match
											{
												if (!client.ArenaMatch.SpawnInArena((Owner as Entities.GameClient))) // if the owner is a watcher
													continue; // do not send any packets ...
											}
										}
									}
									
									if (!deadonly || deadonly && !client.Alive)
										client.Send(Packet);
								}
							}
						}
						MapObject.Screen.AddToScreen(MapObject);
					}
					else
					{
						if (Owner is Entities.GameClient)
						{
							(Owner as Entities.GameClient).Send(Packets.GeneralDataPacket.Create(MapObject.EntityUID, Enums.DataAction.RemoveEntity, 0, 0, 0, 0, 0, 0));
						}
						if (MapObject is Entities.GameClient)
						{
							(MapObject as Entities.GameClient).Send(Packets.GeneralDataPacket.Create(Owner.EntityUID, Enums.DataAction.RemoveEntity, 0, 0, 0, 0, 0, 0));
						}

						Remove(MapObject);
					}
				}
				catch { }
			}
			
			ProjectX_V3_Lib.Network.DataPacket spawn;
			if (Owner is Data.GroundItem)
				spawn = (Owner as Data.GroundItem).CreateItemSpawnPacket(1);
			else if (Owner is Entities.NPC)
				spawn = (Owner as Entities.NPC).CreateNPCSpawnPacket();
			else
				spawn = Owner.CreateSpawnPacket(); // Player + Monster + AI
			
			foreach (Maps.IMapObject MapObject in Owner.Map.MapObjects.Values)
			{
				try
				{
					if (MapObject == Owner)
						continue;
					
					if (!MapObject.IsInMap(Owner))
					{
						Remove(MapObject);
						continue;
					}
					
					if (MapObjects.ContainsKey(MapObject.EntityUID))
						continue;
					
					if (ValidDistance(Owner.X, Owner.Y, MapObject.X, MapObject.Y) && MapObject.CanUpdateSpawn)
					{
						if (MapObject is Entities.Monster)
						{
							if (!(MapObject as Entities.Monster).Alive && (DateTime.Now >= (MapObject as Entities.Monster).DieTime.AddMilliseconds(5000)))
							{
								if (Owner is Entities.GameClient)
								{
									(Owner as Entities.GameClient).Send(Packets.GeneralDataPacket.Create(MapObject.EntityUID, Enums.DataAction.RemoveEntity, 0, 0, 0, 0, 0, 0));
								}
								
								Remove(MapObject);
								
								continue; // we don't want to add it, because it died before getting in screen
							}
						}
						if (MapObject is Entities.GameClient)
						{
							Entities.GameClient client = MapObject as Entities.GameClient;

							if (client != null) // if the person in screen is not null
							{
								if (Packet != null)
								{
									if (!deadonly || deadonly && !client.Alive)
										client.Send(Packet);
								}
								
								client.Send(spawn);
							}
						}
						if (Owner is Entities.GameClient)
						{
							if (MapObject is Entities.NPC)
							{
								using (var objspawn = (MapObject as Entities.NPC).CreateNPCSpawnPacket())
									(Owner as Entities.GameClient).Send(objspawn);
							}
							else
							{
								bool send_spawn = true;
								
								if (send_spawn)
								{
									using (var objspawn = MapObject.CreateSpawnPacket())
										(Owner as Entities.GameClient).Send(objspawn);
								}
							}
						}
						MapObject.Screen.AddToScreen(Owner);
						AddToScreen(MapObject);
					}
					else
					{
						if (Owner is Entities.GameClient)
						{
							(Owner as Entities.GameClient).Send(Packets.GeneralDataPacket.Create(MapObject.EntityUID, Enums.DataAction.RemoveEntity, 0, 0, 0, 0, 0, 0));
						}
						
						Remove(MapObject);
					}
				}
				catch { }
			}
			spawn.Dispose();
			
			if (!(Owner is Entities.NPC))
				UpdateScreenItems();
			
//			if (Owner is Entities.Monster)
//			{
//				if (MapObjects.Count == 0)
//				{
//					Entities.Monster mob;
//					Core.Kernel.SpawnedMonsters.TryRemove(Owner.EntityUID, out mob);
//				}
//				else if (!Core.Kernel.SpawnedMonsters.ContainsKey(Owner.EntityUID))
//					Core.Kernel.SpawnedMonsters.TryAdd(Owner.EntityUID, Owner);
//			}
		}
		 
		#endregion
		
		/// <summary>
		/// Updates the screen with items.
		/// </summary>
		public void UpdateScreenItems()
		{
			if (!Owner.CanUpdateSpawn)
				return;

			foreach (Maps.IMapObject MapObject in Items.Values)
			{
				try
				{
					if (MapObject == Owner)
						continue;
					if (!MapObject.IsInMap(Owner))
					{
						Remove(MapObject);
						continue;
					}
					
					if (ValidDistance(Owner.X, Owner.Y, MapObject.X, MapObject.Y) && MapObject.CanUpdateSpawn)
					{
						MapObject.Screen.AddToScreen(Owner);
					}
					else
					{
						if (Owner is Entities.GameClient)
						{
							using (var itemspawn = (MapObject as Data.GroundItem).CreateItemSpawnPacket(2))
							{
								itemspawn.X = 0;
								itemspawn.Y = 0;
								(Owner as Entities.GameClient).Send(itemspawn);
							}
						}
						
						Remove(MapObject);
					}
				}
				catch { }
			}
			foreach (Maps.IMapObject MapObject in Owner.Map.Items.Values)
			{
				try
				{
					if (MapObject == Owner)
						continue;
					
					if (!MapObject.IsInMap(Owner))
					{
						Remove(MapObject);
						continue;
					}
					
					if (Items.ContainsKey(MapObject.EntityUID))
						continue;
					
					if (ValidDistance(Owner.X, Owner.Y, MapObject.X, MapObject.Y) && MapObject.CanUpdateSpawn)
					{
						if (Owner is Entities.GameClient)
						{
							using (var itemspawn = (MapObject as Data.GroundItem).CreateItemSpawnPacket(1))
							{
								(Owner as Entities.GameClient).Send(itemspawn);
							}
						}
						
						MapObject.Screen.AddToScreen(Owner);
						AddToScreen(MapObject);
					}
					else
					{
						if (Owner is Entities.GameClient)
						{
							using (var itemspawn = (MapObject as Data.GroundItem).CreateItemSpawnPacket(2))
							{
								itemspawn.X = 0;
								itemspawn.Y = 0;
								(Owner as Entities.GameClient).Send(itemspawn);
							}
						}
						
						Remove(MapObject);
					}
				}
				catch { }
			}
		}
		
		public bool ContainsEntity(uint UID)
		{
			return MapObjects.ContainsKey(UID);
		}
		
		public bool GetEntity(uint UID, out Maps.IMapObject map)
		{
			return MapObjects.TryGetValue(UID, out map);
		}
		/// <summary>
		/// Checks if there is a valid distance.
		/// </summary>
		/// <param name="x1">The first x coordinate.</param>
		/// <param name="y1">The first y coordinate.</param>
		/// <param name="x2">The second x coordinate.</param>
		/// <param name="y2">The second y coordinate.</param>
		/// <returns>Returns true if the distance is valid.</returns>
		public static bool ValidDistance(ushort x1, ushort y1, ushort x2, ushort y2)
		{
			return (GetDistance(x1, y1, x2, y2) <= 18);
		}
		
		/// <summary>
		/// Checks if there is a valid distance.
		/// </summary>
		/// <param name="x1">The first x coordinate.</param>
		/// <param name="y1">The first y coordinate.</param>
		/// <param name="x2">The second x coordinate.</param>
		/// <param name="y2">The second y coordinate.</param>
		/// <returns>Returns true if the distance is valid.</returns>
		public static bool ValidDistance(int x1, int y1, int x2, int y2)
		{
			return (GetDistance(x1, y1, x2, y2) <= 18);
		}
		
		/// <summary>
		/// Checks if there is a valid distance.
		/// </summary>
		/// <param name="x1">The first x coordinate.</param>
		/// <param name="y1">The first y coordinate.</param>
		/// <param name="x2">The second x coordinate.</param>
		/// <param name="y2">The second y coordinate.</param>
		/// <returns>Returns true if the distance is valid.</returns>
		public static bool ValidDistance(double x1, double y1, double x2, double y2)
		{
			return (GetDistance(x1, y1, x2, y2) <= 18);
		}
		
		/// <summary>
		/// Gets the distance between coordinates.
		/// </summary>
		/// <param name="x1">The first x coordinate.</param>
		/// <param name="y1">The first y coordinate.</param>
		/// <param name="x2">The second x coordinate.</param>
		/// <param name="y2">The second y coordinate.</param>
		/// <returns>Returns the distance.</returns>
		public static int GetDistance(ushort x1, ushort y1, ushort x2, ushort y2)
		{
			return GetDistance((double)x1, (double)y1, (double)x2, (double)y2);
		}
		
		/// <summary>
		/// Gets the distance between coordinates.
		/// </summary>
		/// <param name="x1">The first x coordinate.</param>
		/// <param name="y1">The first y coordinate.</param>
		/// <param name="x2">The second x coordinate.</param>
		/// <param name="y2">The second y coordinate.</param>
		/// <returns>Returns the distance.</returns>
		public static int GetDistance(int x1, int y1, int x2, int y2)
		{
			return GetDistance((double)x1, (double)y1, (double)x2, (double)y2);
		}
		
		/// <summary>
		/// Gets the distance between coordinates.
		/// </summary>
		/// <param name="x1">The first x coordinate.</param>
		/// <param name="y1">The first y coordinate.</param>
		/// <param name="x2">The second x coordinate.</param>
		/// <param name="y2">The second y coordinate.</param>
		/// <returns>Returns the distance.</returns>
		public static int GetDistance(double x1, double y1, double x2, double y2)
		{
			return (int)System.Math.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));;
		}
		
		public static int GetDegree(int X, int X2, int Y, int Y2)
		{
			int direction = 0;

			double AddX = X2 - X;
			double AddY = Y2 - Y;
			double r = (double)Math.Atan2(AddY, AddX);
			if (r < 0) r += (double)Math.PI * 2;

			direction = (int)(360 - (r * 180 / Math.PI));

			return direction;
		}
		
		public static short GetAngle(ushort X, ushort Y, ushort x2, ushort y2)
		{
			double r = Math.Atan2(y2 - Y, x2 - X);
			if (r < 0)
				r += Math.PI * 2;
			return (short)Math.Round(r * 180 / Math.PI);
		}
		
		public static Enums.ConquerAngle GetFacing(short angle)
		{
			sbyte c_angle = (sbyte)((angle / 46) - 1);
			return (c_angle == -1) ? Enums.ConquerAngle.South : (Enums.ConquerAngle)c_angle;
		}
	}
}
