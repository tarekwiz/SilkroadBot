using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Silkroad_Fusion
{
    class Media
    {
        public static Dictionary<uint, Object> Objects = new Dictionary<uint, Object>();
        public static Dictionary<uint, Item> Items = new Dictionary<uint, Item>();
        public static Dictionary<uint, Skill> Skills = new Dictionary<uint, Skill>();
        public static Dictionary<uint, Shop> Shops = new Dictionary<uint, Shop>();
        public static Dictionary<int, string> Areas = new Dictionary<int, string>();

        public struct Object
        {
            public uint Pk2Id;
            public string Pk2Name;
            public string Name;
            public byte Level;
            public uint MaximumHitpoints;

            public Object(uint Pk2Id, string Pk2Name, string Name, byte Level, uint MaximumHitpoints)
            {
                this.Pk2Id = Pk2Id;
                this.Pk2Name = Pk2Name;
                this.Name = Name;
                this.Level = Level;
                this.MaximumHitpoints = MaximumHitpoints;
            }
        }

        public struct Item
        {
            public uint Pk2Id;
            public string Pk2Name;
            public string Name;
            public byte Class_A;
            public byte Class_B;
            public byte Class_C;
            public byte Race;
            public byte Level;

            public Item(uint Pk2Id, string Pk2Name, string Name, byte Class_A, byte Class_B, byte Class_C, byte Race, byte Level)
            {
                this.Pk2Id = Pk2Id;
                this.Pk2Name = Pk2Name;
                this.Name = Name;
                this.Class_A = Class_A;
                this.Class_B = Class_B;
                this.Class_C = Class_C;
                this.Race = Race;
                this.Level = Level;
            }
        }

        public struct Skill
        {
            public uint Pk2Id;
            public string Pk2Name;
            public string Name;
            public byte Level;
            public ushort Required_Magicpoints;
            public long CastTime;

            public Skill(uint Pk2Id, string Pk2Name, string Name, byte Level, ushort Required_Magicpoints, long CastTime)
            {
                this.Pk2Id = Pk2Id;
                this.Pk2Name = Pk2Name;
                this.Name = Name;
                this.Level = Level;
                this.Required_Magicpoints = Required_Magicpoints;
                this.CastTime = CastTime;
            }
        }

        public class Shop
        {
            public uint Pk2Id;
            public List<List<uint>> Items = new List<List<uint>>();

            public Shop(uint Pk2Id, List<List<uint>> Items)
            {
                this.Pk2Id = Pk2Id;
                this.Items = Items;
            }
        }

        public struct shopdata { public byte Tab, ItemPosition;}
        public static shopdata GetShopDataByItemId(uint Pk2Id, uint ItemId)
        {
            shopdata shop = new shopdata();

            for (byte i = 0; i < Shops[Pk2Id].Items.Count; i++)
            {
                for (byte j = 0; j < Shops[Pk2Id].Items[i].Count; j++)
                {
                    if (Shops[Pk2Id].Items[i][j] == ItemId)
                    {
                        shop.Tab = i;
                        shop.ItemPosition = j;
                    }
                }
            }
            return shop;
        }
    }
}
