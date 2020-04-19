using ITSS.Repository.ConsoleMVC.Models.ForSearch;
using System.Collections.Concurrent;

namespace ITSS.Repository.ConsoleMVC.Logic.Interfaces
{
    public interface ISearchServiceFactory
    {
        ConcurrentDictionary<string, SearchResult> GetSearchResultCacheDictionary();
        ConcurrentDictionary<string, Search> GetSearchCacheDictionary();
    }
}
