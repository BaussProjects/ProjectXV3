//Project by BaussHacker aka. L33TS

using System;
using System.IO;
using ProjectX_V3_Lib.IO;
using ProjectX_V3_Lib.Sql;

namespace ProjectX_V3_Game.Database
{
	/// <summary>
	/// The spell database.
	/// </summary>
	public class SpellDatabase
	{
		/// <summary>
		/// Loads all the spells.
		/// </summary>
		/// <returns></returns>
		public static bool LoadSpells()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("\tLoading Spells...");

			using (var spellinfo = new SqlHandler(Program.Config.ReadString("GameConnectionString")))
			{
				using (var cmd = new SqlCommandBuilder(spellinfo, SqlCommandType.SELECT, false))
				{
					cmd.Finish("DB_SpellInfo");
				}
				while (spellinfo.Read())
				{
					Data.Spell spell = new ProjectX_V3_Game.Data.Spell();
					spell.ID = spellinfo.ReadUInt16("SpellID");
					if (spell.ID == 0)
					{
						return false;
					}
					spell.SpellID = spellinfo.ReadUInt16("Type");
					if (spell.SpellID == 0)
					{
						return false;
					}

					spell.Sort = spellinfo.ReadByte("Sort");
					spell.Crime = spellinfo.ReadBoolean("Crime");
					spell.Ground = spellinfo.ReadBoolean("Ground");
					spell.Multi = spellinfo.ReadBoolean("Multi");
					spell.Target = spellinfo.ReadByte("Target");
					spell.Level = spellinfo.ReadByte("SpellLevel");
					spell.UseMP = spellinfo.ReadUInt16("UseMP");
					spell.Power = spellinfo.ReadUInt16("Power");
					if (spell.Power == 0)
						spell.PowerPercentage = 1;
					else
						spell.PowerPercentage = (float)(spell.Power % 1000) / 100;
					spell.IntoneSpeed = spellinfo.ReadUInt16("IntoneSpeed");
					spell.Percentage = spellinfo.ReadByte("SpellPercent");
					spell.Duration = spellinfo.ReadByte("StepSecs");
					spell.Range = spellinfo.ReadUInt16("Range");
					spell.Sector = spell.Range * 20;
					spell.Distance = spellinfo.ReadUInt16("Distance");
					if (spell.Distance >= 4)
						spell.Distance--;
					if (spell.Distance > 17)
						spell.Distance = 17;
					spell.Status = spellinfo.ReadUInt64("Status");
					spell.NeedExp = spellinfo.ReadUInt32("NeedExp");
					spell.NeedLevel = spellinfo.ReadByte("NeedLevel");
					spell.UseXP = spellinfo.ReadByte("UseXP");
					spell.WeaponSubtype = spellinfo.ReadUInt16("WeaponSubtype");
					spell.UseEP = spellinfo.ReadByte("UseEP");
					spell.NextMagic = spellinfo.ReadUInt16("NextMagic");
					spell.UseItem = spellinfo.ReadByte("UseItem");
					spell.UseItemNum = spellinfo.ReadByte("UseItemNum");
					
					if (Core.Kernel.SpellInfos.ContainsKey(spell.SpellID))
					{
						Core.Kernel.SpellInfos[spell.SpellID].TryAdd(spell.Level, spell);
					}
					else
					{
						if (!Core.Kernel.SpellInfos.TryAdd(spell.SpellID))
							return false;
						
						if (!Core.Kernel.SpellInfos[spell.SpellID].TryAdd(spell.Level, spell))
							return false;						
					}
					
					switch (spell.SpellID)
					{
						case 5010:
						case 7020:
						case 1290:
						case 1260:
						case 5030:
						case 5040:
						case 7000:
						case 7010:
						case 7030:
						case 7040:
						case 1250:
						case 5050:
						case 5020:
						case 10490:
						case 1300:
							if (spell.Distance >= 3)
								spell.Distance = 3;
							if (spell.Range > 3)
								spell.Range = 3;
							if (!Core.Kernel.WeaponSpells.ContainsKey(spell.WeaponSubtype))
							{
								if (!Core.Kernel.WeaponSpells.TryAdd(spell.WeaponSubtype, spell.SpellID))
								{
									return false;
								}
							}
							break;
					}
				}
			}
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\tLoaded {0} Spells...", Core.Kernel.SpellInfos.Count);
			return true;
		}
	}
}
