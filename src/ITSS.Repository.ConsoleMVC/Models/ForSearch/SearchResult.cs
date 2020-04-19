using System.Collections.Generic;

namespace ITSS.Repository.ConsoleMVC.Models.ForSearch
{
    public class SearchResult
    {
        public string Metainfo { get; set; }
        public List<Record> Records { get; set; }
        ///<value>This uri gets tou to next batch of results</value>
        public string NextResultUri { get; set; }
    }
}
