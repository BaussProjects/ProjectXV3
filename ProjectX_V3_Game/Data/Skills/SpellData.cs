//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// Spell Data.
	/// </summary>
	public class SpellData
	{
		/// <summary>
		/// The owner of the spell data.
		/// </summary>
		private Entities.GameClient Owner;
		
		/// <summary>
		/// Creates a new instance of SpellData.
		/// </summary>
		/// <param name="owner">The owner.</param>
		public SpellData(Entities.GameClient owner)
		{
			this.Owner = owner;
			Profs = new ConcurrentDictionary<ushort, SpellInfo>();
			Spells = new ConcurrentDictionary<ushort, SpellInfo>();
		}
		/// <summary>
		/// The profs.
		/// </summary>
		private ConcurrentDictionary<ushort, Data.SpellInfo> Profs;
		/// <summary>
		/// The spells.
		/// </summary>
		private ConcurrentDictionary<ushort, Data.SpellInfo> Spells;
		
		/// <summary>
		/// Adds a prof.
		/// </summary>
		/// <param name="info">The spellinfo.</param>
		public void AddProf(SpellInfo info)
		{
			if (Profs.TryAdd(info.ID, info))
			{
				info.SendProfToClient(Owner);
				Database.CharacterDatabase.SaveProf(Owner, info);
			}
		}
		
		/// <summary>
		/// Adds a spell.
		/// </summary>
		/// <param name="info">The spellinfo.</param>
		public bool AddSpell(SpellInfo info)
		{
			if (Spells.ContainsKey(info.ID))
				return false;
			
			if (Spells.TryAdd(info.ID, info))
			{
				info.SendSpellToClient(Owner);
				Database.CharacterDatabase.SaveSpell(Owner, info);
				return true;
			}
			return false;
		}
		
		public bool AddSpell(ushort ID)
		{
			if (Spells.ContainsKey(ID))
				return false;
			
			SpellInfo info = new SpellInfo();
			info.ID = ID;
			if (Spells.TryAdd(info.ID, info))
			{
				info.SendSpellToClient(Owner);
				Database.CharacterDatabase.SaveSpell(Owner, info);
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// Gets a prof.
		/// </summary>
		/// <param name="ID">The prof id.</param>
		/// <returns>Returns the spellinfo.</returns>
		public SpellInfo GetProf(ushort ID)
		{
			return Profs[ID];
		}
		
		/// <summary>
		/// Gets a spell.
		/// </summary>
		/// <param name="ID">The spell id.</param>
		/// <returns>Returns the spellinfo.</returns>
		public SpellInfo GetSpell(ushort ID)
		{
			return Spells[ID];
		}
		
		public bool RemoveProf(ushort ID)
		{
			throw new NotImplementedException("REMOVE PROF");
		}
		public bool RemoveSpell(ushort ID)
		{
			throw new NotImplementedException("REMOVE SPELL");
		}
		
		/// <summary>
		/// Checks whether a prof exist or not in the spelldata.
		/// </summary>
		/// <param name="ID">The prof id.</param>
		/// <returns>Returns true if it exist.</returns>
		public bool ContainsProf(ushort ID)
		{
			return Profs.ContainsKey(ID);
		}
		
		/// <summary>
		/// Checks whether a spell exist or not in the spelldata.
		/// </summary>
		/// <param name="ID">The spell id.</param>
		/// <returns>Returns true if it exist.</returns>
		public bool ContainsSpell(ushort ID)
		{
			return Spells.ContainsKey(ID);
		}
		
		/// <summary>
		/// Sends all the spells to the client.
		/// </summary>
		public void SendSpellInfos()
		{
			foreach (SpellInfo spell in Spells.Values)
				spell.SendSpellToClient(Owner);
		}
		
		/// <summary>
		/// Sends all the profs to the client.
		/// </summary>
		public void SendProfInfos()
		{
			foreach (SpellInfo prof in Profs.Values)
				prof.SendProfToClient(Owner);
		}
	}
}
