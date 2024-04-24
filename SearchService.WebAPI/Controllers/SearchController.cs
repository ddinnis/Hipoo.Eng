using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using SearchService.Domain;

namespace SearchService.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchRepository repository;
        private readonly ICapPublisher capPublisher;

        public SearchController(ISearchRepository repository, ICapPublisher capPublisher)
        {
            this.repository = repository;
            this.capPublisher = capPublisher;
        }

        [HttpGet]
        public Task<SearchEpisodesResponse> SearchEpisodes([FromQuery] SearchEpisodesRequest req)
        {
            return repository.SearchEpisodes(req.Keyword, req.PageIndex, req.PageSize);
        }


    }
}
