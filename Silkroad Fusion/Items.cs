using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Silkroad_Fusion
{
   class _Items
    {
        public struct item
        {
            public uint model;
            public byte type;
            public ushort quantity;
            public uint durability;
            public byte plusvalue;
            public byte blueamount;
        }

        public item[] items;

        public void SetItemCount(int i)
        {
            items = new item[i];
        }

        public void Add(byte slot,
                                uint model,
                                byte type,
                                ushort quantity,
                                uint durability,
                                byte plusvalue,
                                byte blueamount)
        {

            items[slot].model = model;
            items[slot].type = type;
            items[slot].quantity = quantity;
            items[slot].durability = durability;
            items[slot].plusvalue = plusvalue;
            items[slot].blueamount = blueamount;
        }

        public void Swap(byte source, byte dest)
        {
            item tmp = items[source];
            items[source] = items[dest];
            items[dest] = tmp;
        }

        public void Slot(byte source, byte dest)
        {
            items[dest] = items[source];
            items[source] = new item();
        }

        public void Del(byte slot)
        {
            items[slot] = new item();
        }

        public byte FreeSlot()
        {
            for (byte i = 13; i < items.Length; i++)
            {
                if (items[i].model == 0) return i;
            }
            return 0;
        }

        public bool IsFull()
        { 
            return FreeSlot() == 0 ? true : false;
        }

        public byte GetItemCount()
        {
            byte bItemCount = 0;

            for (byte i = 0; i < items.Length; i++)
            {
                if (items[i].model != 0)
                    bItemCount++;
            }
            return bItemCount;
        }

        public byte GetItemCount(byte bMax)
        {
            byte bItemCount = 0;

            for (byte i = 0; i < bMax; i++)
            {
                if (items[i].model != 0)
                    bItemCount++;
            }
            return bItemCount;
        }

        public ushort CountItem(uint Pk2Id)
        {
            ushort Quantity = 0;
            for (byte i = 13; i < items.Length; i++)
            {
                if (items[i].model == Pk2Id) Quantity += items[i].quantity;
            }
            return Quantity;
        }

        public byte GetItemSlot(uint Pk2Id,ushort Amount,int NotSlot)
        {
            if (NotSlot == -1)
            {
                for (byte i = 13; i < items.Length; i++)
                {
                    if (items[i].model == Pk2Id && items[i].quantity == Amount) return i;
                }
            }
            else
            {
                for (byte i = 13; i < items.Length; i++)
                {
                    if (items[i].model == Pk2Id && items[i].quantity == Amount && i != NotSlot) return i;
                }
            }
            return 0;
        }
    }
}
