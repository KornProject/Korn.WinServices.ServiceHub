using Korn.Installer.Core;
using Korn.Shared;
using Korn.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;

namespace Korn.AutorunService
{
    public partial class Service : ServiceBase
    {
        public Service() => InitializeComponent();

        const string KORN_DISABLE_AUTORUN_SERVICE_VAR = "KORN_DISABLE_AUTORUN_SERVICE";
        const string ServicePath = Korn.Interface.ServiceModule.Service.ExecutableFile;
        const string ServiceProcessName = Korn.Interface.ServiceModule.Service.ServiceProcessName;
        static TimeSpan Delay = TimeSpan.FromSeconds(3);

        protected override void OnStart(string[] args) => new Thread(Body).Start();

        void Body()
        {
            while (true)
            {
                if (!ProcessUtils.IsProcessExists(ServiceProcessName))
                {
                    if (!SystemVariablesUtils.IsVariableTrue(KORN_DISABLE_AUTORUN_SERVICE_VAR))
                    {
                        InstallerCore.CheckUpdates();
                        Process.Start(ServicePath);
                    }
                }

                Thread.Sleep(Delay);
            }
        }

        protected override void OnStop() { }
    }
}
