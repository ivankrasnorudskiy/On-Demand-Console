using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using ITSS.Repository.Relay.Interfaces;
using ITSS.Repository.ConsoleMVC.Logic.Interfaces;
using ITSS.Repository.ConsoleMVC.Models.ForSearch;

namespace ITSS.Repository.ConsoleMVC.Logic
{
    public class SearchService : ISearchService
    {
        private const string SearchResultUriTemplate = "/searches/{0}/results";

        private ConcurrentDictionary<string, SearchResult> _searchResultsDict;
        private ConcurrentDictionary<string, Search> _searches;
        private ISearchServiceFactory _searchServiceFactory;
        private IRelaySender _relaySender;

        public SearchService(IRelaySender sender, ISearchServiceFactory searchServiceFactory)
        {
            _relaySender = sender ?? throw new ArgumentNullException(nameof(sender));
            _searchServiceFactory = searchServiceFactory ?? throw new ArgumentNullException(nameof(searchServiceFactory));
            _searchResultsDict = _searchServiceFactory.GetSearchResultCacheDictionary();
            _searches = _searchServiceFactory.GetSearchCacheDictionary();
        }

        public async Task<List<Search>> GetAllSearchesAsync()
        {
           return await Task.Run(() => _searches.Values.ToList());
        }

        public async Task<SearchResult> GetSearchResultAsync(string searchId)
        {
            return await GetSearchResult(searchId);
        }

        public async Task DeleteSearchAsync(string searchId)
        {
            await DeleteSearch(searchId);
        }

        public async Task<Search> CreateSearchAsync(SearchParam param)
        {
            var id = Guid.NewGuid().ToString();
            var resultUri = string.Format(SearchResultUriTemplate, id);
            var search = new Search(id, resultUri, param);
 
            if (_searches.TryAdd(search.Id, search))
            {
                await Task.Factory.StartNew(() => StartSearch(search));
                return search;
            }

            throw new ArgumentException($"Search with same id {search.Id} and search params is already exists");
        }

        private async void StartSearch (Search search)
        {
            try
            {
                search.State = SearchState.Running;
                var response = await _relaySender.SendRequest(search.SearchParam.Query, new CancellationToken());
                var foundRecords = JsonConvert.DeserializeObject<List<Record>>(response);
                AddSearchResultsToDictionary(search.Id, foundRecords);
                search.State = SearchState.Finished;
            }
            catch (Exception)
            {
                search.State = SearchState.Failed;
            }
        }

        private void AddSearchResultsToDictionary(string searchId, List<Record> foundRecords)
        {
            _searchResultsDict[searchId] = new SearchResult
            {
                Metainfo = "Search is finished",
                NextResultUri = "",
                Records = foundRecords
            };
        }

        private Task<SearchResult> GetSearchResult(string searchId)
        {
            if (_searchResultsDict.TryGetValue(searchId, out var searchResult))
                return Task.FromResult(searchResult);

            throw new Exception("TODO: Results is not found exception");
        }

        private async Task DeleteSearch(string searchId)
        {
            if (!_searches.TryRemove(searchId, out _))
                throw new Exception("TODO: Search is not found exception");

            await DeleteSearchResults(searchId);
        }

        private Task DeleteSearchResults(string searchId)
        {
            if (_searchResultsDict.TryRemove(searchId, out _))
                return Task.CompletedTask;

            throw new Exception("TODO: Search results are not found exception");
        }
    }
}
