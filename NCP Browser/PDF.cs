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
    public partial class PDF : BaseForm
    {
        public PDF(String File)
        {            
            InitializeComponent();
            this.axAcroPDF.LoadFile(File);
            this.axAcroPDF.setView("FitH");
            this.axAcroPDF.setShowToolbar(false);
            this.Text = String.Format("PDF - {0}", File);
        }

        private void PDF_Load(object sender, EventArgs e)
        {

        }
    }
}
