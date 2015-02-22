//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.GeneralData
{
	/// <summary>
	/// Subtype: 74
	/// </summary>
	public class EnterMap
	{
		/// <summary>
		/// Handling the EnterMap action from GeneralDataPacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="General">The GeneralDataPacket.</param>
		public static void Handle(Entities.GameClient client, GeneralDataPacket General)
		{
			if (!Core.Kernel.Clients.Contains(client.EntityUID))
			{
				client.NetworkClient.Disconnect("Not added in clients.");
				return;
			}
			
			ushort SendMap = client.Map.InheritanceMap;
			GeneralDataPacket mapEnter = new GeneralDataPacket(Enums.DataAction.EnterMap)
			{
				Id = SendMap,
				Data1 = SendMap,
				Data3Low = client.X,
				Data3High = client.Y
			};
			client.Send(mapEnter);
			using (var mapinfo = new Packets.MapInfoPacket())
			{
				mapinfo.MapID = SendMap;
				mapinfo.DocID = SendMap; // not entire sure what docid is, will figure this out later!
				foreach (Enums.MapTypeFlags flag in client.Map.Flags.Values)
					mapinfo.AddFlag(flag);
				
				mapinfo.Finish();
				client.Send(mapinfo);
			}
		}
	}
}
