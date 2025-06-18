using System.ServiceProcess;
using Korn.Installer.Core;
using Korn.Interface;
using Korn.Utils;
using System.Threading;
using System;
using Microsoft.Win32;

namespace Korn.AutorunService
{
    public partial class Service : ServiceBase
    {
        public Service() => InitializeComponent();

        static ProcessableService[] Services = new ProcessableService[]
        {
            new ProcessableService(InjectorService.ServiceEntry),
            new ProcessableService(LoggerService.ServiceEntry)
        };

        protected override void OnStart(string[] args)
        {
            InstallerCore.CheckUpdates();

            foreach (var service in Services)
                foreach (var process in Process.Processes.GetProcessesByName(service.Entry.ProcessName))
                    process.Kill();

            foreach (var service in Services)
                service.StartProcess();

            new Thread(UpdaterThreadBody).Start();
        }

        void UpdaterThreadBody()
        {
            RegistryKey registryKey = null;
            try
            {
                registryKey = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Session Manager\\Environment", writable: false);
            } catch { }

            while (true)
            {
                Thread.Sleep(10000);

                if (registryKey != null)
                {
                    var registryValue = registryKey.GetValue("KORN_DISABLE_AUTORUN_SERVICE") as string;
                    if (registryValue != null)
                        if (registryValue.Equals("true", StringComparison.OrdinalIgnoreCase))
                            continue;
                }                

                foreach (var service in Services)
                    if (!Process.Processes.IsProcessExists(service.Entry.ProcessName))
                        service.StartProcess();
            }
        }
    }

    class ProcessableService
    {
        public ProcessableService(ServiceEntry entry) => Entry = entry;

        public readonly ServiceEntry Entry;
        public int CurrentProcessID;

        public void StartProcess()
        {
            var process = System.Diagnostics.Process.Start(Entry.ExecutableFilePath);
            CurrentProcessID = process.Id;
        }
    }
}
