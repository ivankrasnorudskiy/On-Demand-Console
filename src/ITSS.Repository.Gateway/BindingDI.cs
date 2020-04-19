using ITSS.Repository.Relay;
using ITSS.Repository.Relay.Interfaces;
using ITSS.Repository.SearchModule;
using Ninject.Modules;

namespace ITSS.Repository.Gateway
{
    public class BindingDI : NinjectModule
    {
        public override void Load()
        {
            var relayParam = RelayDependencies.GetRelayParamFromAppConfig();       
            Bind<IRelayListener>().To<RelayListener>().WithConstructorArgument("relayParam", relayParam);
            Bind<IListenerRequestHandler>().To<SearchByQueryHandler>().WithConstructorArgument("maxRecordsSearchCount", 1000);
            Bind<IRepositorySearchService>().To<SearchFromFileService>();
        }
    }
}
