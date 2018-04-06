﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        private UserCard.UserData userData;

        public FormUserCard(Messenger messenger)
        {
            InitializeComponent();
            this.messenger = messenger;
            userData = new UserCard.UserData();
            userData.id = messenger.key;
            LoadUserCard();
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

        private void button6_Click(object sender, EventArgs e)
        {
            if (userData.id != "")
                userData.SaveDataToFile(userData.id + ".xml");
        }
    }
}
