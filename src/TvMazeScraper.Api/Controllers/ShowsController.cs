using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
        public async Task<IEnumerable<TvMazeShow>> Get([Required] int skip, [Required] int take, CancellationToken ct = default)
        {
            var showsDictionary = await _showStorage.GetShowsAsync(ct);

            // Unfortunately, Cosmos DB does not yet support skip/take so we can not implement this at the storage level.
            // https://feedback.azure.com/forums/263030-azure-cosmos-db/suggestions/6350987--documentdb-allow-paging-skip-take
            var shows = showsDictionary.Values.Skip(skip).Take(take);
            return shows;
        }
    }
}
