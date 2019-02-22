using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvMazeScraper.Api.Lib;

namespace TvMazeScraper.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowsController : ControllerBase
    {
        private readonly IShowStorage _showStorage;

        public ShowsController(IShowStorage showStorage)
        {
            _showStorage = showStorage;
        }

        [HttpGet]
        public async Task<IEnumerable<TvMazeShow>> Get(CancellationToken ct)
        {
            var showsDictionary = await _showStorage.GetShowsAsync(ct);

            // Sort cast on birthday.
            var shows = showsDictionary.Values;
            return shows;
        }
    }
}
