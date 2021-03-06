﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using ITSS.Repository.Relay.Interfaces;
using ITSS.Repository.ConsoleMVC.Logic.Interfaces;
using ITSS.Repository.ConsoleMVC.Models.ForSearch;
using Microsoft.Extensions.Logging;

namespace ITSS.Repository.ConsoleMVC.Logic
{
    public class SearchService : ISearchService
    {
        private const string SearchResultUriTemplate = "/searches/{0}/results";
        private ConcurrentDictionary<string, SearchResult> _searchResultsDict;
        private ConcurrentDictionary<string, Search> _searches;
        private ISearchServiceFactory _searchServiceFactory;
        private IRelaySender _relaySender;
        private readonly ILogger _log;

        public SearchService(IRelaySender sender, ISearchServiceFactory searchServiceFactory, ILogger<SearchService> log)
        {
            _relaySender = sender ?? throw new ArgumentNullException(nameof(sender));
            _searchServiceFactory = searchServiceFactory ?? throw new ArgumentNullException(nameof(searchServiceFactory));
            _searchResultsDict = _searchServiceFactory.GetSearchResultCacheDictionary();
            _searches = _searchServiceFactory.GetSearchCacheDictionary();
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<Search> CreateSearchAsync(SearchParam param)
        {
            var id = Guid.NewGuid().ToString();
            var resultUri = string.Format(SearchResultUriTemplate, id);
            var search = new Search(id, resultUri, param);

            _log.LogDebug($"Try add search with id {search.Id} to search dictionary");
            if (_searches.TryAdd(search.Id, search))
            {
                await Task.Factory.StartNew(() => StartSearch(search));
                return search;
            }

            _log.LogError($"Search with same id {search.Id} and search params is already exists");
            throw new ArgumentException($"Search with same id {search.Id} and search params is already exists");
        }

        public List<Search> GetAllSearches()
        {
            return _searches.Values.ToList();
        }

        public SearchResult GetSearchResult(string searchId)
        {
            _log.LogDebug($"Try get search({searchId}) result ");
            if (_searchResultsDict.TryGetValue(searchId, out var searchResult))
                return searchResult;

            throw new ArgumentNullException($"Search with id {searchId} is not found in repository");
        }

        public void DeleteSearch(string searchId)
        {
            _log.LogDebug($"Try remove search with id {searchId}");
            if (!_searches.TryRemove(searchId, out _))
                throw new ArgumentNullException($"Search with id {searchId} is not found in repository");

            _searchResultsDict.TryRemove(searchId, out _);
        }

        private async void StartSearch (Search search)
        {
            try
            {
                _log.LogDebug($"Start search with id {search.Id}");
                search.State = SearchState.Running;
                var response = await _relaySender.SendRequest(search.SearchParam.Query, new CancellationToken());
                var foundRecords = JsonConvert.DeserializeObject<List<Record>>(response);
                AddSearchResultsToDictionary(search.Id, foundRecords);
                search.State = SearchState.Finished;
            }
            catch (Exception ex)
            {
                _log.LogError($"Set search state failed, error message: {ex.Message}");
                search.State = SearchState.Failed;
            }
        }

        private void AddSearchResultsToDictionary(string searchId, List<Record> foundRecords)
        {
            _log.LogDebug($"Try add search({searchId}) results to dictionary  ");
            _searchResultsDict[searchId] = new SearchResult
            {
                Metainfo = "Search is finished",
                NextResultUri = "",
                Records = foundRecords
            };
        }
    }
}
