//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;
using ProjectX_V3_Lib.Time;

namespace ProjectX_V3_Game.Packets
{
	// temp structure from albetros...
	
	public unsafe struct GeneralDataPacket
	{
		#region structure
		public uint Id;
		public uint Data1;
		public uint Data2;
		public uint Timestamp;
		public Enums.DataAction Action;
		public ushort Direction;
		public uint Data3;
		public uint Data4;
		public uint Data5;
		public byte Data6;

		#region Data1

		/// <summary>
		/// Offset [8/0x08]
		/// </summary>
		public ushort Data1Low
		{
			get { return (ushort)Data1; }
			set { Data1 = (uint)((Data1High << 16) | value); }
		}

		/// <summary>
		/// Offset [10/0x0a]
		/// </summary>
		public ushort Data1High
		{
			get { return (ushort)(Data1 >> 16); }
			set { Data1 = (uint)((value << 16) | Data1Low); }
		}

		#endregion

		#region Data2

		/// <summary>
		/// Offset [12/0x0c]
		/// </summary>
		public ushort Data2Low
		{
			get { return (ushort)Data2; }
			set { Data2 = (uint)((Data2High << 16) | value); }
		}

		/// <summary>
		/// Offset [14/0x0e]
		/// </summary>
		public ushort Data2High
		{
			get { return (ushort)(Data2 >> 16); }
			set { Data2 = (uint)((value << 16) | Data2Low); }
		}

		#endregion

		#region Data3

		/// <summary>
		/// Offset [24/0x18]
		/// </summary>
		public ushort Data3Low
		{
			get { return (ushort)Data3; }
			set { Data3 = (uint)((Data3High << 16) | value); }
		}

		/// <summary>
		/// Offset [26/0x1a]
		/// </summary>
		public ushort Data3High
		{
			get { return (ushort)(Data3 >> 16); }
			set { Data3 = (uint)((value << 16) | Data3Low); }
		}

		#endregion

		#region Data4

		/// <summary>
		/// Offset [28/0x1c]
		/// </summary>
		public ushort Data4Low
		{
			get { return (ushort)Data4; }
			set { Data4 = (uint)((Data4High << 16) | value); }
		}

		/// <summary>
		/// Offset [30/0x1e]
		/// </summary>
		public ushort Data4High
		{
			get { return (ushort)(Data4 >> 16); }
			set { Data4 = (uint)((value << 16) | Data4Low); }
		}

		#endregion

		#region Data5

		/// <summary>
		/// Offset [32/0x20]
		/// </summary>
		public ushort Data5Low
		{
			get { return (ushort)Data5; }
			set { Data5 = (uint)((Data5High << 16) | value); }
		}

		/// <summary>
		/// Offset [34/0x32]
		/// </summary>
		public ushort Data5High
		{
			get { return (ushort)(Data5 >> 16); }
			set { Data5 = (uint)((value << 16) | Data5Low); }
		}

		#endregion
		public static DataPacket GeneralDataA(uint UID, uint A, uint B, uint C, ushort Type, ushort E, ushort X, ushort Y)
		{
			DataPacket Packet = new DataPacket(45, 10010);
			Packet.WriteUInt32(UID, 4);
			Packet.WriteUInt32(A, 8);
			Packet.WriteUInt32(B, 12);
			Packet.WriteUInt32(C, 16);
			Packet.WriteUInt16(Type, 20);
			Packet.WriteUInt16(E, 22);
			Packet.WriteUInt16(X, 24);
			Packet.WriteUInt16(Y, 26);
			Packet.WriteUInt32(0, 28);
			Packet.WriteUInt32(0, 32);
			Packet.WriteByte(0, 36);
			return Packet;
		}
		public GeneralDataPacket (Enums.DataAction action)
		{
			Id = 0;
			Data1 = 0;
			Data2 = 0;
			Timestamp = (uint)Environment.TickCount;
			Action = action;
			Data3 = 0;
			Data4 = 0;
			Data5 = 0;
			Data6 = 0;
			Direction = 0;
		}

		public static GeneralDataPacket Create(uint id, Enums.DataAction action, uint data1, uint data2, uint data3, ushort data4, ushort data5, uint data6, byte data7)
		{
			return new GeneralDataPacket
			{
				Id = id,
				Data1 = data1,
				Data2 = data2,
				Timestamp = (uint)Environment.TickCount,//SystemTime.Now,
				Action = action,
				Data3 = data3,
				Data4Low = data4,
				Data4High = data5,
				Data5 = data6,
				Data6 = data7
			};
		}

		public static GeneralDataPacket Create(uint id, Enums.DataAction action, uint data1, uint data2, uint data3, uint data4, uint data5, byte data6)
		{
			return new GeneralDataPacket
			{
				Id = id,
				Data1 = data1,
				Data2 = data2,
				Timestamp =  (uint)Environment.TickCount, //SystemTime.Now,
				Action = action,
				Data3 = data3,
				Data4 = data4,
				Data5 = data5,
				Data6 = data6
			};
		}

		public static implicit operator GeneralDataPacket(DataPacket Packet)
		{
			byte* ptr = Packet.dataPointer;
			
			var packet = new GeneralDataPacket();
			packet.Id = *((uint*) (ptr + 4));
			packet.Data1 = *((uint*) (ptr + 8));
			packet.Data2 = *((uint*) (ptr + 12));
			packet.Timestamp = *((SystemTime*) (ptr + 16));
			packet.Action = *((Enums.DataAction*)(ptr + 20));
			packet.Direction = *((ushort*)(ptr + 22));
			packet.Data3 = *((uint*) (ptr + 24));
			packet.Data4 = *((uint*) (ptr + 28));
			packet.Data5 = *((uint*) (ptr + 32));
			packet.Data6 = *(ptr + 36);
			return packet;
		}

		public static implicit operator DataPacket(GeneralDataPacket packet)
		{
			var buffer = new byte[37];
			fixed (byte* ptr = buffer)
			{
				*((ushort*)(ptr + 0)) = (ushort)buffer.Length;
				*((ushort*)(ptr + 2)) = (ushort)10010;
				
				*((uint*) (ptr + 4)) = packet.Id;
				*((uint*) (ptr + 8)) = packet.Data1;
				*((uint*) (ptr + 12)) = packet.Data2;
				*((uint*)(ptr + 16)) = packet.Timestamp;
				*((Enums.DataAction*) (ptr + 20)) = packet.Action;
				*((ushort*)(ptr + 22)) = packet.Direction;
				*((uint*)(ptr + 24)) = packet.Data3;
				*((uint*) (ptr + 28)) = packet.Data4;
				*((uint*) (ptr + 32)) = packet.Data5;
				*(ptr + 36) = packet.Data6;
			}
			return new DataPacket(buffer);
		}
		#endregion
		
		/// <summary>
		/// Handling the GeneralDataPacket.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="packet">The packet.</param>
		public static void Handle(Entities.GameClient client, DataPacket packet)
		{
			GeneralDataPacket general = packet;
			switch (general.Action)
			{
					#region EnterMap : 74
				case Enums.DataAction.EnterMap:
					GeneralData.EnterMap.Handle(client, general);
					break;
					#endregion
					#region GetItemSet : 75
				case Enums.DataAction.GetItemSet:
					GeneralData.GetItemSet.Handle(client, general);
					break;
					#endregion
					#region GetSurroundings : 114
				case Enums.DataAction.GetSurroundings:
					GeneralData.GetSurroundings.Handle(client, general);
					break;
					#endregion
					#region Jump : 137
				case Enums.DataAction.Jump:
					GeneralData.Jump.Handle(client, packet);
					break;
					#endregion
					#region GetAssociation : 76
				case Enums.DataAction.GetAssociation:
					GeneralData.GetAssociation.Handle(client, general);
					break;
					#endregion
					#region GetSynAttr : 97
				case Enums.DataAction.GetSynAttr:
					GeneralData.GetSynAttr.Handle(client, general);
					break;
					#endregion
					#region GetWeaponSkillSet : 77
				case Enums.DataAction.GetWeaponSkillSet:
					GeneralData.GetWeaponSkillSet.Handle(client, general);
					break;
					#endregion
					#region GetMagicSet : 78
				case Enums.DataAction.GetMagicSet:
					GeneralData.GetMagicSet.Handle(client, general);
					break;
					#endregion
					#region SetPkMode : 96
				case Enums.DataAction.SetPkMode:
					GeneralData.SetPkMode.Handle(client, general);
					break;
					#endregion
					#region ChangeDirection : 79
				case Enums.DataAction.ChangeDirection:
					GeneralData.ChangeDirection.Handle(client, general);
					break;
					#endregion
					#region ChangeAction : 81
				case Enums.DataAction.ChangeAction:
					GeneralData.ChangeAction.Handle(client, general);
					break;
					#endregion
					#region QueryPlayer : 102
				case Enums.DataAction.QueryPlayer:
					GeneralData.QueryPlayer.Handle(client, general);
					break;
					#endregion
					#region QueryStatInfo : 408
				case Enums.DataAction.QueryStatInfo:
					GeneralData.QueryStatInfo.Handle(client, general);
					break;
					#endregion
					#region Revive : 94
				case Enums.DataAction.Revive:
					GeneralData.Revive.Handle(client, general);
					break;
					#endregion
					#region  QueryEquipment
				case Enums.DataAction.QueryEquipment:
				case Enums.DataAction.QueryFriendEquip:
					GeneralData.ViewEquipment.Handle(client, general);
					break;
					#endregion
					#region  QueryTeamMember
				case Enums.DataAction.QueryTeamMember:
					GeneralData.QueryTeamMember.Handle(client, general);
					break;
					#endregion
					#region  ChangeMap
				case Enums.DataAction.ChangeMap:
					GeneralData.ChangeMap.Handle(client, general);
					break;
					#endregion
					#region  CreateBooth
				case Enums.DataAction.CreateBooth:
					GeneralData.CreateBooth.Handle(client, general);
					break;
					#endregion
					#region  EndFly
				case Enums.DataAction.EndFly:
					GeneralData.EndFly.Handle(client, general);
					break;
					#endregion
					
					#region default
				default:
					Console.WriteLine("Unhandled GeneralData Type: {0} User: {1}", general.Action, client.Name);
					break;
					#endregion
			}
		}
	}
}
