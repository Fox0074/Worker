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
    public partial class FormUserCard : Form
    {

        private Messenger messenger;
        private LogUserCard log;

        public FormUserCard(Messenger messenger)
        {
            InitializeComponent();
            this.messenger = messenger;
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            log = new LogUserCard();
            log.Show();
            log.Text = "Ожидайте, идет получение лога..";
            Functions.onGettingLog +=  DrawLog;
            messenger.RequestLog();
        }

        private void DrawLog()
        {
            try
            {
                Functions.onGettingLog -= DrawLog;
                if (log.InvokeRequired) Program.form1.BeginInvoke(new Action(() => { log.DrawNewLog(messenger.clientLog.messages); }));
                else log.DrawNewLog(messenger.clientLog.messages);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Functions.OnGettingInfoDevice += () => DrawInfoDevice(messenger.setting.infoDevice);
            string message = "GetInfoDevice";
            messenger.SendMessage(message);
        }

        private void DrawInfoDevice(List<string> _params)
        {
            foreach (string str in _params)
            {
                try
                {
                    if (listBox2.InvokeRequired) listBox2.BeginInvoke(new Action(() => { listBox2.Items.Add(str); }));
                    else listBox2.Items.Add(str);
                }
                catch
                { }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string message = "DownloadFloader" + "_"+"Miner" + "_" + "Data";
            messenger.SendMessage(message);
        }

        //private void button4_Click(object sender, EventArgs e)
        //{
        //    messenger.Update();
        //}

        //private void button5_Click(object sender, EventArgs e)
        //{
        //    string message = "RunProgram" + "_" + @"Data\Miner.exe";
        //    messenger.SendMessage(message);
        //}

        private void button4_Click_1(object sender, EventArgs e)
        {
            string message = "RunProgram" + "_" + @"Data\Miner.exe";
            messenger.SendMessage(message);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            messenger.Update();
        }
    }
}
