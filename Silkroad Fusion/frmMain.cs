using System;
using System.Linq;
using System.Windows.Forms;
using Silkroad_Fusion.API;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
namespace Silkroad_Fusion
{
    public partial class frmMain : Form
    {
        private Silkroad_Fusion.TextBoxStreamWriter novoConsole = null;
        public frmMain()
        {
            InitializeComponent();
        }

       
        public void AddChar(String charName)
        {
            CharName.Items.Add(CharName);
        }


        public static string GetID;
        public static string GetPW;





        Dictionary<string, uint> privSkills = new Dictionary<string, uint>();
        public void ClearSkills()
        {
            lVSkills.Items.Clear();
            privSkills.Clear();
        }
        public void AddSkill(string Name, byte Level, uint Id)
        {
            privSkills.Add(Name, Id);
            ListViewItem item = new ListViewItem();
            ListViewItem.ListViewSubItem subitem;
            item.Text = Name;
            item.Tag = item.Text = Name;
            subitem = new ListViewItem.ListViewSubItem();
            subitem.Text = Level.ToString();
            item.SubItems.Add(subitem);
            lVSkills.Items.Add(item);
        }
        public void AddRecordLog(string sText)
        {
            lBScript.Items.Add(sText);
        }
        protected void MyClosedHandler(object sender, EventArgs e)
        {
            Application.Exit();
        }
        public string CharacterNameSelect0
        {
            set { CharName.SelectedIndex = 0; }
        }
        public class ComboboxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }
        public string CharacterNameAdd
        {
            set
            {
                ComboboxItem item = new ComboboxItem();
                item.Text = value;
                item.Value = value;

                CharName.Items.Add(item);
            }
        }          
      
        public void LoadScript(string[] sText)
        {
            foreach (string sData in sText)
            {
                if (sData.StartsWith("walk"))
                {
                    string[] sElements = sData.Split(',');
                    int X = Convert.ToInt32(sElements[1]);
                    int Y = Convert.ToInt32(sElements[2]);
                    short Z = Convert.ToInt16(sElements[3]);
                    int Walking_Time = Movement.GetTime(Global.Player.Position.XPosition, Global.Player.Position.YPosition, X, Y);

                    Movement.WalkTo(X, Y, Z);
                    //                frmMain.Main.AddLog(string.Format("Walking to {0}/{1}!Time:{2}", X, Y, Walking_Time));
                    System.Threading.Thread.Sleep(Walking_Time);
                    lBScript.Items.Add(sData);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadScript(System.IO.File.ReadAllLines("Script\\Const.txt"));
        }


        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                Process[] proc = Process.GetProcessesByName("SRO_CLIENT");
                proc[0].Kill();
            }
            catch { }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Proxy.PerformLogin(ID.Text, PW.Text, passCode.Text, CharName.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Packet p = new Packet(0x7001);
            p.WriteAscii(CharName.Text);
            Proxy.SendAgent(p);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Packet p = new Packet(0x704F);
            p.WriteInt(2);
            p.WriteByte(4);
            Proxy.SendAgent(p);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            System.IO.File.WriteAllText(@"D:\WriteLines.txt", txtConsole.Text);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.FormClosed += MyClosedHandler;
            // Redirect the out Console stream
            this.novoConsole = new Silkroad_Fusion.TextBoxStreamWriter(txtConsole);
            Console.SetOut(this.novoConsole);
            Dump.TextData();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            long txtbox1;
            txtbox1 = Convert.ToInt32(textBox1.Text);
            txtbox1 = int.Parse(textBox1.Text);
            long txtbox2;
            txtbox2 = Convert.ToInt32(textBox2.Text);
            txtbox2 = int.Parse(textBox2.Text);
            short txtbox3;
            txtbox3 = Convert.ToInt16(textBox3.Text);
            txtbox3 = short.Parse(textBox3.Text);
            Movement.WalkTo(txtbox1, txtbox2, txtbox3);
        }





            
    }
}
