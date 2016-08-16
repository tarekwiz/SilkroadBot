using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Silkroad_Fusion
{
    class Dump
    {
        public static void TextData()
        {
            if (!File.Exists("Data\\objects.txt"))
                MessageBox.Show("Could not find the essentiel data!");

            StreamReader sr_Reader = new StreamReader("Data\\objects.txt");
            while (!sr_Reader.EndOfStream)
            {
                string[] s_objects = sr_Reader.ReadLine().Split(',');
                //Name;Pk2Name;Pk2Id;Level;MaximumHp
                uint Pk2Id = Convert.ToUInt32(s_objects[2]);
                Media.Objects.Add(Pk2Id, new Media.Object(Pk2Id, s_objects[1], s_objects[0], Convert.ToByte(s_objects[3]), Convert.ToUInt32(s_objects[4])));
            }
            sr_Reader.Close();

            sr_Reader = new StreamReader("Data\\items.txt");
            while (!sr_Reader.EndOfStream)
            {

                string[] s_objects = sr_Reader.ReadLine().Split(',');
                //Name;Pk2Name;Pk2Id;Level;Race;A;B;C
                uint Pk2Id = Convert.ToUInt32(s_objects[2]);
                Media.Items.Add(Pk2Id, new Media.Item(Pk2Id, s_objects[1], s_objects[0], Convert.ToByte(s_objects[5]), Convert.ToByte(s_objects[6]), Convert.ToByte(s_objects[7]), Convert.ToByte(s_objects[4]), Convert.ToByte(s_objects[3])));
            }
            sr_Reader.Close();

            sr_Reader = new StreamReader("Data\\skills.txt");
            while (!sr_Reader.EndOfStream)
            {
                string[] s_objects = sr_Reader.ReadLine().Split(',');
                //Name;Pk2Name;Pk2Id;Level;RequiredMana;CastTime
                uint Pk2Id = Convert.ToUInt32(s_objects[2]);
                Media.Skills.Add(Pk2Id, new Media.Skill(Pk2Id, s_objects[1], s_objects[0], Convert.ToByte(s_objects[3]), Convert.ToUInt16(s_objects[4]), Convert.ToInt64(s_objects[5])));
            }
            sr_Reader.Close();


            sr_Reader = new StreamReader("Data\\npcdata.txt");
            Dictionary<uint, List<List<uint>>> npcdata = new Dictionary<uint, List<List<uint>>>();

            while (!sr_Reader.EndOfStream)
            {
                string[] s_objects = sr_Reader.ReadLine().Split(',');
                //Pk2Id,Tab,ItemPosition,ItemId
                uint Pk2Id = Convert.ToUInt32(s_objects[0]);
                int Tab = Convert.ToInt32(s_objects[1]);
                int Position = Convert.ToInt32(s_objects[2]);
                uint ItemId = Convert.ToUInt32(s_objects[3]);

                if (npcdata.ContainsKey(Pk2Id))
                {
                    while (npcdata[Pk2Id].Count - 1 != Tab)
                        npcdata[Pk2Id].Add(new List<uint>());
                    npcdata[Pk2Id][Tab].Add(ItemId);
                }
                else
                {
                    List<List<uint>> npc = new List<List<uint>>();
                    npc.Add(new List<uint>());

                    npcdata.Add(Pk2Id, npc);

                    npcdata[Pk2Id][Tab].Add(ItemId);
                }
            }

            foreach (uint Pk2Id in npcdata.Keys)
            {
                Media.Shops.Add(Pk2Id, new Media.Shop(Pk2Id, npcdata[Pk2Id]));
            }

            npcdata.Clear();
            sr_Reader.Close();

            sr_Reader = new StreamReader("Data\\area.txt");
            while (!sr_Reader.EndOfStream)
            {
                string[] s_objects = sr_Reader.ReadLine().Split(';');
                //AreaId,Area
                Media.Areas.Add(Convert.ToInt32(s_objects[0]), s_objects[1]);
            }
            sr_Reader.Close();
        }
    }
}
