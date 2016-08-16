using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Silkroad_Fusion.API;
using Silkroad_Fusion;
namespace Silkroad_Fusion
{
    class Actions
    {
        public static void WriteMessage(string message)
        {
            try
            {
                Packet data = new Packet((ushort)Opcodes.Opcode.SERVER_CHAT, true);

                data.WriteByte((byte)Global.enumChatMode.Notice);
                data.WriteUInt16(Convert.ToUInt16(message.Length));
                data.WriteAscii(message);
                Client Client = new Silkroad_Fusion.Client();
                Client.Send(data);
            }
            catch { }
        }

    }
}
