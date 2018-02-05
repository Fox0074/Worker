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

namespace ServerWorker
{
    public partial class Form1 : Form
    {
        public static List<string> cash = new List<string>();

        public Form1()
        {
            InitializeComponent();
  
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.serverThread.Abort();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Messenger.UpdateAll();
        }

        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Program.server.StopServer();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Form1_FormClosing " + ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            WrileClientsInList();
        }

        public  void WrileClientsInList()
        {
            listBox1.Items.Clear();
            foreach (Messenger messang in Messenger.messangers)
            {
                try
                {
                    listBox1.Items.Add(messang.client.Client.RemoteEndPoint);
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
            Log.UpdateAction += Send;
        }

        private void Send (string str)
        {
            cash.Add(str);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            foreach (string str in cash)
            {
                listBox2.Items.Add(str);
            }
        }
    }
}
