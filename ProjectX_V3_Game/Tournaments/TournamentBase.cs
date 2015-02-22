//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Tournaments
{
	/// <summary>
	/// Description of ITournament.
	/// </summary>
	public abstract class TournamentBase
	{
		public int[] StartHours = new int[] { 00 };
		public int StartMinute = 00;
		public string Name;
		public TournamentBase(string Name, int StartMinute, params int[] StartHours)
		{
			this.StartHours = StartHours;
			this.StartMinute = StartMinute;
			this.Name = Name;
		}

		public abstract int MeasureEndTime();
		public abstract void Start();
		public abstract void Send();
		public abstract void End();
		public abstract bool SignUp(Entities.GameClient client, out bool AlreadySigned);
		
		public void SendMessage(string Message)
		{
			using (var msg = Packets.Message.MessageCore.CreateCenter(Message))
			{
				Packets.Message.MessageCore.SendGlobalMessage(msg);
			}
		}
		public void SendMessageBC(string Message)
		{
			using (var msg = Packets.Message.MessageCore.CreateBroadcast("SYSTEM", Message))
			{
				Packets.Message.MessageCore.SendGlobalMessage(msg);
			}
		}
		
		public void SendMessage(Entities.GameClient client, string Message)
		{
			using (var msg = Packets.Message.MessageCore.CreateCenter(Message))
			{
				client.Send(msg);
			}
		}
		
		public abstract void UpdateBroadcast();
	}
}
