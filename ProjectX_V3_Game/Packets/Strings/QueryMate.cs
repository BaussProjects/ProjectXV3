//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Strings
{
	/// <summary>
	/// Description of QueryMate.
	/// </summary>
	public class QueryMate
	{
		public static void Handle(Entities.GameClient client, StringPacket strings)
		{
			Entities.GameClient reqClient;
			if (Core.Kernel.Clients.TrySelect(strings.Data, out reqClient))
			{
				using (var newstrings = new StringPacket(new StringPacker(reqClient.SpouseName)))
				{
					newstrings.Action = Enums.StringAction.QueryMate;
					client.Send(newstrings);
				}
			}
		}
	}
}
