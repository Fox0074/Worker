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
    public partial class WorkerForm : Form
    {
        public WorkerForm(Bitmap BM)
        {
            InitializeComponent();
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Image = BM;
        }

        private void WorkerForm_Load(object sender, EventArgs e)
        {

        }
    }
}
