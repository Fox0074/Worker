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
    public partial class DirectoryViewForm : Form
    {
        private ContextMenuStrip menu;
        private string currDirectory = "";
        User user;
        List<string> tree = new List<string>();
        public DirectoryViewForm(User user)
        {
            this.user = user;
            InitializeComponent();    
            menu = new ContextMenuStrip(this.components);
            menu.Items.Add("Delete");
            menu.Items.Add("Run");
            menu.Items.Add("RunHide");
            menu.Items.Add("Download");
            menu.Click += Test;

            List<string> drivers = user.UsersCom.GetDrives();
            listBox1.Items.Clear();
            foreach (string driver in drivers)
            {
                listBox1.Items.Add(driver);
            }

            tree.Add("");
        }

        private void listBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {                              
                menu.Show(listBox1, e.X, e.Y);
            }
        }

        private void Test(object sender, EventArgs e)
        {
            MessageBox.Show("olololo");
        }
        private void listBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                Forward();
            }
            if (e.KeyCode == Keys.Back)
            {
                Back();
            }
        }

        private void Forward()
        {
            try
            {
                currDirectory = listBox1.SelectedItem + @"\";
                tree.Add(listBox1.SelectedItem + @"\");
                string[] dirs = user.UsersCom.GetDirectoryFiles(currDirectory, "*");
                listBox1.Items.Clear();
                listBox1.Items.AddRange(dirs);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                tree.RemoveAt(tree.Count-1);
            }
        }
        private void Back()
        {
            try
            {                
                tree.RemoveAt(tree.Count - 1);
                if (tree.Count > 1 )
                {
                    currDirectory = tree.Last();
                    string[] dirs = user.UsersCom.GetDirectoryFiles(currDirectory, "*");
                    listBox1.Items.Clear();
                    listBox1.Items.AddRange(dirs);
                }
                else
                {
                    currDirectory = tree[0];
                    listBox1.Items.Clear();
                    listBox1.Items.AddRange(user.UsersCom.GetDrives().ToArray());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
