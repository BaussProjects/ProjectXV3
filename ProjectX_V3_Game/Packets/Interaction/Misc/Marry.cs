//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Interaction.Misc
{
	/// <summary>
	/// Subtype: ?
	/// </summary>
	public class Marry
	{
		/// <summary>
		/// Handling the Marry action of the interaction packet.
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
				Entities.GameClient spouse = obj as Entities.GameClient;
				if (client.SpouseDatabaseUID > 0)
				{
					using (var fmsg = Message.MessageCore.CreateSystem2(client.Name, Core.MessageConst.MARRIAGE_SELF_SPOUSE))
						client.Send(fmsg);
					return;
				}
				if (spouse.SpouseDatabaseUID > 0)
				{
					using (var fmsg = Message.MessageCore.CreateSystem2(client.Name, Core.MessageConst.MARRIAGE_TARGET_SPOUSE))
						client.Send(fmsg);
					return;
				}
				
				client.SpouseDatabaseUID = spouse.DatabaseUID;
				spouse.SpouseDatabaseUID = client.DatabaseUID;
				//client.CharDB.AddSpouse(client.DatabaseUID, client.SpouseDatabaseUID);
				//spouse.CharDB.AddSpouse(spouse.DatabaseUID, spouse.SpouseDatabaseUID);
				Database.CharacterDatabase.AddSpouse(client, spouse);
				
				using (var mate = new Packets.StringPacket(new StringPacker(spouse.Name)))
				{
					mate.Data = client.EntityUID;
					mate.Action = Enums.StringAction.Mate;
					client.Send(mate);
				}
				using (var mate = new Packets.StringPacket(new StringPacker(client.Name)))
				{
					mate.Data = spouse.EntityUID;
					mate.Action = Enums.StringAction.Mate;
					spouse.Send(mate);
				}
				using (var fireworks = new Packets.StringPacket(new StringPacker("firework-2love")))
				{
					fireworks.Action = Enums.StringAction.MapEffect;
					fireworks.PositionX = client.X;
					fireworks.PositionY = client.Y;
					client.SendToScreen(fireworks, true);
					fireworks.PositionX = spouse.X;
					fireworks.PositionY = spouse.Y;
					spouse.SendToScreen(fireworks, true);
				}
				
				using (var msg =Packets.Message.MessageCore.CreateSystem("ALL",
				                                                         string.Format(Core.MessageConst.MARRIAGE_CONGRATZ, client.Name, spouse.Name)))
				{
					Packets.Message.MessageCore.SendGlobalMessage(msg);
				}
			}
		}
	}
}
