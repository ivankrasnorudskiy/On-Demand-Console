using System;
using System.IO;
using System.Diagnostics;
using System.ServiceProcess;
using Ninject;
using System.Reflection;
using ITSS.Repository.Relay.Interfaces;

namespace ITSS.Repository.Gateway
{
    class Program
    {
        static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            var relayListener = kernel.Get<IRelayListener>();
            var listenerRequestHandler = kernel.Get<IListenerRequestHandler>();

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            if (Environment.UserInteractive)
            {
                var service = new HostWindowsService(relayListener, listenerRequestHandler);
                service.Start();

                Process.GetCurrentProcess().WaitForExit();
            }
            else
            {
                var servicesToRun = new ServiceBase[]
                {
                    new HostWindowsService(relayListener, listenerRequestHandler)
                };

                ServiceBase.Run(servicesToRun);
            }
        }
    }
}
