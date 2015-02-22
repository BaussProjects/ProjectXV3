//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;
using System.Collections.Generic;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Server -> Client
	/// </summary>
	public class UseSpellPacket
	{
		public UseSpellPacket()
		{
			targets = new List<UseSpellPacket.spell_target>();
		}
		
		private class spell_target
		{
			public uint EntityUID;
			public uint Damage;
			public bool Hit;
			public uint ActivationType;
			public uint ActivationValue;
		}
		private List<spell_target> targets;
		public void AddTarget(uint EntityUID, uint Damage, bool Hit = true, uint ActivationType = 0, uint ActivationValue = 0)
		{
			if (targets.Count > 28)
				return;
			
			spell_target target = new UseSpellPacket.spell_target();
			target.EntityUID = EntityUID;
			target.Damage = Damage;
			target.Hit = Hit;
			target.ActivationType = ActivationType;
			target.ActivationValue = ActivationValue;
			targets.Add(target);
		}
		
		public uint EntityUID;
		public ushort SpellX;
		public ushort SpellY;
		public ushort SpellID;
		public ushort SpellLevel;
		
		public int TargetCount { get { return targets.Count; } }
		
		public static implicit operator DataPacket(UseSpellPacket usespell)
		{
			DataPacket packet = new DataPacket((ushort)(60 + (usespell.targets.Count * 32)), PacketType.UseSpellPacket);
			packet.WriteUInt32(usespell.EntityUID, 4);
			packet.WriteUInt16(usespell.SpellX, 8);
			packet.WriteUInt16(usespell.SpellY, 10);
			packet.WriteUInt16(usespell.SpellID, 12);
			packet.WriteUInt16(usespell.SpellLevel, 14);
			packet.WriteUInt32((uint)usespell.targets.Count, 16);
			int pos = 20;
			foreach (spell_target target in usespell.targets)
			{
				packet.WriteUInt32(target.EntityUID, pos);
				pos += 4;
				packet.WriteUInt32(target.Damage, pos);
				pos += 4;
				if (target.Hit)
					packet.WriteUInt32(1, pos);
				pos += 4;
				packet.WriteUInt32(target.ActivationType, pos);
				pos += 4;
				packet.WriteUInt32(target.ActivationValue, pos);
				pos += 16;
			}
			/*                *((uint*)(ptr + 4)) = spell.AttackerID;
                *((ushort*)(ptr + 8)) = spell.TargetX;
                *((ushort*)(ptr + 10)) = spell.TargetY;
                * ((ushort*)(ptr + 12)) = spell.SpellID;
                *((ushort*)(ptr + 14)) = spell.SpellLevel;
                *((uint*)(ptr + 16)) = (uint)spell.Targets.Count;
                int Offset = 20;
                foreach (Target t in spell.Targets)
                {
                    *((uint*)(ptr + Offset)) = t.TargetID; Offset += 4;
                    *((uint*)(ptr + Offset)) = t.Damage; Offset += 4;
                    if(t.Hit)
                        *((uint*)(ptr + Offset)) = 1;
                    Offset += 4;
                    *((uint*)(ptr + Offset)) = t.ActivationType; Offset += 4;
                    *((uint*)(ptr + Offset)) = t.ActivationValue; 
                    Offset += 16;
                }*/
			return packet;
		}
	}
}
