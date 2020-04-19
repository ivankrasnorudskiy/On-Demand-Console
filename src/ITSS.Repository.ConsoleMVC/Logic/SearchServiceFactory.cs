using ITSS.Repository.ConsoleMVC.Logic.Interfaces;
using ITSS.Repository.ConsoleMVC.Models.ForSearch;
using System.Collections.Concurrent;

namespace ITSS.Repository.ConsoleMVC.Logic
{
    public class SearchServiceFactory : ISearchServiceFactory
    {
        public ConcurrentDictionary<string, SearchResult> GetSearchResultCacheDictionary()
        {
            return new ConcurrentDictionary<string, SearchResult>();        
        }

        public ConcurrentDictionary<string, Search> GetSearchCacheDictionary()
        {
            return new ConcurrentDictionary<string, Search>();
        }
    }
}
