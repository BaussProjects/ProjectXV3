//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets.GeneralData
{
	/// <summary>
	/// Subtype: 137
	/// </summary>
	public class Jump
	{
		/// <summary>
		/// Handling the Jump action from GeneralDataPacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="General">The GeneralDataPacket.</param>
		public static void Handle(Entities.GameClient client, GeneralDataPacket General)
		{
			if (!client.Alive)
				return;
			if (client.ContainsFlag1(Enums.Effect1.IceBlock))
				return;
			
			if (client.Booth != null)
			{
				client.Booth.CancelBooth();
				client.Pullback();
			}
			
			client.Action = Enums.ActionType.None;
			client.AttackPacket = null;
			
			ushort JumpX = General.Data1Low;
			ushort JumpY = General.Data1High;
			
			if (Core.Screen.GetDistance(client.X, client.Y, JumpX, JumpY) > 28)
			{
				client.Pullback();
				return;
			}
			
			if (!client.Map.ValidCoord(JumpX, JumpY))
			{
				client.Pullback();
				return;
			}
			/*DateTime time = new ProjectX_V3_Lib.Time.SystemTime(General.Timestamp);
			if (client.LastMovement > time)
			{
				client.Pullback(); // speedhack
				return;
			}*/
			if (!(DateTime.Now >= client.LastMovement.AddMilliseconds(400)) && client.LastMoveJump)
			{
				client.Pullback(); // speedhack
				return;
			}
			
			if (client.Battle != null)
			{
				if (!client.Battle.EnterArea(client))
				{
					client.Pullback();
					return;
				}
				else if (!client.Battle.LeaveArea(client))
				{
					client.Pullback();
					return;
				}
			}
			
			if (Calculations.BasicCalculations.ChanceSuccess(50))
				client.Stamina += 7;
			
			client.LastMoveJump = true;
			client.LastMovement = DateTime.Now;
			client.LastX = client.X;
			client.LastY = client.Y;
			client.X = JumpX;
			client.Y = JumpY;
			client.SendToScreen(General, true);
			Data.AdvancedSkill.SkillInArea(client, client.X, client.Y);
		}
	}
}
