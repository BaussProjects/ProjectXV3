//Project by BaussHacker aka. L33TS

using System;
using System.Collections.Concurrent;

namespace ProjectX_V3_Game.Data
{
	/// <summary>
	/// Description of Team.
	/// </summary>
	public class Team
	{
		public ConcurrentDictionary<uint, Entities.GameClient> Members;
		private uint Leader = 0;
		public bool IsLeader(Entities.GameClient client)
		{
			if (Leader == 0)
				return false;
			
			return Leader == client.EntityUID;
		}
		public Team()
		{
			Members = new ConcurrentDictionary<uint, ProjectX_V3_Game.Entities.GameClient>();
		}
		
		public void Kick(Entities.GameClient client, Packets.TeamActionPacket packet)
		{
			if (Leader != client.EntityUID)
				return;
			Entities.GameClient rClient;
			if (Members.TryRemove(packet.EntityUID, out rClient))
			{
				rClient.Team = null;
				rClient.Send(packet);
				foreach (Entities.GameClient member in Members.Values)
					member.Send(packet);
			}
		}
		
		public void Delete(Entities.GameClient client, Packets.TeamActionPacket packet)
		{
			if (!client.Alive)
				return;
			if (Leader != client.EntityUID)
				return;
			
			client.RemoveFlag1(Enums.Effect1.TeamLeader);
			foreach (Entities.GameClient member in Members.Values)
			{
				member.Team = null;
				member.Send(packet);
			}
			
			Members.Clear();
			Leader = 0;
		}
		
		public void Leave(Entities.GameClient client, Packets.TeamActionPacket packet)
		{
			if (Leader == client.EntityUID)
				return;
			Entities.GameClient rClient;
			if (Members.TryRemove(client.EntityUID, out rClient))
			{
				rClient.Team = null;
				packet.EntityUID = client.EntityUID;
				foreach (Entities.GameClient member in Members.Values)
					member.Send(packet);
				
				client.Send(packet);
			}
		}
		
		public static void Create(Entities.GameClient client, Packets.TeamActionPacket packet)
		{
			if (packet.EntityUID != client.EntityUID)
				return;
			if (client.Team != null)
				return;
			
			client.Team = new Team();
			client.Team.Leader = client.EntityUID;
			if (client.Team.Members.TryAdd(client.EntityUID, client))
			{
				using (var create = new Packets.TeamActionPacket())
				{
					create.EntityUID = client.EntityUID;
					create.Action = Enums.TeamAction.Leader;
					client.Send(create);
					create.Action = Enums.TeamAction.Create;
					client.Send(create);
				}
				client.AddStatusEffect1(Enums.Effect1.TeamLeader, 0);
			}
			else
			{
				client.Team = null;
			}
		}
		
		private uint NextJoin = 0;
		public void Join(Entities.GameClient client, Packets.TeamActionPacket packet)
		{
			if (Members.Count >= 5)
				return;
			
			if (Leader == client.EntityUID && packet.Action == Enums.TeamAction.AcceptJoin)
			{
				// accept
				Entities.GameClient newMember;
				if (Core.Kernel.Clients.TrySelect(NextJoin, out newMember))
				{
					if (Members.TryAdd(newMember.EntityUID, newMember))
					{
						newMember.Team = this;
						// send team member ...
						using (var teammember = new Packets.TeamMemberPacket())
						{
							teammember.Name = newMember.Name;
							teammember.EntityUID = newMember.EntityUID;
							teammember.Mesh = newMember.Mesh;
							teammember.MaxHealth = (ushort)newMember.MaxHP;
							teammember.CurrentHealth = (ushort)newMember.HP;
							foreach (Entities.GameClient member in Members.Values)
							{
								if (member.EntityUID != newMember.EntityUID)
								{
									member.Send(teammember);
									using (var teammember2 = new Packets.TeamMemberPacket())
									{
										teammember2.Name = member.Name;
										teammember2.EntityUID = member.EntityUID;
										teammember2.Mesh = member.Mesh;
										teammember2.MaxHealth = (ushort)member.MaxHP;
										teammember2.CurrentHealth = (ushort)member.HP;
										newMember.Send(teammember2);
									}
								}
							}
							newMember.Send(teammember);
						}
						
						packet.EntityUID = client.EntityUID;
						newMember.Send(packet);
					}
				}
			}
			else if (packet.EntityUID == Leader && packet.Action == Enums.TeamAction.RequestJoin)
			{
				Entities.GameClient LeaderClient;
				if (Members.TryGetValue(Leader, out LeaderClient))
				{
					NextJoin = client.EntityUID;
					packet.EntityUID = client.EntityUID;
					LeaderClient.Send(packet);
				}
			}
		}
		
		private uint NextInvite = 0;
		public void Invite(Entities.GameClient client, Packets.TeamActionPacket packet)
		{
			if (Members.Count >= 5)
				return;
			
			if (Leader != client.EntityUID &&
			    packet.Action == Enums.TeamAction.AcceptInvite &&
			    NextInvite == client.EntityUID)
			{
				// accept
				if (Members.TryAdd(client.EntityUID, client))
				{
					// send team member ...
					client.Team = this;
					using (var teammember = new Packets.TeamMemberPacket())
					{
						teammember.Name = client.Name;
						teammember.EntityUID = client.EntityUID;
						teammember.Mesh = client.Mesh;
						teammember.MaxHealth = (ushort)client.MaxHP;
						teammember.CurrentHealth = (ushort)client.HP;
						foreach (Entities.GameClient member in Members.Values)
						{
							if (member.EntityUID != client.EntityUID)
							{
								member.Send(teammember);
								using (var teammember2 = new Packets.TeamMemberPacket())
								{
									teammember2.Name = member.Name;
									teammember2.EntityUID = member.EntityUID;
									teammember2.Mesh = member.Mesh;
									teammember2.MaxHealth = (ushort)member.MaxHP;
									teammember2.CurrentHealth = (ushort)member.HP;
									client.Send(teammember2);
								}
							}
						}
						client.Send(teammember);
					}
					
					packet.EntityUID = Leader;
					client.Send(packet);
				}
			}
			else if (packet.EntityUID != Leader && packet.Action == Enums.TeamAction.RequestInvite && client.EntityUID == Leader)
			{
				Entities.GameClient newMember;
				if (Core.Kernel.Clients.TrySelect(packet.EntityUID, out newMember))
				{
					NextInvite = newMember.EntityUID;
					packet.EntityUID = client.EntityUID;
					newMember.Send(packet);
				}
			}
		}
	}
}
