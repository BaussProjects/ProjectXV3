//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Strings
{
	/// <summary>
	/// Description of WhisperWindowInfo.
	/// </summary>
	public class WhisperWindowInfo
	{
		public static void Handle(Entities.GameClient client, StringPacket strings)
		{
			string Name = strings.Strings[0];
			Entities.GameClient reqClient;
			if (Core.Kernel.Clients.TrySelect(Name, out reqClient))
			{
				string toAdd = reqClient.EntityUID + " ";
				toAdd += reqClient.Level + " ";
				toAdd += reqClient.Level + " ";//battle power
				toAdd += "# ";//unknown
				toAdd += "# ";//unknown
				toAdd += reqClient.SpouseName + " ";
				toAdd += 0 + " ";//unknown
				if (reqClient.Mesh % 10 < 3)
					toAdd += "1 ";
				else
					toAdd += "0 ";
				
				using (var newstrings = new StringPacket(new StringPacker(Name, toAdd)))
				{
					newstrings.Action = Enums.StringAction.WhisperWindowInfo;
					client.Send(newstrings);
				}
			}
		}
	}
}
