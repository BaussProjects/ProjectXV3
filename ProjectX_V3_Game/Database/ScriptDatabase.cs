//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.ScriptEngine;
using ProjectX_V3_Lib.Extensions;
using System.Linq;
using System.Reflection;

namespace ProjectX_V3_Game.Database
{
	/// <summary>
	/// Description of ScriptDatabase.
	/// </summary>
	public class ScriptDatabase
	{
		private static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
		{
			return assembly.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
		}
		
		public static ScriptSettings cssettings;
		
		/// <summary>
		/// Loads the global script settings.
		/// </summary>
		public static void LoadSettings()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("\tLoading Script Settings...");
			
			cssettings = new ScriptSettings();
			cssettings.AddNamespace("System");
			cssettings.AddNamespace("ProjectX_V3_Game.Entities");
			cssettings.AddNamespace("ProjectX_V3_Game.Core");
			cssettings.AddNamespace("ProjectX_V3_Game.Data");
			cssettings.AddNamespace("ProjectX_V3_Game.Calculations");
			cssettings.AddNamespace("ProjectX_V3_Game.Packets.Message");
			cssettings.AddNamespace("ProjectX_V3_Game.Packets");
			cssettings.AddNamespace("ProjectX_V3_Game.Maps");
			cssettings.AddNamespace("ProjectX_V3_Game.Tournaments");
			cssettings.AddNamespace("ProjectX_V3_Game.Packets.NPC"); // will add some functions to this later, not entirely npc restricted, but mostly hence why it's also used by ItemScripts!
			ScriptEngine.SetNamespaces(cssettings);
			
			cssettings.Framework = "v4.0";
			cssettings.Language = ScriptLanguage.CSharp;
			
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\tLoaded Script Settings...");
		}
		
		/// <summary>
		/// Loads all the npc scripts.
		/// </summary>
		public static void LoadNPCScripts()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("\tLoading NPC Scripts...");
			
			ScriptSettings x = ScriptDatabase.cssettings.DeepClone();
			
			x.ScriptLocation = ServerDatabase.DatabaseLocation + "\\NPCScripts";
			x.AddScriptType(typeof(Packets.MessagePacket));
			x.AddScriptType(typeof(Packets.Message.MessageCore));
			x.AddScriptType(typeof(Packets.NPC.NPCHandler));
			x.AddScriptType(typeof(Data.ItemInfo));
			x.AddScriptType(typeof(Data.Equipments));
			x.AddScriptType(typeof(Data.SpellInfo));
			x.AddScriptType(typeof(Data.Spell));
			x.AddScriptType(typeof(Data.SpellData));
			x.AddScriptType(typeof(Data.Team));
			x.AddScriptType(typeof(Data.NobilityBoard));
			x.AddScriptType(typeof(Data.NobilityDonation));
			x.AddScriptType(typeof(Maps.Map));
			x.AddScriptType(typeof(Maps.MapPoint));
			x.AddScriptType(typeof(Maps.MapTools));
			x.AddScriptType(typeof(Core.Kernel));
			x.AddScriptType(typeof(Data.Guild));
			x.AddScriptType(typeof(Data.GuildMember));
			x.AddScriptType(typeof(Database.CharacterDatabase));
			x.AddScriptType(typeof(Calculations.BasicCalculations));

			Type[] typelist = GetTypesInNamespace(System.Reflection.Assembly.GetExecutingAssembly(), "ProjectX_V3_Game.Enums");
			foreach (Type type in typelist)
				x.AddScriptType(type);
			
			Type[] typelist2 = GetTypesInNamespace(System.Reflection.Assembly.GetExecutingAssembly(), "ProjectX_V3_Game.Tournaments");
			foreach (Type type in typelist2)
				x.AddScriptType(type);
			
			Type[] typelist3 = GetTypesInNamespace(System.Reflection.Assembly.GetExecutingAssembly(), "ProjectX_V3_Game.Entities");
			foreach (Type type in typelist3)
				x.AddScriptType(type);
			
			Core.Kernel.ScriptEngine = new ScriptEngine(x, Core.TimeIntervals.ScriptUpdate); // scripts updates every 10 sec.
			Core.Kernel.ScriptEngine.Check_Updates();
			
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\tLoaded NPC Scripts...");
		}
		
		/// <summary>
		/// Loads all the item scripts.
		/// </summary>
		public static void LoadItemScripts()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("\tLoading Item Scripts...");
			
			ScriptSettings x = ScriptDatabase.cssettings.DeepClone();
			x.ScriptLocation = ServerDatabase.DatabaseLocation + "\\ItemScripts";
			x.AddScriptType(typeof(Packets.MessagePacket));
			x.AddScriptType(typeof(Packets.Message.MessageCore));
			x.AddScriptType(typeof(Packets.NPC.NPCHandler));
			x.AddScriptType(typeof(Data.ItemInfo));
			x.AddScriptType(typeof(Data.Equipments));
			x.AddScriptType(typeof(Data.SpellInfo));
			x.AddScriptType(typeof(Data.Spell));
			x.AddScriptType(typeof(Data.SpellData));
			x.AddScriptType(typeof(Data.Team));
			x.AddScriptType(typeof(Data.NobilityBoard));
			x.AddScriptType(typeof(Data.NobilityDonation));
			x.AddScriptType(typeof(Maps.Map));
			x.AddScriptType(typeof(Maps.MapPoint));
			x.AddScriptType(typeof(Maps.MapTools));
			x.AddScriptType(typeof(Core.Kernel));
			x.AddScriptType(typeof(Data.Guild));
			x.AddScriptType(typeof(Data.GuildMember));
			x.AddScriptType(typeof(Database.CharacterDatabase));
			x.AddScriptType(typeof(Calculations.BasicCalculations));
			
			Type[] typelist = GetTypesInNamespace(System.Reflection.Assembly.GetExecutingAssembly(), "ProjectX_V3_Game.Enums");
			foreach (Type type in typelist)
				x.AddScriptType(type);
			
			Type[] typelist2 = GetTypesInNamespace(System.Reflection.Assembly.GetExecutingAssembly(), "ProjectX_V3_Game.Tournaments");
			foreach (Type type in typelist2)
				x.AddScriptType(type);
			
			Type[] typelist3 = GetTypesInNamespace(System.Reflection.Assembly.GetExecutingAssembly(), "ProjectX_V3_Game.Entities");
			foreach (Type type in typelist3)
				x.AddScriptType(type);
			
			Core.Kernel.ItemScriptEngine = new ScriptEngine(x, Core.TimeIntervals.ScriptUpdate); // scripts updates every 10 sec.
			Core.Kernel.ItemScriptEngine.Check_Updates();
			
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\tLoaded Item Scripts...");
		}
	}
}
