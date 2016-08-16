using System;
using System.Linq;
using System.Text;

namespace Silkroad_Fusion
{
    class Global
    {
        public class Connection
        {
            public static bool _Connected;
        }

        public static bool SentWelcomeMessage = false;

        public static bool bDebug = true;

        public static _Player Player = new _Player();

        public enum enumChatMode : byte
        {
            General = 0x01,
            Private = 0x02,
            GameMaster = 0x03,
            Party = 0x04,
            Guild = 0x05,
            Global = 0x06,
            Notice = 0x07,
            Union = 0x0B,
            Unique
        }
        public static void UpdateServerList()
        {
            string[] lines = {""};
            int x = 0 ;
            Silkroad_Fusion.Data.ServerList.ForEach(server =>
            {
                lines[x] += server.name + "," + server.ID + System.Environment.NewLine;
                x++;
            });

            System.IO.File.WriteAllText(@"D:/Serverlist.txt", ConvertStringArrayToString(lines));
        }
        static string ConvertStringArrayToString(string[] array)
        {
            //
            // Concatenate all the elements into a StringBuilder.
            //
            StringBuilder builder = new StringBuilder();
            foreach (string value in array)
            {
                builder.Append(value);
                builder.Append('.');
            }
            return builder.ToString();
        }
    }
}
