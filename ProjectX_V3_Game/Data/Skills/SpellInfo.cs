//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// Spell information class.
	/// </summary>
	public class SpellInfo
	{
		/// <summary>
		/// The experience.
		/// </summary>
		public uint Experience;
		
		/// <summary>
		/// The spell ID.
		/// </summary>
		public ushort ID;
		
		/// <summary>
		/// The level.
		/// </summary>
		public ushort Level;
		
		/// <summary>
		/// Sends prof info to the client.
		/// </summary>
		/// <param name="client">The client.</param>
		public void SendProfToClient(Entities.GameClient client)
		{
			if (!client.LoggedIn)
				return;
			
			using (var spell = new Packets.SendProfPacket())
			{
				spell.ID = ID;
				spell.Level = Level;
				spell.Experience = Experience;
				client.Send(spell);
			}
		}
		
		/// <summary>
		/// Sends spell info to the client.
		/// </summary>
		/// <param name="client">The client.</param>
		public void SendSpellToClient(Entities.GameClient client)
		{
			if (!client.LoggedIn)
				return;
			
			using (var spell = new Packets.SendSpellPacket())
			{
				spell.ID = ID;
				spell.Level = Level;
				spell.Experience = Experience;
				client.Send(spell);
			}
		}
		
		public static bool IsFixed(ushort spell, byte level)
		{
			if (Core.Kernel.SpellInfos.ContainsKey(spell))
			{
				return (!Core.Kernel.SpellInfos[spell].ContainsKey(((byte)(level + 1))));
			}
			return false;
		}
	}
}
