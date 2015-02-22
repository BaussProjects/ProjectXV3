//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Interaction.Battle.Skills
{
	/// <summary>
	/// Description of FlySkill.
	/// </summary>
	public class FlySkill
	{
		public static bool Handle(Entities.IEntity attacker, Entities.IEntity target, InteractionPacket interaction, UseSpellPacket usespell, Data.Spell spell, out uint damage)
		{
			damage = 0;
			if (!attacker.Alive)
				return false;
			
			if (!(attacker is Entities.GameClient))
				return false;
			
			if (!(attacker as Entities.GameClient).Equipments.Contains(Enums.ItemLocation.WeaponR))
				return false;
			
			if (!(attacker as Entities.GameClient).Equipments[Enums.ItemLocation.WeaponR].IsBow())
				return false;
			
			(attacker as Entities.GameClient).AddStatusEffect1(Enums.Effect1.Fly, 40000);
		
			return true;
		}
	}
}
