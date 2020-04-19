using System;

namespace ITSS.Repository.ConsoleMVC.Models.ForSearch
{
    /// <summary>
    /// This class contains model which describes properties for Search.
    /// </summary>
    public class SearchParam
    {
        ///<value>Search works by this query</value>
        public string Query { get; set; } = "";
        public int MaxCount { get; set; } = 100;
        ///<value>It sets left date limit of records for search</value>
        public DateTime From { get; set; } = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1));
        ///<value>It sets right date limit of records for search</value>
        public DateTime To { get; set; } = DateTime.UtcNow;

        public bool Equals(SearchParam obj)
        {
            if (obj == null)
                return false;

            return obj.From == this.From && obj.MaxCount == this.MaxCount && obj.To == this.To && obj.Query == this.Query;
        }

        public override int GetHashCode()
        {
            var hash = 12;
            hash *= Query.GetHashCode();
            hash *= MaxCount.GetHashCode();
            hash *= From.GetHashCode();
            hash *= To.GetHashCode();

            return hash;
        }
    }

    
}
