using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NCP_Background
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private NCP_Browser.Salesforce Salesforce { get; set; }
        private NCP_Browser.Salesforce.ReloadMe reloader { get; set; }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Visible = false;
            this.ShowInTaskbar = false;

            reloader = new NCP_Browser.Salesforce.ReloadMe(ReloadSalesforce);

            LoadSalesforce();
        }

        private void LoadSalesforce()
        {
            //Salesforce = new NCP_Browser.Salesforce(reloader);
        }

        private void ReloadSalesforce()
        {
            LoadSalesforce();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {            
            if(e.CloseReason != CloseReason.WindowsShutDown)
                e.Cancel = true;
        }
    }
}
