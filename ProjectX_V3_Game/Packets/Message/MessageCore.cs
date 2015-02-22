//Project by BaussHacker aka. L33TS

using System;
using System.Linq;

namespace ProjectX_V3_Game.Packets.Message
{
	/// <summary>
	/// The core handler of MessagePacket.
	/// </summary>
	public class MessageCore
	{
		public static MessagePacket CreateSystem(string to, string message)
		{
			return Create(Enums.ChatType.System, System.Drawing.Color.Red, "SYSTEM", to, message);
		}
		public static MessagePacket CreateHawk(string sender, string message)
		{
			return Create(Enums.ChatType.HawkMessage, System.Drawing.Color.Yellow, sender, "ALL", message);
		}
		public static MessagePacket CreateSystem2(string to, string message)
		{
			return Create(Enums.ChatType.TopLeft, System.Drawing.Color.Red, "SYSTEM", to, message);
		}
		public static MessagePacket CreateSystem3(string to, string message)
		{
			return Create(Enums.ChatType.TopLeft, System.Drawing.Color.White, "SYSTEM", to, message);
		}
		public static MessagePacket CreateBroadcast(string sender, string message)
		{
			return Create(Enums.ChatType.Broadcast, System.Drawing.Color.White, sender, "ALL", message);
		}
		public static MessagePacket CreateSystem4(System.Drawing.Color color, string to, string message)
		{
			return Create(Enums.ChatType.TopLeft, color, "SYSTEM", to, message);
		}
		public static MessagePacket CreateSystem5(System.Drawing.Color color, string to, string message)
		{
			return Create(Enums.ChatType.System, color, "SYSTEM", to, message);
		}
		public static MessagePacket CreateTalk(string sender, string to, string message)
		{
			return Create(Enums.ChatType.Talk, System.Drawing.Color.Yellow, sender, to, message);
		}
		public static MessagePacket CreateTopLeft(string sender, string to, string message)
		{
			return Create(Enums.ChatType.TopLeft, System.Drawing.Color.Yellow, sender, to, message);
		}
		public static MessagePacket CreateSlide(string message)
		{
			return Create(Enums.ChatType.SlideFromRight, System.Drawing.Color.Purple, "SYSTEM", "ALLUSERS", message);
		}
		public static MessagePacket CreateSlide(System.Drawing.Color color, string message)
		{
			return Create(Enums.ChatType.SlideFromRight, color, "SYSTEM", "ALLUSERS", message);
		}
		public static MessagePacket CreateScore(string message)
		{
			return Create(Enums.ChatType.Right, "SYSTEM", "ALLUSERS", message);
		}
		public static MessagePacket CreateCenter(string message)
		{
			return Create(Enums.ChatType.Center, "SYSTEM", "ALLUSERS", message);
		}
		public static MessagePacket CreateCenter(string sender, string message)
		{
			return Create(Enums.ChatType.Center, "SYSTEM", "ALLUSERS", sender + ": " + message);
		}
		public static MessagePacket ClearScore()
		{
			return Create(Enums.ChatType.BeginRight, "SYSTEM", "ALLUSERS", string.Empty);
		}
		public static MessagePacket Create(Enums.ChatType chattype, string sender, string to, string message)
		{
			return Create(chattype, System.Drawing.Color.Yellow, sender, to, message);
		}

		public static MessagePacket Create(Enums.ChatType chattype, System.Drawing.Color color, string sender, string to, string message)
		{
			StringPacker msg = new StringPacker(sender, to, "", message);
			MessagePacket msgpacket = new MessagePacket(msg);
			msgpacket.Color = color;
			msgpacket.ChatType = chattype;
			return msgpacket;
		}
		
		public static MessagePacket CreateLogin(string message)
		{
			return Create(Enums.ChatType.Login, System.Drawing.Color.Red, "SYSTEM", "ALLUSERS", message);
		}
		
		public static MessagePacket CreateCharacter(string message)
		{
			return Create(Enums.ChatType.CharacterCreation, System.Drawing.Color.Red, "SYSTEM", "ALLUSERS", message);
		}
		
		public static void SendGlobalMessage(MessagePacket packet)
		{
			int failed;
			if (!Core.Kernel.Clients.TryForeachAction((uid, client) =>
			                                          {
			                                          		client.Send(packet);
			                                          }, out failed))
			{
				Console.WriteLine("{0} clients did not receive the message.");
			}
		}
		
		public static void SendGlobalMessage(MessagePacket packet, params uint[] exclude)
		{
			int failed;
			if (!Core.Kernel.Clients.TryForeachAction((uid, client) =>
			                                          {
			                                          	if (!exclude.Contains(uid))
			                                          		client.Send(packet);
			                                          }, out failed))
			{
				Console.WriteLine("{0} clients did not receive the message.");
			}
		}
	}
}
