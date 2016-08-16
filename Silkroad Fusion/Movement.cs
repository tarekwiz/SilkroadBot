using System;
using System.Linq;
using Silkroad_Fusion.API;
using System.Collections.Generic;

namespace Silkroad_Fusion
{
    class Movement
    {
        public static bool bRecording = false;
        public static bool bUsedGate = false;
        public static List<string> sMovePath = new List<string>();

        public static void WalkTo(long X, long Y, short Z)
        {
            long xPos = 0;
            long yPos = 0;

            if (X > 0 && Y > 0)
            {
                xPos = (X % 192) * 10;
                yPos = (Y % 192) * 10;
            }
            else if (X < 0 && Y < 0)
            {
                xPos = (192 + (X % 192)) * 10;
                yPos = (192 + (Y % 192)) * 10;
            }
            else if (X < 0 && Y > 0)
            {
                xPos = (192 + (X % 192)) * 10;
                yPos = (Y % 192) * 10;
            }
            else if (X > 0 && Y < 0)
            {
                xPos = (X % 192) * 10;
                yPos = (192 + (Y % 192)) * 10;
            }

            byte xSector = (byte)((X - xPos / 10) / 192 + 135);
            byte ySector = (byte)((Y - yPos / 10) / 192 + 92);

            xPos = (X - ((xSector - 0x87) * 0xC0)) * 0x0A;
            yPos = (Y - ((ySector - 0x5C) * 0xC0)) * 0x0A;

            Packet p_Writer = new Packet(Silkroad_Fusion.Opcodes.Opcode.CLIENT_MOVEMENT);
            if (xSector == 1)
            {
                ySector++;	

                //Recalculate X, Y coordinates
                xPos = (X - ((xSector - 0x87) * 0xC0)) * 0x0A;
                yPos = (Y - ((ySector - 0x5C) * 0xC0)) * 0x0A;

                p_Writer.WriteByte(1);

                p_Writer.WriteByte(1);
                p_Writer.WriteByte(128);

                p_Writer.WriteShort((short)xPos);
                p_Writer.WriteShort(Z);
                p_Writer.WriteShort((short)yPos);
            }
            else
            {
                p_Writer.WriteByte(1);

                p_Writer.WriteByte(xSector);
                p_Writer.WriteByte(ySector);

                p_Writer.WriteShort((short)xPos);
                p_Writer.WriteShort(Z);
                p_Writer.WriteShort((short)yPos);
            }


            Proxy.SendAgent(p_Writer);
        }
        public static void CharacterMoving(Packet p_Reader)
        {
            uint uUniqueId = p_Reader.ReadUInt32();
            byte bFlag = p_Reader.ReadByte();
            if (bFlag == 1)
            {
                if (uUniqueId == Global.Player.General.UniqueID)
                {
                    byte bX = p_Reader.ReadByte();
                    byte bY = p_Reader.ReadByte();

                    short sX = p_Reader.ReadShort();
                    short sZ = p_Reader.ReadShort();
                    short sY = p_Reader.ReadShort();
                    Global.Player.Position.XPosition = CalculatePositionX(bX, sX);
                    Global.Player.Position.YPosition = CalculatePositionY(bY, sY);
                    Console.WriteLine("Character Moving to X:" + CalculatePositionX(bX, sX) + " Y:" + CalculatePositionY(bY, sY) + ".");
                }
            }
        }
        public static int CalculatePositionX(ushort xSector, float X)
        {
            return (int)((xSector - 135) * 192 + X / 10);
        }
        public static int CalculatePositionY(ushort ySector, float Y)
        {
            return (int)((ySector - 92) * 192 + Y / 10);
        }
        public static int GetTime(int x, int y, int DesX, int DesY)
        {
            float time = 0f;

            float speed = 0;
            //if (Global.Player.Transport.UniqueID != 0)
                speed = 90.0f * Math.PacketToMeterSeconds_Transformation;
            //else
             //   speed = Global.Player.Speeds.RunSpeed * Math.PacketToMeterSeconds_Transformation;

            double tx = (DesX - x);
            double ty = (DesY - y);
            double distance = System.Math.Sqrt(tx * tx + ty * ty);

            time = (float)distance / speed;

            decimal intervalTime = (decimal)time;

            Console.WriteLine("| Walk speed " + Global.Player.Speeds.RunSpeed + "+ Time: " + System.Math.Round(intervalTime) + " spot X: " + DesX + " Y: " + DesY + "Des: " + distance);

            return (int)(intervalTime * 1000);
        }
        public class Math
        {
            public const float
            PacketToMeterSeconds_Transformation = 0.0768f;
        }

    }
}