namespace ITSS.Repository.ConsoleMVC.Models.ForSearch
{
    /// <summary>
    /// This class contains abstract model of search
    /// </summary>
    public class Search
    {
        public string Id { get; set; }
        public SearchParam SearchParam { get; set; }
        public SearchState State { get; set; }
        public string ResultUri { get; set; }

        public Search(string id, string resultUri, SearchParam param)
        {
            Id = id;
            ResultUri = resultUri;
            SearchParam = param;
            State = SearchState.Scheduled;
        }

        public bool Equals(Search obj)
        {
            if (obj == null)
                return false;

            return 
                obj.Id == this.Id 
                && obj.ResultUri == this.ResultUri 
                && obj.SearchParam.Equals(this.SearchParam) 
                && obj.State == this.State;
        }

        public override int GetHashCode()
        {
            var hash = 12;
            hash *= Id.GetHashCode();
            hash *= ResultUri.GetHashCode();
            hash *= SearchParam.GetHashCode();
            hash *= State.GetHashCode();

            return hash;
        }
    }
}
