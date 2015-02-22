/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 5/30/2013
 * Time: 5:06 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ProjectX_V3_Lib.ThreadSafe;

namespace ProjectX_V3_Game.Entities
{
	/// <summary>
	/// Description of BattlePet.
	/// </summary>
	[Serializable()]
	public class BattlePet: Monster
	{
		private Entities.GameClient Owner;
		
		public BattlePet()
			: base()
		{
			Attacks = new Selector<byte, string, Data.BattleMonsterSkill>();
		}
		
		public int PetID;
		public byte EvolveState;
		public byte EvolveLevel;
		public int EvolveID;
		public byte MeetRateA;
		public byte MeetRateB;
		public byte CatchPower;
		public Enums.PetType Type1;
		public Enums.PetType Type2;
		public int Power;
		public Selector<byte, string, Data.BattleMonsterSkill> Attacks;
		public Data.PetBattle Battle;
		public Enums.PetState State;
		
		public void Save()
		{
			if (Owner == null)
				return;
		}
		
		public void Send(ProjectX_V3_Lib.Network.DataPacket packet)
		{
			if (Owner == null)
				return;
			
			Owner.Send(packet);
		}
		
		public void SendToScreen(ProjectX_V3_Lib.Network.DataPacket packet)
		{
			if (Owner == null)
				return;
			
			Owner.SendToScreen(packet, true, false);
		}
	}
}
