using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Silkroad_Fusion
{
    class Formula
    {
        public static bool IsEuropean(uint uModel)
        {
            if (uModel >= 14948 && uModel <= 14973)
                return true;
            else
                return false;
        }

        public static int CreateSector(byte bLow, byte bHigh)
        {
            return bLow | bHigh << 8;
        }

        public static int CalculatePositionX(ushort xSector, float X)
        {
            return (int)((xSector - 135) * 192 + X / 10);
        }
        public static int CalculatePositionY(ushort ySector, float Y)
        {
            return (int)((ySector - 92) * 192 + Y / 10);
        }

        public static double CalculateDistance(_Position Object_1, _Position Object_2)
        {
            return Math.Sqrt(Math.Pow(Object_2.XPosition - Object_1.XPosition, 2) + Math.Pow(Object_2.YPosition - Object_1.YPosition, 2));
        }

        public static double CalculateDistance(_Position Object_1, int X, int Y)
        {
            return Math.Sqrt(Math.Pow(X - Object_1.XPosition, 2) + Math.Pow(Y - Object_1.YPosition, 2));
        }

        public static double CalculateDistance2(_Position Object_1, _Position Object_2)
        {
            return Math.Sqrt(Math.Pow(Object_2.X - Object_1.X, 2) + Math.Pow(Object_2.Y - Object_1.Y, 2));
        }
    }
}
