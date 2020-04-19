using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ITSS.Repository.ConsoleMVC.Models.ForSearch
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SearchState
    {
        Scheduled,
        Running,
        Finished,
        Failed
    }
}
