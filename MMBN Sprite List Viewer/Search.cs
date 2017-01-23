using System;
using System.Windows.Forms;

namespace MMBN_Sprite_List_Viewer
{
    public partial class Search : Form
    {
        public string returnvalue;
        public Search()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            returnvalue = textBox1.Text;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void Search_Load(object sender, EventArgs e)
        {
            textBox1.Select();
        }
    }
}
