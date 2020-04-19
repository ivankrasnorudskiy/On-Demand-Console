using System.Collections.Generic;
using System.Threading.Tasks;
using ITSS.Repository.ConsoleMVC.Models.ForSearch;

namespace ITSS.Repository.ConsoleMVC.Logic.Interfaces
{
    /// <summary>
    /// The interface describes search service functionality.
    /// It is used to manage searches in the system.
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        /// Creates instance of a new search using input parameters.
        /// </summary>
        /// <param name="param">Search creation parameters.</param>
        /// <returns>Newly created search instance.</returns>
        Task<Search> CreateSearchAsync(SearchParam param);

        /// <summary>
        /// Returns all searches collection.
        /// </summary>
        Task<List<Search>> GetAllSearchesAsync();

        /// <summary>
        /// Returns search result by id.
        /// </summary>
        /// <param name="searchId">Id of the target search.</param>
        Task<SearchResult> GetSearchResultAsync(string searchId);

        /// <summary>
        /// Deletes search instance by id.
        /// </summary>
        /// <param name="searchId">Id of the target search.</param>
        Task DeleteSearchAsync(string searchId);


    }
}
