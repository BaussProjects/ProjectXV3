//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Description of TeamMemberPacket.
	/// </summary>
	public class TeamMemberPacket : DataPacket
	{
		public TeamMemberPacket()
			: base(160, PacketType.TeamMemberPacket)
		{
			WriteByte(1, 5);
		}
		
		public string Name
		{
			get { return ReadString(8, 16); }
			set { WriteString(value, 8); }
		}
		
		public uint EntityUID
		{
			get { return ReadUInt32(24); }
			set { WriteUInt32(value, 24); }
		}
		
		public uint Mesh
		{
			get { return ReadUInt32(28); }
			set { WriteUInt32(value, 28); }
		}
		
		public ushort MaxHealth
		{
			get { return ReadUInt16(32); }
			set { WriteUInt16(value, 32); }
		}
		
		public ushort CurrentHealth
		{
			get { return ReadUInt16(34); }
			set { WriteUInt16(value, 34); }
		}
		/*            var buffer = new byte[160];
            fixed (byte* ptr = buffer)
            {
                PacketBuilder.AppendHeader(ptr, buffer.Length, 1026);
                Packets.WriteByte(1,5, buffer);
                Packets.WriteString(packet.Name, 8, buffer);
                Packets.WriteUInt32(packet.PlayerID, 24, buffer);
                Packets.WriteUInt32(packet.PlayerMesh, 28, buffer);
                Packets.WriteUInt16(packet.MaxHealth, 32, buffer);
                Packets.WriteUInt16(packet.CurrentHealth, 34, buffer);
            }*/
	}
}
