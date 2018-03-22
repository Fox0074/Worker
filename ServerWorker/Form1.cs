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

namespace ServerWorker
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Messenger.UpdateAll();
            Log.Send("Сервер: Обновить клиенты");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Console.WriteLine("Form1_FormClosing start");

                if (Program.server != null)
                    Program.server.StopServer();

                if (Program.aviableServer != null)
                    Program.aviableServer.Close();

                Program.serverThread.Abort();
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
            int numRb = 0;
            int merginX = 10;
            int merginY = 20;

            foreach (IPAddress ip in AvailableLocalIp.ListAviableIp)
            {
                RadioButton rb = new RadioButton();

                rb.Parent = groupBox1;
                rb.Location = new System.Drawing.Point(merginX, merginY * numRb + 20);
                rb.Name = "radioButton"+numRb;
                rb.AutoSize = true;
                rb.Text = ip.ToString();

                if (ip == ServerNet.localIp)
                {
                    rb.Checked = true;
                }

                rb.Click += (object send, EventArgs ea) => SetInternetIp(rb.Text);
                groupBox1.Controls.Add(rb);
                numRb++;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //num = listBox1.SelectedIndex;
            //Functions.onGettingLog += StartForm2;

            if ((listBox1.Items.Count > listBox1.SelectedIndex)&&(listBox1.SelectedIndex>=0))
            {
                if (Messenger.messangers.Count > listBox1.SelectedIndex)
                {
                    Messenger.messangers[listBox1.SelectedIndex].RequestLog();
                    StartForm2(Messenger.messangers[listBox1.SelectedIndex]);
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

        private void StartForm2(Messenger messenger )
        {
            Form2 form2 = new Form2(messenger);
            form2.Show();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void SetInternetIp(string ip)
        {
            if (Program.aviableServer.isWorking)
            {
                Program.aviableServer.Close();
                Program.serverThread.Abort();
            }

            if (ServerNet.localIp.ToString() != ip)
            {
                Log.Send("Изменение ip на " + ip);
                ServerNet.localIp = IPAddress.Parse(ip);

                Program.server.StopServer();
                Program.serverThread = new Thread(new ThreadStart(Program.server.StartServer));
                Program.serverThread.Start();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
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
