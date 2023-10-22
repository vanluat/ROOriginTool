using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetsUpdate
{
    public partial class InputDialog : Form
    {
        public int Value { get; private set; }
        public InputDialog()
        {
            InitializeComponent();
        }

        private void uiCancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void uiOkButton_Click(object sender, EventArgs e)
        {
            DialogResult=DialogResult.OK;
            Value=(int)uiInputValua.Value;
            Close();
        }
    }
}
