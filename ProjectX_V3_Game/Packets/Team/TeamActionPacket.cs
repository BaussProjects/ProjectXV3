//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Description of TeamActionPacket.
	/// </summary>
	public class TeamActionPacket : DataPacket
	{
		public TeamActionPacket()
			: base(20, PacketType.TeamActionPacket)
		{
		}
		
		public TeamActionPacket(DataPacket inPacket)
			: base(inPacket)
		{
		}
		
		public Enums.TeamAction Action
		{
			get { return (Enums.TeamAction)ReadUInt32(4); }
			set { WriteUInt32((uint)value, 4); }
		}
		
		public uint EntityUID
		{
			get { return ReadUInt32(8); }
			set { WriteUInt32(value, 8); }
		}
		
		public static void Handle(Entities.GameClient client, DataPacket packet)
		{
			using (var team = new TeamActionPacket(packet))
			{
				switch (team.Action)
				{
					case Enums.TeamAction.Create:
						if (client.Battle != null)
							return;
						Data.Team.Create(client, team);
						break;
					case Enums.TeamAction.Dismiss:
						{
							if (client.Battle != null)
								return;
							if (client.Team == null)
								return;
							client.Team.Delete(client, team);
							break;
						}
					case Enums.TeamAction.RequestJoin:
						{
							if (client.Battle != null)
								return;
							if (client.Team != null)
								return;
							Entities.GameClient Leader;
							if (Core.Kernel.Clients.TrySelect(team.EntityUID, out Leader))
							{
								if (Leader.Team == null)
									return;
								if (!Leader.IsInMap(client))
									return;
								
								Leader.Team.Join(client, team);
							}
							break;
						}
					case Enums.TeamAction.AcceptJoin:
						{
							if (client.Battle != null)
								return;
							if (client.Team == null)
								return;
							client.Team.Join(client, team);
							break;
						}
					case Enums.TeamAction.RequestInvite:
						{
							if (client.Battle != null)
								return;
							if (client.Team == null)
								return;
							client.Team.Invite(client, team);
							break;
						}
					case Enums.TeamAction.AcceptInvite:
						{
							if (client.Battle != null)
								return;
							if (client.Team != null)
								return;
							Entities.GameClient Leader;
							if (Core.Kernel.Clients.TrySelect(team.EntityUID, out Leader))
							{
								if (Leader.Team == null)
									return;
								if (!Leader.IsInMap(client))
									return;
								
								Leader.Team.Invite(client, team);
							}
							break;
						}
					case Enums.TeamAction.Kick:
						{
							if (client.Battle != null)
								return;
							if (client.Team == null)
								return;
							client.Team.Kick(client, team);
							break;
						}
					case Enums.TeamAction.LeaveTeam:
						{
							if (client.Battle != null)
								return;
							if (client.Team == null)
								return;
							client.Team.Leave(client, team);
							break;
						}
				}
			}
		}
	}
}
