//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.GeneralData
{
	/// <summary>
	/// Description of ChangeMap.
	/// </summary>
	public class ChangeMap
	{
		public static void Handle(Entities.GameClient client, GeneralDataPacket General)
		{
			client.AttackPacket = null;
			
			if (client.Booth != null)
			{
				client.Booth.CancelBooth();
			}
			if (client.DynamicMap != null)
			{
				if (!client.LeaveDynamicMap())
				{
					client.NetworkClient.Disconnect("FAILED_TO_PORTAL_DYNAMIC_MAP");
				}
				return;
			}
			if (client.Map.InheritanceMap == client.Map.MapID)
			{
				Core.PortalPoint StartLocation = new Core.PortalPoint(client.Map.MapID, General.Data1Low, General.Data1High);
				if (Core.Screen.ValidDistance(client.X, client.Y, StartLocation.X, StartLocation.Y))
				{
					if (Core.Kernel.Portals.ContainsKey(StartLocation))
					{
						Core.PortalPoint Destination;
						if (Core.Kernel.Portals.TryGetValue(StartLocation, out Destination))
						{
							client.Teleport(Destination.MapID, Destination.X, Destination.Y);
							return;
						}
					}
				}
			}
			client.Teleport(client.Map.MapID, client.LastX, client.LastY);
		}
	}
}
