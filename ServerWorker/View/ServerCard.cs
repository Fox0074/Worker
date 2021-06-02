using Interfaces;
using ServerWorker.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;

namespace ServerWorker.ServerCard
{
    public partial class ServerCard : Form
    {
        private User _user;
        public ServerCard(User user)
        {
            _user = user;
            InitializeComponent();
            SetStateConection();
        }

        public ServerCard()
        {
            InitializeComponent();
            SetNonConectionState();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            var usersId = new List<string>();
            foreach (var arrayUser in ServerNet.ConnectedUsers.ToArray())
            {
                try
                {
                    usersId.Add(arrayUser.userData.id);
                }
                catch (Exception ex) { }
            }

            string externalip = new WebClient().DownloadString("http://icanhazip.com").Replace("\n", "");
            ServerNet.SendMessage(_user.nStream, new Unit("ConnectExcludeUsers", new object[] { usersId, externalip, (int)7777 }));
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            throw new Exception("Метод не реализован");
            //TODO: Исправить
            //ServerNet serv = new ServerNet();
            //_user = serv.ConnectionToServer(textBox1.Text);
            if (_user != null)
            {
                SetStateConection();
            }
            else
            {
                MessageBox.Show("Не удалось подключится к серверу");
            }
        }

        private void SetStateConection()
        {
            textBox1.Visible = false;
            button1.Visible = false;
            groupBox1.Visible = true;
            label1.Visible = true;
            label1.Text = _user.EndPoint.ToString();
        }

        private void SetNonConectionState()
        {
            textBox1.Visible = true;
            button1.Visible = true;
            groupBox1.Visible = false;
            label1.Visible = false;
        }
    }
}
