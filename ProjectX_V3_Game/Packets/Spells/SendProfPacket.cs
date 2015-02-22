//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;
	
namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Server -> Client
	/// </summary>
	public class SendProfPacket : DataPacket
	{
		public SendProfPacket()
			: base(20, PacketType.SendProfPacket)
		{
		}
		
		public uint ID
		{
			get { return ReadUInt32(4); }
			set { WriteUInt32(value, 4); }
		}
		
		public uint Level
		{
			get { return ReadUInt32(8); }
			set
			{
				WriteUInt32(value, 8);
				WriteUInt32(Calculations.LevelCalculations.GetProfExperience((byte)value), 16);
			}
		}
		
		public uint Experience
		{
			get { return ReadUInt32(12); }
			set { WriteUInt32(value, 12); }
		}
	}
}
