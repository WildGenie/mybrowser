using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NCP_CallRecording_Service
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void serviceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void serviceProcessInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void ProjectInstaller_Committed(object sender, InstallEventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = @"cmd.exe";
            p.StartInfo.Arguments = "/c sc failure \"NCP CallRecording Service\" reset= 0 actions= restart/60000/restart/60000/restart/60000";
            p.Start();
            p.WaitForExit();
        }
    }
}
