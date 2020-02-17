using ServerWorker.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerWorker
{
    public partial class Download : Form
    {
        User user;
        string ftpDirectory = "C:\\inetpub\\wwwroot\\";
        string poofFtpDirectory = "Poof";
        public Download(User user)
        {
            this.user = user;
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (File.Exists(textBox1.Text))
            {
                if (textBox1.Text.Contains(ftpDirectory))
                {
                    textBox1.Text = textBox1.Text.Replace(ftpDirectory,"");
                }
                else
                {
                    FileInfo fileInfo = new FileInfo(textBox1.Text);
                    File.Copy(textBox1.Text, ftpDirectory + poofFtpDirectory + "\\" + fileInfo.Name,true);
                    textBox1.Text = poofFtpDirectory + "\\" + fileInfo.Name;
                }


                if (checkedListBox1.GetItemChecked(0))
                {
                    var x = textBox1.Text;
                    var y = textBox2.Text;
                    var z = textBox3.Text;
                    user.UsersCom.DownloadAndRun(textBox1.Text, textBox3.Text, checkedListBox1.GetItemChecked(2) ? "runas" : "", textBox2.Text,
                        checkedListBox1.GetItemChecked(1) ? System.Diagnostics.ProcessWindowStyle.Hidden : System.Diagnostics.ProcessWindowStyle.Normal, 
                        checkedListBox1.GetItemChecked(3));
                }
                else
                {
                    user.UsersCom.DownloadF(textBox1.Text, textBox3.Text);
                }
            }
            else
            {
                textBox1.Text = textBox1.Text.Replace(ftpDirectory, "");
                user.UsersCom.DownloadF(textBox1.Text, textBox3.Text);
            }
            Close();
        }
    }
}
