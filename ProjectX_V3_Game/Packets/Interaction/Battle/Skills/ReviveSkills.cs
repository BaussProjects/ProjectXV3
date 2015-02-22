//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;

namespace ProjectX_V3_Game.Packets.Interaction.Battle.Skills
{
	/// <summary>
	/// Description of ReviveSkills.
	/// </summary>
	public class ReviveSkills
	{
		public static bool Handle(Entities.IEntity attacker, Entities.IEntity target, InteractionPacket interaction, UseSpellPacket usespell, Data.Spell spell, out uint damage)
		{
			damage = 0;
			if (!attacker.Alive)
				return false;
			if (target == null)
				return false;
			if (target.Alive)
				return false;
			if (attacker.EntityUID == target.EntityUID)
				return false;
			if (!(target is Entities.GameClient))
				return false;
			Entities.GameClient TargetClient = (target as Entities.GameClient);
			TargetClient.ForceRevive();
			usespell.AddTarget(TargetClient.EntityUID, 0);
			return true;
		}
	}
}
