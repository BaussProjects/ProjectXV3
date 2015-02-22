//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.GeneralData
{
	/// <summary>
	/// Description of CreateBooth.
	/// </summary>
	public class CreateBooth
	{
		public static void Handle(Entities.GameClient client, GeneralDataPacket General)
		{
			if (client.Booth != null)
			{
				client.Booth.CancelBooth();
				return;
			}
			if (client.Map.MapID != 1036)
				return;
			
			client.Booth = new Data.Booth();
			client.Booth.ShopOwner = client;
			client.Booth.CreateBooth(General);
		}
	}
}
