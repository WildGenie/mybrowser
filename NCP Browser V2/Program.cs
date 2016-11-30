using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using IWshRuntimeLibrary;
using System.Runtime.InteropServices;

namespace NCP_Browser
{
    static class Program
    {
        /*
        [DllImport("win32tapi.dll", CallingConvention = CallingConvention.StdCall)]
        public static  extern IntPtr win32tapi_new();

        [DllImport("win32tapi.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static  extern void win32tapi_set(IntPtr refer, string blah);

        [DllImport("win32tapi.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr win32tapi_get(IntPtr refer);
        */

        private static string shortcutFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "Start TestApp.lnk");
        private static string publisherName = Application.CompanyName;
        private static string productName = "NCP Web";
        private static string allProgramsPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
        private static string shortcutPath = Path.Combine(allProgramsPath, publisherName);
        private static string shortcutFile = Path.Combine(shortcutPath, productName) + ".appref-ms";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static string PtrToStringUtf8(IntPtr ptr) // aPtr is nul-terminated
        {
            return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ptr);
            /*if (ptr == IntPtr.Zero)
                return "";
            int len = 0;
            while (System.Runtime.InteropServices.Marshal.ReadByte(ptr, len) != 0)
                len++;
            if (len == 0)
                return "";
            byte[] array = new byte[len];
            System.Runtime.InteropServices.Marshal.Copy(ptr, array, 0, len);
            return System.Text.Encoding.ASCII.GetString(array);*/
        }
        
        [STAThread]
        static void Main(string[] args)
        {
            /*IntPtr blah = win32tapi_new();
            win32tapi_set(blah, "BLOOOP");
            var peeteerrrr = win32tapi_get(blah);
            MessageBox.Show(PtrToStringUtf8(peeteerrrr));*/
           try
           {
                if (Directory.Exists("cache"))
                {
                    Directory.Delete("cache", true);
                }
            }
            catch
            {

            }

            try
            {
                if (Directory.Exists("cookies"))
                {
                    Directory.Delete("cookies", true);
                }
            }
            catch
            {

            }       
            
            try
            {
                //Install();
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
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                MessageBox.Show(e.StackTrace);
            }
            //*/
            
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
