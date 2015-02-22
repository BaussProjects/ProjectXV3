//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Interaction.Misc
{
	/// <summary>
	/// Subtype: ?
	/// </summary>
	public class Court
	{
		/// <summary>
		/// Handling the Court action of the interaction packet.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="interact">The interaction packet.</param>
		public static void Handle(Entities.GameClient client, InteractionPacket interact)
		{
			//uint marryUID = interact.TargetUID;
			
			//Entities.GameClient marrytarget;
			Maps.IMapObject obj;
			if (client.Map.MapObjects.TryGetValue(interact.TargetUID, out obj))
			{
				Entities.GameClient targ = obj as Entities.GameClient;
				if (client.SpouseDatabaseUID > 0)
				{
					using (var fmsg = Message.MessageCore.CreateSystem2(client.Name, Core.MessageConst.MARRIAGE_SELF_SPOUSE))
						client.Send(fmsg);
					return;
				}
				if (targ.SpouseDatabaseUID > 0)
				{
					using (var fmsg = Message.MessageCore.CreateSystem2(client.Name, Core.MessageConst.MARRIAGE_TARGET_SPOUSE))
						client.Send(fmsg);
					return;
				}
				
				targ.Send(interact);
			}
		}
	}
}
