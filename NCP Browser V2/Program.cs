using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            CefSharp.Cef.EnableHighDPISupport();
            Application.Run(new Salesforce(true, "https://test.salesforce.com"));
            //Application.Run(new Jabber.Credentials());
        }
    }
}
