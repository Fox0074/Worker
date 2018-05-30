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
    public partial class InputForm : Form
    {
        User[] users;
        public InputForm(User[] users)
        {
            InitializeComponent();
            this.users = users;
            textBox1.Text = users[0].userData.setting.Comp_name;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (users.Length > 0)
            {
                foreach (User user in users)
                {
                    user.UsersCom.SetCompName(textBox1.Text);
                }
            }
            this.Close();
        }
    }
}
