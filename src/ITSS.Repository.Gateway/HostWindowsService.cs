using System;
using System.ServiceProcess;
using ITSS.Repository.Relay.Interfaces;
using NLog;

namespace ITSS.Repository.Gateway
{
    partial class HostWindowsService : ServiceBase
    {
        private readonly IRelayListener _relayListener;
        private readonly ILogger _log = LogManager.GetLogger(typeof(HostWindowsService).FullName);
        private readonly IListenerRequestHandler _listenerRequestHandler;
        public HostWindowsService(IRelayListener relayListener, IListenerRequestHandler listenerRequestHandler)
        {
            InitializeComponent();
            _relayListener = relayListener ?? throw new ArgumentNullException(nameof(relayListener));
            _listenerRequestHandler = listenerRequestHandler ?? throw new ArgumentNullException(nameof(listenerRequestHandler));
        }

        public void Start()
        {
            OnStart(new string[0]);
        }

        protected override void OnStart(string[] args)
        {
            _log.Debug("Starting host Windows service..");
            _relayListener.Start(_listenerRequestHandler);
        }

        protected override void OnStop()
        {
            _log.Debug("Stopping host Windows service..");
            _relayListener.Stop();
        }
    }
}
