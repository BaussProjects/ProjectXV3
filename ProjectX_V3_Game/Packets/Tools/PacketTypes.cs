//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Packets
{
	/// <summary>
	/// Packet type constants.
	/// </summary>
	public class PacketType
	{
		public const ushort
			AuthMessagePacket = 1052,
			MessagePacket = 1004,
			CharacterCreationPacket = 1001,
			CharacterInfoPacket = 1006,
			GeneralDataPacket = 10010,
			MapInfoPacket = 1110,
			ItemPacket = 1009,
			SpawnPacket = 10014,
			MovementPacket = 10005,
			SpawnNPCPacket = 2030,
			NPCRequestPacket = 2031,
			NPCResponsePacket = 2032,
			ItemInfoPacket = 1008,
			GroundItemPacket = 1101,
			TradePacket = 1056,
			InteractionPacket = 1022,
			SendProfPacket = 1025,
			SendSpellPacket = 1103,
			StringPacket = 1015,
			UseSpellPacket = 1105,
			CharacterStatsPacket = 1040,
			DateTimeVigorPacket = 1033,
			ViewItemPacket = 1108,
			GuildPacket = 1107,
			GuildAttributePacket = 1106,
			GuildMemberListPacket = 2102,
			GuildDonationPacket = 1058,
			NobilityPacket = 2064,
			BroadcastPacket = 2050,
			SubClassPacket = 2320,
			TeamActionPacket = 1023,
			TeamMemberPacket = 1026,
			GemSocketingPacket = 1027,
			WarehousePacket = 1102,
			CompositionPacket = 2036,
			ArenaActionPacket = 2205,
			ArenaPlayersPacket = 2208,
			ArenaBattleInfoPacket = 2206,
			ArenaStatisticPacket = 2209,
			ArenaMatchPacket = 2210,
			ArenaWatchPacket = 2211,
			WeatherPacket = 1016,
			SobSpawnPacket = 1109;
	}
}
