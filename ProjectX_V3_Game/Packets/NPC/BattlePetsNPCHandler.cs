//Project by BaussHacker aka. L33TS
using System;

namespace ProjectX_V3_Game.Packets.NPC
{
	/// <summary>
	/// Description of BattlePetsNPCHandler.
	/// </summary>
	public class BattlePetsNPCHandler
	{
		public static void HandleAttack(Entities.GameClient client, byte option)
		{
			if (client.PetBattle ==  null)
				return;
			Data.PetBattle Battle = client.PetBattle;
			Entities.BattlePet Pet;
			if (client.Pets.ContainsKey(Battle.Pet1.PetID))
				Pet = Battle.Pet1;
			else
				Pet = Battle.Pet2;
			if (Pet == null)
				return;
			
			switch (option)
			{
					#region Show Moves
				case 0:
					{
						foreach (Data.BattleMonsterSkill skill in Pet.Attacks.selectorCollection1.Values)
						{
							NPCHandler.SendDialog(client, "Choose an attack.");
							NPCHandler.SendOption(client, skill.Name, option);
							option += 1;
						}
						break;
					}
					#endregion
					#region Attack
				case 1:
					{
						option -= 1;
						Data.BattleMonsterSkill skill;
						if (Pet.Attacks.TrySelect(option, out skill))
						{
							Entities.BattlePet Pet2;
							if (client.Pets.ContainsKey(Battle.Pet1.PetID))
								Pet2 = Battle.Pet2;
							else
								Pet2 = Battle.Pet1;
							if (Pet2 == null)
								return;
							
							skill.Attack(Pet, Pet2, false, 0);
						}
						break;
					}
					#endregion
			}
		}
		
		public static void HandlePetSwitch(Entities.GameClient client, byte option)
		{
			if (client.PetBattle ==  null)
				return;
			
			switch (option)
			{
					#region Show Pets
				case 0:
					{
						break;
					}
					#endregion
					#region Switch
				case 1:
					{
						break;
					}
					#endregion
			}
		}
	}
}
