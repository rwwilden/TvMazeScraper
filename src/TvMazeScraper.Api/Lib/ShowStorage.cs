using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace TvMazeScraper.Api.Lib
{
    class ShowStorage : IShowStorage
    {
        private readonly CosmosContainer _container;

        public ShowStorage(IOptions<CosmosOptions> cosmosOptions)
        {
            var cosmosSettings = cosmosOptions.Value;
            var cosmosEndpoint = cosmosSettings.Endpoint;
            var cosmosPrimaryKey = cosmosSettings.PrimaryKey;
            var cosmosConfiguration = new CosmosConfiguration(cosmosEndpoint, cosmosPrimaryKey);
            var cosmosClient = new CosmosClient(cosmosConfiguration);

            var databaseName = cosmosSettings.Database;
            var containerName = cosmosSettings.Container;
            var cosmosDatabase = cosmosClient.Databases[databaseName];
            _container = cosmosDatabase.Containers[containerName];
        }

        public async Task<IDictionary<string, TvMazeShow>> GetShowsAsync(CancellationToken ct = default)
        {
            var shows = new Dictionary<string, TvMazeShow>();
            var query = _container.Items
                .CreateItemQuery<TvMazeShow>(new CosmosSqlQueryDefinition("SELECT * FROM c"), maxConcurrency: 4);
            while (query.HasMoreResults)
            {
                var tvMazeShows = await query.FetchNextSetAsync(ct);

                foreach (var tvMazeShow in tvMazeShows)
                {
                    shows[tvMazeShow.Id] = tvMazeShow;
                }
            }

            return shows;
        }

        public async Task CreateOrUpdateShow(JObject show, CancellationToken ct = default)
        {
            // Hand-parse show because we do not need everything that is returned from the API.
            var id = show["id"].Value<string>();
            var name = show["name"].Value<string>();
            var updated = show["updated"].Value<long>();

            var castMembers =
                from t in show.SelectTokens("$..person")
                let birthday = t["birthday"].Value<string>()
                orderby birthday descending
                select new
                {
                    id = t["id"].Value<string>(),
                    name = t["name"].Value<string>(),
                    birthday
                };

            var tvMazeShow = new TvMazeShow
            {
                Id = id,
                Name = name,
                Updated = updated,
                Cast = castMembers
                    .Select(castMember => new TvMazeShowCastMember
                    {
                        Id = castMember.id,
                        Name = castMember.name,
                        Birthday = castMember.birthday
                    })
                    .ToArray()
            };

            // Add to Cosmos db.
            await _container.Items.UpsertItemAsync(tvMazeShow.Name, tvMazeShow, cancellationToken: ct);
        }
    }
}
