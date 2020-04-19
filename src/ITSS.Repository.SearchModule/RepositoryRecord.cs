using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITSS.Repository.SearchModule
{
    public class RepositoryRecord
    {
        public string Timestamp { get; set; }
        public string Message { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}
