using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.ServiceProcess;

namespace CloudflareDDNS
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void serviceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            try
            {
                using (var sc = new ServiceController
                    (serviceInstaller.ServiceName, Environment.MachineName))
                {
                    if (sc.Status != ServiceControllerStatus.Running)
                        sc.Start();
                }
            }
            catch (Exception ee)
            {
                EventLog.WriteEntry("Application", ee.ToString(), EventLogEntryType.Error);
            }
        }

        private void serviceInstaller_Committed(object sender, InstallEventArgs e)
        {
            try
            {
                using (var sc = new ServiceController(serviceInstaller.ServiceName))
                {
                    SetRecoveryOptions(sc.ServiceName);
                }
            }
            catch (Exception ee)
            {
                EventLog.WriteEntry("Application", ee.ToString(), EventLogEntryType.Error);
            }
        }

        private static void SetRecoveryOptions(string serviceName)
        {
            int exitCode;
            using (var process = new Process())
            {
                var startInfo = process.StartInfo;
                startInfo.FileName = "sc";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                // tell Windows that the service should restart if it fails
                startInfo.Arguments = $"failure \"{serviceName}\" reset= 0 actions= restart/60000";

                process.Start();
                process.WaitForExit();
                exitCode = process.ExitCode;
            }

            if (exitCode != 0)
                throw new InvalidOperationException();
        }
    }
}