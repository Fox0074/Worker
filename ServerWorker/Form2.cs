using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerWorker
{
    public partial class Form2 : Form
    {
        Messenger messenger;
        public Form2(Messenger test)
        {
            InitializeComponent();
            messenger = test;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (string str in messenger.clientLog.messages)
            {
                //try
                //{
                //    if (listBox1.InvokeRequired) listBox1.BeginInvoke(new Action(() => { listBox1.Items.Add(str); }));
                //    else listBox1.Items.Add(str);
                //}
                //catch
                //{ }
                listBox1.Items.Add(str);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Functions.OnGettingInfoDevice += () => Test(messenger.setting.infoDevice);
            byte[] data = Encoding.Unicode.GetBytes("GetInfoDevice");
            messenger.stream.Write(data, 0, data.Length);
        }

        private void Test(List<string> test)
        {
            foreach (string str in test)
            {
                //listBox2.Items.Add(str);

                try
                {
                    if (listBox2.InvokeRequired) listBox2.BeginInvoke(new Action(() => { listBox2.Items.Add(str); }));
                    else listBox2.Items.Add(str);
                }
                catch
                { }
            }
        }
    }
}
