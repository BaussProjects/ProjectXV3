//Project by BaussHacker aka. L33TS
using System;
using System.Collections.Generic;

namespace ProjectX_V3_Game.Data.Skills
{
	/// <summary>
	/// Description of MapSkill.
	/// </summary>
	[Serializable()]
	public class MapSkill
	{
		public MapSkill()
		{
		}
		
		public string SafeAreaEffect = "dispel";
		public string DestructionEffect = "bombarrow";
		
		public int DamageEffect = 0;
		public int PercentTageEffect = -1;
		
		public Maps.Map map;
		
		private List<System.Drawing.Point> DestructionAreas;
		private List<System.Drawing.Point> DestructionEffectAreas;
		private List<System.Drawing.Point> SafeSpots;
		
		public Entities.IEntity Killer;
		
		public bool Shake; // if true, the screen will shake
		public bool Dark; // if true darkness will happen on the screen
		public bool Zoom; // if true the screen will zoom
		
		public ushort Range;
		
		public int EffectRatio = 2;
		
		public void ExecuteStart(ushort StartX, ushort StartY)
		{
			if (Range < 10)
				return;
			
			if (DestructionAreas != null)
				DestructionAreas.Clear();
			else
				DestructionAreas = new List<System.Drawing.Point>();
			
			if (SafeSpots != null)
				SafeSpots.Clear();
			else
				SafeSpots = new List<System.Drawing.Point>();
			
			if (DestructionEffectAreas != null)
				DestructionEffectAreas.Clear();
			else
				DestructionEffectAreas = new List<System.Drawing.Point>();
			
			for (int i = 0; i < (Range / 4); i++)
			{
				ushort X = (ushort)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)StartX, (int)(StartX + Range));
				ushort Y = (ushort)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)StartY, (int)(StartY + Range));
				System.Drawing.Point p = new System.Drawing.Point((int)X, (int)Y);
				if (SafeSpots.Contains(p))
					i--;
				else
					SafeSpots.Add(p);
			}
			
			for (int i = 0; i < Range / 4; i++)
			{
				ushort X = (ushort)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)(StartX - Range), (int)StartX);
				ushort Y = (ushort)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)(StartY - Range), (int)StartY);
				System.Drawing.Point p = new System.Drawing.Point((int)X, (int)Y);
				if (SafeSpots.Contains(p))
					i--;
				else
					SafeSpots.Add(p);
			}
			
			for (int i = 0; i < Range / EffectRatio; i++)
			{
				ushort X = (ushort)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)StartX, (int)(StartX + Range));
				ushort Y = (ushort)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)StartY, (int)(StartY + Range));
				System.Drawing.Point p = new System.Drawing.Point((int)X, (int)Y);
				if (DestructionEffectAreas.Contains(p))
					i--;
				else
					DestructionEffectAreas.Add(p);
			}
			for (int i = 0; i < Range / EffectRatio; i++)
			{
				ushort X = (ushort)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)(StartX - Range), (int)StartX);
				ushort Y = (ushort)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)(StartY - Range), (int)StartY);
				System.Drawing.Point p = new System.Drawing.Point((int)X, (int)Y);
				if (DestructionEffectAreas.Contains(p))
					i--;
				else
					DestructionEffectAreas.Add(p);
			}
			
			for (ushort x = (ushort)(StartX - Range); x < (StartX + Range); x++)
			{
				for (ushort y = (ushort)(StartY - Range); y < (StartY + Range); y++)
				{
					System.Drawing.Point p = new System.Drawing.Point((int)x, (int)y);
					if (!SafeSpots.Contains(p))
						DestructionAreas.Add(p);
				}
			}
			
			for (int i = 0; i < 5; i++)
			{
				ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(() => {
				                                                       	foreach (System.Drawing.Point p in SafeSpots)
				                                                       	{
				                                                       		using (var str = new Packets.StringPacket(new Packets.StringPacker(SafeAreaEffect)))
				                                                       		{
				                                                       			str.Action = Enums.StringAction.MapEffect;
				                                                       			str.PositionX = (ushort)p.X;
				                                                       			str.PositionY = (ushort)p.Y;
				                                                       			
				                                                       			foreach (Maps.IMapObject MapObject in map.MapObjects.Values)
				                                                       			{
				                                                       				if (MapObject is Entities.GameClient)
				                                                       				{
				                                                       					if (Core.Screen.GetDistance(MapObject.X, MapObject.Y, p.X, p.Y) <= 40)
				                                                       					{
				                                                       						(MapObject as Entities.GameClient).Send(str);
				                                                       					}
				                                                       				}
				                                                       			}
				                                                       		}
				                                                       	} }, 1000 * i, 0);
			}
			ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(ExecuteDestruction, 5000, 0);
		}
		
		private void ExecuteDestruction()
		{
			foreach (System.Drawing.Point p in DestructionEffectAreas)
			{
				using (var str = new Packets.StringPacket(new Packets.StringPacker(DestructionEffect)))
				{
					str.Action = Enums.StringAction.MapEffect;
					str.PositionX = (ushort)p.X;
					str.PositionY = (ushort)p.Y;
					
					foreach (Maps.IMapObject MapObject in map.MapObjects.Values)
					{
						if (MapObject is Entities.GameClient)
						{
							if (Core.Screen.GetDistance(MapObject.X, MapObject.Y, p.X, p.Y) <= 40)
							{
								(MapObject as Entities.GameClient).Send(str);
							}
						}
					}
				}
			}
			#region Shake, Dark, Zoom
			List<uint> UsedUIDs = new List<uint>();
			if (Shake || Dark || Zoom)
			{
				using (var effect = new Packets.MapEffectPacket())
				{
					effect.Shake = Shake;
					effect.Darkness = Dark;
					effect.Zoom = Zoom;
					effect.AppendFlags();
					foreach (System.Drawing.Point p in DestructionAreas)
					{
						foreach (Maps.IMapObject MapObject in map.MapObjects.Values)
						{
							if (MapObject is Entities.GameClient)
							{
								if (Core.Screen.GetDistance(MapObject.X, MapObject.Y, p.X, p.Y) <= 40 && !UsedUIDs.Contains(MapObject.EntityUID))
								{
									effect.X = MapObject.X;
									effect.Y = MapObject.Y;
									(MapObject as Entities.GameClient).Send(effect);
									UsedUIDs.Add(MapObject.EntityUID);
								}
							}
						}
					}
				}
			}
			#endregion

			foreach (System.Drawing.Point p in DestructionAreas)
			{
				foreach (Maps.IMapObject MapObject in map.MapObjects.Values)
				{
					if (MapObject is Entities.GameClient)
					{
						System.Drawing.Point p2 = new System.Drawing.Point((int)MapObject.X, (int)MapObject.Y);
						if (!SafeSpots.Contains(p2) && p == p2)
						{
							Entities.GameClient target = (MapObject as Entities.GameClient);

							int damage = DamageEffect;
							if (PercentTageEffect != -1 && target.HP > PercentTageEffect)
							{
								damage = ((target.HP / 100) * PercentTageEffect);
							}

							if (damage > 0)
							{
								target.HP -= damage;
								if (target.HP <= 0)
								{
									Packets.Interaction.Battle.Combat.Kill(Killer, target, (uint)damage);
								}
							}
						}
					}
				}
			}
		}
	}
}
