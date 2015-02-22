//Project by BaussHacker aka. L33TS
using System;
using ProjectX_V3_Lib.ThreadSafe;

namespace ProjectX_V3_Game.Tournaments
{
	/// <summary>
	/// Description of TournamentTeam.
	/// </summary>
	public class TournamentTeam
	{
		public ConcurrentArrayList<Entities.GameClient> TeamMembers;
		public uint MaskedWeaponL;
		public uint MaskedWeaponR;
		public uint MaskedGarment;
		
		public TournamentTeam()
		{
			TeamMembers = new ConcurrentArrayList<ProjectX_V3_Game.Entities.GameClient>();
		}
		
		public void Join(Entities.GameClient client)
		{
			TeamMembers.Add(client);
			if (MaskedWeaponL > 0)
				client.Equipments.AddMask(MaskedWeaponL, Enums.ItemLocation.WeaponL);
			if (MaskedWeaponR > 0)
				client.Equipments.AddMask(MaskedWeaponR, Enums.ItemLocation.WeaponR);
			if (MaskedGarment > 0)
				client.Equipments.AddMask(MaskedGarment, Enums.ItemLocation.Garment);
		}
		public void Leave(Entities.GameClient client)
		{
			TeamMembers.Remove(client);
			client.Equipments.ClearMask();
			client.Teleport(client.LastMapID, client.LastMapX, client.LastMapY);
		}
	}
}
