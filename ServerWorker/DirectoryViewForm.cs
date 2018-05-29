using ServerWorker.Server;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Interfaces;

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
            menu.Items[0].Click += DeleteFile;
            menu.Items.Add("Run");
            menu.Items[1].Click += Run;
            menu.Items.Add("RunHide");
            menu.Items[2].Click += RunHide;
            menu.Items.Add("Upload");
            menu.Items[3].Click += Upload;

            List<string> drivers = user.UsersCom.GetDrives();
            listView1.Items.Clear();
            foreach (string driver in drivers)
            {
                listView1.Items.Add(driver);
            }

            tree.Add("");
        }

        private void DeleteFile(object sender, EventArgs e)
        {
            user.UsersCom.DeleteFile(listView1.SelectedItems[0].Text.ToString());
            ReRequest();
        }
        private void Run(object sender, EventArgs e)
        {
            MessageBox.Show("Функця еще не реализована");
        }
        private void RunHide(object sender, EventArgs e)
        {
            user.UsersCom.RunHideProgram(listView1.SelectedItems[0].Text.ToString());
        }
        private void Upload(object sender, EventArgs e)
        {
            user.UsersCom.UploadDirectory(listView1.SelectedItems[0].Text.ToString(), "Upload");
        }

        private void ReRequest()
        {
            try
            {
                IDirectoryInfo directoryInfo = user.UsersCom.GetDirectoryFiles(currDirectory, "*");

                listView1.Items.Clear();
                foreach (string dir in directoryInfo.Directories)
                    listView1.Items.Add(dir);

                foreach (IFileInfo file in directoryInfo.FilesInfo)
                    listView1.Items.Add(file.fullName).SubItems.Add(GetSize(file.length));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Forward()
        {
            try
            {
                currDirectory = listView1.SelectedItems[0].Text + @"\";
                tree.Add(listView1.SelectedItems[0].Text + @"\");
                IDirectoryInfo directoryInfo = user.UsersCom.GetDirectoryFiles(currDirectory, "*");
                listView1.Items.Clear();

                foreach (string dir in directoryInfo.Directories)
                    listView1.Items.Add(dir);

                foreach (IFileInfo file in directoryInfo.FilesInfo)
                    listView1.Items.Add(file.fullName).SubItems.Add(GetSize(file.length));           

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
                    IDirectoryInfo directoryInfo = user.UsersCom.GetDirectoryFiles(currDirectory, "*");

                    listView1.Items.Clear();
                    foreach (string dir in directoryInfo.Directories)
                        listView1.Items.Add(dir);

                    foreach (IFileInfo file in directoryInfo.FilesInfo)
                        listView1.Items.Add(file.fullName).SubItems.Add(GetSize(file.length));
                }
                else
                {
                    currDirectory = tree[0];
                    listView1.Items.Clear();
                    foreach (string driver in user.UsersCom.GetDrives().ToArray())
                        listView1.Items.Add(driver);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private string GetSize(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0)
            {
                return "0" + suf[0];
            }
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Forward();
            }
            if (e.KeyCode == Keys.Back)
            {
                Back();
            }
        }

        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                menu.Show(listView1, e.X, e.Y);
            }
        }
    }
}
