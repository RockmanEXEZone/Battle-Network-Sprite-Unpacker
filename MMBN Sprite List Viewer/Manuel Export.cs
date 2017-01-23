using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MMBN_Sprite_List_Viewer
{
    public partial class Manuel_Export : Form
    {
        Form1 f;
        Bitmap bmap;
        int ac,
            fc,
            w,
            h;
        public Manuel_Export(Form1 ff)
        {
            InitializeComponent();
            f = ff;
            update();
        }
        void update()
        {
            List<Bitmap> frames = new List<Bitmap>();
            byte[] source;
            int offset;
            if (f.romSpritePointersListbox.SelectedItem.ToString().Substring(0, 2) == "88")
            {
                source = Nintenlord.GBA.Compressions.LZ77.Decompress(f.ROM, (int.Parse((string)f.romSpritePointersListbox.SelectedItem, System.Globalization.NumberStyles.HexNumber) & 0x00FFFFFF));
                offset = 8;
            }
            else
            {
                offset = (int.Parse(f.romSpritePointersListbox.SelectedItem.ToString(), System.Globalization.NumberStyles.HexNumber) & 0x00FFFFFF) + 4;
                source = f.ROM;
            }
            List<int> breaks = new List<int>();
            int realframecount = 0;
            Rectangle r = new Rectangle((int)numericUpDown1.Value, (int)numericUpDown2.Value, (int)numericUpDown3.Value, (int)numericUpDown4.Value);
            ac = (f.getint32(source, offset) / 4);
            f.GetFrames(frames, source, offset, breaks,ref realframecount);
            bmap = f.ToAnimationStrip(frames, checkBox1.Checked, breaks,checkBox2.Checked);
            fc = realframecount;
            w = (int)numericUpDown3.Value;
            h = (int)numericUpDown4.Value;
            vScrollBar1.Maximum = bmap.Height;
            hScrollBar1.Maximum = bmap.Width;
            scrollupdate();
        }
        void scrollupdate()
        {
            pictureBox1.Image = bmap.Clone(new Rectangle(hScrollBar1.Value, vScrollBar1.Value, (bmap.Width - hScrollBar1.Value > pictureBox1.Width) ? (pictureBox1.Width) : (bmap.Width - hScrollBar1.Value), (bmap.Height- vScrollBar1.Value > pictureBox1.Height) ? (pictureBox1.Height) : (bmap.Height - vScrollBar1.Value)), bmap.PixelFormat);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            update();
            hScrollBar1.Value = 0;
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            scrollupdate();
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            scrollupdate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = " PNG IMAGE|*.png";
            if (sfd.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                string filename = sfd.FileName.Substring(0, sfd.FileName.Length - 4) + " FC[" + fc.ToString() + "] AC[" + ac.ToString() + "]" + " W[" + w.ToString() + "] H[" + h.ToString() + "] .png";
                bmap.Save(filename);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            update();
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            update();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                pictureBox1.BackColor = cd.Color;
            }
        }
    }
}
