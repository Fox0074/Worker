using ServerWorker.UserCard;
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
    public partial class LockForm : Form
    {
        public LockForm()
        {
            InitializeComponent();
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;

            this.KeyPreview = true;
            this.KeyUp += new KeyEventHandler((object e, KeyEventArgs s) =>Test(s));
            //this.button1.Click += new System.EventHandler(this.button2_Click);

                UserData userData = new UserData
                {
                    countStartProgram = 11,
                    dateFirstStart = "1May",
                    infoDevice = new List<string> { "Cpu , ", "Gpu=" },
                    startTime = "30",
                    name = "test",
                    version = "0.01b"
                };
                userData.SaveDataToFile("test.xml");

        }

        private bool TryAuthorization(string login, string pass)
        {
            return Program.authSystem.Authorization(login,pass);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (TryAuthorization(textBox1.Text, textBox2.Text))
            {              
                Close();
            }
        }

        private void LockForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!Program.authSystem.IsAuthorizate)
            {
                Environment.Exit(0);
            }
        }

        private void LockForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.Shift && e.KeyCode == Keys.R)
            {
                Program.authSystem.IsAuthorizate = true;
                Close();
            }
        }

        private void Test(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (TryAuthorization(textBox1.Text, textBox2.Text))
                {
                    Close();
                }
            }
        }
    }
}
