//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// Description of BattleClass.
	/// </summary>
	public abstract class BattleClass
	{
		public abstract bool HandleBeginAttack(Entities.GameClient Attacker);
		public abstract bool HandleAttack(Entities.GameClient Attacker, Entities.GameClient Attacked, ref uint damage);
		public abstract bool HandleBeginHit_Physical(Entities.GameClient Attacker);
		public abstract bool HandleBeginHit_Ranged(Entities.GameClient Attacker);
		public abstract bool HandleBeginHit_Magic(Entities.GameClient Attacker, Packets.UseSpellPacket usespell);
		public abstract bool HandleDeath(Entities.GameClient Attacker, Entities.GameClient Attacked);
		public abstract bool HandleRevive(Entities.GameClient Killed);
		public abstract bool EnterArea(Entities.GameClient client);
		public abstract bool LeaveArea(Entities.GameClient client);
		public abstract void KillMob(Entities.GameClient Attacker, uint MobUID);
	}
}
