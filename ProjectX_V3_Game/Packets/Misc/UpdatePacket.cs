//Project by BaussHacker aka. L33TS

// Temp structure, ripped from albetros
// Will structure this proper later, so please bare over with it :)

using System;
using System.Collections.Generic;
using ProjectX_V3_Lib.Network;

namespace ProjectX_V3_Game.Packets
{
	public struct UpdateData
	{
		public const int SIZE = 20;

		public Enums.UpdateType Type;
		public ulong Data1;
		public ulong Data2;
		public uint Data1Low
		{
			get { return (uint)Data1; }
			set { Data1 = ((ulong)Data1High << 32) | value; }
		}

		public uint Data1High
		{
			get { return (uint)(Data1 >> 32); }
			set { Data1 = ((ulong)value << 32) | Data1Low; }
		}

		public uint Data2Low
		{
			get { return (uint)Data2; }
			set { Data2 = ((ulong)Data2High << 32) | value; }
		}

		public uint Data2High
		{
			get { return (uint)(Data2 >> 32); }
			set { Data2 = ((ulong)value << 32) | Data2Low; }
		}

		public long SignedData1
		{
			get { return (long)Data1; }
			set { Data1 = (ulong)value; }
		}

		public long SignedData2
		{
			get { return (long)Data2; }
			set { Data2 = (ulong)value; }
		}
	}

	public unsafe struct UpdatePacket
	{
		public uint UserId;
		public uint AttributeCount;
		private List<UpdateData> _values;
		
		public static UpdatePacket Create(uint userId)
		{
			var packet = new UpdatePacket();
			packet.UserId = userId;
			packet.AttributeCount = 0;
			packet._values = new List<UpdateData>();
			return packet;
		}

		public static UpdatePacket Create(uint userId, Enums.UpdateType type, long data)
		{
			var packet = new UpdatePacket();
			if (!(type == Enums.UpdateType.None || type >= Enums.UpdateType.HP))
				throw new ArgumentException("type must be a valid UpdateType!", "type");

			packet.UserId = userId;
			packet.AttributeCount = 0;

			packet._values = new List<UpdateData>();
			if (type >= Enums.UpdateType.HP)
			{
				packet._values.Add(new UpdateData { Type = type, SignedData1 = data });
				packet.AttributeCount++;
			}

			return packet;
		}

		public static UpdatePacket Create(uint userId, Enums.UpdateType type, ulong data)
		{
			var packet = new UpdatePacket();
			if (!(type == Enums.UpdateType.None || type >= Enums.UpdateType.HP))
				throw new ArgumentException("type must be a valid UpdateType!", "type");

			packet.UserId = userId;
			packet.AttributeCount = 0;

			packet._values = new List<UpdateData>();
			if (type >= Enums.UpdateType.HP)
			{
				packet._values.Add(new UpdateData { Type = type, Data1 = data });
				packet.AttributeCount++;
			}

			return packet;
		}

		public UpdateData GetUpdate(int index)
		{
			return _values[index];
		}

		public bool AddUpdate(Enums.UpdateType type, ulong data1)
		{
			return AddUpdate(type, data1, 0);
		}

		public bool AddUpdate(Enums.UpdateType type, ulong data1, ulong data2)
		{
			if (!(type >= Enums.UpdateType.HP)) return false;

			_values.Add(new UpdateData { Type = type, Data1 = data1, Data2 = data2 });
			AttributeCount++;

			return true;
		}

		public bool AddUpdate(Enums.UpdateType type, long data1)
		{
			return AddUpdate(type, data1, 0);
		}

		public bool AddUpdate(Enums.UpdateType type, long data1, long data2)
		{
			if (!(type >= Enums.UpdateType.HP)) return false;

			_values.Add(new UpdateData { Type = type, SignedData1 = data1, SignedData2 = data2});
			AttributeCount++;

			return true;
		}

		public static implicit operator UpdatePacket(DataPacket dataPacket)
		{
			byte* ptr = dataPacket.dataPointer;
			
			var packet = new UpdatePacket();
			packet.UserId = *((uint*) (ptr + 4));
			packet.AttributeCount = *((uint*) (ptr + 8));
			packet._values = new List<UpdateData>((int) packet.AttributeCount);
			for (byte i = 0; i < packet.AttributeCount; i++)
			{
				var type = *((Enums.UpdateType*) (ptr + 12 + i * UpdateData.SIZE));
				var data1 = *((ulong*) (ptr + 16 + i * UpdateData.SIZE));
				var data2 = *((ulong*) (ptr + 24 + i * UpdateData.SIZE));
				packet._values.Add(new UpdateData { Type = type, Data1 = data1, Data2 = data2 });
			}
			return packet;
		}

		public static implicit operator DataPacket(UpdatePacket packet)
		{
			var buffer = new byte[12 + packet.AttributeCount * UpdateData.SIZE];
			fixed (byte* ptr = buffer)
			{
				*((ushort*)(ptr + 0)) = (ushort)buffer.Length;
				*((ushort*)(ptr + 2)) = 10017;
				*((uint*) (ptr + 4)) = packet.UserId;
				*((uint*) (ptr + 8)) = packet.AttributeCount;
				for (byte i = 0; i < packet.AttributeCount; i++)
				{
					var data = packet._values[i];
					*((Enums.UpdateType*) (ptr + 12 + i * UpdateData.SIZE)) = data.Type;
					*((ulong*) (ptr + 16 + i * UpdateData.SIZE)) = data.Data1;
					*((ulong*) (ptr + 24 + i * UpdateData.SIZE)) = data.Data2;
				}
			}
			return new DataPacket(buffer);
		}
		
		public static void SetUpdate(Entities.GameClient client, Enums.UpdateType type, ulong data, Enums.SynchroType synchro, ulong data2 = 0)
		{
			var packet = UpdatePacket.Create(client.EntityUID);
			if (!packet.AddUpdate(type, data))
				return;
			if (synchro != Enums.SynchroType.False)
			{
				switch (synchro)
				{
					case Enums.SynchroType.True:
						{
							client.Send(packet);
							break;
						}
					case Enums.SynchroType.Broadcast:
						{
							client.SendToScreen(packet, true);
							break;
						}
				}
			}
		}
	}
}
