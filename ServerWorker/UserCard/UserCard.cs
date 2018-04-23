using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Interfaces;
using ServerWorker.Server;

namespace ServerWorker
{
    public partial class FormUserCard : Form
    {

        private User user;
        private LogUserCard log;
        private UserCard.UserData userData;

        public FormUserCard(User user)
        {
            InitializeComponent();
            this.user = user;
            userData = new UserCard.UserData();
            LoadUserCard();
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            log = new LogUserCard();
            log.Show();
            log.Text = "Ожидайте, идет получение лога..";
            DrawLog(user.UsersCom.GetLog());
        }

        private void DrawLog(List<string> events)
        {
            try
            {
                if (log.InvokeRequired) Program.form1.BeginInvoke(new Action(() => { log.DrawNewLog(events); }));
                else log.DrawNewLog(events);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DrawInfoDevice(user.UsersCom.GetInfoDevice());
        }

        private void DrawInfoDevice(List<string> _params)
        {
            if (listBox2.InvokeRequired) listBox2.BeginInvoke(new Action(() => { listBox2.Items.Clear(); }));
            else listBox2.Items.Clear();
            userData.infoDevice = _params;
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

        private void LoadUserCard()
        {
            try
            {
                string[] dirs = Directory.GetFiles(UserCard.UserData.parthUserCard, "*.xml");
                foreach (string file in dirs)
                {
                    if (file == UserCard.UserData.parthUserCard + @"\"+ userData.id + ".xml")
                    {
                        userData = userData.RearDataFromFile(file);
                        break;
                    }
                }

                foreach (string str in userData.infoDevice)
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
            catch (Exception e)
            {
                Log.Send("The process failed: " + e.ToString());
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            user.UsersCom.DownloadFloader("Miner", "Data");
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
            user.UsersCom.RunProgram(@"Data\Miner.exe");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            user.UsersCom.DownloadUpdate();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (userData.id != "")
                userData.SaveDataToFile(userData.id + ".xml");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            user.UsersCom.Reconnect();
        }
    }
}
