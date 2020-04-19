using ITSS.Repository.Relay.Interfaces;
using ITSS.Repository.SearchModule;
using Newtonsoft.Json;
using NLog;

namespace ITSS.Repository.Gateway
{
    public class SearchByQueryHandler : IListenerRequestHandler
    {
        private readonly IRepositorySearchService _repositoryService;
        private readonly int _maxRecordsSearchCount;
        private readonly ILogger _log = LogManager.GetLogger(typeof(HostWindowsService).FullName);

        public SearchByQueryHandler(IRepositorySearchService repositoryService, int maxRecordsSearchCount)
        {
            _repositoryService = repositoryService;
            _maxRecordsSearchCount = maxRecordsSearchCount;
        }
        public string Handle(string listenerRequest)
        {
            _log.Debug("Starting a new search by query '{0}'", listenerRequest);
            var records = _repositoryService.Search(listenerRequest, _maxRecordsSearchCount);
            var json = JsonConvert.SerializeObject(records);
            return json;
        }
    }
}
