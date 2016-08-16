using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Silkroad_Fusion
{
    class Information
    {
        public class Items
        {
            public const byte
            _Gold = 0,
            _Equipment = 1,
            _Pet = 2,
            _ETC = 3,
            _QSP = 4;
        }

        public class Math
        {
            public const float
            PacketToMeterSeconds_Transformation = 0.0768f;
        }

        public class Mastery
        {
            public const ushort
            CH_BICHEON = 257,
            CH_HEUKSAL = 258,
            CH_PACHEON = 259,
            CH_COLD = 273,
            CH_LIGHTNING = 274,
            CH_FIRE = 275,
            CH_FORCE = 276,

            EU_WARRIOR = 513,
            EU_WIZARD = 514,
            EU_ROUGE = 515,
            EU_WARLOCK = 516,
            EU_BARD = 517,
            EU_CLERIC = 518;

            public static string GetName(uint Pk2Id)
            {
                switch (Pk2Id)
                {
                    case CH_BICHEON:
                        return "Bicheon";
                    case CH_HEUKSAL:
                        return "Heuksal";
                    case CH_PACHEON:
                        return "Pacheon";
                    case CH_COLD:
                        return "Cold";
                    case CH_LIGHTNING:
                        return "Lightning";
                    case CH_FIRE:
                        return "Fire";
                    case CH_FORCE:
                        return "Force";

                    case EU_WARRIOR:
                        return "Warrior";
                    case EU_WIZARD:
                        return "Wizard";
                    case EU_ROUGE:
                        return "Rouge";
                    case EU_WARLOCK:
                        return "Warlock";
                    case EU_BARD:
                        return "Bard";
                    case EU_CLERIC:
                        return "Cleric";
                }
                return "";
            }
        }
    }
}