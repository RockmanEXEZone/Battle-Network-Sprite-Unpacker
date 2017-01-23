using System;
using System.Windows.Forms;

namespace MMBN_Sprite_List_Viewer
{
    public partial class Replace_Settings : Form
    {

        public int returnoffset = 0;
        public bool compress = false;
        public bool hasheader = false;
        public Replace_Settings(int rombottom)
        {
            InitializeComponent();
            textBox1.Text = rombottom.ToString("X8");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int.Parse(textBox1.Text,System.Globalization.NumberStyles.HexNumber);
            }
            catch (Exception)
            {
                MessageBox.Show("Bad Offset", "Objection!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            returnoffset = int.Parse(textBox1.Text, System.Globalization.NumberStyles.HexNumber);
            hasheader = checkBox2.Checked;
            compress = checkBox1.Checked;
            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
