using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Honeywell.Forms
{
    public partial class FormInvokeFunction : Form
    {
        public new void ShowDialog()
        {
            this.Mode = 0;
            base.ShowDialog();
        }

        public FormInvokeFunction()
        {
            InitializeComponent();
            this.Mode = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Mode = 2;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Mode = 1;
            this.Close();

        }

     

        public int Mode = 0;

        private void FormInvokeFunction_Load(object sender, EventArgs e)
        {

        }
    }
}
