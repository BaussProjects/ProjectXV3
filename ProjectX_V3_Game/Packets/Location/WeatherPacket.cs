//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Description of WeatherPacket.
	/// </summary>
	public class WeatherPacket : DataPacket
	{
		public WeatherPacket()
			: base(20, PacketType.WeatherPacket)
		{
		}
		
		public Enums.Weather Weather
		{
			get { return (Enums.Weather)ReadByte(4); }
			set { WriteByte((byte)value, 4); }
		}
		
		public Enums.WeatherIntensity Intensity
		{
			get { return (Enums.WeatherIntensity)ReadUInt32(8); }
			set { WriteUInt32((uint)value, 8); }
		}
		
		public Enums.ConquerAngle Direction
		{
			get { return (Enums.ConquerAngle)ReadUInt32(12); }
			set { WriteUInt32((uint)value, 12); }
		}
		
		public Enums.WeatherAppearance Appearance
		{
			get { return (Enums.WeatherAppearance)ReadUInt32(16); }
			set { WriteUInt32((uint)value, 16); }
		}
		/*0 	ushort 	20
2 	ushort 	1016
4 	byte 	Weather Type
8 	uint 	Weather_Intensity
12 	uint 	Weather_Direction
16 	uint 	Weather_Appearance */
	}
}
