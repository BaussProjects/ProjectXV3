//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Interaction.Battle.Skills
{
	/// <summary>
	/// Description of MountSkill.
	/// </summary>
	public class MountSkill
	{
		public static bool Handle(Entities.GameClient client, UseSpellPacket usespell)
		{
			if (!client.Equipments.Contains(Enums.ItemLocation.Steed))
				return false;
			
			if (client.ContainsFlag1(Enums.Effect1.Riding))
				client.RemoveFlag1(Enums.Effect1.Riding);
			else if (client.Stamina < 100)
				return false;
			else
				client.AddStatusEffect1(Enums.Effect1.Riding);
			
			using (var vigor = new Packets.SteedVigorPacket())
			{
				vigor.Type = 2;
				vigor.Amount = 9001;
				client.Send(vigor);
			}
			
			usespell.AddTarget(client.EntityUID, 0);
			return true;
		}
	}
}
