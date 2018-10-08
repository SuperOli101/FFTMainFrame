using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Honeywell.Forms
{
    public partial class CFormInstalRessource : Form
    {
        public CFormInstalRessource()
        {
            InitializeComponent();
        }

        private void progressBarDeviceInstal_Click(object sender, EventArgs e)
        {

        }

        public void WriteToLabel(string Msg)
        {
            this.LabelMsg.Text = Msg;
        }
    }
}