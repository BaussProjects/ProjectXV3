//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;
using ProjectX_V3_Lib.IO;
using ProjectX_V3_Lib.Extensions;

namespace ProjectX_V3_Auth
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
		
		/// <summary>
		/// Program Entry.
		/// </summary>
		/// <param name="args">Process arguments.</param>
		public static void Main(string[] args)
		{
			Console.Title = "ProjectX V3 - Auth Server";
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
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Starting the server...");
			try
			{
				SocketEvents sockEvents = new SocketEvents();
				sockEvents.OnConnection = new ConnectionEvent(Network.NetworkConnections.Handle_Connection);
				sockEvents.OnReceive = new BufferEvent(Network.NetworkConnections.Handle_Receive);
				int[] ports;
				config.ReadString("Ports").Split(',').ConverToInt32(out ports);
				foreach (int port in ports)
					BeginServer(sockEvents, port);
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("The server is open...");
				Console.ResetColor();
				
				while (true)
					Console.ReadLine();
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Could not open the server...");
				Console.WriteLine(e.ToString());
				Console.ReadLine();
			}
		}
		
		/// <summary>
		/// Begins the server.
		/// </summary>
		/// <param name="sockEvents">The socket events.</param>
		/// <param name="port">The port.</param>
		private static void BeginServer(SocketEvents sockEvents, int port)
		{
			SocketServer server = new SocketServer(sockEvents);
			server.Start(config.ReadString("IPAddress"), port);
		}
	}
}