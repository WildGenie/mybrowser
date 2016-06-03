using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Globalization;
using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;

namespace NCP_Web
{
    
    class Program
    {
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
        private ApplicationContext ac;
        private bool intendedClose = false;
        private String VersionFolder;
        private String SalesforceInstance;

        public ProgramRunner(ApplicationContext ac)
        {
            try
            { 
                this.ac = ac;
                VersionFolder = "";
                SalesforceInstance = "";
                if (Directory.GetCurrentDirectory() != Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"NCP_Salesforce_APP") && !System.Diagnostics.Debugger.IsAttached)
                {
                    MessageBox.Show(String.Format("This application must reside in your Documents\\NCP_Salesforce_APP({0})",Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)));
                    this.ac.ExitThread();
                    Application.ExitThread();
                    Application.Exit();
                    Environment.Exit(1);
                }
                else
                {
                    UpdateAndLaunch();
                }           
                
            } catch (Exception e)
            {
                MessageBox.Show(e.Message + new System.Diagnostics.StackTrace(e).GetFrame(0).GetFileLineNumber());
                ac.ExitThread();
                Application.ExitThread();
                Application.Exit();
                Environment.Exit(1);
            }
        }

        private void UpdateAndLaunch()
        {
            // Load Variables from App.config
            try
            {
                LoadVariables(ref VersionFolder, ref SalesforceInstance);
            }
            catch (Exception e)
            {
                // We need to Load the root config
                if (File.Exists("NCP Web.exe.config"))
                {
                    FileSystem.DeleteFile("NCP Web.exe.config", UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }

                // Write the App.config
                File.WriteAllText("NCP Web.exe.config", File.ReadAllText(@"\\schear-fs\Software\browser\RootConfig.config"));
                System.Configuration.ConfigurationManager.RefreshSection("appSettings");
                LoadVariables(ref VersionFolder, ref SalesforceInstance);
            }

            // Validate Version Info
            if (!File.Exists("VersionInfo.txt"))
            {
                File.WriteAllText("VersionInfo.txt", "0");
            }

            // Check that we have the latest version
            String VersionInfo = File.ReadAllText("VersionInfo.txt");
            decimal VersionNumber = 0;
            decimal NewVersion = 0;
            string NewVersionFolder = "";
            Decimal.TryParse(VersionInfo, out VersionNumber);
            // We store a decimal number in a file and compare it to the folder name, if the folder name is a greater decimal than what is in the file it is a new version
            var VersionFolders = Directory.GetDirectories(VersionFolder);
            for (int i = 0; i < VersionFolders.Length; i++)
            {
                decimal FolderVersion = 0;
                Decimal.TryParse(new DirectoryInfo(VersionFolders[i]).Name, out FolderVersion);
                if (FolderVersion > VersionNumber)
                {
                    // New version, log the number and the folder
                    NewVersion = FolderVersion;
                    NewVersionFolder = VersionFolders[i];
                }
            }

            // Pull down new version if there is one
            if (NewVersion != 0)
            {
                DeleteFolders(Directory.GetCurrentDirectory());
                CopyFolder(NewVersionFolder, Directory.GetCurrentDirectory());
                File.WriteAllText("VersionInfo.txt", NewVersion.ToString());
            }

            // Load the browser
            LoadSalesforce();
        }

        private static void LoadVariables(ref String VersionFolder, ref String SalesforceInstance)
        {
            VersionFolder = System.Configuration.ConfigurationManager.AppSettings["VersionFolder"].ToString();
            SalesforceInstance = System.Configuration.ConfigurationManager.AppSettings["SalesforceInstance"].ToString();
        }

        /// <summary>
        /// Recursivley Delete Folders
        /// </summary>
        /// <param name="CurrentFolder">Folder Root</param>
        private static void DeleteFolders(String CurrentFolder)
        {
            Directory.GetFileSystemEntries(CurrentFolder).ToList().ForEach(x =>
            {
                if((File.GetAttributes(x) & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    DeleteFolders(x);
                }
                else
                { 
                    if (!x.EndsWith("NCP Web.exe"))
                    {
                        try
                        {
                            FileSystem.DeleteFile(x, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                        }
                        catch
                        {

                        }
                        
                        //
                    }
                }
            });
        }

        /// <summary>
        /// Recursively copy files
        /// </summary>
        /// <param name="NewVersionFolder">Root folder for new version</param>
        /// <param name="RootFolder">Root folder of application</param>
        private static void CopyFolder(string NewVersionFolder, String RootFolder)
        {
            if(!Directory.Exists(RootFolder))
            {
                Directory.CreateDirectory(RootFolder);
            }
            Directory.GetFileSystemEntries(NewVersionFolder).ToList().ForEach(x =>
            {
                if ((File.GetAttributes(x) & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    CopyFolder(x, Path.Combine(RootFolder, new DirectoryInfo(x).Name));
                }
                else
                { 
                    if (!x.EndsWith("NCP Web.exe"))
                    {
                        try
                        {
                            File.Copy(x, Path.Combine(RootFolder, new FileInfo(x).Name), true);
                        }
                        catch
                        {

                        }
                    }
                }
            });
        }

        Process salesforceProcess;
        private void LoadSalesforce()
        {
            salesforceProcess = new Process();
            salesforceProcess.EnableRaisingEvents = true;
            salesforceProcess.Exited += salesforceProcess_Exited;
            salesforceProcess.StartInfo = new ProcessStartInfo(Directory.GetCurrentDirectory() + "/V2/NCPBrowserV2.exe", String.Format("\"{0}\"", SalesforceInstance));
            salesforceProcess.Start();
        }

        private void salesforceProcess_Exited(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
