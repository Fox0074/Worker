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

namespace ServerWorker.Forms
{
    public partial class RunForm : Form
    {
        User user;
        string file;
        public RunForm(User user, string file)
        {
            InitializeComponent();
            this.user = user;
            this.file = file;
            label3.Text = file;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string runas = checkedListBox1.GetItemChecked(0) ? "runas" : "";
            string args = textBox1.Text;
            System.Diagnostics.ProcessWindowStyle style = checkedListBox1.GetItemChecked(1) ? System.Diagnostics.ProcessWindowStyle.Hidden : System.Diagnostics.ProcessWindowStyle.Normal;
            user.UsersCom.RunProgram(file, runas, args, style, checkedListBox1.GetItemChecked(2));
            Close();
        }
    }
}
