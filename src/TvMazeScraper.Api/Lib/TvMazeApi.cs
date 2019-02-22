using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TvMazeScraper.Api.Lib
{
    class TvMazeApi : ITvMazeApi
    {
        private readonly HttpClient _client;

        public TvMazeApi(HttpClient client)
        {
            _client = client;
        }


        public async Task<IDictionary<string, DateTimeOffset>> GetShowUpdatesAsync(CancellationToken ct = default)
        {
            using (var response = await _client.GetAsync("updates/shows", ct))
            {
                var showUpdates = new Dictionary<string, DateTimeOffset>();
                var showUpdatesJson = await response.Content.ReadAsStringAsync();
                var showUpdatesJObject = JObject.Parse(showUpdatesJson);

                foreach (var showUpdate in showUpdatesJObject)
                {
                    var epochTime = showUpdate.Value.Value<long>();
                    showUpdates[showUpdate.Key] = TvMazeShow.Epoch.AddSeconds(epochTime);
                }

                return showUpdates;
            }
        }

        public async Task<JObject> GetShowAsync(string showId, CancellationToken ct = default)
        {
            using (var response = await _client.GetAsync($"shows/{showId}?embed=cast", ct))
            {
                var showString = await response
                    .EnsureSuccessStatusCode()
                    .Content.ReadAsStringAsync();
                return JObject.Parse(showString);
            }
        }
    }
}