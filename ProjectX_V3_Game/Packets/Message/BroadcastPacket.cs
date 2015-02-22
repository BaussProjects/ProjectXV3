//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Description of BroadcastPacket.
	/// </summary>
	public class BroadcastPacket
	{
		public static void Handle(Entities.GameClient client, DataPacket packet)
		{
			Console.WriteLine("TYPE: {0}", packet.ReadByte(4));
			if (packet.ReadByte(4) > 2)
			{
				string[] Strings = StringPacker.Analyze(packet, 12);
				if (Strings != null)
				{
					if (Strings.Length > 0)
					{
						if (client.CPs >= 5)
						{
							client.CPs -= 5;
							Threads.BroadcastThread.AddBroadcast(Packets.Message.MessageCore.CreateBroadcast(client.Name, Strings[0]));
						}
					}
				}
			}
		}
	}
}
