using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NCP_Browser
{
    public partial class QFund : BaseBaseForm
    {
        public QFund()
        {
            InitializeComponent();
            
        }

        private void QFund_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = false;
        }

        private void QFund_Load(object sender, EventArgs e)
        {

        }

        public string BrowserName { get; set; }

        private void RefreshButton_Click(object sender, EventArgs e)
        {

        }
    }
}
