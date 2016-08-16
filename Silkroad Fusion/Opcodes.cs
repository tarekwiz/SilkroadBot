using System;
using System.Collections.Generic;
using System.Text;

namespace Silkroad_Fusion
{
    class Opcodes
    {
        public class Opcode
        {
            public const ushort
                LOGIN_CLIENT_INFO = 0x2001,
                LOGIN_CLIENT_KEEP_ALIVE = 0x2002,
                LOGIN_CLIENT_PATCH_REQUEST = 0x6100,
                LOGIN_CLIENT_SERVERLIST_REQUEST = 0x6101,
                LOGIN_CLIENT_AUTH = 0x6102,
                LOGIN_CLIENT_ACCEPT_HANDSHAKE = 0x9000,
                LOGIN_CLIENT_LAUNCHER = 0x6104,
                LOGIN_CLIENT_SECONDARY_PASSCODE = 0x7625,

                LOGIN_SERVER_INFO = 0x2001,
                LOGIN_SERVER_HANDSHAKE = 0x5000,
                LOGIN_SERVER_PATCH_INFO = 0x600D,
                LOGIN_SERVER_LAUNCHER = 0x600D,
                LOGIN_SERVER_LIST = 0xA101,
                LOGIN_SERVER_AUTH_INFO = 0xA102,
                LOGIN_SERVER_RESULT = 0xB624,

                 CLIENT_INFO = 0x2001,
                 CLIENT_ACCEPT_HANDSHAKE = 0x9000,
                 CLIENT_KEEP_ALIVE = 0x2002,
                 CLIENT_PATCH_REQUEST = 0x6100,
                 CLIENT_AUTH = 0x6103,
                 CLIENT_ITEM_MOVE = 0x7034,
                 CLIENT_INGAME_NOTIFY = 0x3012,//0x70EA,
                 CLIENT_CLOSE = 0x7005,
                 CLIENT_COUNTDOWN_INTERRUPT = 0x7006,
                 CLIENT_CHARACTER = 0x7007,
                 CLIENT_CHAT = 0x7025,
                 CLIENT_INGAME_REQUEST = 0x7001,
                 CLIENT_TARGET = 0x7045,
                 CLIENT_GM = 0x7010,
                 CLIENT_MOVEMENT = 0x7021,
                 CLIENT_TRANSPORT_MOVE = 0x70C5,
                 CLIENT_PLAYER_ACTION = 0x7074,
                 CLIENT_STR_UPDATE = 0x7050,
                 CLIENT_INT_UPDATE = 0x7051,
                 CLIENT_CHARACTER_STATE = 0x704F,
                 CLIENT_RESPAWN = 0x3053,
                 CLIENT_MASTERYUPDATE = 0x70A2,
                 CLIENT_SKILLUPDATE = 0x70A1,
                 CLIENT_EMOTION = 0x3091,
                 CLIENT_ITEM_USE = 0x704C,
                 CLIENT_HOTKEY_CHANGE = 0x7158,
                 CLIENT_OPEN_SHOP = 0x7046,
                 CLIENT_CLOSE_SHOP = 0x704B,
                 CLIENT_TELEPORT = 0x705A,
                 CLIENT_PARTY_FORM = 0x7069,
                 CLIENT_PARTY_EDIT = 0x706A,
                 CLIENT_PARTY_DELETE = 0x706B,
                 CLIENT_PARTY_MATCHING = 0x706C,
                 CLIENT_PARTY_REQUEST = 0x706D,
                 CLIENT_PARTY_ACCEPT = 0x306E,
                 CLIENT_PARTY_INVITE = 0x7060,
                 CLIENT_PARTY_DISMISS = 0x7061,
                 CLIENT_PARTY_KICK = 0x7063,
                 CLIENT_ANIMATION_INVITE = 0x3080,
                 CLIENT_ALCHEMY = 0x7150,
                 CLIENT_TRANSPORT_HOME = 0x70CB,
                 CLIENT_TRANSPORT_DELETE = 0x70CB,
                 CLIENT_OPEN_STORAGE = 0x703C,
                 CLIENT_REPAIR = 0x703E,
                 CLIENT_USE_BERSERK = 0x70A7,

                 SERVER_INFO = 0x2001,
                 SERVER_HANDSHAKE = 0x5000,
                 SERVER_PATCH_INFO = 0x600D,
                 SERVER_LOGIN_RESULT = 0xA103,

                 SERVER_CHARACTER = 0xB007,
                 SERVER_CHARDATA = 0x3013,
                 SERVER_INGAME_ACCEPT = 0xB001,
                 SERVER_LOADING_START = 0x34A5,
                 SERVER_LOADING_END = 0x34A6,
                 SERVER_CHAR_ID = 0x3020,

                 SERVER_SPAWN = 0x3015,
                 SERVER_DESPAWN = 0x3016,

                 SERVER_GROUPSPAWN_HEAD = 0x3017,
                 SERVER_GROUPSPAWN_BODY = 0x3019,
                 SERVER_GROUPSPAWN_TAIL = 0x3018,

                 SERVER_ITEM_EQUIP = 0x3038,
                 SERVER_ITEM_UNEQUIP = 0x3039,
                 SERVER_ITEM_MOVEMENT = 0xB034,
                 SERVER_NEW_GOLD_AMOUNT = 0x304E,
                 SERVER_ANIMATION_ITEM_PICKUP = 0x3036,
                 SERVER_ITEM_USE = 0xB04C,
                 SERVER_ANIMATION_ITEM_USE = 0x305C,
                 SERVER_ANIMATION_CAPE = 0x3041,
                 SERVER_ITEM_QUANTITY_UPDATE = 0x3040,

                 SERVER_QUIT_GAME = 0x300A,
                 SERVER_COUNTDOWN = 0xB005,
                 SERVER_COUNTDOWN_INTERRUPT = 0xB006,

                 SERVER_STATS = 0x303D,
                 SERVER_STR_UPDATE = 0xB050,
                 SERVER_INT_UPDATE = 0xB051,
                 SERVER_CHARACTER_STATE = 0x30BF,
                 SERVER_HPMP_UPDATE = 0x3057,
                 SERVER_ANIMATION_LEVEL_UP = 0x3054,
                 SERVER_EXP = 0x3056,
                 SERVER_MASTERYUPDATE = 0xB0A2,
                 SERVER_SKILLPOINTS = 0x304E,
                 SERVER_SKILLUPDATE = 0xB0A1,

                 SERVER_CHAT = 0x3026,
                 SERVER_CHAT_ACCEPT = 0xB025,

                 SERVER_TARGET = 0xB045,
                 SERVER_MOVEMENT = 0xB021,
                 SERVER_UNIQUE = 0x300C,

                 SERVER_ANIMATION_COS_SPAWN = 0x30C8,
                 SERVER_COS_SIT_UP = 0xB0CB,
                 SERVER_ANIMATION_COS_REMOVE_MENU = 0x30C9,
                 SERVER_COS_DELETE = 0xB0C6,

                 SERVER_ATTACK = 0xB070,
                 SERVER_SKILL_ATTACK = 0xB074,
                 SERVER_END_SKILL = 0xB071,

                 SERVER_BUFF_START = 0xB0BD,
                 SERVER_BUFF_END = 0xB072,

                 SERVER_DEAD = 0x3011,
                 SERVER_DEAD2 = 0x30D2,

                 SERVER_PARTY_FORM = 0xB069,
                 SERVER_PARTY_EDIT = 0xB06A,
                 SERVER_PARTY_DELETE = 0xB06B,
                 SERVER_PARTY_MATCHING = 0xB06C,
                 SERVER_PARTY_ACCEPT = 0xB06D,
                 SERVER_PARTY_REQUEST = 0x706D,
                 SERVER_PARTY_NEW_PARTY = 0x3065,
                 SERVER_PARTY_CHANGES = 0x3864,
                 SERVER_PARTY_INVITE = 0xB060,
                 SERVER_ANIMATION_INVITE = 0x3080,

                 SERVER_OPEN_SHOP = 0xB046,
                 SERVER_CLOSE_SHOP = 0xB04B,
                 SERVER_SILK_AMOUNT = 0x3153,

                 SERVER_TELEPORT = 0xB05A,
                 SERVER_ANIMATION_TELEPORT = 0x34B5,

                 SERVER_STORAGE_GOLD = 0x3047,
                 SERVER_STORAGE_ITEMS = 0x3049,
                 SERVER_STORAGE_END = 0x3048,

                 SERVER_ALCHEMY = 0xB150,

                 SERVER_REPAIR = 0xB03E,
                 SERVER_ITEM_DURABILITY_CHANGE = 0x3052,

                 SERVER_CHARACTER_STUCK = 0xB023;
        }
        
    }
}