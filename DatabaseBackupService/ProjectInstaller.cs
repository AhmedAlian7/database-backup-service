using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace DatabaseBackupService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        private ServiceProcessInstaller processInstaller;
        private ServiceInstaller serviceInstaller;

        public ProjectInstaller()
        {
            InitializeComponent();

            // Process installer configures the service's account
            processInstaller = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem
            };

            // Service installer configures the service details
            serviceInstaller = new ServiceInstaller
            {
                ServiceName = "DatabaseBackupService",          // Must match the service class name
                DisplayName = "Database Backup Service",
                Description = "Automatically backs up a SQL Server database at specified intervals.",
                StartType = ServiceStartMode.Manual,
                ServicesDependedOn = new string[] { "EventLog", "RpcSs", "MSSQLSERVER" }
            };

            // Add both installers to the Installers collection
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
