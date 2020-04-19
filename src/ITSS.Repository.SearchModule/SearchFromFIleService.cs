using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace ITSS.Repository.SearchModule
{
    // It's just a mock functional for search...
    public class SearchFromFileService : IRepositorySearchService
    {
        // Only SearchModule should know about file for search! 
        const string SourceDataPath = "events_data.json";
        public SearchFromFileService()
        {
            if (string.IsNullOrWhiteSpace(SourceDataPath))
                throw new ArgumentNullException(nameof(SourceDataPath));
        }

        public List<RepositoryRecord> Search(string relQuery, int maxCount)
        {
            var eventsData = GetRepositoryRecordsFromFile(SourceDataPath);

            if (string.IsNullOrEmpty(relQuery))
                return eventsData;

            var results = eventsData.Where(
                rec => JsonConvert.SerializeObject(rec)
                        .ToLower()
                        .Contains(relQuery.ToLower()))
                        .ToList();

            Thread.Sleep(3000);
            if (results.Count > maxCount)
                results.RemoveRange(maxCount, results.Count - maxCount);

            return results;
        }

        private List<RepositoryRecord> GetRepositoryRecordsFromFile(string sourceDataPath)
        {          
            using (var reader = new StreamReader(sourceDataPath))
            {
                var text = reader.ReadToEnd();
                var eventsData = JsonConvert.DeserializeObject<List<RepositoryRecord>>(text);
                return eventsData;
            }
        }
    }
}
