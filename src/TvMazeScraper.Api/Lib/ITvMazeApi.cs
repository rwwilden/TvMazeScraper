using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TvMazeScraper.Api.Lib
{
    public interface ITvMazeApi
    {
        Task<IDictionary<string, DateTimeOffset>> GetShowUpdatesAsync(CancellationToken ct = default);

        Task<JObject> GetShowAsync(string showId, CancellationToken ct = default);
    }
}