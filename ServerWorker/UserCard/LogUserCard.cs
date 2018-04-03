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
    public partial class LogUserCard : Form
    {
        public LogUserCard()
        {
            InitializeComponent();
        }

        public void DrawNewLog(List<string> messages)
        {
            listBox1.Items.Clear();
            foreach (string str in messages)
            {
                listBox1.Items.Add(str);
            }
        }

        public void AddItem(string message)
        {
                listBox1.Items.Add(message);
        }
    }
}
