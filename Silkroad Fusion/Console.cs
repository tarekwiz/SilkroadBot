using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Silkroad_Fusion
{
    public class TextBoxStreamWriter : TextWriter
    {
        TextBox txbOutput = null;
        private readonly object writerLock = new object();

        public TextBoxStreamWriter(TextBox output)
        {
            this.txbOutput = output;
        }

        public override void Write(char value)
        {
            //base.Write(value);
            if (this.txbOutput.InvokeRequired)
            {
                Action<TextBox, string> action = new Action<TextBox, string>((txb, str) =>
                {
                    txb.AppendText(str);
                });
                this.txbOutput.Invoke(action, new object[] { this.txbOutput, value.ToString() });

            }
            else
                this.txbOutput.AppendText(value.ToString()); // When character data is written, append it to the text box.
        }

        public override void WriteLine(string str)
        {
            //base.Write(str);
            //lock (this.writerLock)
            //{
            str += "\r\n";

            if (this.txbOutput.InvokeRequired)
            {
                Action<TextBox, string> action = new Action<TextBox, string>((txb1, str1) =>
                {
                    txb1.AppendText(str1);
                });
                this.txbOutput.Invoke(action, new object[] { this.txbOutput, str });

            }
            else
                this.txbOutput.AppendText(str); // When character data is written, append it to the text box.
            //}
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}
