using System;
using System.Collections.Generic;
using System.Text;

namespace ITSS.Repository.Relay.Interfaces
{
    public interface IRelayListener
    {
        void Start(IListenerRequestHandler listenerRequestHandler);
        void Stop();
    }
}
