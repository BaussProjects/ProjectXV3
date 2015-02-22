//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Client -> Server
	/// Server -> Client
	/// </summary>
	public class MovementPacket : DataPacket
	{
		static readonly sbyte[] DeltaX = new sbyte[] { 0, -1, -1, -1, 0, 1, 1, 1, 0 };
		static readonly sbyte[] DeltaY = new sbyte[] { 1, 1, 0, -1, -1, -1, 0, 1, 0 };
		static readonly sbyte[] DeltaMountX = new sbyte[] { 0, -2, -2, -2, 0, 2, 2, 2, 1, 0, -2, 0, 1, 0, 2, 0, 0, -2, 0, -1, 0, 2, 0, 1, 0 };
		static readonly sbyte[] DeltaMountY = new sbyte[] { 2, 2, 0, -2, -2, -2, 0, 2, 2, 0, -1, 0, -2, 0, 1, 0, 0, 1, 0, -2, 0, -1, 0, 2, 0 };
		
		public MovementPacket(DataPacket inPacket)
			: base(inPacket)
		{
		}
		
		public MovementPacket()
			: base (24, PacketType.MovementPacket)
		{
			
		}
		
		/// <summary>
		/// Gets or sets the direction.
		/// </summary>
		public uint Direction
		{
			get { return ReadUInt32(4); }
			set { WriteUInt32(value, 4); }
		}
		
		/// <summary>
		/// Gets or sets the entity UID.
		/// </summary>
		public uint EntityUID
		{
			get { return ReadUInt32(8); }
			set { WriteUInt32(value, 8); }
		}
		
		/// <summary>
		/// Gets or sets the walk mode.
		/// </summary>
		public Enums.WalkMode WalkMode
		{
			get { return (Enums.WalkMode)ReadByte(12); }
			set { WriteByte((byte)value, 12); }
		}
		
		/// <summary>
		/// Gets or sets the timestamp.
		/// </summary>
		public ProjectX_V3_Lib.Time.SystemTime TimeStamp
		{
			get { return new ProjectX_V3_Lib.Time.SystemTime(ReadUInt32(16)); }
			set { WriteUInt32(value, 16); }
		}
		/// <summary>
		/// Handles the movement packet.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="packet">The packet.</param>
		public static void Handle(Entities.GameClient client, DataPacket packet)
		{
			if (client.ContainsFlag1(Enums.Effect1.IceBlock))
				return;
			
			client.Action = Enums.ActionType.None;
			client.AttackPacket = null;
			client.LastMoveJump = false;
			using (var move = new MovementPacket(packet))
			{
				if (move.EntityUID != client.EntityUID)
					return;
				
				int NewX = 0, NewY = 0;
				int NewDir = 0;
				
				switch (move.WalkMode)
				{
					case Enums.WalkMode.Run:
					case Enums.WalkMode.Walk:
						{
							NewDir = (int)move.Direction % 8;
							NewX = client.X + DeltaX[NewDir];
							NewY = client.Y + DeltaY[NewDir];
							break;
						}
					case Enums.WalkMode.Mount:
						{
							NewDir = (int)move.Direction % 24;
							NewX = client.X + DeltaMountX[NewDir];
							NewY = client.Y + DeltaMountY[NewDir];
							break;
						}
				}
				
				if (client.Map.ValidCoord(NewX, NewY))
				{
					client.LastX = client.X;
					client.LastY = client.LastY;
					
					client.LastMovement = DateTime.Now;
					client.X = (ushort)NewX;
					client.Y = (ushort)NewY;
					client.Direction = (byte)NewDir;
					client.SendToScreen(move, true);
					
					if (client.Battle != null)
					{
						if (!client.Battle.EnterArea(client))
						{
							client.X = client.LastX;
							client.Y = client.LastY;
							client.Pullback();
							return;
						}
						else if (!client.Battle.LeaveArea(client))
						{
							client.X = client.LastX;
							client.Y = client.LastY;
							client.Pullback();
							return;
						}
					}					
					
					Data.AdvancedSkill.SkillInArea(client, client.X, client.Y);
				}
			}
		}
		
		public static Maps.MapPoint CreateDirectionPoint(ushort X, ushort Y, byte dir)
		{
			return new Maps.MapPoint(0, (ushort)(X + DeltaX[dir]), (ushort)(Y + DeltaY[dir]));
		}
	}
}
