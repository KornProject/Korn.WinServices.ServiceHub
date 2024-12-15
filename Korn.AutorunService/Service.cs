using Korn.Utils.System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;

namespace Korn.AutorunService
{
    public partial class Service : ServiceBase
    {
        public Service()
        {
            InitializeComponent();
        }

        const string KORN_PATH_VAR_NAME = "KORN_PATH";

        protected override void OnStart(string[] args)
        {
            var kornPath = SystemVariablesUtils.GetVariable(KORN_PATH_VAR_NAME);
            var servicePath = Path.Combine(kornPath, "Service\\bin\\Korn.Service.exe");

            while (true)
            {
                if (Process.GetProcessesByName("NoKornAutorunSign").Length == 0)
                    if (Process.GetProcessesByName("Korn.Service").Length == 0)
                        Process.Start(servicePath);

                Thread.Sleep(5000);
            }
        }

        protected override void OnStop()
        {

        }
    }
}
