//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Entities
{
	/// <summary>
	/// The base entity handler.
	/// </summary>
	[Serializable()]
	public class BaseEntity
	{
		/// <summary>
		/// The entity owner.
		/// </summary>
		private IEntity Owner;
		
		/// <summary>
		/// Creates a new instance of BaseEntity.
		/// </summary>
		/// <param name="owner">The entity owner.</param>
		public BaseEntity(IEntity owner)
		{
			this.Owner = owner;
		}
		
		/// <summary>
		/// Sets the basics stats based on level.
		/// </summary>
		public void SetBaseStats()
		{
			if (Owner.Reborns > 0)
				return;
			if (Owner.Level > 120)
				return;
			byte levelworker = Owner.Level;
			
			switch (Core.Kernel.GetBaseClass(Owner.Class))
			{
				case Enums.Class.InternTrojan:
					{
						ushort[] data = Core.Kernel.TrojanStats[levelworker];
						Owner.Strength = data[0];
						Owner.Vitality = data[1];
						Owner.Agility = data[2];
						Owner.Spirit = data[3];
						break;
					}
				case Enums.Class.InternWarrior:
					{
						ushort[] data = Core.Kernel.WarriorStats[levelworker];
						Owner.Strength = data[0];
						Owner.Vitality = data[1];
						Owner.Agility = data[2];
						Owner.Spirit = data[3];
						break;
					}
				case Enums.Class.InternNinja:
					{
						ushort[] data = Core.Kernel.NinjaStats[levelworker];
						Owner.Strength = data[0];
						Owner.Vitality = data[1];
						Owner.Agility = data[2];
						Owner.Spirit = data[3];
						break;
					}
				case Enums.Class.InternMonk_InternSaint:
					{
						ushort[] data = Core.Kernel.MonkStats[levelworker];
						Owner.Strength = data[0];
						Owner.Vitality = data[1];
						Owner.Agility = data[2];
						Owner.Spirit = data[3];
						break;
					}
				case Enums.Class.InternArcher:
					{
						ushort[] data = Core.Kernel.ArcherStats[levelworker];
						Owner.Strength = data[0];
						Owner.Vitality = data[1];
						Owner.Agility = data[2];
						Owner.Spirit = data[3];
						break;
					}
				case Enums.Class.InternTaoist:
					{
						ushort[] data = Core.Kernel.TaoistStats[levelworker];
						Owner.Strength = data[0];
						Owner.Vitality = data[1];
						Owner.Agility = data[2];
						Owner.Spirit = data[3];
						break;
					}
			}
		}
		
		/// <summary>
		/// Calculates the base stats.
		/// </summary>
		public void CalculateBaseStats()
		{
			ushort maxhp = (ushort)(Owner.Strength * 3);
			maxhp += (ushort)(Owner.Agility * 3);
			maxhp += (ushort)(Owner.Vitality * 24);
			maxhp += (ushort)(Owner.Spirit * 3);
			if (Core.Kernel.GetBaseClass(Owner.Class) == Enums.Class.InternTrojan)
			{
				byte reborncount = 1;
				reborncount += Owner.Reborns;
				maxhp += (ushort)((maxhp / 10) * reborncount);
			}
			Owner.MaxHP = maxhp;
			
			Owner.MaxMP = (ushort)(Owner.Spirit * 18);
			if (Core.Kernel.GetBaseClass(Owner.Class) == Enums.Class.InternTaoist)
			{
				byte reborncount = 1;
				reborncount += Owner.Reborns;
				Owner.MaxMP += (ushort)((Owner.MaxMP / 10) * reborncount);
			}
			
			if (Owner is GameClient)
			{
				GameClient client = Owner as GameClient;
				client.CalculateCombatStats();
				client.MaxHP += (ushort)client.BonusHP;
				client.MaxMP += (ushort)client.BonusMP;
				
				if (client.LoggedIn)
				{
					using (var stats = client.CreateStatsPacket())
						client.Send(stats);
					
					#if SHOW_STATS
					using (var msg = Packets.Message.MessageCore.Create(Enums.ChatType.Center, "SYSTEM", Owner.Name,
					                                                    string.Format(
					                                                    	"MaxHP: {0}, MaxMP: {1}, BonusHP: {2}, BonusMP: {3}, Atk: {4}, Def: {5}, MagicAttack: {6}, MagicDefense: {7}",
					                                                    	Owner.MaxHP,
					                                                    	Owner.MaxMP,
					                                                    	client.BonusHP,
					                                                    	client.BonusMP,
					                                                    	client.MinAttack + " - " + client.MaxAttack,
					                                                    	client.Defense,
					                                                    	client.MagicAttack,
					                                                    	client.MagicDefense
					                                                    )))
						(Owner as GameClient).Send(msg);
					#endif
				}
			}
		}
	}
}