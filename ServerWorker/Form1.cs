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

namespace ServerWorker
{
    public enum UserInterface { mainServer, secondServer }
    public partial class Form1 : Form
    {
        public delegate void InterfaceEventHandler(UserInterface arg);
        public event InterfaceEventHandler InterfaceChanged;

        private List<Control> mainControls = new List<Control>();
        private List<Control> secondControls = new List<Control>();

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
            Program.server.Events.OnConnected += UpdateClientList;
            Program.server.Events.OnDisconnect += UpdateClientList;
            Program.server.Events.OnAuthorized += UpdateClientList;


            mainControls.Add(button1);
            secondControls.Add(button8);
        }

        private void UpdateClientList()
        {
            if (listBox1.InvokeRequired) listBox1.BeginInvoke(new Action(() =>
            {
                listBox1.Items.Clear();
                foreach (User user in ServerNet.ConnectedUsers.ToArray())
                {
                    listBox1.Items.Add(user.UserType + ", ip: " + user.EndPoint);
                }
            }));
            else
            {
                listBox1.Items.Clear();
                foreach (User user in ServerNet.ConnectedUsers.ToArray())
                {
                    listBox1.Items.Add(user.UserType + ", ip: " + user.EndPoint);
                }
            }
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

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (User user in ServerNet.ConnectedUsers.ToArray().Where(x => x.UserType == UserType.User))
                user.UsersCom.DownloadUpdate();
            Log.Send("Сервер: Обновить клиенты");
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

        private void button4_Click(object sender, EventArgs e)
        {
            WrileClientsInList();
        }

        public  void WrileClientsInList()
        {
            listBox1.Items.Clear();
            foreach (User user in ServerNet.ConnectedUsers.ToArray())
            {
                try
                {
                    listBox1.Items.Add(user.UserType + ", ip: " + user.EndPoint);
                }
                catch (Exception ex)
                {
                    Log.Send("Send " + ex.Message);
                }
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {          
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int num = listBox1.SelectedIndex;
            //Functions.onGettingLog += StartForm2;

            if ((listBox1.Items.Count > listBox1.SelectedIndex)&&(listBox1.SelectedIndex>=0))
            {
                if (ServerNet.ConnectedUsers.ToArray().Count() > listBox1.SelectedIndex)
                {
                    StartForm2(ServerNet.ConnectedUsers.ToArray()[num]);
                }
                else
                {
                    Log.Send("Ошибка получения доступу к элементу, такого элемента не существует или он имеет другой индекс");
                }
            }
            else
            {
                Log.Send("Ошибка, индекс элемента не попадает в допустимый диапазон");
            }
        }

        private void StartForm2(User user )
        {
            FormUserCard userCard = new FormUserCard(user);
            userCard.Show();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }



        private void button7_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Program.aviableServer.SendMessage("GetListUsers");         
        }
    }
}
