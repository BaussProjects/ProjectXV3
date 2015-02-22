//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Threads
{
	/// <summary>
	/// Description of WeatherThread.
	/// </summary>
	public class WeatherThread
	{
		static ushort[] WeatherMaps = new ushort[]
		{
			1002,
			1011,
			1020,
			1000,
			1015,
			1036,
			1038
		};
		static DateTime LastWeather = DateTime.Now;
		static DateTime LastThunder = DateTime.Now;
		
		public static void Handle()
		{
			if (DateTime.Now >= LastWeather)
			{
				LastWeather = DateTime.Now.AddMinutes(20);
				foreach (ushort map in WeatherMaps)
				{
					Maps.Map maphandler;
					if (Core.Kernel.Maps.TrySelect(map, out maphandler))
					{
						try
						{
							ChangeWeather(maphandler);
						}
						catch { }
					}
				}
			}
			//int amount = ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(100, 7000);
			//LastThunder = DateTime.Now.AddMilliseconds(amount);
			if (DateTime.Now >= LastThunder)
			{
				int amount = ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(10000, 30000);
				LastThunder = DateTime.Now.AddMilliseconds(amount);
				
				amount = ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(5);
				
				for (int i = 0; i < amount; i++)
				{
					foreach (ushort map in WeatherMaps)
					{
						Maps.Map maphandler;
						if (Core.Kernel.Maps.TrySelect(map, out maphandler))
						{
							ShowThunder(maphandler);
						}
					}
					System.Threading.Thread.Sleep(100);
				}
			}
		}
		
		public static void ShowWeather(Entities.GameClient client)
		{
			try
			{
				using (var weather = new Packets.WeatherPacket())
				{
					weather.Weather = client.Map.Weather;
					weather.Intensity = client.Map.WeatherIntensity;
					weather.Appearance = client.Map.WeatherAppearance;
					weather.Direction = client.Map.WeatherDirection;
					client.Send(weather);
				}
				if (client.Map.Weather == Enums.Weather.Rain)
				{
					Packets.GeneralDataPacket general = new ProjectX_V3_Game.Packets.GeneralDataPacket(Enums.DataAction.MapARGB);
					general.Id = client.EntityUID;
					general.Data1 = 5855577;
					client.Send(general);
				}
				else
				{
					Packets.GeneralDataPacket general = new ProjectX_V3_Game.Packets.GeneralDataPacket(Enums.DataAction.MapARGB);
					general.Id = client.EntityUID;
					general.Data1 = 0;
					client.Send(general);
				}
			}
			catch { }
		}
		
		public static void RemoveWeather(Entities.GameClient client)
		{
			using (var weather = new Packets.WeatherPacket())
			{
				weather.Weather = Enums.Weather.Nothing;
				client.Send(weather);
			}
		}
		
		public static void ShowThunder(Maps.Map map)
		{
			if (map.ShowThunder)
			{
				foreach (Maps.IMapObject obj in map.MapObjects.Values)
				{
					if (obj is Entities.GameClient)
					{
						using (var thunder = new Packets.StringPacket(new Packets.StringPacker("lounder1")))
						{
							thunder.Action = Enums.StringAction.MapEffect;
							thunder.PositionX = (ushort)(obj.X - 20);
							thunder.PositionY = obj.Y;
							(obj as Entities.GameClient).Send(thunder);
						}
					}
				}
			}
		}
		
		public static void ChangeWeather(Maps.Map map)
		{
			#region null weather
			map.ShowThunder = false;
			map.Weather = Enums.Weather.Nothing;
			map.WeatherIntensity = Enums.WeatherIntensity.None;
			map.WeatherAppearance = Enums.WeatherAppearance.None;
			map.WeatherDirection = Enums.ConquerAngle.SouthWest;
			#endregion
			
			int amount = ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(300);
			if (amount < 100)
			{
				#region Small Rain
				if (amount > 90)
				{
					map.Weather = Enums.Weather.Rain;
					map.WeatherIntensity = Enums.WeatherIntensity.Normal;
					map.WeatherAppearance = Enums.WeatherAppearance.None;
					map.WeatherDirection = (Enums.ConquerAngle)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(8);
					
					if (Calculations.BasicCalculations.ChanceSuccess(25))
					{
						if (map.MapID == 1002 || map.MapID == 1036) // thunder can become annoying in other maps ...
						{
							map.ShowThunder = true;
						}
					}
				}
				#endregion
				#region Medium Rain
				else if (amount > 80)
				{
					map.Weather = Enums.Weather.Rain;
					map.WeatherIntensity = Enums.WeatherIntensity.MediumHigh;
					map.WeatherAppearance = Enums.WeatherAppearance.None;
					map.WeatherDirection = (Enums.ConquerAngle)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(8);
					
					if (Calculations.BasicCalculations.ChanceSuccess(50))
					{
						if (map.MapID == 1002 || map.MapID == 1036) // thunder can become annoying in other maps ...
						{
							map.ShowThunder = true;
						}
					}
				}
				#endregion
				#region Hard Rain
				else if (amount > 70)
				{
					map.Weather = Enums.Weather.Rain;
					map.WeatherIntensity = Enums.WeatherIntensity.Highest;
					map.WeatherAppearance = Enums.WeatherAppearance.None;
					map.WeatherDirection = (Enums.ConquerAngle)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(8);
					if (map.MapID == 1002 || map.MapID == 1036) // thunder can become annoying in other maps ...
					{
						map.ShowThunder = true;
					}
				}
				#endregion
				#region Small Snow
				else if (amount > 60)
				{
					map.Weather = Enums.Weather.Snow;
					map.WeatherIntensity = Enums.WeatherIntensity.Normal;
					map.WeatherAppearance = Enums.WeatherAppearance.None;
					map.WeatherDirection = (Enums.ConquerAngle)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(8);
				}
				#endregion
				#region Medium Snow
				else if (amount > 50)
				{
					map.Weather = Enums.Weather.Snow;
					map.WeatherIntensity = Enums.WeatherIntensity.MediumHigh;
					map.WeatherAppearance = Enums.WeatherAppearance.None;
					map.WeatherDirection = (Enums.ConquerAngle)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(8);
				}
				#endregion
				#region Hard Snow
				else if (amount > 40)
				{
					map.Weather = Enums.Weather.Snow;
					map.WeatherIntensity = Enums.WeatherIntensity.Highest;
					map.WeatherAppearance = Enums.WeatherAppearance.None;
					map.WeatherDirection = (Enums.ConquerAngle)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(8);
				}
				#endregion
				#region Autmn Small
				else if (amount > 30)
				{
					map.Weather = Enums.Weather.AutumnLeaves;
					map.WeatherIntensity = Enums.WeatherIntensity.Low;
					map.WeatherAppearance = Enums.WeatherAppearance.None;
					map.WeatherDirection = (Enums.ConquerAngle)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(8);
				}
				#endregion
				#region Authmn Big
				else if (amount > 20)
				{
					map.Weather = Enums.Weather.Snow;
					map.WeatherIntensity = Enums.WeatherIntensity.Normal;
					map.WeatherAppearance = Enums.WeatherAppearance.None;
					map.WeatherDirection = (Enums.ConquerAngle)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(8);
				}
				#endregion
				#region Flower Small
				else if (amount > 10)
				{
					map.Weather = Enums.Weather.CherryBlossomPetals;
					map.WeatherIntensity = Enums.WeatherIntensity.Low;
					map.WeatherAppearance = Enums.WeatherAppearance.None;
					map.WeatherDirection = (Enums.ConquerAngle)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(8);
				}
				#endregion
				#region Flower Big
				else
				{
					map.Weather = Enums.Weather.CherryBlossomPetals;
					map.WeatherIntensity = Enums.WeatherIntensity.Normal;
					map.WeatherAppearance = Enums.WeatherAppearance.None;
					map.WeatherDirection = (Enums.ConquerAngle)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(8);
				}
				#endregion
			}
			
			foreach (Maps.IMapObject obj in map.MapObjects.Values)
			{
				if (obj is Entities.GameClient)
				{
					RemoveWeather((obj as Entities.GameClient));
					ShowWeather((obj as Entities.GameClient));
				}
			}
		}
	}
}
