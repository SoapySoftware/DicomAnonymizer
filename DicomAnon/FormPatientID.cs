using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DicomAnon
{
    public partial class FormPatientID : Form
    {
        public string strType;

        public FormPatientID()
        {
            InitializeComponent();
            strType = "none";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            strType = "existing filenames";
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            strType = "sequential numbering";
            this.DialogResult = DialogResult.OK;
        }
    }
}
