using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;


namespace SessionTime.SessionTimeMonitor
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            this.AfterInstall += new InstallEventHandler(ProjectInstaller_AfterInstall);
        }

        void ProjectInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            using (ServiceController sc = new ServiceController(serviceInstaller1.ServiceName))
            {
                sc.Start();
            }
        }

        private void ServiceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void ServiceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
