using ITSS.Repository.ConsoleMVC.Logic.Interfaces;
using ITSS.Repository.ConsoleMVC.Models.ForSearch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITSS.Repository.ConsoleMVC.Controllers
{
    [Route("[controller]")]
    public class SearchesController : Controller
    {
        private readonly ILogger _log;
        private readonly ISearchService _searchService;
        public SearchesController(ILogger<SearchesController> log, ISearchService searchService)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
        }

        /// <param name="searchParam">This parameter sets search parameters for creating new search</param>
        [HttpPost]
        public async Task<IActionResult> CreateSearch([FromBody]SearchParam searchParam)
        {
            if (!this.ModelState.IsValid)
                return BadRequest();

            _log.LogDebug("Creating a new search with query {0}", searchParam.Query);
            var search = await _searchService.CreateSearchAsync(searchParam);

            if (search == null)
                return NotFound();

            return Ok(search);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSearches()
        {
            _log.LogDebug("Returning the list of all searches");
            var searches = await _searchService.GetAllSearchesAsync();

            return Ok(searches ?? new List<Search>());
        }

        [HttpGet("{searchId}/results")]
        public async Task<ViewResult> GetSearchResults([FromRoute]string searchId)
        {
            _log.LogDebug($"Returning search result by id {searchId}");
            var searchResults = await _searchService.GetSearchResultAsync(searchId);
            return (searchResults == null) ? View("SearchResults") : View("SearchResults", searchResults);
        }

        [HttpDelete("{searchId}")]
        public async Task<IActionResult> DeleteSearchById([FromRoute]string searchId)
        {
            _log.LogDebug($"Deleting search by id {searchId}");
            await _searchService.DeleteSearchAsync(searchId);
            return Ok();
        }
    }
}
