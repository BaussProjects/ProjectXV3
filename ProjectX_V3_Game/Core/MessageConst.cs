//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Game.Core
{
	/// <summary>
	/// Message constants.
	/// </summary>
	public class MessageConst
	{
		#region Misc
		public const string LEVEL_ONLY = "This feature is available in the level server only!";
		#endregion
		
		#region Login
		public const string ANSWER_OK = "ANSWER_OK";
		public const string LOGIN_MSG = "{0} has logged in. Account: {1} EntityUID: {2} DatabaseUID: {3}";
		public const string ANSWER_NO = "Failed to load the character, please try again later!";
		public const string NEW_ROLE = "NEW_ROLE";
		public const string SERVER_CONNECTION_OFF = "The server does not allow connections at the moment.";
		#endregion
		#region Character Creation
		public const string INVALID_MODEL = "Invalid model!";
		public const string INVALID_CLASS = "Invalid class!";
		public const string INVALID_CHARS = "The name contains illegal characters!";
		public const string INVALID_NAME = "The name is banned!";
		public const string CHAR_EXIST = "This name is already in use!";
		#endregion
		#region Online
		public const string ONLINE = "There is currently {0} players online.";
		#endregion
		#region Whisper
		public const string PLAYER_OFFLINE_WHISPER = "Could not whisper {0}. The user may be offline. The player will receive the message once online.";
		public const string PLAYER_OFFLINE_WHISPER2 = "Could not whisper {0}. Please make sure the name is correct.";
		#endregion
		#region Chat
		public const string WORLD_CHAT_NO_PERMISSION = "You need to be level 70 to use the world chat!";
		public const string WORLD_CHAT_WAIT = "You have to wait at least {0} seconds before using the world chat again!";
		#endregion
		#region NPC
		public const string NPC_NOT_FOUND = "NPCID: {0}";
		public const string TOO_FAR_NPC = "You are too far away from this npc!";
		#endregion
		#region Command
		public const string UNKNOWN_COMMAND = "{0} is an unknown command.";
		public const string INVALID_COMMAND = "Invalid input for the {0} command";
		#endregion
		#region Shop
		public const string FAIL_BUY_ERROR = "Failed to buy. Please try again!";
		public const string LOW_MONEY = "You need more money. The price is {0} {1} for {2} {3}(s)!";
		public const string LOW_MONEY2 = "You need more money to buy this item!";
		public const string LOW_CPS = "You need more cps to buy this item!";
		public const string TOO_MANY_BUY = "You do not have space for this much!";
		public const string BOOTH_BUY = "{0} has bought {1} from your booth.";
		#endregion
		#region Items
		public const string INVENTORY_FULL = "Your inventory is full.";
		public const string ITEM_FEMALE = "This item is female only.";
		public const string INVALID_ITEM_TYPE = "This item cannot be equipped, because it's an invalid type.";
		public const string INVALID_ITEM_POS = "This item position is not allowing equipments.";
		public const string TWO_HAND_EQUIP_FAIL = "You cannot wear a two handed weapon while having a left hand item equipped.";
		public const string ITEM_JOB_INVALID = "This item can only be equipped by {0}'s";
		public const string LEVEL_LOW = "Your level is too low.";
		public const string STATS_LOW = "Your stats is too low.";
		public const string INVALD_ITEM_USE = "This item cannot be used. Item name: {0} Item ID: {1}";
		public const string ITEM_LOW_DURA = "This item's dura is too low for this.";
		public const string ITEM_AMOUNT_FAIL = "You do not have enough {0} to do this.";
		public const string ITEM_NOT_FOUND = "What the fuck dude?";
		public const string ITEM_MAX_PLUS = "This item is already highest plus.";
		public const string ITEM_NO_PLUS = "This item has no plus.";
		public const string ITEM_LOW_PLUS = "You cannot use an item with a lower + than the main item.";
		public const string ITEM_INVALID_UPGRADE = "This is an invalid item type.";
		public const string ITEM_SUPER = "This item is already super!";
		public const string ITEM_MAX_LEVEL = "This item cannot be higher level!";
		public const string VIEW_EQUIP = "{0} is viewing your equipments.";
		#endregion
		#region Trade
		public const string SELF_ALREADY_TRADE = "You're already trading someone.";
		public const string TARGET_BUSY = "Target is busy. Please try again.";
		public const string ALREADY_TRADE = "Target is already trading.";
		public const string NO_PERMISSION_ITEM = "Please make sure you have permission to use this item for this action. (Cause: Locked, Suspicious or Free.)";
		public const string TARGET_FULL_INVENTORY = "The inventory of the target is full.";
		public const string LOW_MONEY_TRADE = "Not enough money.";
		public const string LOW_CPS_TRADE = "Not enough cps.";
		public const string TRADE_SUCCESS = "Trade success.";
		public const string TRADE_FAIL = "Trading failed.";
		#endregion
		#region Love
		public const string MARRIAGE_CONGRATZ = "Congratulations to {0} and {1}! They just got married! #49 #49 #52";
		public const string MARRIAGE_SELF_SPOUSE = "You're already married. Aren't you happy enough?";
		public const string MARRIAGE_TARGET_SPOUSE = "He or she is already married.";
		public const string DIVORCE = "{0} and {1} has just been divorce :'(";
		#endregion
		#region Combat
		public const string REST = "Please rest for a while!";
		public const string AUTO_RELOAD_ARROW_FAIL = "Failed to reload arrows, please do it manual.";
		public const string EMPTY_ARROWS = "You have no more arrows. Visit a blacksmith to buy new.";
		public const string INVALID_SKILL = "This skill is not yet handled. Please report this!";
		#endregion
		#region PKPoints
		public const string PK_POINTS_REMOVED = "Your sins are gone.";
		#endregion
		#region Exp
		public const string GAIN_EXP = "You have gained {0} experiences.";
		public const string GAIN_PROF_EXP = "You have gained {0} prof experiences.";
		public const string GAIN_SPELL_EXP = "You have gained {0} spell experiences.";
		#endregion
		#region Drop
		public const string NOT_OWNER_ITEM = "This is not yours.";
		#endregion
		#region Guild
		public const string GUILD_DISBAN = "Your guild has been disbanned!";
		public const string NEW_DEPUTY_LEADER = "{0} is now a deputy leader of the guild!";
		public const string NEW_MEMBER = "{0} is now a member of the guild! Was added by {1}";
		public const string DEPUTY_REMOVED = "{0} is no longer a deputy leader.";
		public const string NEW_GUILD = "{0} has been created by {1}!";
		public const string LEAVE_GUILD = "{0} is no longer a member of the guild!";
		public const string NEW_ALLIE = "{0} is your new allie!";
		public const string REMOVE_ALLIE = "{0} is no longer your allie!";
		public const string NEW_ENEMY = "{0} is your new enemy!";
		public const string REMOVE_ENEMY = "{0} is no longer your enemy!";
		public const string BULLENTIN_UPDATE = "{0} has updated the guild announcement!";
		public const string GUILD_DONATE = "{0} has donated {1} {2} to the guild!";
		public const string USE_GUILD_DIRECTOR = "Use GuildDirector in TwinCity to do this action.";
		#endregion
		#region Arena
		public const string ARENA_DRAW = "The battle in the arena between {0} and {1} has ended in a draw.";
		public const string ARENA_WIN = "{0} has won over {1} in the arena.";
		#endregion
		#region Nobility
		public const string NEW_KING = "Bow down everyone! {0} is the new king!";
		public const string NEW_QUEEN = "Bow down everyone! {0} is the new queen!";
		public const string NEW_PRINCE = "A new prince has been born, named {0}!";
		public const string NEW_PRINCESS = "A new princess has been born, named {0}!";
		#endregion
		#region BattlePets
		public const string TRAINER_BATTLE = "{0} wants to battle you!";
		#endregion
		#region Travel
		public const string TRAVEL_TIME = "Reaching destination in {0} seconds.";
		public const string TRAVEL_REACH = "Destination reached.";
		#endregion
		#region Fun
		public const string SUPER_AIDS = "{0} was raped and killed by the horrible disease super aids!!!! LOLZZZZ #28#27";
		#endregion
	}
}
