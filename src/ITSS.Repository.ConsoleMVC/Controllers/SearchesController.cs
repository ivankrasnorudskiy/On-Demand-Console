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
        public IActionResult GetAllSearches()
        {
            _log.LogDebug("Returning the list of all searches");
            var searches =  _searchService.GetAllSearches();

            return Ok(searches ?? new List<Search>());
        }

        [HttpGet("{searchId}/results")]
        public ViewResult GetSearchResults([FromRoute]string searchId)
        {
            _log.LogDebug($"Returning search result by id {searchId}");
            var searchResults = _searchService.GetSearchResult(searchId);
            return (searchResults == null) ? View("SearchResults") : View("SearchResults", searchResults);
        }

        [HttpDelete("{searchId}")]
        public IActionResult DeleteSearchById([FromRoute]string searchId)
        {
            _log.LogDebug($"Deleting search by id {searchId}");
            _searchService.DeleteSearch(searchId);
            return Ok();
        }
    }
}
