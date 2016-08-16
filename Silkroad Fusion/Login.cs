using System;
using System.Linq;
using Silkroad_Fusion.API;
using System.Collections.Generic;
using System.IO;
using Silkroad_Fusion;

namespace Silkroad_Fusion
{
    class Login
    {
        public static void HandleServerList(Packet tPacket)
        {
            byte OperationFlag = tPacket.ReadByte();
            while (OperationFlag == 0x01)
            {
                tPacket.ReadByte();
                tPacket.ReadAscii();
                OperationFlag = tPacket.ReadByte();
            }

            byte ShardFlag = tPacket.ReadByte();
            ushort serverID = 0;
            string serverName = "";
            uint current = 0;
            uint max = 0;
            byte status = 0x00;
            string Combined = "";
            while (ShardFlag == 0x01)
            {
                 serverID = tPacket.ReadUInt16();
                 serverName = tPacket.ReadAscii();
                 current = tPacket.ReadUInt16();
                 max = tPacket.ReadUInt16();
                 status = tPacket.ReadByte();
                 tPacket.ReadByte();
                 ShardFlag = tPacket.ReadByte();
                 Combined = Combined + "Server Name:" + serverName + " ID:" + serverID;
            }
            System.IO.File.WriteAllText(@"D:/Serverlist.txt", Combined);
        }

        public static void Server_CharacterData(Packet p_Reader)
        {
            StreamWriter s_Writer = new StreamWriter("characterdata.dat", false);
            byte[] p_byte = p_Reader.GetBytes();
            s_Writer.Write(BitConverter.ToString(p_byte).Replace("-", " "));
            s_Writer.Close();

            try
            {
                uint uModel = p_Reader.ReadUInt32();
                ushort uLength = p_Reader.ReadUInt16();
                string sName = p_Reader.ReadAscii();
                Console.WriteLine("Charname : " + sName);
                byte bVolume = p_Reader.ReadByte();
                byte bLevel = p_Reader.ReadByte();
                byte bHighestLevel = p_Reader.ReadByte();
                ulong uExperience = p_Reader.ReadUInt64();
                uint uBar = p_Reader.ReadUInt32();
                ulong uGold = p_Reader.ReadUInt64();
                uint uSkillpoints = p_Reader.ReadUInt32();
                ushort uStatpoints = p_Reader.ReadUInt16();
                byte bBerserkerguage = p_Reader.ReadByte();
                uint uINT = p_Reader.ReadUInt32();
                uint uCurrentHitpoints = p_Reader.ReadUInt32();
                uint uCurrentMagicpoints = p_Reader.ReadUInt32();
                byte bNoob = p_Reader.ReadByte();

                byte bDailyPk = p_Reader.ReadByte();
                ushort uTotalPk = p_Reader.ReadUInt16();
                uint uPkPoints = p_Reader.ReadUInt32();

                p_Reader.ReadUInt64();
                p_Reader.ReadUInt64();
                p_Reader.ReadUInt64();
                p_Reader.ReadByte();
                p_Reader.ReadByte();
                p_Reader.ReadByte();

                
                byte bMaxSlots = p_Reader.ReadByte();
                byte bItemCount = p_Reader.ReadByte();

                Global.Player.Items.SetItemCount(bMaxSlots);

                for (byte i = 0; i < bItemCount; i++)
                {
                    byte bSlot = p_Reader.ReadByte();
                    Console.WriteLine("Slot: " + bSlot);

                    uint uPk2Id = p_Reader.ReadUInt32();
                    Console.WriteLine("uPk2Id: " + uPk2Id);

                    Console.WriteLine(Media.Items[uPk2Id].Name);

                    switch (Media.Items[uPk2Id].Class_A)
                    {
                        case Information.Items._Equipment:
                            byte bPlus = p_Reader.ReadByte();
                            Console.WriteLine("Modifier: " + p_Reader.ReadUInt64());
                            uint uDurability = p_Reader.ReadUInt32();
                            Console.WriteLine("Plus: " + bPlus);
                            Console.WriteLine("Durability: " + uDurability);
                            byte bBlueAmount = p_Reader.ReadByte();
                            Console.WriteLine("Blueamount: " + bBlueAmount);

                            for (byte j = 0; j < bBlueAmount; j++)
                            {
                                Console.WriteLine("Blue: " + p_Reader.ReadUInt32());
                                Console.WriteLine("Blue2: " + p_Reader.ReadUInt32());
                            }

                            Global.Player.Items.Add(bSlot, uPk2Id, Media.Items[uPk2Id].Class_A, 1, uDurability, bPlus, bBlueAmount);
                            break;

                        case Information.Items._Pet:
                            Console.WriteLine("?type?: " + p_Reader.ReadByte());
                            uint object_id = p_Reader.ReadUInt32();
                            Console.WriteLine("?object-id?: " + object_id);
                            p_Reader.ReadUInt16();
                            Console.WriteLine("?name?" + p_Reader.ReadAscii());
                            if (!Media.Objects[object_id].Pk2Name.Contains("WOLF"))
                            {
                                Console.WriteLine("?2: " + p_Reader.ReadUInt32());
                            }

                            Global.Player.Items.Add(bSlot, uPk2Id, Media.Items[uPk2Id].Class_A, 1, 0, 0, 0);
                            break;

                        case Information.Items._ETC:
                            ushort uQuantity = p_Reader.ReadUInt16();
                            Console.WriteLine("Quantity: " + uQuantity);
                            Global.Player.Items.Add(bSlot, uPk2Id, Media.Items[uPk2Id].Class_A, uQuantity, 1, 0, 0);
                            break;
                    }
                    Console.WriteLine();
                }

                byte bAvatarList = p_Reader.ReadByte();

                Global.Player.Masteries.Clear();

                byte bMasteries = 0;
                if (Formula.IsEuropean(uModel))
                    bMasteries = 8;
                else
                    bMasteries = 7;

                for (byte i = 0; i < bMasteries; i++)
                {
                    bool bFlag = p_Reader.ReadBool();
                    if (bFlag)
                    {
                        Console.WriteLine("bFlag: " + bFlag);
                        uint uMasteryId = p_Reader.ReadUInt32();
                        Console.WriteLine("uMasteryId: " + uMasteryId);
                        byte bMasteryLevel = p_Reader.ReadByte();
                        Console.WriteLine("Level: " + bMasteryLevel);
                        Console.WriteLine();

                        Global.Player.Masteries.Add(uMasteryId, bMasteryLevel);
                    }
                }

                byte bSkillList = p_Reader.ReadByte();
                byte bSkillListStart = p_Reader.ReadByte();

                Global.Player.Skills._skill = new List<_Skills.skill>();

                byte bType = p_Reader.ReadByte();
                while (bType == 1)
                {
                    uint uPk2Id = p_Reader.ReadUInt32();
                    Console.WriteLine("uPk2Id: " + uPk2Id);
                    Console.WriteLine(Media.Skills[uPk2Id].Name);
                    Console.WriteLine("Level: " + Media.Skills[uPk2Id].Level);

                    byte bFlag = p_Reader.ReadByte();

                    Global.Player.Skills.Add(uPk2Id);

                    bType = p_Reader.ReadByte();
                }


                Console.WriteLine();

                byte bAmountOfCompletedQuests = p_Reader.ReadByte();

                for (byte i = 0; i < bAmountOfCompletedQuests; i++)
                {
                    Console.WriteLine("QuestId: " + p_Reader.ReadUInt32());
                }

                byte bAmountOfPendingQuests = p_Reader.ReadByte();
                for (byte i = 0; i < bAmountOfPendingQuests; i++)
                {
                    Console.WriteLine("QuestId: " + p_Reader.ReadUInt32());
                    Console.WriteLine("QuestRepetitionAmount: " + p_Reader.ReadByte());
                    Console.WriteLine("QuestCompletetionAmount: " + p_Reader.ReadByte());
                    byte bQuestType = p_Reader.ReadByte();
                    Console.WriteLine("QuestType: " + bQuestType);
                    Console.WriteLine("Unkown: " + p_Reader.ReadByte());
                    Console.WriteLine("Unkown: " + p_Reader.ReadByte());
                    Console.WriteLine("Objective Order Number: " + p_Reader.ReadByte());
                    Console.WriteLine("Status: " + p_Reader.ReadByte());
                    ushort uQLength = p_Reader.ReadUInt16();
                    Console.WriteLine("Objective REF: " + p_Reader.ReadAscii());
                    if (bQuestType == 24)
                    {
                        byte bAmountOfObjectives = p_Reader.ReadByte();
                        Console.WriteLine("Amount of objectives: " + bAmountOfObjectives);
                        for (int j = 0; j < bAmountOfObjectives; j++)
                        {
                            Console.WriteLine(" Amount of Items/Kills/NpcId: " + p_Reader.ReadUInt32());
                        }
                    }
                    else if (bQuestType == 88)
                    {
                        byte bAmountOfObjectives = p_Reader.ReadByte();
                        Console.WriteLine("Amount of objectives: " + bAmountOfObjectives);
                        for (int j = 0; j < bAmountOfObjectives; j++)
                        {
                            Console.WriteLine("Items/Kills/NpcId: " + p_Reader.ReadUInt32());
                        }

                        byte bAmountOfQuestNpcs = p_Reader.ReadByte();
                        Console.WriteLine("Amount of Quest Npcs: " + bAmountOfQuestNpcs);
                        for (int j = 0; j < bAmountOfQuestNpcs; j++)
                        {
                            Console.WriteLine("QuestId: " + p_Reader.ReadUInt32());
                        }
                    }
                }

                Console.WriteLine();

                byte bQuestListEnd = p_Reader.ReadByte();

                uint uPlayerId = p_Reader.ReadUInt32();
                byte bX = p_Reader.ReadByte();
                byte bY = p_Reader.ReadByte();
                float fX = p_Reader.ReadUInt32();
                float fZ = p_Reader.ReadUInt32();
                float fY = p_Reader.ReadUInt32();
                ushort uAngle = p_Reader.ReadUInt16();

                byte bHaveDestination = p_Reader.ReadByte();
                byte bNoDestination = p_Reader.ReadByte();
                if (bHaveDestination == 0)
                {
                    bNoDestination = p_Reader.ReadByte();
                    uAngle = p_Reader.ReadUInt16();
                }
                else
                {
                    byte bX_ = p_Reader.ReadByte();
                    byte bY_ = p_Reader.ReadByte();
                    ushort fX_ = p_Reader.ReadUInt16();
                    ushort fZ_ = p_Reader.ReadUInt16();
                    ushort fY_ = p_Reader.ReadUInt16();
                }

                byte bDeathFlag = p_Reader.ReadByte();
                byte bMovementFlag = p_Reader.ReadByte();
                byte bBerserkermode = p_Reader.ReadByte();
                float fWalkingSpeed = p_Reader.ReadUInt32();
                float fRunningSpeed = p_Reader.ReadUInt32();
                float fBerserkerSpeed = p_Reader.ReadUInt32();


                Global.Player.General.CharacterName = sName;
                Console.WriteLine("CharacterName : " + sName);

                Global.Player.General.UniqueID = uPlayerId;
                Console.WriteLine("UniqueID : " + uPlayerId);

                Global.Player.Speeds.WalkSpeed = fWalkingSpeed;
                Global.Player.Speeds.RunSpeed = fRunningSpeed;
                Global.Player.Speeds.BerserkSpeed = fBerserkerSpeed;

                Global.Player.Stats.Attributes = uStatpoints;
                Global.Player.Stats.BerserkBar = bBerserkerguage;

                Global.Player.Stats.CurrentHitpoints = uCurrentHitpoints;
                Console.WriteLine("CurrentHitpoints : " + uCurrentHitpoints);

                Global.Player.Stats.CurrentManapoints = uCurrentMagicpoints;
                Global.Player.Stats.Experience = uExperience;

                Global.Player.Stats.Gold = uGold;
                Console.WriteLine("Gold : " + uGold);

                Global.Player.Stats.Level = bLevel;
                Global.Player.Stats.Model = uModel;

                Global.Player.Stats.SkillPoints = uSkillpoints;
                Console.WriteLine("SkillPoints : " + uSkillpoints);

                Global.Player.Stats.Volume = bVolume;

                Global.Player.Position.XSector = bX;
                Global.Player.Position.YSector = bY;
                Global.Player.Position.X = fX;
                Global.Player.Position.Z = fZ;
                Global.Player.Position.Y = fY;

                Global.Player.Position.XPosition = Formula.CalculatePositionX(bX, fX);
                Global.Player.Position.YPosition = Formula.CalculatePositionY(bY, fY);
                frmMain frmMain = new frmMain();
                frmMain.ClearSkills();

                foreach (_Skills.skill skill in Global.Player.Skills._skill)
                {
                    string Name = Media.Skills[skill.uPk2Id].Name;
                    byte Level = Media.Skills[skill.uPk2Id].Level;
                    frmMain.AddSkill(Name, Level, skill.uPk2Id);
                }

                if (!Movement.bUsedGate && Movement.bRecording)
                {
                    Movement.sMovePath.Add("wait");
                    frmMain.AddRecordLog("wait");
                }

                Movement.bUsedGate = false;

                if (bSkillList != 2)
                {
                    Console.WriteLine("Could not parse the characterdata packet correctly! Please report this bug with the uploaded characterdata.dat file in your bot directory!");
                }

                //if (Loop.bWaitForTown)
                //{
              //      Thread BotThread = new Thread(WaitForTeleport);
             //       BotThread.Start();
             //   }

              //  if (Loop.bWaitForTeleport)
             //       Loop.bTeleported = true;


            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("Could not parse the characterdata packet correctly! Please report this bug with the uploaded characterdata.dat file in your bot directory!");
            }
        } //Bugged
    }
}
        

