using System;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;
using CloudflareDDNS.Logs;

namespace CloudflareDDNS
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            ILog log = new Log(@"CloudflareDDNS");

            try
            {
                if (Environment.UserInteractive)
                {
                    ManagedInstallerClass.InstallHelper(new[] {Assembly.GetExecutingAssembly().Location});
                }
                else
                {
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[]
                    {
                        new CloudflareDDNSService(log)
                    };

                    ServiceBase.Run(ServicesToRun);
                }
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
            }
        }
    }
}