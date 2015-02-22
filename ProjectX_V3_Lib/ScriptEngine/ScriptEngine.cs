//Project by BaussHacker aka. L33TS

using System;
using System.Threading;
using System.Text;
using System.Linq;
using Microsoft.CSharp;
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.VisualBasic;
using System.Collections.Generic;

namespace ProjectX_V3_Lib.ScriptEngine
{
	/// <summary>
	/// Description of ScriptEngine.
	/// </summary>
	public class ScriptEngine
	{
		/// <summary>
		/// The settings associated with the script engine.
		/// </summary>
		private ScriptSettings Settings;
		
		/// <summary>
		/// The thread checking for script updates.
		/// </summary>
	//	private Threading.BaseThread scriptCheckerThread;
		
		/// <summary>
		/// The interval between each script update.
		/// </summary>
		private int checkInterval;
		
		/// <summary>
		/// Creates a new instance of ScriptEngine.
		/// </summary>
		/// <param name="Settings">The settings associated to the script engine.</param>
		/// <param name="scriptcheckinterval">The interval between each script update.</param>
		public ScriptEngine(ScriptSettings Settings, int scriptcheckinterval)
		{
			this.checkInterval = scriptcheckinterval;
			this.Settings = Settings;
			scriptCollection = new ScriptCollection(Settings);
			//scriptCheckerThread = new ProjectX_V3_Lib.Threading.BaseThread(new Threading.ThreadAction(Check_Updates),
			                                                      //         scriptcheckinterval, "Script Engine");
			//scriptCheckerThread.Start();
		}
		
		public static void SetNamespaces(ScriptSettings settings)
		{
			Content = Content.Replace("__namespace__", getns(settings));
			Content2 = Content2.Replace("__namespace__", getns2(settings));
		}
		
		/// <summary>
		/// The c# code content.
		/// </summary>
		private static string Content = @"__namespace__

namespace scriptnamespace
{
	class scriptclass
	{
		__method__
	}
}";
		
		/// <summary>
		/// The vb code content.
		/// </summary>
		private static string Content2 = @"__namespace__

Namespace scriptnamespace
	Class scriptclass
	
		__method__
		
	End Class
End Namespace";
		
		/// <summary>
		/// Gets the namespace code for c#.
		/// </summary>
		/// <returns>Returns the namespace code.</returns>
		private static string getns(ScriptSettings Settings)
		{
			StringBuilder namespaceBuilder = new StringBuilder();
			foreach (string _namespace in Settings._namespaces.Values)
			{
				namespaceBuilder.Append("using ").Append(_namespace).Append(";").Append(Environment.NewLine);
			}
			return namespaceBuilder.ToString();
		}
		
		/// <summary>
		/// Gets the namespace code for vb.
		/// </summary>
		/// <returns>Returns the namespace code.</returns>
		private static string getns2(ScriptSettings Settings)
		{
			StringBuilder namespaceBuilder = new StringBuilder();
			foreach (string _namespace in Settings._namespaces.Values)
			{
				namespaceBuilder.Append("Imports ").Append(_namespace).Append(Environment.NewLine);
			}
			return namespaceBuilder.ToString();
		}
		
		private string currentcompilefile;
		
		/// <summary>
		/// Checks for updates.
		/// </summary>
		public void Check_Updates()
		{
			try
			{
				foreach (string file in System.IO.Directory.GetFiles(Settings.ScriptLocation + "\\cmpl"))
				{
					try
					{
						System.IO.File.Delete(file);
					}
					catch { } // no permission
				}
				
				DateTime now = DateTime.Now;
				currentcompilefile = "\\cmpl\\cmpl_" + now.Month + "-" + now.Day + "-" + now.Hour + "-" + now.Minute + "-" + now.Second;
				
				switch (Settings.Language)
				{
					case ScriptLanguage.CSharp:
						{
							StringBuilder scriptBuilder = new StringBuilder();
							foreach (string file in System.IO.Directory.GetFiles(Settings.ScriptLocation))
							{
								if (file.EndsWith(".cs"))
								{
									scriptBuilder.Append(System.IO.File.ReadAllText(file));
									scriptBuilder.Append(Environment.NewLine);
								}
							}
							System.IO.File.WriteAllText(Settings.ScriptLocation + currentcompilefile + ".cs", Content.Replace("__method__", scriptBuilder.ToString()));
							CompileCSScripts();
							break;
						}
					case ScriptLanguage.VisualBasic:
						{
							StringBuilder scriptBuilder = new StringBuilder();
							foreach (string file in System.IO.Directory.GetFiles(Settings.ScriptLocation))
							{
								if (file.EndsWith(".vb"))
								{
									scriptBuilder.Append(System.IO.File.ReadAllText(file));
									scriptBuilder.Append(Environment.NewLine);
								}
							}
							System.IO.File.WriteAllText(Settings.ScriptLocation + currentcompilefile + ".vb", Content2.Replace("__method__", scriptBuilder.ToString()));
							CompileCSScripts();
							break;
						}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Script loading failed... Exception: {0}{1}", Environment.NewLine, e.ToString());
			}
		}
		
		/// <summary>
		/// Compiles all the c# scripts.
		/// </summary>
		private void CompileCSScripts()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("CompilerVersion", Settings.Framework);
			CompilerParameters compilerParameters = new CompilerParameters
			{
				GenerateInMemory = true
			};
			
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Assembly assembly = assemblies[i];
				compilerParameters.ReferencedAssemblies.Add(assembly.Location);
			}
			foreach (string refs in System.IO.Directory.GetFiles(Settings.ScriptLocation + "\\references"))
				compilerParameters.ReferencedAssemblies.Add(refs);
			foreach (Type type in Settings.types.Values)
				compilerParameters.ReferencedAssemblies.Add(Assembly.GetAssembly(type).Location);
			CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider();
			CompilerResults compilerResults = cSharpCodeProvider.CompileAssemblyFromFile(compilerParameters, Settings.ScriptLocation + currentcompilefile + ".cs");
			if (compilerResults.Errors.Count != 0)
			{
				foreach (CompilerError err in compilerResults.Errors)
					Console.WriteLine(err.ToString());
			}
			else
			{
				foreach (Type type in compilerResults.CompiledAssembly.GetTypes())
				{
					if (type.Namespace == "scriptnamespace" && type.IsClass && type.Name == "scriptclass")
					{
						foreach (MethodInfo method in type.GetMethods())
						{
							if (method.IsStatic)
							{
								if (method.Name.StartsWith("script_"))
									scriptCollection.AddOrUpdate(uint.Parse(method.Name.Split('_')[1]), method);
							}
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Compiles all the vb scripts.
		/// </summary>
		private void CompileVBScripts()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("CompilerVersion", Settings.Framework);
			CompilerParameters compilerParameters = new CompilerParameters
			{
				GenerateInMemory = true
			};
			
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Assembly assembly = assemblies[i];
				compilerParameters.ReferencedAssemblies.Add(assembly.Location);
			}
			foreach (string refs in System.IO.Directory.GetFiles(Settings.ScriptLocation + "\\references"))
				compilerParameters.ReferencedAssemblies.Add(refs);
			foreach (Type type in Settings.types.Values)
				compilerParameters.ReferencedAssemblies.Add(Assembly.GetAssembly(type).Location);
			VBCodeProvider vbCodeProvider = new VBCodeProvider();
			CompilerResults compilerResults = vbCodeProvider.CompileAssemblyFromFile(compilerParameters, Settings.ScriptLocation + currentcompilefile + ".vb");
			if (compilerResults.Errors.Count != 0)
			{
				foreach (CompilerError err in compilerResults.Errors)
					Console.WriteLine(err.ToString());
			}
			else
			{
				foreach (Type type in compilerResults.CompiledAssembly.GetTypes())
				{
					if (type.Namespace == "scriptnamespace" && type.IsClass && type.Name == "scriptclass")
					{
						foreach (MethodInfo method in type.GetMethods())
						{
							if (method.IsStatic)
							{
								if (method.Name.StartsWith("script_"))
									scriptCollection.AddOrUpdate(uint.Parse(method.Name.Split('_')[1]), method);
							}
						}
					}
				}
			}
		}
		
		/// <summary>
		/// The collection of the scripts.
		/// </summary>
		private ScriptCollection scriptCollection;
		
		/// <summary>
		/// Invokes a script.
		/// </summary>
		/// <param name="key">The script key.</param>
		/// <param name="paramters">Parameters associated with the script. [null, if no parameters]</param>
		/// <returns>Returns true if the script exist.</returns>
		public bool Invoke(uint key, object[] paramters)
		{
			return scriptCollection.Invoke(key, paramters);
		}
	}
}
