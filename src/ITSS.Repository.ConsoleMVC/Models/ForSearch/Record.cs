using System.Collections.Generic;

namespace ITSS.Repository.ConsoleMVC.Models.ForSearch
{
    /// <summary>
    /// This class contains abstract model of Intrust record
    /// </summary>
    public class Record
    {
        public string Timestamp { get; set; }
        public string Message { get; set; }
        public Dictionary<string, string> Properties { get; set; }

        public bool Equals(Record obj)
        {
            if (obj == null)
                return false;

            if (obj.Properties == null)
                return false;

            if (obj.Properties.Count != 0 && this.Properties.Count != 0)
            {
                //ToDO compare logic 
            }

            return obj.Message == this.Message &&  obj.Timestamp == this.Timestamp;
        }

        public override int GetHashCode()
        {
            var hash = 12;
            hash *= Message.GetHashCode();
            hash *= Properties.GetHashCode();
            hash *= Timestamp.GetHashCode();

            return hash;
        }
    }
}
