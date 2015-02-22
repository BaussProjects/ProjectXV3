//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;
using ProjectX_V3_Lib.IO;
using ProjectX_V3_Lib.Extensions;
using ProjectX_V3_Lib.Sql;

namespace ProjectX_V3_Game
{
	class Program
	{
		/// <summary>
		/// The configuration file.
		/// </summary>
		private static XmlConfig config;
		
		/// <summary>
		/// Gets the configuration file.
		/// </summary>
		public static XmlConfig Config
		{
			get { return config; }
		}
		
		public static void Test()
		{
			Console.WriteLine("{0}", DateTime.Now.Millisecond);
		}
		
		/// <summary>
		/// Program entry.
		/// </summary>
		/// <param name="args">Process arguments.</param>
		public static void Main(string[] args)
		{
			
			Console.Title = "ProjectX V3 - Game Server";

			try
			{
				config = new XmlConfig();
				config.LoadConfig(Database.ServerDatabase.DatabaseLocation + "\\Config.xml");
			}
			catch (Exception e)
			{
				Console.WriteLine("Could not load the config.");
				Console.WriteLine("Error:");
				Console.WriteLine(e);
				Console.ReadLine();
				return;
			}

			if (!Database.ServerDatabase.Load())
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Could not load the database.");
				Console.ReadLine();
				return;
			}

			Threads.GlobalThreads.Start();
			
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Starting the server...");
			try
			{
				Network.GameAuth.Start();
				SocketEvents sockEvents = new SocketEvents();
				sockEvents.OnConnection = new ConnectionEvent(Network.NetworkConnections.Handle_Connection);
				sockEvents.OnDisconnection = new ConnectionEvent(Network.NetworkConnections.Handle_Disconnection);
				sockEvents.OnReceive = new BufferEvent(Network.NetworkConnections.Handle_Receive);
				BeginServer(sockEvents, config.ReadInt32("GamePort"));
				
				ProjectX_V3_Lib.Native.Kernel32.SetConsoleCtrlHandler(
					new ProjectX_V3_Lib.Native.Kernel32.ConsoleEventHandler(Console_CloseEvent),
					true);
				
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("The server is open...");
				Console.ResetColor();
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Could not open the server...");
				Console.WriteLine(e.ToString());
				Console.ReadLine();
				return;
			}
			
			while (true)
				HandleCmd(Console.ReadLine());
		}
		
		public static bool AllowConnections = true;
		public static void Shutdown(bool force = false)
		{
			AllowConnections = false;
			if (!force)
			{
				using (var msg = Packets.Message.MessageCore.CreateCenter("Server shutdown in 1 minute. Please logout."))
					Packets.Message.MessageCore.SendGlobalMessage(msg);
				System.Threading.Thread.Sleep(60000);
			}
			int failed = 1;
			do
			{
				Core.Kernel.Clients.TryForeachAction((uid, client) => { client.NetworkClient.Disconnect("Server shutdown."); }, out failed);
			}
			while (failed != 0);
			if (!force)
			{
				Environment.Exit(0);
			}
		}
		/// <summary>
		/// Begins the server.
		/// </summary>
		/// <param name="sockEvents">The socket events.</param>
		/// <param name="port">The port.</param>
		private static void BeginServer(SocketEvents sockEvents, int port)
		{
			AllowConnections = true;
			SocketServer server = new SocketServer(sockEvents);
			server.Start(config.ReadString("IPAddress"), port);
		}
		
		
		/// <summary>
		/// Handles server command. [Nothing yet]
		/// </summary>
		/// <param name="cmd">The command string.</param>
		public static void HandleCmd(string cmd)
		{
			string[] Command = cmd.Split(' ');
			switch (Command[0])
			{
				case "/exit":
					Shutdown(false);
					break;
			}
		}
		
		/// <summary>
		/// Called whenever the server shuts down CORRECT!
		/// </summary>
		/// <param name="sig"></param>
		/// <returns></returns>
		private static bool Console_CloseEvent(ProjectX_V3_Lib.Native.Kernel32.CtrlType sig)
		{
			AllowConnections = false;
			try
			{
				Shutdown(true);
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				return false;
			}
		}
	}
}