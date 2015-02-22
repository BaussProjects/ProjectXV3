//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Item
{
	/// <summary>
	/// Description of BoothDelete.
	/// </summary>
	public class BoothDelete
	{
		public static void Handle(Entities.GameClient client, ItemPacket packet)
		{
			if (client.Booth == null)
				return;
			
			uint uid = packet.UID;
			if (client.Booth.BoothItems.ContainsKey(uid))
			{
				Data.BoothItem rItem;
				if (client.Booth.BoothItems.TryRemove(uid, out rItem))
				{
					client.Send(packet);
				}
			}
		}
	}
}
