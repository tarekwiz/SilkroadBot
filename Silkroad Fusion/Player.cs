using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Silkroad_Fusion.API;
using Silkroad_Fusion;
namespace Silkroad_Fusion
{
    class _Player
    {
        public struct _Stats
        {
            public uint CurrentHP, CurrentMP;
            public uint CurrentHitpoints, CurrentManapoints;
            public uint Model;
            public uint SkillPoints;
            public byte Volume;
            public byte Level;
            public ushort MinMag, MaxMag;
            public ushort MinPhy, MaxPhy;
            public ushort PhyDef, MagDef;
            public ushort AttackRatio, ParryRatio;
            public ushort Strength;
            public ushort Intelligence;
            public ushort Attributes;
            public ulong Gold;
            public ulong Experience;
            public ulong AvailableStatPoints;
            public byte BerserkBar;
        }

        public struct _Speeds
        {
            public float WalkSpeed;
            public float RunSpeed;
            public float BerserkSpeed;
        }

        public struct _General
        {
            public uint UniqueID;
            public string CharacterName;
            public string GuildName;
        }

        public struct _Party
        {
            public int iMembers;
            public bool Formed;
        }

        public struct _Transport
        {
            public uint UniqueID;
            public uint MaxHP;
            public uint HP;
        }

        public _Stats Stats = new _Stats();
        public _Speeds Speeds = new _Speeds();
        public _General General = new _General();
        public _Items Items = new _Items();
        public _Skills Skills = new _Skills();
        public _Masteries Masteries = new _Masteries();
        public _Position Position = new _Position();
        public _Party Party = new _Party();
        public _Transport Transport = new _Transport();

        
    }
}
