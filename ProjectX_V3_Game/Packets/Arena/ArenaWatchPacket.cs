//Project by BaussHacker aka. L33TS
using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Description of ArenaWatchPacket.
	/// </summary>
	public class ArenaWatchPacket : DataPacket
	{
		public ArenaWatchPacket(DataPacket inPacket)
			: base(inPacket)
		{
			
		}
		
		public ushort Type
		{
			get { return ReadUInt16(4); }
			set { WriteUInt16(value, 4); }
		}
		
		public uint EntityUID
		{
			get { return ReadUInt32(10); }
			set { WriteUInt32(value, 10); }
		}		
		
		public static void Handle(Entities.GameClient client, DataPacket packet)
		{
			/*using (var watch = new ArenaWatchPacket(packet))
			{
				Entities.GameClient Fighter;
				if (Core.Kernel.Clients.TrySelect(watch.EntityUID, out Fighter))
				{
					if (Fighter.Battle == null)
						return;
					if (!(Fighter.Battle is Data.ArenaMatch))
						return;
					Data.ArenaMatch Match = Fighter.Battle as Data.ArenaMatch;
					
					switch (watch.Type)
					{
						case 0:
							{
								Match.JoinAsWatcher(client);
								//client.Send(packet);
								break;
							}
						case 1:
							{
								Match.LeaveWatcher(client);
								break;
							}
					}
				}
			}*/
		}
	}
}
