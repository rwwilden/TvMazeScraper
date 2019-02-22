using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TvMazeScraper.Api.Lib
{
    public interface IShowStorage
    {
        Task<IDictionary<string, TvMazeShow>> GetShowsAsync(CancellationToken ct = default);

        Task CreateOrUpdateShow(JObject show, CancellationToken ct = default);
    }
}