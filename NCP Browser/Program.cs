using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace NCP_Browser
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationContext ac = new ApplicationContext();
            program = new ProgramRunner(ac);            
            Application.Run(ac);
        }


        private static ProgramRunner program;

    }

    class ProgramRunner
    {
        private Salesforce.ReloadMe ReloadMe;
        private Salesforce.CloseMe CloseMe;
        private Salesforce Salesforce;
        private ApplicationContext ac;
        private bool intendedClose = false;

        public ProgramRunner(ApplicationContext ac)
        {
            this.ac = ac;
            
            LoadSalesforce(true);
        }

        private void LoadSalesforce(bool Initialize)
        {
            ReloadMe = new NCP_Browser.Salesforce.ReloadMe(ReloadSalesforce);
            CloseMe = new NCP_Browser.Salesforce.CloseMe(SalesforceClosing);
            Salesforce = new NCP_Browser.Salesforce(ReloadMe, CloseMe, Initialize, "test.salesforce.com");
            Salesforce.Show();
        }

        private void ReloadSalesforce()
        {
            this.intendedClose = true;
            Salesforce.Close();
            LoadSalesforce(false);            
        }

        private void SalesforceClosing(FormClosingEventArgs e)
        {
            if(!this.intendedClose)
            {
                var res = MessageBox.Show("Are you sure you want to close?", "Are you sure you want to close?", MessageBoxButtons.YesNo);
                if(res == DialogResult.Yes)
                {
                    ac.ExitThread();
                    Application.Exit();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        public Salesforce getSalesforceReference()
        {
            return this.Salesforce;
        }
    }
}
