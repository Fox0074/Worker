using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClientWorker;
using Interfaces;

namespace Service
{
    public partial class ChatForm : Form
    {
        public static ChatForm current;
        public static bool isWorking = false;
        private Client client;

        public ChatForm(Client client)
        {          
            InitializeComponent();
            this.client = client;
            current = this;
            isWorking = true;
            Log.Send("Chat.isWorking = true");
        }

        public new void Show()
        {
            base.Show();
            Log.Send("Chat.Show()");
        }

        public void Send(string text)
        {
            AddMessage(text);
            Unit MUint = new Unit("ChatMessage", new object[] { text });
            client.SendData(MUint);
            Log.Send("Chat.Send(" + text + ")");
        }

        public void AddMessage(string text)
        {
            listBox1.Items.Add(text);
        }

        public new void  Close()
        {
            isWorking = false;
            base.Close();
            Log.Send("Chat.isWorking = false");
        }

        private void ReadMessage()
        {
            string message = textBox1.Text;
            if (String.IsNullOrWhiteSpace(message)) return;
            message = DateTime.Now.ToString("HH:mm") + " Я: " + message;
            Send(message);
            textBox1.Text = "";
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ReadMessage();
            }
        }
    }
}
