//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Description of SubClassPacket.
	/// </summary>
	public class SubClassPacket : DataPacket
	{
		public SubClassPacket(Entities.SubClass[] subclasses)
			: base((ushort)(26 + (subclasses.Length * 3)), PacketType.SubClassPacket)
		{
			WriteUInt16((ushort)subclasses.Length, 22);
			int ClassOffset = 26;
			foreach (Entities.SubClass subclass in subclasses)
			{
				WriteBytes(new byte[]
				           {
				           	((byte)subclass.ID),
				           	subclass.Phase,
				           	subclass.Level
				           }, ClassOffset);
				ClassOffset += 3;
			}
		}
		public SubClassPacket()
			: base(26, PacketType.SubClassPacket)
		{
			
		}
		
		public SubClassPacket(DataPacket inPacket)
			: base(inPacket)
		{
			
		}
		
		public Enums.SubClassActions Action
		{
			get { return (Enums.SubClassActions)ReadUInt16(4); }
			set { WriteUInt16((ushort)value, 4); }
		}
		
		public ushort StudyPoints
		{
			get { return ReadUInt16(6); }
			set { WriteUInt16(value, 6); }
		}
		
		public Enums.SubClasses SubClass
		{
			get { return (Enums.SubClasses)ReadByte(6); }
			set { WriteByte((byte)value, 6); }
		}
		
		public byte Level
		{
			get { return ReadByte(7); }
			set { WriteByte(value, 7); }
		}
		
		public static void Handle(Entities.GameClient client, DataPacket packet)
		{
			using (var subclass = new SubClassPacket(packet))
			{
				switch (subclass.Action)
				{
					case Enums.SubClassActions.Info:
						{
							//client.SendSubClasses2();
							break;
						}
				}
			}
		}
	}
}
