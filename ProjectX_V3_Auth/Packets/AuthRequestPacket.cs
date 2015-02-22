//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;
using ProjectX_V3_Lib.Native;
using ProjectX_V3_Lib.Extensions;
using System.Net;
using System.Net.Sockets;

namespace ProjectX_V3_Auth.Packets
{
	/// <summary>
	/// Client -> Server.
	/// </summary>
	public class AuthRequestPacket : DataPacket
	{
		public AuthRequestPacket(DataPacket inPacket)
			: base(inPacket)
		{
		}
		
		/// <summary>
		/// Gets the account.
		/// </summary>
		public string Account
		{
			get { return ReadString(4, 16); }
		}
		
		/// <summary>
		/// Boolean defining whether the password has already been decrypted.
		/// </summary>
		private bool decrypted = false;
		
		/// <summary>
		/// The password holder.
		/// </summary>
		private string password;
		
		/// <summary>
		/// Gets the password and decrypts it if it's not already decrypted.
		/// </summary>
		public string Password
		{
			get
			{
				if (decrypted)
					return password;
				
				
				byte[] passbytes = ReadBytes(132, 16);
				Msvcrt.srand(90011337);
				byte[] rc5key = new byte[16];
				for (int i = 0; i < rc5key.Length; i++)
					rc5key[i] = (byte)Msvcrt.rand();
				
				password = new ProjectX_V3_Lib.Cryptography.PasswordCryptography(Account)
					.Decrypt(new ProjectX_V3_Lib.Cryptography.RC5(rc5key).Decrypt(passbytes), 16).GetString();
				password = password.Replace("-", "0");
				password = password.Replace("#", "1");
				password = password.Replace("(", "2");
				password = password.Replace("\"", "3");
				password = password.Replace("%", "4");
				password = password.Replace("\f", "5");
				password = password.Replace("'", "6");
				password = password.Replace("$", "7");
				password = password.Replace("&", "8");
				password = password.Replace("!", "9");
				password = password.MakeReadable(true, false, true, false, false);
				decrypted = true;
				return password;
			}
		}
		
		/// <summary>
		/// Handles the auth request packet.
		/// </summary>
		/// <param name="client">The auth client.</param>
		/// <param name="Packet">The packet.</param>
		public static void Handle(Client.AuthClient client, DataPacket Packet)
		{
			using (var auth = new AuthRequestPacket(Packet))
			{
				Enums.AccountStatus status = Database.ServerDatabase.Authenticate(client, auth.Account, auth.Password);
				client.EntityUID = (uint)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(1000000, 699999999);
				
				if (status == Enums.AccountStatus.Ready)
				{
					try
					{
						Database.ServerDatabase.UpdateAuthentication(client);
						Socket quicksock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
						
						quicksock.Connect(new IPEndPoint(IPAddress.Parse(Program.Config.ReadString("GameIP")), Program.Config.ReadInt32("GameAuthPort")));
						
						using (DataPacket packet = new DataPacket(44, 9001))
						{
							packet.WriteString(Program.Config.ReadString("ServerPassword"), 4);
							packet.WriteString(client.Account, 20);
							packet.WriteInt32(client.DatabaseUID, 36);
							packet.WriteUInt32(client.EntityUID, 40);
							quicksock.BeginSend(packet.Copy(), 0, 44, SocketFlags.None,
							                    new AsyncCallback((ia) =>
							                                      {
							                                      	int send = quicksock.EndSend(ia);
							                                      	if (send != 44)
							                                      	{
							                                      		status = Enums.AccountStatus.Datebase_Error;
							                                      		client.EntityUID = 0;
							                                      	}
							                                      	Console.WriteLine("Database Notified: [Account: {0}] [DUID: {1}] [EUID: {2}]", client.Account, client.DatabaseUID, client.EntityUID);
							                                      }), null);
							
						}
						System.Threading.Thread.Sleep(2000);
					}
					catch
					{
						status = Enums.AccountStatus.Datebase_Error;
					}
				}
				
				using (var resp = new AuthResponsePacket())
				{
					if (status == Enums.AccountStatus.Ready)
					{
						Console.WriteLine("Incoming login. [Account: {0}] [Password: {1}]", client.Account, client.Password);

						resp.EntityUID = client.EntityUID;
						resp.Port = Program.Config.ReadUInt32("GamePort");
						resp.IPAddress = Program.Config.ReadString("GameIP");
					}
					else
					{
						resp.EntityUID = 0;
					}
					resp.AccountStatus = status;
					client.Send(resp);
				}
				System.Threading.Thread.Sleep(5000);
				client.NetworkClient.Disconnect("TIME_OUT");
			}
		}
	}
}
