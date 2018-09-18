using ServerWorker.Server;
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
    public partial class ServerChatForm : Form
    {

        private User user;
        public ServerChatForm(User user)
        {
            InitializeComponent();
            this.user = user;
            user.OnSendChatMessage += AddMessage;
            Text = user.userData.setting.Comp_name + "      " + user.EndPoint.ToString();
        }


        public void Send(string text)
        {
            AddMessage(text);
            user.UsersCom.ReadMessage(text);
        }

        public void AddMessage(string text)
        {
            if (Program.form1.InvokeRequired) Program.form1.BeginInvoke(new Action(() => { listBox1.Items.Add(text); }));
            else listBox1.Items.Add(text);
        }


        private void ReadMessage()
        {
            string message = textBox1.Text;
            if (String.IsNullOrWhiteSpace(message)) return;
            message = DateTime.Now.ToString("HH:mm") + " Создатель: " + message;
            Send(message);
            textBox1.Text = "";
        }

        private void textBox1_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ReadMessage();
            }
        }

        private void ServerChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            user.UsersCom.StopChat();
        }
    }
}
