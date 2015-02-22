//Project by BaussHacker aka. L33TS
using System;
using System.Collections.Concurrent;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// Description of PetBattle.
	/// </summary>
	[Serializable()]
	public class PetBattle
	{
		public Entities.BattlePet Pet1;
		public Entities.BattlePet Pet2;
		public ArenaMatch Match;
		
		public void UpdateScreen()
		{
			
		}
		
		public void Finish(Entities.BattlePet Winner)
		{
			if (Winner != null)
			{
				
			}
		}
		
		public void ShowOpponent()
		{
			Pet1.Direction = (byte)Enums.ConquerAngle.NorthEast;
			Pet1.X = 51;
			Pet1.Y = 54;
			Match.ArenaMap.EnterMap(Pet1);
			Pet2.Direction = (byte)Enums.ConquerAngle.SouthWest;
			Pet2.X = 51;
			Pet2.Y = 48;
			Match.ArenaMap.EnterMap(Pet2);
		}
		
		public void SendDialog()
		{
			
		}
		
		public static ConcurrentDictionary<int, Entities.Trainer> Trainers = new ConcurrentDictionary<int, Entities.Trainer>();
		
		public static void OpenBattle(Entities.GameClient Opponent, int PetID, int BattleID)
		{
			//if (!Trainers.ContainsKey(BattleID))
			//	return;
			//Entities.Trainer Trainer = Trainers[BattleID];
			
			ArenaMatch Match = ArenaMatch.CreateMatch(Opponent, null);
			Match.FightingMatch = false;
			Match.Started = true;
			Match.Player1 = Opponent;
			Opponent.Battle = Match;
			Opponent.PetBattle = new PetBattle();
			Opponent.PetBattle.Match = Match;
			
			Entities.BattlePet pet1 = new ProjectX_V3_Game.Entities.BattlePet();
			pet1.EntityUID = Core.UIDGenerators.GetMonsterUID();
			pet1.Mesh = 332;
			pet1.Name = "Zapdos";
			pet1.MaxHP = 1000;
			pet1.HP = 1000;
			Opponent.PetBattle.Pet1 = pet1;
			
			Entities.BattlePet pet2 = new ProjectX_V3_Game.Entities.BattlePet();
			pet2.EntityUID = Core.UIDGenerators.GetMonsterUID();
			pet2.Mesh = 332;
			pet2.Name = "Articuno";
			pet2.MaxHP = 1000;
			pet2.HP = 1000;
			Opponent.PetBattle.Pet2 = pet2;
			
			/*Opponent.PetBattle.Pet1 = Opponent.Pets[PetID];
			Opponent.PetBattle.Pet2 = Trainer.Pet;*/
			// JailGuard = 150001
			
			Entities.NPC Trainer = Core.Kernel.NPCs[150001];
			
			Match.ArenaMap = Core.Kernel.Maps[700].CreateDynamic(out Match.DynamicID);
			Opponent.TeleportDynamic(Match.DynamicID, 51, 58);
			
			Match.SendMatch();
			Match.SendCountDown(Trainer.Name, Trainer.Level, 15, 0, Trainer.EntityUID, Opponent);
			Match.SendPacket(pet1.EntityUID, pet1.Name, pet1.HP, pet2.EntityUID, pet2.Name, pet2.HP);
			
			Trainer.X = 51;
			Trainer.Y = 44;
			
			Match.ArenaMap.EnterMap(Trainer);
			
			Opponent.PetBattle.ShowOpponent();
			Opponent.PetBattle.SendDialog();
		}
	}
}
