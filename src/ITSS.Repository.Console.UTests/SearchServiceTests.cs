using ITSS.Repository.ConsoleMVC.Logic;
using ITSS.Repository.ConsoleMVC.Logic.Interfaces;
using ITSS.Repository.ConsoleMVC.Models.ForSearch;
using ITSS.Repository.Relay.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualBasic;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Record = ITSS.Repository.ConsoleMVC.Models.ForSearch.Record;

namespace ITSS.Repository.Console.UTests
{
    public class SearchServiceTests
    {
        private Mock<IRelaySender> _relaySenderMock;
        private Mock<ISearchServiceFactory> _searchServiceFactoryMock;
        private SearchService _searchService;
        public SearchServiceTests()
        {
            _relaySenderMock = new Mock<IRelaySender>();
            _searchServiceFactoryMock = new Mock<ISearchServiceFactory>();
        }

        [Fact]
        public async void TestCreateSearchAndGetSearchResults()
        {
            var searchDict = new ConcurrentDictionary<string, Search>();
            var searchResultsDict = new ConcurrentDictionary<string, SearchResult>();

            var testRecordsCollection = GetTestRecordsCollection();
            var searchParam = GetTestSearchParam();
            var search = await CreateSearchWithSearchResults(searchDict, searchResultsDict, searchParam);

            Assert.True(search.Equals(searchDict.FirstOrDefault().Value));
            Assert.Single(searchDict);
            Assert.Single(searchResultsDict);

            var searchId = searchDict.FirstOrDefault().Key ?? throw new ArgumentException();
            var correctUri = string.Format("/searches/{0}/results", searchId);
            Assert.Equal(correctUri, searchDict.FirstOrDefault().Value.ResultUri);
            Assert.True(searchParam.Equals(searchDict.FirstOrDefault().Value.SearchParam));
            Assert.Equal(SearchState.Finished, searchDict.FirstOrDefault().Value.State);

            Assert.Equal("Search is finished", searchResultsDict.FirstOrDefault().Value.Metainfo);
            Assert.Empty(searchResultsDict.FirstOrDefault().Value.NextResultUri);

            Assert.True(testRecordsCollection[0].Equals(searchResultsDict.FirstOrDefault().Value.Records[0]));
            Assert.True(testRecordsCollection[1].Equals(searchResultsDict.FirstOrDefault().Value.Records[1]));           
        }

        [Fact]
        public async void TestStartSearchAndGetException()
        {      
            var searchDict = new ConcurrentDictionary<string, Search>();
            _searchServiceFactoryMock.Setup(x => x.GetSearchCacheDictionary()).Returns(searchDict);
            _relaySenderMock.Setup(x => x.SendRequest(It.IsAny<string>(), It.IsAny<CancellationToken>())).Throws(new Exception());
            _searchService = new SearchService(_relaySenderMock.Object, _searchServiceFactoryMock.Object);

            var returnedSearch = await _searchService.CreateSearchAsync(GetTestSearchParam());

            Assert.Equal(SearchState.Failed, returnedSearch.State);

            Assert.Single(searchDict);
            Assert.Equal(SearchState.Failed, searchDict.FirstOrDefault().Value.State);
        }

        [Fact]
        public async void TestGetSearchResults()
        {
            var searchDict = new ConcurrentDictionary<string, Search>();
            var searchResultsDict = new ConcurrentDictionary<string, SearchResult>();
            var searchParam = GetTestSearchParam();

            var search = await CreateSearchWithSearchResults(searchDict, searchResultsDict, searchParam);
            var searchResults = await _searchService.GetSearchResultAsync(search.Id);

            Assert.Equal(searchResultsDict.FirstOrDefault().Value, searchResults);
        }

        [Fact]
        public async void TestGetAllSearches()
        {
            var searchDict = new ConcurrentDictionary<string, Search>();
            var searchResultsDict = new ConcurrentDictionary<string, SearchResult>();
            var searchParam = GetTestSearchParam();

            var search = await CreateSearchWithSearchResults(searchDict, searchResultsDict, searchParam);
            var search2 = await CreateSearchWithSearchResults(searchDict, searchResultsDict, searchParam);

            var searches = await _searchService.GetAllSearchesAsync();
            var searchesFromDict = searchDict.Select(p=>p.Value).ToList();

            Assert.Equal(2, searches.Count);
            Assert.Equal(2, searchesFromDict.Count);

            Assert.Equal(searchesFromDict[0], searches[0]);
            Assert.Equal(searchesFromDict[1], searches[1]);
        }

        [Fact]
        public async void TestDeleteSearchAndSearchResults()
        {
            var searchDict = new ConcurrentDictionary<string, Search>();
            var searchResultsDict = new ConcurrentDictionary<string, SearchResult>();
            var searchParam = GetTestSearchParam();

            var search = await CreateSearchWithSearchResults(searchDict, searchResultsDict, searchParam);

            Assert.Single(searchDict);
            Assert.Single(searchResultsDict);

            await _searchService.DeleteSearchAsync(search.Id);

            Assert.Empty(searchDict);
            Assert.Empty(searchResultsDict);
        }

        private List<Record> GetTestRecordsCollection()
        {
            var testRecordsCollection = new List<Record>()
            {
                new Record
                {
                    Message = "I test message (1)",
                    Properties = new Dictionary<string, string>(),
                    Timestamp = ""
                },

                new Record()
                {
                    Message = "I test message (2)",
                    Properties = new Dictionary<string, string>(),
                    Timestamp = ""
                }
            };

            return testRecordsCollection;
        }

        private SearchParam GetTestSearchParam()
        {
            var testSearchParam = new SearchParam()
            {
                From = DateTime.Now,
                To = DateTime.Today.AddDays(2),
                MaxCount = 100,
                Query = "Irina"
            };

            return testSearchParam;
        }

        private async Task<Search> CreateSearchWithSearchResults(ConcurrentDictionary<string, Search> searchDict, 
            ConcurrentDictionary<string, SearchResult> searchResultsDict, SearchParam searchParam)
        {
            var testRecordsCollection = GetTestRecordsCollection();
            var testRecordsCollectionJson = JsonConvert.SerializeObject(testRecordsCollection);
            _relaySenderMock.Setup(x => x.SendRequest(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(testRecordsCollectionJson));
            _searchServiceFactoryMock.Setup(x => x.GetSearchCacheDictionary()).Returns(searchDict);
            _searchServiceFactoryMock.Setup(x => x.GetSearchResultCacheDictionary()).Returns(searchResultsDict);

            _searchService = new SearchService(_relaySenderMock.Object, _searchServiceFactoryMock.Object);
            var search = await _searchService.CreateSearchAsync(searchParam);
            return search;
        }

    }
}
