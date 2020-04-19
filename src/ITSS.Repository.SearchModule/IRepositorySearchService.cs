using System.Collections.Generic;

namespace ITSS.Repository.SearchModule
{
    public interface IRepositorySearchService
    {
        List<RepositoryRecord> Search(string relQuery, int maxCount);
    }
}
