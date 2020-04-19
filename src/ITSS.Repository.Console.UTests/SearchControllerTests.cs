using ITSS.Repository.ConsoleMVC.Logic.Interfaces;
using ITSS.Repository.ConsoleMVC.Models.ForSearch;
using ITSS.Repository.Relay.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
    public class SearchControllerTests
    {
        private List<ServiceDescriptor> _serviceDescriptors;
        private Mock<IRelaySender> _relaySenderMock;
        private Mock<ISearchServiceFactory> _searchServiceFactoryMock;
        public SearchControllerTests()
        {
            _relaySenderMock = new Mock<IRelaySender>();
            _serviceDescriptors = new List<ServiceDescriptor>();
            _searchServiceFactoryMock = new Mock<ISearchServiceFactory>();
        }

        [Fact]
        public async void TestCreateSearchEndpointWithoutExceptions()
        {
            var searchDict = new ConcurrentDictionary<string, Search>();
            var searchResultsDict = new ConcurrentDictionary<string, SearchResult>();

            var testRecordsCollection = GetTestRecordsCollection();
            var testRecordsCollectionJson = JsonConvert.SerializeObject(testRecordsCollection);
            _relaySenderMock.Setup(x => x.SendRequest(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(testRecordsCollectionJson));
            _serviceDescriptors.Add(new ServiceDescriptor(typeof(IRelaySender), _relaySenderMock.Object));

            _searchServiceFactoryMock.Setup(x => x.GetSearchCacheDictionary()).Returns(searchDict);
            _searchServiceFactoryMock.Setup(x => x.GetSearchResultCacheDictionary()).Returns(searchResultsDict);
            _serviceDescriptors.Add(new ServiceDescriptor(typeof(ISearchServiceFactory), _searchServiceFactoryMock.Object));

            var searchParam = GetTestSearchParam();
            var searchParamJson = JsonConvert.SerializeObject(searchParam);
            var content = new StringContent(searchParamJson, Encoding.UTF8, "application/json");

            using (var client = GetHttpClient())
            {
                var result = await client.PostAsync($"searches/", content);
                var json = await result.Content.ReadAsStringAsync();
                var returnedSearch = JsonConvert.DeserializeObject<Search>(json);

                Assert.True(returnedSearch.Equals(searchDict.FirstOrDefault().Value));

                Assert.Single(searchDict);
                Assert.Single(searchResultsDict);

                var searchId = searchDict.FirstOrDefault().Key ?? throw new ArgumentException();
                var correctUri = string.Format("searches/{0}/results", searchId);
                Assert.Equal(correctUri, searchDict.FirstOrDefault().Value.ResultUri);
                Assert.True(searchParam.Equals(searchDict.FirstOrDefault().Value.SearchParam));
                Assert.Equal(SearchState.Scheduled, searchDict.FirstOrDefault().Value.State);

                Assert.Equal("Search is finished", searchResultsDict.FirstOrDefault().Value.Metainfo);
                Assert.Empty(searchResultsDict.FirstOrDefault().Value.NextResultUri);

                Assert.True(testRecordsCollection[0].Equals(searchResultsDict.FirstOrDefault().Value.Records[0]));
                Assert.True(testRecordsCollection[1].Equals(searchResultsDict.FirstOrDefault().Value.Records[1]));
            }
        }

        [Fact]
        public async void TestCreateSearchIfException()
        {
            //ToDo
        }

        [Fact]
        public async void TestAddSearchResultsToDictionaryIfException()
        {
            //ToDo
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

        private HttpClient GetHttpClient()
        {
            var testServer = new ConfigurableServer(sc => ReplaceServiceCollection(sc));
            return testServer.CreateClient();
        }
        
        private IServiceCollection ReplaceServiceCollection(IServiceCollection servCollection)
        {
            foreach (var serviceDescriptor in _serviceDescriptors)
            {
                servCollection.Replace(serviceDescriptor);
            }

            return servCollection;
        }
    }
}

