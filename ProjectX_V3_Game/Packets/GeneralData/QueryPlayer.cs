//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.GeneralData
{
	/// <summary>
	/// Subtype: 102
	/// </summary>
	public class QueryPlayer
	{
		/// <summary>
		/// Handling the QueryPlayer action from GeneralDataPacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="General">The GeneralDataPacket.</param>
		public static void Handle(Entities.GameClient client, GeneralDataPacket General)
		{
			uint uid = General.Data1;
			if (client.Map.MapObjects.ContainsKey(uid))
			{
				Maps.IMapObject target = client.Map.MapObjects[uid];

				if (target is Entities.GameClient)
					(target as Entities.GameClient).Send(client.CreateSpawnPacket());
				
				if ((target is Entities.GameClient)) // or mob etc.
					client.Send(target.CreateSpawnPacket());
			}
		}
	}
}
