using System;
using System.Linq;
using System.IO;
using System.Windows.Forms;

namespace Silkroad_Fusion
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var frmMain = new frmMain();
            frmMain.Show();

            //Always required even if using Clientless
            //rSRO = 212.24.57.34
            //iSRO = gwgt1.joymax.com = 121.128.133.28
            //For Clientless
            Proxy.Start(15778, "121.128.133.28");
            if (args.Length > 0)
            {
                if (args[0] == "-clientless")
                {
                    Proxy.PerformClientless();
                    Console.WriteLine("Clientless");
                }
            }
            Application.Run();

        }


    }
}