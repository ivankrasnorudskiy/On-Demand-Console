using System;
using System.Collections.Generic;
using System.Text;

namespace ITSS.Repository.Relay.Interfaces
{
    public interface IListenerRequestHandler
    {
        string Handle(string listenerRequest);
    }
}
