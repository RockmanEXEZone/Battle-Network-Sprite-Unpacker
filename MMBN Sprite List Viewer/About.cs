using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace MMBN_Sprite_List_Viewer
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }
        
        private void label1_DoubleClick(object sender, EventArgs e)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerAsync();
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Random r = new Random();
            for (int i = 0; i < 500; i++)
            {
                this.BackColor = Color.FromArgb((int)((r.Next() &0x00FFFFFF) + 0xFF000000));
                Thread.Sleep(10);
            }
        }
    }
}
