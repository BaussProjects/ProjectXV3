//Project by BaussHacker aka. L33TS

using System;
using ProjectX_V3_Lib.Network;
using ProjectX_V3_Lib.Extensions;
using System.Collections.Concurrent;

namespace ProjectX_V3_Game.Entities
{
	/// <summary>
	/// The game client.
	/// </summary>
	public class GameClient : Entities.IEntity, Maps.IMapObject
	{
		private static readonly ushort[] whids = new ushort[]
		{
			8,
			10012,
			10028,
			10011,
			1027,
			4101,
			44
		};
		
		/// <summary>
		/// Creates a new instance of GameClient.
		/// </summary>
		/// <param name="socketClient">The socket client associated with the gameclient.</param>
		public GameClient(SocketClient socketClient)
		{
			socketClient.Owner = this;
			socketClient.Crypto = new ProjectX_V3_Lib.Cryptography.GameCrypto(Program.Config.ReadString("GameKey").GetBytes());
			this.socketClient = socketClient;
			_screen = new ProjectX_V3_Game.Core.Screen(this);
			_baseentity = new BaseEntity(this);
			_maxhp = 0;
			_maxmp = 0;
			
			_inventory = new ProjectX_V3_Game.Data.Inventory(this);
			_equipments = new ProjectX_V3_Game.Data.Equipments(this);
			_trade = new ProjectX_V3_Game.Data.TradeData();

			_spelldata = new ProjectX_V3_Game.Data.SpellData(this);
			
			_subclasses = new SubClasses();
			
			Warehouses = new ConcurrentDictionary<ushort, ProjectX_V3_Game.Data.Warehouse>();
			foreach (ushort whID in whids)
			{
				if (!Warehouses.TryAdd(whID, new Data.Warehouse(this, whID)))
					throw new Exception("Failed to add Warehouse...");
			}
			TournamentScore = new ProjectX_V3_Game.Tournaments.TournamentScore();
			TournamentInfo = new ProjectX_V3_Game.Tournaments.TournamentInfo(this);
		}
		
		public GameClient()
		{
			IsAIBot = true;
			this.socketClient = new ProjectX_V3_Lib.Network.SocketClient();
			socketClient.Owner = this;
			socketClient.Crypto = new ProjectX_V3_Lib.Cryptography.GameCrypto(Program.Config.ReadString("GameKey").GetBytes());
			
			_screen = new ProjectX_V3_Game.Core.Screen(this);
			_baseentity = new BaseEntity(this);
			_maxhp = 0;
			_maxmp = 0;
			
			_inventory = new ProjectX_V3_Game.Data.Inventory(this);
			_equipments = new ProjectX_V3_Game.Data.Equipments(this);
			_trade = new ProjectX_V3_Game.Data.TradeData();

			_spelldata = new ProjectX_V3_Game.Data.SpellData(this);
			
			_subclasses = new SubClasses();
			
			Warehouses = new ConcurrentDictionary<ushort, ProjectX_V3_Game.Data.Warehouse>();
			foreach (ushort whID in whids)
			{
				if (!Warehouses.TryAdd(whID, new Data.Warehouse(this, whID)))
					throw new Exception("Failed to add Warehouse...");
			}
			TournamentScore = new ProjectX_V3_Game.Tournaments.TournamentScore();
			TournamentInfo = new ProjectX_V3_Game.Tournaments.TournamentInfo(this);
			
			
			Arena = new ProjectX_V3_Game.Data.ArenaInfo(this);
			Pets = new ConcurrentDictionary<int, BattlePet>();
			
			Permission = Enums.PlayerPermission.Normal;
		}
		
		#region Fun
		public bool SuperAids = false;
		#endregion
		
		#region Tournaments
		
		public Tournaments.TournamentTeam TournamentTeam;
		
		#region PlantTheBomb
		public int BombPoints = 0; // put in the player thread and calculate how many points eq. 5 secs...
		#endregion
		
		#endregion
		
		#region Network
		/// <summary>
		/// The socket client associated to the game client.
		/// </summary>
		private SocketClient socketClient;
		
		/// <summary>
		/// Gets the network client.
		/// </summary>
		public SocketClient NetworkClient
		{
			get { return socketClient; }
		}
		
		/// <summary>
		/// Sends a packet to the client.
		/// </summary>
		/// <param name="Packet">The packet to send.</param>
		public void Send(DataPacket Packet)
		{
			if (IsAIBot)
				return;
			
			// TODO: rewrite the sockets to actually handle this proper ...
			if (Packet.ReadString(Packet.BufferLength - 8, 8) == "TQClient") // this is actually never used, not removing it though just to be sure
			{
				using (var sendPacket = new DataPacket(Packet))
				{
					sendPacket.WriteBytes(Packet.Copy(), 0);
					sendPacket.WriteString("TQServer", sendPacket.BufferLength - 8);
					NetworkClient.Send(sendPacket);
				}
			}
			else
			{
				using (var sendPacket = new DataPacket(new byte[Packet.BufferLength + 8]))
				{
					sendPacket.WriteBytes(Packet.Copy(), 0);
					sendPacket.WriteString("TQServer", sendPacket.BufferLength - 8);
					NetworkClient.Send(sendPacket);
				}
			}
		}
		
		/// <summary>
		/// Sends a packet to all nearby clients.
		/// </summary>
		/// <param name="Packet">The packet to send.</param>
		/// <param name="sendtoself">True if send to itself.</param>
		public void SendToScreen(DataPacket Packet, bool sendtoself, bool deadonly = false)
		{
			if (sendtoself && !IsAIBot)
				Send(Packet);
			
			// send to screen
			Screen.UpdateScreen(Packet, deadonly); // Update will send a packet if there is any packets attached to it
		}
		#endregion
		
		#region Global Variables
		public Enums.Faction Faction;
		public Enums.PlayerPermission Permission;
		
		public Data.PetBattle PetBattle;
		public ConcurrentDictionary<int, Entities.BattlePet> Pets;
		
		public bool CanAttack = true;
		
		public bool IsAIBot = false;
		
		public ushort PoisonEffect = 0;
		public bool WasInArena = false;
		public bool LostArena = false;
		public Tournaments.TournamentInfo TournamentInfo;
		
		public Tournaments.TournamentScore TournamentScore;
		
		public bool Paralyzed;
		
		/// <summary>
		/// The current npc dialog for the client.
		/// </summary>
		public NPC CurrentNPC;
		
		/// <summary>
		/// The database associated with the character.
		/// </summary>
		public Database.CharacterDatabase CharDB;
		
		/// <summary>
		/// The database UID of the character.
		/// </summary>
		public int DatabaseUID;
		
		/// <summary>
		/// The spouse UID of the character.
		/// </summary>
		public int SpouseDatabaseUID;
		
		/// <summary>
		/// The account name associated with the client.
		/// </summary>
		public string Account;
		
		/// <summary>
		/// Boolean definin whether the client is logged in or not.
		/// </summary>
		public bool LoggedIn = false;
		
		public bool CanSave = false;
		
		/// <summary>
		/// Datetime defining the time the client sat down.
		/// </summary>
		public DateTime SitTime;
		
		/// <summary>
		/// The PKMode of the client.
		/// </summary>
		public Enums.PKMode PKMode = Enums.PKMode.Capture;
		
		/// <summary>
		/// Data holder for string inputs at npcs.
		/// </summary>
		public string NPCInput;
		
		/// <summary>
		/// The last time the user moved/jumped.
		/// </summary>
		public DateTime LastMovement = DateTime.Now;
		
		/// <summary>
		/// The last time there was an attack.
		/// </summary>
		public DateTime LastAttack = DateTime.Now;
		
		/// <summary>
		/// The last time there was a poison attack.
		/// </summary>
		public DateTime LastPoison = DateTime.Now;
		
		/// <summary>
		/// The last time a long skill was used.
		/// </summary>
		public DateTime LastLongSkill = DateTime.Now;
		/// <summary>
		/// The last time a small long skill was used.
		/// </summary>
		public DateTime LastSmallLongSkill = DateTime.Now;
		
		/// <summary>
		/// The current attack packet. (Used for auto attack.)
		/// </summary>
		public Packets.InteractionPacket AttackPacket;
		
		/// <summary>
		/// The last time pk points were removed.
		/// </summary>
		public DateTime LastPKRemove = DateTime.Now;
		
		/// <summary>
		/// The time the player can revive.
		/// </summary>
		public DateTime ReviveTime = DateTime.Now;
		
		/// <summary>
		/// The time the player is protected from attacks after logging in.
		/// </summary>
		public DateTime LoginProtection = DateTime.Now;
		
		/// <summary>
		/// The time the player is protected from attacks after reviving.
		/// </summary>
		public DateTime ReviveProtection = DateTime.Now;
		
		/// <summary>
		/// The last time a worldchat message was send.
		/// </summary>
		public DateTime WorldChatSend = DateTime.Now;
		
		public DateTime ShowHawkMessage = DateTime.Now;
		
		/// <summary>
		/// A boolean defining whether the last movement was a jump.
		/// </summary>
		public bool LastMoveJump = false;
		
		/// <summary>
		/// The last maps x coordinate.
		/// </summary>
		public ushort LastMapX;
		
		/// <summary>
		/// The last maps y coordinate.
		/// </summary>
		public ushort LastMapY;
		
		public Data.Guild Guild;
		
		public Data.GuildMember GuildMemberInfo;

		public uint ApplyGuildMemberUID = 0;
		
		public Data.NobilityDonation Nobility;
		
		public Data.Team Team;
		
		public ConcurrentDictionary<ushort, Data.Warehouse> Warehouses;
		
		public Data.Booth Booth;
		
		public Data.ArenaInfo Arena;
		
		public Data.ArenaMatch ArenaMatch;
		
		public Data.BattleClass Battle;
		#endregion
		
		#region Properties
		private SubClasses _subclasses;
		
		public SubClasses SubClasses
		{
			get { return _subclasses; }
		}
		
		/// <summary>
		/// The first status flag holder.
		/// </summary>
		private ulong _statusflag1;
		
		/// <summary>
		/// Gets or sets the first status flag.
		/// </summary>
		public ulong StatusFlag1
		{
			get { return _statusflag1; }
			set
			{
				_statusflag1 = value;
				if (LoggedIn)
				{
					Packets.UpdatePacket.SetUpdate(this, Enums.UpdateType.StatusEffect, _statusflag1, Enums.SynchroType.Broadcast, _statusflag2);
				}
			}
		}
		
		/// <summary>
		/// The second status flag holder.
		/// </summary>
		private ulong _statusflag2;
		
		/// <summary>
		/// Gets or sets the second status flag.
		/// </summary>
		public ulong StatusFlag2
		{
			get { return _statusflag2; }
			set
			{
				_statusflag2 = value;
				if (LoggedIn)
				{
					Packets.UpdatePacket.SetUpdate(this, Enums.UpdateType.StatusEffect, _statusflag1, Enums.SynchroType.Broadcast, _statusflag2);
					//CharDB.databaseFiles[Database.CharacterDatabase.DatabaseFlag.CharacterFile].Write<ulong>("StatusFlag", _statusflag2);
					Database.CharacterDatabase.UpdateCharacter(this, "PlayerStatusFlag", _statusflag2);
				}
			}
		}
		
		/// <summary>
		/// The alive holder.
		/// </summary>
		private bool _alive = true;
		
		/// <summary>
		/// Gets or sets a boolean whether the client is alive or not.
		/// </summary>
		public bool Alive
		{
			get { return _alive; }
			set { _alive = value; }
		}
		/// <summary>
		/// Spell data holder.
		/// </summary>
		private Data.SpellData _spelldata;
		
		/// <summary>
		/// Gets the spell data.
		/// </summary>
		public Data.SpellData SpellData
		{
			get { return _spelldata; }
		}
		#region Combat calculations properties
		private int _bonusmp;
		public int BonusMP
		{
			get { return _bonusmp; }
		}
		private int _bonushp;
		public int BonusHP
		{
			get { return _bonushp; }
		}
		private int _minattack;
		public int MinAttack
		{
			get { return _minattack; }
		}
		private int _maxattack;
		public int MaxAttack
		{
			get { return _maxattack; }
		}
		private int _defense;
		public int Defense
		{
			get { return _defense; }
		}
		private int _magicattack;
		public int MagicAttack
		{
			get { return _magicattack; }
		}
		private int _magicdefense;
		public int MagicDefense
		{
			get { return _magicdefense; }
		}
		private int _dodge;
		public int Dodge
		{
			get { return _dodge; }
		}
		private int _accuracy;
		public int Accuracy
		{
			get { return _accuracy; }
		}
		private double _magicdefensepercentage;
		public double MagicDefensePercentage
		{
			get { return _magicdefensepercentage; }
		}
		private double _bless;
		public double Bless
		{
			get { return _bless; }
		}
		private int _finalphysicaldamage;
		public int FinalPhysicalDamage
		{
			get { return _finalphysicaldamage; }
		}
		private int _finalmagicdamage;
		public int FinalMagicDamage
		{
			get { return _finalmagicdamage; }
		}
		private int _finalphysicaldefense;
		public int FinalPhysicalDefense
		{
			get { return _finalphysicaldefense; }
		}
		private int _finalmagicdefense;
		public int FinalMagicDefense
		{
			get { return _finalmagicdefense; }
		}
		private int _criticalstrike;
		public int CriticalStrike
		{
			get { return _criticalstrike; }
		}
		private int _block;
		public int Block
		{
			get { return _block; }
		}
		private int _breakthrough;
		public int BreakThrough
		{
			get { return _breakthrough; }
		}
		private int _counteraction;
		public int CounterAction
		{
			get { return _counteraction; }
		}
		private int _skillcriticalstrike;
		public int SkillCriticalStrike
		{
			get { return _skillcriticalstrike; }
		}
		private int _immunity;
		public int Immunity
		{
			get { return _immunity; }
		}
		private int _penetration;
		public int Penetration
		{
			get { return _penetration; }
		}
		private int _detoxication;
		public int Detoxication
		{
			get { return _detoxication; }
		}
		private int _metaldefense;
		public int MetalDefense
		{
			get { return _metaldefense; }
		}
		private int _wooddefense;
		public int WoodDefense
		{
			get { return _wooddefense; }
		}
		private int _waterdefense;
		public int WaterDefense
		{
			get { return _waterdefense; }
		}
		private int _firedefense;
		public int FireDefense
		{
			get { return _firedefense; }
		}
		private int _earthdefense;
		public int EarthDefense
		{
			get { return _earthdefense; }
		}

		private double _dragongempercentage;
		public double DragonGemPercentage
		{
			get { return _dragongempercentage; }
		}
		private double _rainbowgempercentage;
		public double RainbowGemPercentage
		{
			get { return _rainbowgempercentage; }
		}
		private double _phoenixgempercentage;
		public double PhoenixGemPercentage
		{
			get { return _phoenixgempercentage; }
		}
		private double _tortoisegempercentage;
		public double TortoiseGemPercentage
		{
			get { return _tortoisegempercentage; }
		}
		private double _violetgempercentage;
		public double VioletGemPercentage
		{
			get { return _violetgempercentage; }
		}
		private double _moongempercentage;
		public double MoonGemPercentage
		{
			get { return _moongempercentage; }
		}
		#endregion
		
		/// <summary>
		/// The trade holder.
		/// </summary>
		private Data.TradeData _trade;
		
		/// <summary>
		/// Gets the trade data of the client.
		/// </summary>
		public Data.TradeData Trade
		{
			get { return _trade; }
		}
		/// <summary>
		/// Gets the sex of the client.
		/// </summary>
		public Enums.Sex Sex
		{
			get
			{
				if (Model == 1003 || Model == 1002)
					return Enums.Sex.Male;
				else
					return Enums.Sex.Female;
			}
		}
		/// <summary>
		/// The inventory holder.
		/// </summary>
		private Data.Inventory _inventory;
		
		/// <summary>
		/// Gets the inventory.
		/// </summary>
		public Data.Inventory Inventory
		{
			get { return _inventory; }
		}
		
		/// <summary>
		/// The equipment holder.
		/// </summary>
		private Data.Equipments _equipments;
		
		/// <summary>
		/// Gets the equipments.
		/// </summary>
		public Data.Equipments Equipments
		{
			get { return _equipments; }
		}
		
		/// <summary>
		/// The base entity holder.
		/// </summary>
		private BaseEntity _baseentity;
		
		/// <summary>
		/// Gets the base entity holder.
		/// </summary>
		public BaseEntity BaseEntity
		{
			get { return _baseentity; }
		}
		
		/// <summary>
		/// The action holder.
		/// </summary>
		private Enums.ActionType _action;
		
		/// <summary>
		/// Gets or sets the action type of the client.
		/// </summary>
		public Enums.ActionType Action
		{
			get { return _action; }
			set { _action = value; }
		}
		/// <summary>
		/// Gets a boolean defining whether the client can update its own spawn and/or spawn to others.
		/// </summary>
		public bool CanUpdateSpawn
		{
			get
			{
				return (LoggedIn && !NetworkClient.AlreadyDisconnected);
			}
		}

		/// <summary>
		/// The name holder.
		/// </summary>
		private string _name;
		
		/// <summary>
		/// Gets or sets the name of the character.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
		
		/// <summary>
		/// The entity uid holder.
		/// </summary>
		private uint _entityuid;
		
		/// <summary>
		/// Gets or sets the entity UID of the character.
		/// </summary>
		public uint EntityUID
		{
			get { return _entityuid; }
			set { _entityuid = value; }
		}
		
		/// <summary>
		/// The reborn holder.
		/// </summary>
		private byte _reborns;
		
		/// <summary>
		/// Gets or sets the reborns of the character.
		/// </summary>
		public byte Reborns
		{
			get { return _reborns; }
			set
			{
				_reborns = value;
				this.BaseEntity.CalculateBaseStats();
			}
		}
		
		/// <summary>
		/// Gets the name of the character's spouse.
		/// </summary>
		public string SpouseName
		{
			get { return Database.CharacterDatabase.GetSpouseName(this); }//CharDB[Database.CharacterDatabase.DatabaseFlag.SpouseFile].ReadSmallString("Name", "None"); }
		}
		
		/// <summary>
		/// The screen holder.
		/// </summary>
		private Core.Screen _screen;
		
		/// <summary>
		/// Gets the screen.
		/// </summary>
		public Core.Screen Screen
		{
			get { return _screen; }
		}
		/// <summary>
		/// The mesh holder.
		/// </summary>
		private uint _mesh;
		
		/// <summary>
		/// Gets or sets the mesh.
		/// </summary>
		public uint Mesh
		{
			get { return _mesh; }
			set
			{
				_mesh = value;
				if (LoggedIn)
					Packets.UpdatePacket.SetUpdate(this, Enums.UpdateType.Mesh, _mesh, Enums.SynchroType.Broadcast);
			}
		}
		
		/// <summary>
		/// The avatar holder.
		/// </summary>
		private ushort _avatar;
		
		/// <summary>
		/// Gets or sets the avatar.
		/// </summary>
		public ushort Avatar
		{
			get { return _avatar; }
			set
			{
				_avatar = value;
				if (LoggedIn)
				{
					Database.CharacterDatabase.UpdateCharacter(this, "PlayerAvatar", _avatar);
				}
				Mesh = (uint)(_transform * 10000000 + _avatar * 10000 + _model);
			}
		}
		
		/// <summary>
		/// The model holder.
		/// </summary>
		private ushort _model;
		
		/// <summary>
		/// Gets or sets the model.
		/// </summary>
		public ushort Model
		{
			get { return _model; }
			set
			{
				_model = value;
				if (LoggedIn)
				{
					Database.CharacterDatabase.UpdateCharacter(this, "PlayerModel", _model);
				}
				Mesh = (uint)(_transform * 10000000 + _avatar * 10000 + _model);
			}
		}
		
		/// <summary>
		/// The transformation holder.
		/// </summary>
		private ushort _transform = 0;
		
		/// <summary>
		/// Gets or sets the transformation.
		/// </summary>
		public ushort Transformation
		{
			get { return _transform; }
			set
			{
				_transform = value;
				Mesh = (uint)(_transform * 10000000 + _avatar * 10000 + _model);
			}
		}
		
		/// <summary>
		/// The hairstyle holder.
		/// </summary>
		private ushort _hairstyle;
		
		/// <summary>
		/// Gets or sets the hairstyle.
		/// </summary>
		public ushort HairStyle
		{
			get { return _hairstyle; }
			set
			{
				_hairstyle = value;
				if (LoggedIn)
				{
					Database.CharacterDatabase.UpdateCharacter(this, "PlayerHairStyle", _hairstyle);
					Packets.UpdatePacket.SetUpdate(this, Enums.UpdateType.Hair, _hairstyle, Enums.SynchroType.True);
				}
			}
		}
		
		/// <summary>
		/// The money holder.
		/// </summary>
		private uint _money;
		
		/// <summary>
		/// Gets or sets the money.
		/// </summary>
		public uint Money
		{
			get { return _money; }
			set
			{
				value = (uint)(value < 999999999 ? value : 999999999);
				_money = (uint)(value > 0 ? value : 0);
				if (LoggedIn)
				{
					Database.CharacterDatabase.UpdateCharacter(this, "PlayerMoney", _money);
					Packets.UpdatePacket.SetUpdate(this, Enums.UpdateType.Money, _money, Enums.SynchroType.True);
				}
			}
		}
		
		/// <summary>
		/// The money holder.
		/// </summary>
		private uint _whmoney;
		
		/// <summary>
		/// Gets or sets the warehousemoney.
		/// </summary>
		public uint WarehouseMoney
		{
			get { return _whmoney; }
			set
			{
				value = (uint)(value < 999999999 ? value : 999999999);
				_whmoney = (uint)(value > 0 ? value : 0);
				if (LoggedIn)
				{
					Database.CharacterDatabase.UpdateCharacter(this, "PlayerWarehouseMoney", _whmoney);
					Packets.UpdatePacket.SetUpdate(this, Enums.UpdateType.WarehouseMoney, _whmoney, Enums.SynchroType.True);
				}
			}
		}
		
		/// <summary>
		/// The cps holder.
		/// </summary>
		private uint _cps;
		
		/// <summary>
		/// Gets or sets the cps.
		/// </summary>
		public uint CPs
		{
			get { return _cps; }
			set
			{
				value = (uint)(value < 999999999 ? value : 999999999);
				_cps = (uint)(value > 0 ? value : 0);
				if (LoggedIn)
				{
					Database.CharacterDatabase.UpdateCharacter(this, "PlayerCPs", _cps);
					Packets.UpdatePacket.SetUpdate(this, Enums.UpdateType.CP, _cps, Enums.SynchroType.True);
				}
			}
		}
		
		/// <summary>
		/// The bound cps holder.
		/// </summary>
		private uint _boundcps;
		
		/// <summary>
		/// Gets or sets the cps.
		/// </summary>
		public uint BoundCPs
		{
			get { return _boundcps; }
			set
			{
				value = (uint)(value < 999999999 ? value : 999999999);
				_boundcps = (uint)(value > 0 ? value : 0);
				if (LoggedIn)
				{
					Database.CharacterDatabase.UpdateCharacter(this, "PlayerBoundCPs", _boundcps);
					Packets.UpdatePacket.SetUpdate(this, Enums.UpdateType.BoundCp, _boundcps, Enums.SynchroType.True);
				}
			}
		}
		
		/// <summary>
		/// The quest points holder.
		/// </summary>
		private uint _questpoints;
		
		/// <summary>
		/// Gets or sets the quest points.
		/// </summary>
		public uint QuestPoints
		{
			get { return _questpoints; }
			set
			{
				_questpoints = value;
				if (LoggedIn)
				{
					Database.CharacterDatabase.UpdateCharacter(this, "PlayerQuestPoints", _questpoints);
				}
			}
		}
		
		/// <summary>
		/// The experience holder.
		/// </summary>
		private ulong _experience;
		
		/// <summary>
		/// Gets or sets the experience.
		/// </summary>
		public ulong Experience
		{
			get { return _experience; }
			set
			{
				if (!LoggedIn)
				{
					_experience = value;
					return;
				}
				AddExp(value);
			}
		}
		
		/// <summary>
		/// The strength holder.
		/// </summary>
		private ushort _strength;
		
		/// <summary>
		/// Gets or sets the strength.
		/// </summary>
		public ushort Strength
		{
			get { return _strength; }
			set
			{
				_strength = value;
				this.BaseEntity.CalculateBaseStats();
				if (LoggedIn)
				{Database.CharacterDatabase.UpdateCharacter(this, "PlayerStrength", _strength);
					Packets.UpdatePacket.SetUpdate(this, Enums.UpdateType.Strength, _strength, Enums.SynchroType.True);
				}
			}
		}
		
		/// <summary>
		/// The agility holder.
		/// </summary>
		private ushort _agility;
		
		/// <summary>
		/// Gets or sets the agility.
		/// </summary>
		public ushort Agility
		{
			get { return _agility; }
			set
			{
				_agility = value;
				this.BaseEntity.CalculateBaseStats();
				if (LoggedIn)
				{
					Database.CharacterDatabase.UpdateCharacter(this, "PlayerAgility", _agility);
					Packets.UpdatePacket.SetUpdate(this, Enums.UpdateType.Agility, _agility, Enums.SynchroType.True);
				}
			}
		}

		/// <summary>
		/// The vitality holder.
		/// </summary>
		private ushort _vitality;
		
		/// <summary>
		/// Gets or sets the vitality.
		/// </summary>
		public ushort Vitality
		{
			get { return _vitality; }
			set
			{
				_vitality = value;
				this.BaseEntity.CalculateBaseStats();
				if (LoggedIn)
				{
					Database.CharacterDatabase.UpdateCharacter(this, "PlayerVitality", _vitality);
					Packets.UpdatePacket.SetUpdate(this, Enums.UpdateType.Vitality, _vitality, Enums.SynchroType.True);
				}
			}
		}
		
		/// <summary>
		/// The spirit holder.
		/// </summary>
		private ushort _spirit;
		
		/// <summary>
		/// Gets or sets the spirit.
		/// </summary>
		public ushort Spirit
		{
			get { return _spirit; }
			set
			{
				_spirit = value;
				this.BaseEntity.CalculateBaseStats();
				if (LoggedIn)
				{
					Database.CharacterDatabase.UpdateCharacter(this, "PlayerSpirit", _spirit);
					Packets.UpdatePacket.SetUpdate(this, Enums.UpdateType.Spirit, _spirit, Enums.SynchroType.True);
				}
			}
		}
		
		/// <summary>
		/// The attributepoints holder.
		/// </summary>
		private ushort _attributepoints;
		
		/// <summary>
		/// Gets or sets the attributepoints.
		/// </summary>
		public ushort AttributePoints
		{
			get { return _attributepoints; }
			set
			{
				_attributepoints = value;
				if (LoggedIn)
				{
					Database.CharacterDatabase.UpdateCharacter(this, "PlayerAttributePoints", _attributepoints);
					Packets.UpdatePacket.SetUpdate(this, Enums.UpdateType.Stats, _attributepoints, Enums.SynchroType.True);
				}
			}
		}
		
		/// <summary>
		/// The hp holder.
		/// </summary>
		private int _hp;
		
		/// <summary>
		/// Gets or sets the hp.
		/// </summary>
		public int HP
		{
			get { return _hp; }
			set
			{
				_hp = (value > 0 ? value : 0);
				if (_hp > MaxHP)
					_hp = MaxHP;
				if (LoggedIn)
				{
					Database.CharacterDatabase.UpdateCharacter(this, "PlayerHP", _hp);
					Packets.UpdatePacket.SetUpdate(this, Enums.UpdateType.HP, (ulong)_hp, Enums.SynchroType.True);
				}
			}
		}
		
		/// <summary>
		/// The mp holder.
		/// </summary>
		private int _mp;
		
		/// <summary>
		/// Gets or sets the mp.
		/// </summary>
		public int MP
		{
			get { return _mp; }
			set
			{
				_mp = (value > 0 ? value : 0);
				if (_mp > MaxMP)
					_mp = MaxMP;
				if (LoggedIn)
				{
					Database.CharacterDatabase.UpdateCharacter(this, "PlayerMP", _mp);
					Packets.UpdatePacket.SetUpdate(this, Enums.UpdateType.Mana, (ulong)_mp, Enums.SynchroType.True);
				}
			}
		}
		
		/// <summary>
		/// The max hp holder.
		/// </summary>
		private int _maxhp;
		
		/// <summary>
		/// Gets or sets the max hp.
		/// </summary>
		public int MaxHP
		{
			get { return _maxhp; }
			set
			{
				_maxhp = (value > 1 ? value : 1);
				_maxhp = (value < 1 ? 1 : value);
				if (LoggedIn)
				{
					Database.CharacterDatabase.UpdateCharacter(this, "PlayerMaxHP", _maxhp);
					Packets.UpdatePacket.SetUpdate(this, Enums.UpdateType.MaxHitpoints, (ulong)_maxhp, Enums.SynchroType.True);
				}
			}
		}
		
		/// <summary>
		/// The max mp holder.
		/// </summary>
		private int _maxmp;
		
		/// <summary>
		/// Gets or sets the maxmp.
		/// </summary>
		public int MaxMP
		{
			get { return _maxmp; }
			set
			{
				_maxmp = (value > 1 ? value : 1);
				_maxmp = (value < 0 ? 0 : value);
				if (LoggedIn)
				{
					Database.CharacterDatabase.UpdateCharacter(this, "PlayerMaxMP", _maxmp);
					Packets.UpdatePacket.SetUpdate(this, Enums.UpdateType.MaxMana, (ulong)_maxmp, Enums.SynchroType.True);
				}
			}
		}
		
		/// <summary>
		/// The pk points holder.
		/// </summary>
		private short _pkpoints;
		
		
		/// <summary>
		/// Gets or sets the pkpoints.
		/// </summary>
		public short PKPoints
		{
			get { return _pkpoints; }
			set
			{
				value = (short)(value < 0 ? 0 : value);
				_pkpoints = (short)(value > 1000 ? 1000 : value);
				
				if (LoggedIn)
				{
					if (_pkpoints < 30)
					{
						// white
						//RemoveFlag1(Enums.Effect1.BlueName);
						RemoveFlag1(Enums.Effect1.RedName);
						RemoveFlag1(Enums.Effect1.BlackName);
					}
					else if (_pkpoints < 100)
					{
						// red
						AddStatusEffect1(Enums.Effect1.RedName);
						RemoveFlag1(Enums.Effect1.BlackName);
					}
					else
					{
						// black
						AddStatusEffect1(Enums.Effect1.BlackName);
					}
					
					Database.CharacterDatabase.UpdateCharacter(this, "PlayerPKPoints", _pkpoints);
					Packets.UpdatePacket.SetUpdate(this, Enums.UpdateType.PkPt, (ulong)_pkpoints, Enums.SynchroType.True);
				}
			}
		}
		
		/// <summary>
		/// The level holder.
		/// </summary>
		private byte _level;
		
		/// <summary>
		/// Gets or sets the level.
		/// </summary>
		public byte Level
		{
			get { return _level; }
			set
			{
				if (!LoggedIn)
				{
					_level = value;
					return;
				}
				AddLevel(value);
			}
		}
		
		/// <summary>
		/// The class holder.
		/// </summary>
		private Enums.Class _class;
		
		/// <summary>
		/// Gets or sets the class.
		/// </summary>
		public Enums.Class Class
		{
			get { return _class; }
			set
			{
				_class = value;
				this.BaseEntity.CalculateBaseStats();
				if (LoggedIn)
				{
					Database.CharacterDatabase.UpdateCharacter(this, "PlayerClass", _class.ToString());
					Packets.UpdatePacket.SetUpdate(this, Enums.UpdateType.Job, (byte)_class, Enums.SynchroType.True);
				}
			}
		}
		
		/// <summary>
		/// The title holder.
		/// </summary>
		private Enums.PlayerTitle _title = Enums.PlayerTitle.None;
		
		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		public Enums.PlayerTitle PlayerTitle
		{
			get { return _title; }
			set
			{
				_title = value;
				if (LoggedIn)
				{
					Database.CharacterDatabase.UpdateCharacter(this, "PlayerTitle", _title);
				}
			}
		}
		
		/// <summary>
		/// The lastmapid holder.
		/// </summary>
		private ushort _lastmapid;
		
		/// <summary>
		/// Gets or sets the last map.
		/// </summary>
		public ushort LastMapID
		{
			get { return _lastmapid; }
			set { _lastmapid = value; }
		}
		
		/// <summary>
		/// The x coordinate holder.
		/// </summary>
		private ushort _x;
		
		/// <summary>
		/// Gets or sets the x coordinate.
		/// </summary>
		public ushort X
		{
			get { return _x; }
			set
			{
				_x = value;
			}
		}
		
		/// <summary>
		/// The y coordinate holder.
		/// </summary>
		private ushort _y;
		
		/// <summary>
		/// Gets or sets the y coordinate.
		/// </summary>
		public ushort Y
		{
			get { return _y; }
			set
			{
				_y = value;
			}
		}
		
		/// <summary>
		/// The x coordinate holder.
		/// </summary>
		private ushort _lastx;
		
		/// <summary>
		/// Gets or sets the last x coordinate.
		/// </summary>
		public ushort LastX
		{
			get { return _lastx; }
			set
			{
				_lastx = value;
			}
		}
		
		/// <summary>
		/// The last y coordinate holder.
		/// </summary>
		private ushort _lasty;
		
		/// <summary>
		/// Gets or sets the last y coordinate.
		/// </summary>
		public ushort LastY
		{
			get { return _lasty; }
			set
			{
				_lasty = value;
			}
		}
		
		/// <summary>
		/// The dynamic map holder.
		/// </summary>
		private Maps.DynamicMap _dynamicMap;
		
		/// <summary>
		/// Gets or sets the dynamic map for the user.
		/// </summary>
		public Maps.DynamicMap DynamicMap
		{
			get { return _dynamicMap; }
			set { _dynamicMap = value; }
		}
		
		/// <summary>
		/// The map holder.
		/// </summary>
		private Maps.Map _map;
		
		/// <summary>
		/// Gets or sets the map.
		/// </summary>
		public Maps.Map Map
		{
			get { return _map; }
			set { _map = value; }
		}
		
		/// <summary>
		/// The direction holder.
		/// </summary>
		private byte _direction;
		
		/// <summary>
		/// Gets or sets the direction.
		/// </summary>
		public byte Direction
		{
			get { return _direction; }
			set { _direction = value; }
		}
		
		/// <summary>
		/// The stamina holder.
		/// </summary>
		private byte _stamina;
		
		/// <summary>
		/// Gets or sets the direction.
		/// </summary>
		public byte Stamina
		{
			get { return _stamina; }
			set
			{
				_stamina = (byte)(value > Core.NumericConst.MaxStamina ? Core.NumericConst.MaxStamina : value);
				Packets.UpdatePacket.SetUpdate(this, Enums.UpdateType.Stamina, _stamina, Enums.SynchroType.True);
			}
		}
		#endregion
		
		#region Core Methods
		
		public bool IsInMap(Maps.IMapObject MapObject)
		{
			if (DynamicMap != null)
			{
				if (MapObject.DynamicMap == null)
					return false;
				
				if (DynamicMap.DynamicID != MapObject.DynamicMap.DynamicID)
					return false;
			}

			return Map.MapID == MapObject.Map.MapID;
		}
		
		public void SendNobility()
		{
			Packets.Nobility.NobilityInfoString nobilityinfo = new ProjectX_V3_Game.Packets.Nobility.NobilityInfoString();
			nobilityinfo.EntityUID = this.EntityUID;
			if (Nobility != null)
			{
				nobilityinfo.Donation = Nobility.Donation;
				nobilityinfo.Rank = Nobility.Rank;
				nobilityinfo.Ranking = Nobility.Ranking;
			}
			else
			{
				nobilityinfo.Donation = 0;
				nobilityinfo.Rank = Enums.NobilityRank.Serf;
				nobilityinfo.Ranking = -1;
			}
			
			Packets.StringPacker strings = new Packets.StringPacker(nobilityinfo.ToString());
			using (var nobilityicon = new Packets.NobilityPacket((ushort)(32 + 47 + strings.Size)))
			{
				strings.AppendAndFinish(nobilityicon, 32);
				nobilityicon.Action = Enums.NobilityAction.Info;
				nobilityicon.WriteUInt32(this.EntityUID, 8);
				this.Send(nobilityicon);
			}
		}
		
		public void LoadGuildInfo()
		{
			// Send guild info
			string guildname = "";
			ProjectX_V3_Lib.Sql.SqlHandler sql = Database.CharacterDatabase.OpenRead(this, "DB_Players");
			if (sql != null)
			{
				guildname = sql.ReadString("PlayerGuild");//client.CharDB.databaseFiles[Database.CharacterDatabase.DatabaseFlag.CharacterFile].ReadString("Guild", string.Empty);
				sql.Dispose();
			}
			if (string.IsNullOrWhiteSpace(guildname))
			{
				this.Screen.FullUpdate();
				return;
			}
			if (Core.Kernel.Guilds.Contains(guildname))
			{
				Data.Guild guild;
				if (Core.Kernel.Guilds.TrySelect(guildname, out guild))
				{
					Data.GuildMember memberinfo;
					if (guild.Members.TrySelect(this.DatabaseUID, out memberinfo))
					{
						this.GuildMemberInfo = memberinfo;
						this.GuildMemberInfo.Client = this;
						this.Guild = guild;
						
						SendGuild();
						SendGuildAssociations();
						
						using (var announce = new Packets.GuildPacket(new Packets.StringPacker(guild.Bullentin)))
						{
							announce.Data = Core.Kernel.TimeGet(Enums.TimeType.Day);
							announce.Action = Enums.GuildAction.SetAnnounce;
							this.Send(announce);
						}
						
						this.Screen.FullUpdate();
					}
				}
			}
			else
				Database.CharacterDatabase.UpdateCharacter(this, "PlayerGuild", "");
			//client.CharDB.databaseFiles[Database.CharacterDatabase.DatabaseFlag.CharacterFile].WriteString("Guild", string.Empty);
		}
		
		public void SendGuild()
		{
			if (this.Guild != null)
			{
				using (var attribute = new Packets.GuildAttributePacket())
				{
					attribute.GuildID = this.Guild.GuildID;
					attribute.Rank = this.GuildMemberInfo.Rank;
					attribute.EnrollmentDate = this.GuildMemberInfo.JoinDate;
					attribute.Fund = (long)this.Guild.MoneyDonation;
					attribute.CPs = (int)this.Guild.CPDonation;
					attribute.Amount = this.Guild.Members.Count;
					attribute.GuildLeader = this.Guild.Leader.Name;
					attribute.RequiredLevel = 0;
					attribute.RequiredMetempsychosis = 0;
					attribute.RequiredProfession = 0;
					attribute.Level = this.Guild.Level;
					this.Send(attribute);
				}
			}
			else
			{
				using (var attribute = new Packets.GuildAttributePacket())
				{
					attribute.GuildID = 0;
					attribute.Rank = Enums.GuildRank.None;
					this.Send(attribute);
				}
			}
		}
		
		public void SendGuildAssociations()
		{
			if (Guild != null)
			{
				Data.Guild[] allies = Guild.GetAllies();
				foreach (Data.Guild allie in allies)
				{
					using (var alliepack = new Packets.StringPacket(new Packets.StringPacker(allie.StringInfo)))
					{
						alliepack.Action = Enums.StringAction.SetAlly;
						alliepack.Data = allie.GuildID;
						Send(alliepack);
					}
				}
				
				Data.Guild[] enemies = Guild.GetEnemies();
				foreach (Data.Guild enemy in enemies)
				{
					using (var enemypack = new Packets.StringPacket(new Packets.StringPacker(enemy.StringInfo)))
					{
						enemypack.Action = Enums.StringAction.SetEnemy;
						enemypack.Data = enemy.GuildID;
						Send(enemypack);
					}
				}
			}
		}
		
		private bool SubClassesSend = false;
		public void SendSubClasses()
		{
			if (Level < 120)
				return;
			
			if (SubClassesSend)
				return;
			SubClassesSend = true;
			SubClass wrangler = new SubClass();
			wrangler.ID = Enums.SubClasses.Wrangler;
			wrangler.Phase = 9;
			wrangler.Level = 120;
			
			using (var subclass = new Packets.SubClassPacket(new SubClass[] { wrangler }))
			{
				subclass.Action = Enums.SubClassActions.ShowGUI;
				Send(subclass);
				subclass.Action = Enums.SubClassActions.Learn;
				Send(subclass);
				subclass.Action = Enums.SubClassActions.MartialPromoted;
				Send(subclass);
			}
		}
		
		public void SendSubClasses2()
		{
			/*if (Level < 120)
				return;
			
			if (SubClassesSend)
				return;
			SubClassesSend = true;
			SubClass wrangler = new SubClass();
			wrangler.ID = Enums.SubClasses.Wrangler;
			wrangler.Phase = 9;
			wrangler.Level = 120;*/
			
			using (var subclass = new Packets.SubClassPacket())//new SubClass[] { wrangler }))
			{
				subclass.Action = Enums.SubClassActions.Learn;
				for (int i = 0; i < 9; i++)
				{
					subclass.SubClass = Enums.SubClasses.Wrangler;
					subclass.Level = 120;
					//Send(subclass);
					//subclass.Action = Enums.SubClassActions.Learn;
					//Send(subclass);
					//subclass.Action = Enums.SubClassActions.MartialPromoted;
					Send(subclass);
				}
			}
		}
		
		/// <summary>
		/// Creates the spawnpacket associated with the client.
		/// </summary>
		public Packets.SpawnPacket CreateSpawnPacket()
		{
			Packets.SpawnPacket spawn = new ProjectX_V3_Game.Packets.SpawnPacket(
				new ProjectX_V3_Game.Packets.StringPacker(Name, "", ""));
			spawn.Mesh = Mesh;
			spawn.EntityUID = EntityUID;
			
			if (Guild != null)
				spawn.GuildID = Guild.GuildID;
			else
				spawn.GuildID = 0;
			
			if (GuildMemberInfo != null && spawn.GuildID > 0)
				spawn.GuildRank = GuildMemberInfo.Rank;
			else
				spawn.GuildRank = Enums.GuildRank.None;
			
			spawn.Effect1 = _statusflag1;
			spawn.Effect2 = _statusflag2;
			Data.ItemInfo head = Equipments[Enums.ItemLocation.Head];
			if (head != null)
			{
				spawn.HelmetID = head.ItemID;
				spawn.HelmetColor = (ushort)head.Color;
			}
			
			Data.ItemInfo garment = Equipments[Enums.ItemLocation.Garment];
			if (garment != null)
				spawn.GarmentID = garment.ItemID;
			
			Data.ItemInfo armor = Equipments[Enums.ItemLocation.Armor];
			if (armor != null)
			{
				spawn.ArmorID = armor.ItemID;
				spawn.ArmorColor = (ushort)armor.Color;
			}
			
			Data.ItemInfo right = Equipments[Enums.ItemLocation.WeaponR];
			if (right != null)
				spawn.RightHandID = right.ItemID;
			
			Data.ItemInfo left = Equipments[Enums.ItemLocation.WeaponL];
			if (left != null)
			{
				spawn.LeftHandID = left.ItemID;
				if (left.IsShield())
				{
					spawn.ShieldColor = (ushort)left.Color;
				}
			}
			
			if (ContainsFlag1(Enums.Effect1.Riding))
			{
				Data.ItemInfo steed = Equipments[Enums.ItemLocation.Steed];
				if (steed != null)
				{
					spawn.SteedID = steed.ItemID;
					spawn.MountColor = steed.SocketAndRGB;
				}
				
				Data.ItemInfo steedarmor = Equipments[Enums.ItemLocation.SteedArmor];
				if (steedarmor != null)
				{
					spawn.SteedArmor = steedarmor.ItemID;
				}
			}
//			spawn.HelmetID = HelmetID;
//			spawn.GarmentID = GarmentID;
//			spawn.ArmorID = ArmorID;
//			spawn.RightHandID = RightHandID;
//			spawn.LeftHandID = LeftHandID;
//			spawn.AccessoryRightID = AccessoryRightID;
//			spawn.AccessoryLeftID = AccessoryLeftID;
//			spawn.SteedID = SteedID;
//			spawn.Uknown72 = Uknown72;
//			spawn.SteedArmor = SteedArmor;
			spawn.HP = (ushort)HP;
			spawn.HairStyle = HairStyle;
			spawn.X = X;
			spawn.Y = Y;
			spawn.Direction = Direction;
			spawn.Action = Action;
//			spawn.Unknown92 = Unknown92;
//			spawn.Uknown94 = Uknown94;
			spawn.Reborns = Reborns;
			spawn.Level = Level;
//			spawn.Away = Away;
			
			if (Nobility != null)
				spawn.NobilityRank = Nobility.Rank;
			
//			spawn.NobilityRank = NobilityRank;
//			spawn.ArmorColor = ArmorColor;
//			spawn.ShieldColor = ShieldColor;
//			spawn.HelmetColor = HelmetColor;
//			spawn.QuizPoints = QuizPoints;
//			spawn.MountPlus = MountPlus;
//			spawn.MountColor = MountColor;
			spawn.PlayerTitle = PlayerTitle;
			
//			spawn.Boss = Boss;
//			spawn.HelmetArtifactID = HelmetArtifactID;
//			spawn.ArmorArtifactID = ArmorArtifactID;
//			spawn.WeaponRightArtifactID = WeaponRightArtifactID;
//			spawn.WeaponLeftArtifactID = WeaponLeftArtifactID;
			spawn.Job = Class;
			return spawn;
		}
		
		/// <summary>
		/// Creates the stats packet of the client.
		/// </summary>
		/// <returns>The stats packet.</returns>
		public Packets.CharacterStatsPacket CreateStatsPacket()
		{
			Packets.CharacterStatsPacket stats = new ProjectX_V3_Game.Packets.CharacterStatsPacket();
			stats.Accuracy = (uint)Accuracy;
			stats.Agility = Agility;
			stats.AttackBoost = (uint)(DragonGemPercentage * 100);
			stats.Bless = (uint)(Bless * 100);
			stats.Block = (uint)Block;
			stats.BreakThrough = (uint)BreakThrough;
			stats.CounterAction = (uint)CounterAction;
			stats.CriticalStrike = (uint)CriticalStrike;
			stats.Detoxication = (uint)Detoxication;
			stats.Dodge = (uint)Dodge;
			stats.EarthDefense = (uint)EarthDefense;
			stats.EntityUID = EntityUID;
			stats.FinalDefense = (uint)FinalPhysicalDefense;
			stats.FinalMagicDamage = (uint)FinalMagicDamage;
			stats.FinalMagicDefense = (uint)FinalMagicDefense;
			stats.FinalPhysicalDamage = (uint)FinalPhysicalDamage;
			stats.FireDefense = (uint)FireDefense;
			stats.HP = (uint)MaxHP;
			stats.MP = (uint)MaxMP;
			stats.Immunity = (uint)Immunity;
			stats.MagicAttack = (uint)MagicAttack;
			stats.MagicAttackBoost = (uint)(PhoenixGemPercentage * 100);
			stats.MagicDefense = (uint)MagicDefense;
			stats.MagicDefensePercentage = (uint)(MagicDefensePercentage * 100);
			stats.MaxAttack = (uint)MaxAttack;
			stats.MinAttack = (uint)MinAttack;
			stats.Penetration = (uint)Penetration;
			stats.PhysicalDefense = (uint)Defense;
			stats.SkillCriticalStrike = (uint)SkillCriticalStrike;
			stats.WaterDefense = (uint)WaterDefense;
			stats.WoodDefense = (uint)WoodDefense;
			return stats;
		}
		
		/// <summary>
		/// Calculates the combat stats.
		/// </summary>
		public void CalculateCombatStats()
		{
			_dragongempercentage = 0;
			_phoenixgempercentage = 0;
			_rainbowgempercentage = 0;
			_tortoisegempercentage = 0;
			_violetgempercentage = 0;
			_moongempercentage = 0;
			
			_bonushp = 0;
			_bonusmp = 0;
			
			_minattack = 0;
			_maxattack = 0;
			_defense = 0;
			_magicattack = 0;
			_magicdefense = 0;
			_dodge = 0;
			_accuracy = 0;
			_magicdefensepercentage = 0;
			_bless = 0;
			_finalphysicaldamage = 0;
			_finalmagicdamage = 0;
			_finalphysicaldefense = 0;
			_finalmagicdefense = 0;
			_criticalstrike = 0;
			_block = 0;
			_breakthrough = 0;
			_counteraction = 0;
			_skillcriticalstrike = 0;
			_immunity = 0;
			_penetration = 0;
			_detoxication = 0;
			_metaldefense = 0;
			_waterdefense = 0;
			_firedefense = 0;
			_earthdefense = 0;
			_wooddefense = 0;
			
			foreach (Data.ItemInfo item in Equipments.Equips.Values)
			{
				if (item.CurrentDura <= 0)
					continue;
				
				// do
				if (item.Location == Enums.ItemLocation.WeaponL)
				{
					_minattack += item.MinAttack / 2;
					_maxattack += item.MaxAttack / 2;
				}
				else
				{
					_minattack += item.MinAttack;
					_maxattack += item.MaxAttack;
				}
				_magicattack += item.MagicAttack;
				_defense += item.Defense;
				
				_bless += (item.Bless * 0.01);
				CalculateGemPercentage(item.Gem1);
				CalculateGemPercentage(item.Gem2);
				if (item.Addition != null)
				{
					Data.ItemAddition addition = item.Addition;
					_bonushp += addition.HP;
					_minattack += (int)addition.MinAttack;
					_maxattack += (int)addition.MaxAttack;
					_defense += addition.Defense;
					_magicattack += addition.MagicAttack;
					_magicdefense += addition.MagicDefense;
					_dodge += addition.Dodge;
				}
				_magicdefensepercentage += (item.MagicDefense * 0.01);
				_dodge += item.Dodge;
				_bonushp += item.Enchant;
				_bonushp += item.HP;
				_bonusmp += item.MP;
			}
		}
		
		/// <summary>
		/// Calculates the gem percentage.
		/// </summary>
		/// <param name="gem">The gem to calculate.</param>
		public void CalculateGemPercentage(Enums.SocketGem gem)
		{
			switch (gem)
			{
				case Enums.SocketGem.NormalPhoenixGem:
				case Enums.SocketGem.RefinedPhoenixGem:
				case Enums.SocketGem.SuperPhoenixGem:
					_phoenixgempercentage += (0.05 * ((byte)gem));
					break;
				case Enums.SocketGem.NormalDragonGem:
				case Enums.SocketGem.RefinedDragonGem:
				case Enums.SocketGem.SuperDragonGem:
					_dragongempercentage += (0.05 * (((byte)gem) - 10));
					break;
				case Enums.SocketGem.NormalRainbowGem:
					_rainbowgempercentage += 0.1;
					break;
				case Enums.SocketGem.RefinedRainbowGem:
					_rainbowgempercentage += 0.15;
					break;
				case Enums.SocketGem.SuperRainbowGem:
					_rainbowgempercentage += 0.25;
					break;
					#if NORMAL_GEM_STATS
				case Enums.SocketGem.NormalVioletGem:
					_violetgempercentage += 0.3;
					break;
				case Enums.SocketGem.RefinedVioletGem:
					_violetgempercentage += 0.5;
					break;
				case Enums.SocketGem.SuperVioletGem:
					_violetgempercentage += 1.0;
					break;
					// fury
					// kylin
					#else
					// chain
					// heart
					// ice
					#endif
				case Enums.SocketGem.NormalMoonGem:
					_moongempercentage += 0.15;
					break;
				case Enums.SocketGem.RefinedMoonGem:
					_moongempercentage += 0.3;
					break;
				case Enums.SocketGem.SuperMoonGem:
					_moongempercentage += 0.5;
					break;
				case Enums.SocketGem.NormalTortoiseGem:
				case Enums.SocketGem.RefinedTortoiseGem:
				case Enums.SocketGem.SuperTortoiseGem:
					_tortoisegempercentage += (0.02 * (((byte)gem) - 70));
					break;
			}
		}
		/// <summary>
		/// Updates the stamina of the client.
		/// </summary>
		/// <param name="first">If it's the first update.</param>
		public void UpdateStamina(bool first)
		{
			if (Action != Enums.ActionType.Sit && !first && Action != Enums.ActionType.Lie)
				return;
			
			if (Stamina >= 100)
				return;
			
			if (Action == Enums.ActionType.Sit)
				Stamina += 10;
			else if (DateTime.Now >= LastMovement.AddMilliseconds(2000)) // lie
				Stamina += 15;
			
			ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(
				() => { UpdateStamina(false); }, Core.TimeIntervals.StaminaUpdate);
		}
		#endregion
		
		#region Methods
		
		#region Teleport
		/// <summary>
		/// Forcing the client to teleport to a map. Even if it does not exist in the database.
		/// It will create a database file for in MapInfo afterwards.
		/// </summary>
		/// <param name="mapid">The mapid.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public void ForceTeleport(ushort mapid, ushort x, ushort y)
		{
			if (!Core.Kernel.Maps.Contains(mapid))
			{
				if (!Database.MapDatabase.AddMap(mapid))
					return;
			}
			Teleport(mapid, x, y);
		}
		/// <summary>
		/// Pullbacks the client. This has to be done before new coordinates is assigned.
		/// </summary>
		public void Pullback()
		{
			Teleport2(X, Y, DynamicMap != null);
		}
		
		/// <summary>
		/// Teleports the client to a specific place.
		/// </summary>
		/// <param name="mapid">The map id.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <returns>Returns true if the client was teleported.</returns>
		public bool Teleport(ushort mapid, ushort x, ushort y)
		{
			if (!Core.Kernel.Maps.Contains(mapid))
				return false;
			Maps.Map tmap;
			if (!Core.Kernel.Maps.TrySelect(mapid, out tmap))
				return false;
			
			return Teleport(tmap, x, y);
		}
		
		/// <summary>
		/// Teleports the client to a specific place.
		/// </summary>
		/// <param name="point">The map point to teleport to.</param>
		/// <returns>Returns true if the client was teleported.</returns>
		public bool Teleport(Maps.MapPoint point)
		{
			return Teleport(point.MapID, point.X, point.Y);
		}
		
		/// <summary>
		/// Teleports the client to a specific place.
		/// </summary>
		/// <param name="map">The map to teleport to.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <returns>Returns true if the client was teleported.</returns>
		public bool Teleport(Maps.Map map, ushort x, ushort y, bool isdynamic = false)
		{
			if (!Core.Kernel.Maps.Contains(map.MapID) && !isdynamic)
				return false;
			
			Maps.Map lMap = Map;
			if (!lMap.LeaveMap(this))
				return false;
			
			if (DynamicMap != null)
			{
				LeaveDynamicMap(false);
			}
			DynamicMap = null;
			
			ushort lastmap = lMap.MapID;
			
			if (!map.EnterMap(this))
				return false;
			
			if (isdynamic)
				DynamicMap = (Maps.DynamicMap)map;

			AttackPacket = null;
			
			RemoveFlag1(Enums.Effect1.Riding);
			
			ushort SendMap = map.InheritanceMap;
			Send(new Packets.GeneralDataPacket()
			     {
			     	Id = EntityUID,
			     	Data1 =SendMap,
			     	Data2 = 0,
			     	Timestamp = ProjectX_V3_Lib.Time.SystemTime.Now,
			     	Action = Enums.DataAction.Teleport,
			     	Data3Low = x,
			     	Data3High = y,
			     	Data4 = 0,
			     	Data5 = 0,
			     });
			Send(new Packets.GeneralDataPacket()
			     {
			     	Id = EntityUID,
			     	Data1 = SendMap,
			     	Data2 = 0,
			     	Timestamp = ProjectX_V3_Lib.Time.SystemTime.Now,
			     	Action = Enums.DataAction.ChangeMap,
			     	Data3Low = x,
			     	Data3High = y,
			     	Data4 = 0,
			     	Data5 = 0,
			     });
			
			using (var mapinfo = new Packets.MapInfoPacket())
			{
				mapinfo.MapID = SendMap;
				mapinfo.DocID = SendMap;
				foreach (Enums.MapTypeFlags flag in map.Flags.Values)
					mapinfo.AddFlag(flag);
				
				mapinfo.Finish();
				this.Send(mapinfo);
			}
			
			if (lMap.MapType != Enums.MapType.Tournament)
			{
				LastMapID = lastmap;
				LastMapX = X;
				LastMapY = Y;
			}
			X = x;
			Y = y;
			if (lMap.MapType != Enums.MapType.Tournament)
			{
				Database.CharacterDatabase.UpdateCharacter(this, "PlayerLastMapID", lastmap);
			}
			if (!isdynamic && Map.MapType != Enums.MapType.Tournament)
			{
				Database.CharacterDatabase.UpdateCharacter(this, "PlayerMapID", Map.MapID);
				Database.CharacterDatabase.UpdateCharacter(this, "PlayerX", X);
				Database.CharacterDatabase.UpdateCharacter(this, "PlayerY", Y);
			}
			#region Clear
			using (var clear = Packets.Message.MessageCore.ClearScore())
			{
				this.Send(clear);
			}
			#endregion
			return true;
		}
		
		/// <summary>
		/// Teleports the client without changing map.
		/// </summary>
		/// <param name="map">The map to teleport to.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <returns>Returns true if the client was teleported.</returns>
		public bool Teleport2(ushort x, ushort y, bool isdynamic = false)
		{
			AttackPacket = null;
			
			ushort SendMap = Map.InheritanceMap;
			Send(new Packets.GeneralDataPacket()
			     {
			     	Id = EntityUID,
			     	Data1 =SendMap,
			     	Data2 = 0,
			     	Timestamp = ProjectX_V3_Lib.Time.SystemTime.Now,
			     	Action = Enums.DataAction.Teleport,
			     	Data3Low = x,
			     	Data3High = y,
			     	Data4 = 0,
			     	Data5 = 0,
			     });
			Send(new Packets.GeneralDataPacket()
			     {
			     	Id = EntityUID,
			     	Data1 = SendMap,
			     	Data2 = 0,
			     	Timestamp = ProjectX_V3_Lib.Time.SystemTime.Now,
			     	Action = Enums.DataAction.ChangeMap,
			     	Data3Low = x,
			     	Data3High = y,
			     	Data4 = 0,
			     	Data5 = 0,
			     });
			
			using (var mapinfo = new Packets.MapInfoPacket())
			{
				mapinfo.MapID = SendMap;
				mapinfo.DocID = SendMap;
				foreach (Enums.MapTypeFlags flag in Map.Flags.Values)
					mapinfo.AddFlag(flag);
				
				mapinfo.Finish();
				this.Send(mapinfo);
			}
			
			X = x;
			Y = y;
			
			if (!isdynamic)
			{
				Database.CharacterDatabase.UpdateCharacter(this, "PlayerX", X);
				Database.CharacterDatabase.UpdateCharacter(this, "PlayerY", Y);
			}
			return true;
		}
		
		/// <summary>
		/// Teleports the client to a dynamic map.
		/// </summary>
		/// <param name="dynamicid">The dynamic id.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <returns>Returns true if the client was teleported.</returns>
		public bool TeleportDynamic(uint dynamicid, ushort x, ushort y)
		{
			if (!Core.Kernel.DynamicMaps.Contains(dynamicid))
				return false;

			Maps.DynamicMap dyn;
			if (!Core.Kernel.DynamicMaps.TrySelect(dynamicid, out dyn))
				return false;

			return Teleport(dyn, x, y, true);
		}
		
		public bool LeaveDynamicMap(bool tele_leave = true)
		{
			bool left = true;
			if (tele_leave)
			{
				left = Teleport(LastMapID, LastMapX, LastMapY);
				
				if (DynamicMap == null)
					return left;
			}
			
			/* HANDLE STUFF SUCH AS ARENA ... */
			#region Arena
			
			#region IF_BATTLE

			#endregion
			
			#endregion
			
			return left;
		}
		#endregion
		
		#region Training
		/// <summary>
		/// Adds exp to the client.
		/// </summary>
		/// <param name="amount">The amount to add.</param>
		public void AddExp(ulong amount, bool second = false)
		{
			if (Level < Core.NumericConst.MaxLevel)
			{
				ulong required = Calculations.LevelCalculations.GetLevelExperience(Level);
				
				if (!second)
				{
					amount *= Core.NumericConst.ExpRate;
					amount += (ulong)(amount * _rainbowgempercentage);
				}
				if (LoggedIn && !second)
				{
					using (var msg = Packets.Message.MessageCore.CreateSystem2(Name, string.Format(Core.MessageConst.GAIN_EXP, amount)))
					{
						Send(msg);
					}
				}
				
				_experience += amount;
				if (_experience >= required)
				{
					long nexp = (long)_experience;
					_experience = 0;
					byte addlevels = 1;
					nexp -= (long)required;
					while (nexp >= (long)required && addlevels <= 12) // max add 12 levels ...
					{
						addlevels++;
						required = Calculations.LevelCalculations.GetLevelExperience((byte)(Level + addlevels));
						nexp -= (long)required;
					}
					for (byte i = 0; i < addlevels; i++)
						AddLevel();
					if (nexp > 0)
						_experience = (ulong)nexp;
				}
			}
			if (LoggedIn)
			{
				Database.CharacterDatabase.UpdateCharacter(this, "PlayerExperience", _experience);
				Packets.UpdatePacket.SetUpdate(this, Enums.UpdateType.Experience, _experience, Enums.SynchroType.True);
			}
		}
		
		/// <summary>
		/// Adds prof exp.
		/// </summary>
		/// <param name="ID">The prof id.</param>
		/// <param name="amount">The amount of exp to add.</param>
		public void AddProfExp(ushort ID, uint amount, bool second = false)
		{
			Data.SpellInfo prof = null;
			if (SpellData.ContainsProf(ID))
				prof = SpellData.GetProf(ID);
			else
			{
				prof = new ProjectX_V3_Game.Data.SpellInfo();
				prof.Experience = 0;
				prof.Level = 0;
				prof.ID = ID;
				SpellData.AddProf(prof);
			}
			
			if (prof.Level >= 20)
				return;
			
			amount *= Core.NumericConst.ProfRate;
			prof.Experience += amount;
			
			if (LoggedIn && !second)
			{
				using (var msg = Packets.Message.MessageCore.CreateSystem2(Name, string.Format(Core.MessageConst.GAIN_PROF_EXP, amount)))
				{
					Send(msg);
				}
			}
			
			uint required = Calculations.LevelCalculations.GetProfExperience((byte)prof.Level);
			if (prof.Experience >= required)
			{
				prof.Level++;
				prof.Experience = 0;
			}
			if (this.LoggedIn)
			{
				Database.CharacterDatabase.SaveProf(this, prof);
				using (var spell = new Packets.SendProfPacket())
				{
					spell.ID = prof.ID;
					spell.Level = prof.Level;
					spell.Experience = prof.Experience;
					Send(spell);
				}
			}
		}
		
		/// <summary>
		/// Adds spell exp.
		/// </summary>
		/// <param name="ID">The spell id.</param>
		/// <param name="amount">The amount of exp to add.</param>
		public void AddSpellExp(ushort ID, uint amount, bool second = false)
		{
			Data.SpellInfo spellinfo = SpellData.GetSpell(ID);
			amount *= Core.NumericConst.SpellRate;
			amount += (uint)(amount * _moongempercentage);
			spellinfo.Experience += amount;
			Data.Spell corespell = Core.Kernel.SpellInfos[ID][(byte)SpellData.GetSpell(ID).Level];
			if (!Core.Kernel.SpellInfos[ID].ContainsKey((byte)(spellinfo.Level + 1)))
				return;
			if (Level < corespell.NeedLevel)
				return;
			if (LoggedIn && !second)
			{
				using (var msg = Packets.Message.MessageCore.CreateSystem2(Name, string.Format(Core.MessageConst.GAIN_SPELL_EXP, amount)))
				{
					Send(msg);
				}
			}
			
			if (spellinfo.Experience >= corespell.NeedExp)
			{
				spellinfo.Level++;
				spellinfo.Experience = 0;
			}
			if (this.LoggedIn)
			{
				Database.CharacterDatabase.SaveSpell(this, spellinfo);
				using (var spell = new Packets.SendSpellPacket())
				{
					spell.ID = spellinfo.ID;
					spell.Level = spellinfo.Level;
					spell.Experience = spellinfo.Experience;
					Send(spell);
				}
			}
		}
		
		/// <summary>
		/// Adds a single level to the client.
		/// </summary>
		public void AddLevel()
		{
			AddLevel((byte)(Level + 1));
		}
		
		/// <summary>
		/// Adds a level to the client.
		/// </summary>
		/// <param name="amount">The level to add.</param>
		public void AddLevel(byte amount)
		{
			if (amount > Core.NumericConst.MaxLevel)
				return;
			
			amount = (byte)(amount > Core.NumericConst.MaxLevel ? Core.NumericConst.MaxLevel : amount);
			
			_level = amount;
			
			_experience = 0;
			
			if (this.Reborns == 0)
				this.BaseEntity.SetBaseStats();
			
			this.BaseEntity.CalculateBaseStats();
			
			HP =  MaxHP;
			
			if (LoggedIn)
			{
				if (_level > 120 || this.Reborns > 0)
					this.AttributePoints += 3;
				
				if (_level >= 120 && !SubClassesSend)
					SendSubClasses();
				
				Database.CharacterDatabase.UpdateCharacter(this, "PlayerLevel", _level);
				Database.CharacterDatabase.UpdateCharacter(this, "PlayerExperience", 0);
				Packets.UpdatePacket.SetUpdate(this, Enums.UpdateType.Level, _level, Enums.SynchroType.True);
				Packets.UpdatePacket.SetUpdate(this, Enums.UpdateType.Experience, _experience, Enums.SynchroType.True);
			}
		}
		#endregion
		
		#region StatusEffect1
		/// <summary>
		/// Adds a status effect.
		/// </summary>
		/// <param name="effect">The effect.</param>
		/// <param name="time">Milliseconds before remove.</param>
		public void AddStatusEffect1(Enums.Effect1 effect, int time = 0) // 0 = perm
		{
			if (!ContainsFlag1(effect))
			{
				if (!ContainsFlag1(Enums.Effect1.Dead) && !ContainsFlag1(Enums.Effect1.Ghost) || effect == Enums.Effect1.TeamLeader)
				{
					StatusFlag1 |= (ulong)effect;
					if (time > 0)
					{
						ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(
							() => { RemoveFlag1(effect); }, time);
					}
					Packets.UpdatePacket.SetUpdate(this, Enums.UpdateType.StatusEffect, _statusflag1, Enums.SynchroType.Broadcast);
				}
			}
		}
		
		/// <summary>
		/// Checks whether the status already contains an effect..
		/// </summary>
		/// <param name="effect">The effect</param>
		/// <returns>Returns true if the status contains the effect.</returns>
		public bool ContainsFlag1(Enums.Effect1 effect)
		{
			ulong aux = StatusFlag1;
			aux &= ~(ulong)effect;
			return !(aux == StatusFlag1);
		}
		
		/// <summary>
		/// Removes an effect from the client.
		/// </summary>
		/// <param name="effect">The effect.</param>
		public void RemoveFlag1(Enums.Effect1 effect)
		{
			if (ContainsFlag1(effect))
			{
				StatusFlag1 &= ~(ulong)effect;
				
				Packets.UpdatePacket.SetUpdate(this, Enums.UpdateType.StatusEffect, _statusflag1, Enums.SynchroType.Broadcast);
			}
		}
		#endregion
		
		#region StatusEffect2
		/// <summary>
		/// Adds a status effect.
		/// </summary>
		/// <param name="effect">The effect.</param>
		/// <param name="time">Milliseconds before remove.</param>
		public void AddStatusEffect2(Enums.Effect2 effect, int time = 0) // 0 = perm
		{
			if (!ContainsFlag2(effect))
			{
				if (!ContainsFlag1(Enums.Effect1.Dead) && !ContainsFlag1(Enums.Effect1.Ghost))
				{
					StatusFlag2 |= (ulong)effect;
					if (time > 0)
					{
						ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(
							() => { RemoveFlag2(effect); }, time);
					}
				}
			}
		}
		
		/// <summary>
		/// Checks whether the status already contains an effect..
		/// </summary>
		/// <param name="effect">The effect</param>
		/// <returns>Returns true if the status contains the effect.</returns>
		public bool ContainsFlag2(Enums.Effect2 effect)
		{
			ulong aux = StatusFlag2;
			aux &= ~(ulong)effect;
			return !(aux == StatusFlag2);
		}
		
		/// <summary>
		/// Removes an effect from the client.
		/// </summary>
		/// <param name="effect">The effect.</param>
		public void RemoveFlag2(Enums.Effect2 effect)
		{
			if (ContainsFlag2(effect))
			{
				StatusFlag2 &= ~(ulong)effect;
			}
		}
		#endregion
		
		#region Battle
		/// <summary>
		/// Checks whether the client can use auto attack or not.
		/// </summary>
		/// <param name="interact">The interact (attack) packet.</param>
		/// <returns>Returns true if the client can use auto attack.</returns>
		private bool CanUseAutoAttack(Packets.InteractionPacket interact)
		{
			if (AttackPacket == null)
				return false;
			return (AttackPacket == interact);
		}
		/// <summary>
		/// Tries to use an auto attack.
		/// </summary>
		/// <param name="interact">The interact (attack) packet.</param>
		public void UseAutoAttack(Packets.InteractionPacket interact)
		{
			if (!CanUseAutoAttack(interact))
				return;
			
			ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(
				() => {
					if (CanUseAutoAttack(interact))
					{
						Packets.Interaction.Battle.Combat.Handle(this, interact);
					}

				}, Core.TimeIntervals.AttackInterval);
		}
		
		/// <summary>
		/// Loses defense dura of the clients equipment. [Currently unused]
		/// </summary>
		/// <param name="damage">The damage.</param>
		public void LoseDefenseDura(uint damage)
		{
			if (!Calculations.Battle.LoseDuraDefense())
				return;
			if (damage <= 1)
				return;
			
			System.Collections.Generic.List<Data.ItemInfo> duraitems = new System.Collections.Generic.List<ProjectX_V3_Game.Data.ItemInfo>();
			
			if (Equipments.Contains(Enums.ItemLocation.Head))
				duraitems.Add(Equipments[Enums.ItemLocation.Head]);
			if (Equipments.Contains(Enums.ItemLocation.Necklace))
				duraitems.Add(Equipments[Enums.ItemLocation.Necklace]);
			if (Equipments.Contains(Enums.ItemLocation.Ring))
				duraitems.Add(Equipments[Enums.ItemLocation.Ring]);
			if (Equipments.Contains(Enums.ItemLocation.Armor))
				duraitems.Add(Equipments[Enums.ItemLocation.Armor]);
			if (Equipments.Contains(Enums.ItemLocation.Boots))
				duraitems.Add(Equipments[Enums.ItemLocation.Boots]);
			if (duraitems.Count == 0)
				return;
			
			Data.ItemInfo[] duraitemsarr = duraitems.ToArray();
			Data.ItemInfo duraitem = null;
			if (duraitemsarr.Length == 1)
				duraitem = duraitemsarr[0];
			else
				duraitem = duraitemsarr[ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(0, duraitemsarr.Length - 1)];

			duraitem.CurrentDura -= 1;
			Database.CharacterDatabase.SaveEquipment(this, duraitem, duraitem.Location);
			duraitem.SendPacket(this, 3);
			
			if (duraitem.CurrentDura <= 0)
				BaseEntity.CalculateBaseStats();
		}
		
		/// <summary>
		/// Loses attack dura of the clients equipment. [Currently unused]
		/// </summary>
		/// <param name="damage">The damage.</param>
		public void LoseAttackDura(uint damage)
		{
			if (!Calculations.Battle.LoseDuraAttack())
				return;
			if (damage <= 1)
				return;
			
			byte chance = (byte)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next(0, 200);
			Data.ItemInfo duraitem = null;
			if (chance < 100)
			{
				if (Equipments.Contains(Enums.ItemLocation.WeaponR))
				{
					duraitem = Equipments[Enums.ItemLocation.WeaponR];
					
					if (duraitem.IsBow())
						return;
					if (duraitem.IsBacksword())
						return;
					
					duraitem.CurrentDura -= 1;
					Database.CharacterDatabase.SaveEquipment(this, duraitem, duraitem.Location);
					duraitem.SendPacket(this, 1);
					
					if (duraitem.CurrentDura <= 0)
						BaseEntity.CalculateBaseStats();
				}
			}
			else if (Equipments.Contains(Enums.ItemLocation.WeaponL))
			{
				duraitem = Equipments[Enums.ItemLocation.WeaponL];
				if (!duraitem.IsArrow() && !duraitem.IsShield())
				{
					duraitem.CurrentDura -= 1;
					Database.CharacterDatabase.SaveEquipment(this, duraitem, duraitem.Location);
					duraitem.SendPacket(this, 3);
					
					if (duraitem.CurrentDura <= 0)
						BaseEntity.CalculateBaseStats();
				}
			}
		}
		#endregion
		
		#region Revive
		/// <summary>
		/// Revives a player and teleports them to the revive spawn for the map.
		/// </summary>
		/// <returns>Returns true if the player could revive.</returns>
		public bool Revive()
		{
			if (!(DateTime.Now >= ReviveTime))
				return false;
			
			bool ReviveSpot = true;
			if (Battle != null)
			{
				ReviveSpot = Battle.HandleRevive(this);
			}
			// if the map has no revive point then revive here
			if (Map.RevivePoint != null && ReviveSpot)
			{
				Teleport(Map.RevivePoint);
			}
			
			ForceRevive();
			return true;
		}
		
		/// <summary>
		/// Revives a player at their current spot.
		/// </summary>
		/// <returns>Returns true if the player could revive.</returns>
		public bool ReviveHere()
		{
			if (!(DateTime.Now >= ReviveTime))
				return false;
			
			ForceRevive();
			return true;
		}
		
		/// <summary>
		/// Forces a revive upon the player. This is an underlying call for Revive() and ReviveHere() as well.
		/// </summary>
		public void ForceRevive()
		{
			SuperAids = false;
			Alive = true;
			AttackPacket = null;
			Transformation = 0;
			RemoveFlag1(Enums.Effect1.Dead);
			RemoveFlag1(Enums.Effect1.Ghost);
			RemoveFlag1(Enums.Effect1.BlueName);
			
			HP = MaxHP;
			if (MaxMP > 50)
			{
				if (MP < (MaxMP / 2))
					MP = (MaxMP / 2);
			}
			Stamina = Core.NumericConst.MaxStamina;
			ReviveProtection = DateTime.Now;
		}
		#endregion
		
		#region Misc Methods
		public void Kill()
		{
			Packets.Interaction.Battle.Combat.Kill(null, this);
		}
		
		public void ParalyzeClient(int Time)
		{
			Paralyzed = true;
			if (Time > 0)
			{
				ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(
					() => { Paralyzed = false; }, Time);
			}
		}
		#endregion
		
		#endregion
		
		#region Misc
		#endregion
	}
}
