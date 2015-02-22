//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.Interaction.Battle.Skills
{
	/// <summary>
	/// Description of CureSkills.
	/// </summary>
	public class CureSkills
	{
		public static bool HandleSelf(Entities.IEntity attacker, Entities.IEntity target, InteractionPacket interaction, UseSpellPacket usespell, Data.Spell spell, out uint damage)
		{
			damage = 0;
			if (target != null)
			{
				if (target is Entities.NPC)
					return false;
				
				if (target.EntityUID != attacker.EntityUID)
					return false;
			}
			if (attacker is Entities.GameClient)
			{
				Entities.GameClient client = (attacker as Entities.GameClient);
				if (!(DateTime.Now >= client.LastSmallLongSkill.AddMilliseconds(Core.TimeIntervals.SmallLongSkillInterval)) && client.AttackPacket == null)
				{
					using (var fmsg = Packets.Message.MessageCore.CreateSystem2(client.Name, Core.MessageConst.REST))
						client.Send(fmsg);
					return false;
				}
				client.LastSmallLongSkill = DateTime.Now;
			}
			if (spell.ID == 1190 || spell.ID == 7016)
				attacker.HP += spell.Power;
			else
				attacker.MP += spell.Power;
			
			uint exp = (uint)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)spell.Power, (int)spell.Power * 2);
			if ((attacker is Entities.GameClient))
				(attacker as Entities.GameClient).AddSpellExp(spell.SpellID, exp);
			
			usespell.AddTarget(attacker.EntityUID, spell.Power);
			
			return true;
		}
		
		public static bool HandleSurrounding(Entities.IEntity attacker, Entities.IEntity target, InteractionPacket interaction, UseSpellPacket usespell, Data.Spell spell, out uint damage)
		{
			damage = 0;
			if (target == null)
				return false;
			if (target is Entities.NPC)
				return false;
			if (target is Entities.Monster)
			{
				if (((byte)(target as Entities.Monster).Behaviour) >= 3)
					return false;
			}
			if (target is Entities.GameClient)
			{
				if (!(target as Entities.GameClient).LoggedIn)
					return false;
			}
			if (attacker is Entities.GameClient)
			{
				Entities.GameClient client = (attacker as Entities.GameClient);
				if (!(DateTime.Now >= client.LastSmallLongSkill.AddMilliseconds(Core.TimeIntervals.SmallLongSkillInterval)) && client.AttackPacket == null)
				{
					using (var fmsg = Packets.Message.MessageCore.CreateSystem2(client.Name, Core.MessageConst.REST))
						client.Send(fmsg);
					return false;
				}
				client.LastSmallLongSkill = DateTime.Now;
			}
			
			target.HP += spell.Power;
			uint targetcount = 1;
			if (spell.SpellID == 1055)
			{
				foreach (Maps.IMapObject obj in target.Screen.MapObjects.Values)
				{
					if (obj == null)
						continue;
					if (obj.EntityUID == attacker.EntityUID)
						continue;
					if (obj.EntityUID == target.EntityUID)
						continue;
					if (obj is Entities.NPC)
						continue;
					if (Core.Screen.GetDistance(obj.X, obj.Y, target.X, target.Y) > spell.Distance)
						continue;
					if (obj is Entities.Monster)
					{
						if (((byte)(obj as Entities.Monster).Behaviour) >= 3)
							continue;
					}
					if (obj is Entities.GameClient)
					{
						if (!(obj as Entities.GameClient).LoggedIn)
							continue;
					}
					(obj as Entities.IEntity).HP += spell.Power;
					usespell.AddTarget(obj.EntityUID, spell.Power);
					targetcount++;
				}
			}
			
			uint exp = (uint)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)spell.Power, (int)spell.Power * 2);
			exp *= targetcount;
			if ((attacker is Entities.GameClient))
				(attacker as Entities.GameClient).AddSpellExp(spell.SpellID, exp);
			
			usespell.AddTarget(target.EntityUID, spell.Power);
			
			return true;
		}
	}
}
