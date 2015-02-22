//Project by BaussHacker aka. L33TS
using System;

namespace ProjectX_V3_Game.Entities
{
	/// <summary>
	/// Description of Trainer.
	/// </summary>
	public class Trainer
	{
		public BattlePet Pet;
		public Entities.NPC AssociatedNPC;
		public uint EntityUID
		{
			get { return AssociatedNPC.EntityUID; }
		}
		public string Name
		{
			get { return AssociatedNPC.Name; }
			set { AssociatedNPC.Name = value; }
		}
		
		public byte Level
		{
			get { return AssociatedNPC.Level; }
			set { AssociatedNPC.Level = value; }
		}
	}
}
