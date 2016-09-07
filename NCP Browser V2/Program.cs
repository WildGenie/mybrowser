using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using IWshRuntimeLibrary;

namespace NCP_Browser
{
    static class Program
    {
        private static string shortcutFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "Start TestApp.lnk");
        private static string publisherName = Application.CompanyName;
        private static string productName = "NCP Web";
        private static string allProgramsPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
        private static string shortcutPath = Path.Combine(allProgramsPath, publisherName);
        private static string shortcutFile = Path.Combine(shortcutPath, productName) + ".appref-ms";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Install();
            if (args.Contains("-L"))
            {
                Launch();
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                CefSharp.Cef.EnableHighDPISupport();
                //Application.Run(new NCP_Browser.Forms.SelectCase());
                Application.Run(new Salesforce(true, (String)System.Configuration.ConfigurationSettings.AppSettings["HOME_ADDRESS"]));
            }
            
            //Application.Run(new Jabber.Credentials());
        }

        public static void Launch()
        {
            System.Diagnostics.Process.Start(shortcutFile);
        }

        public static void Install()
        {
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutFileName);
            shortcut.Description = "NCP Browser Autostart Shortcut";
            shortcut.TargetPath = shortcutFile;
            shortcut.Save();
            shortcut.WorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            shortcut.Arguments = "-l";
            shortcut.Save();      
        }
    }
}
