//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Description of ArenaPlayersPacket.
	/// </summary>
	public class ArenaPlayersPacket : DataPacket
	{
		public ArenaPlayersPacket(Data.ArenaInfo[] ArenaInfos)
			: base((ushort)(8 + 52 * ArenaInfos.Length), PacketType.ArenaPlayersPacket)
		{
			WriteUInt32((uint)ArenaInfos.Length, 4);
			foreach (Data.ArenaInfo info in ArenaInfos)
				AppendInfo(info);
		}
		
		private int InfoOffset = 8;
		private int I = 0;
		private void AppendInfo(Data.ArenaInfo Info)
		{
			WriteUInt32((uint)Info.ArenaID, InfoOffset);
			InfoOffset += 4;
			WriteString(Info.Name, InfoOffset);
			InfoOffset += 16;
			WriteUInt32(Info.Mesh, InfoOffset);
			InfoOffset += 4;
			WriteUInt32(Info.Level, InfoOffset);
			InfoOffset += 4;
			WriteUInt32(Info.Class, InfoOffset);
			InfoOffset += 4;
			WriteUInt32((uint)I + 1, InfoOffset);
			InfoOffset += 4;
			WriteUInt32((uint)I + 1, InfoOffset);
			InfoOffset += 4;
			WriteUInt32(Info.ArenaTotal, InfoOffset);
			InfoOffset += 4;
			WriteUInt32((uint)I + 1, InfoOffset);
			InfoOffset += 4;
			WriteUInt32((uint)I + 1, InfoOffset);
			InfoOffset += 4;
			I++;
		}
		
		public static void Handle(Entities.GameClient client, DataPacket packet)
		{
			using (var arenaplayers = new ArenaPlayersPacket(Data.ArenaQualifier.GetTop10()))
			{
				client.Send(arenaplayers);
			}
		}
	}
}
