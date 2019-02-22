using System;
using Newtonsoft.Json;

namespace TvMazeScraper.Api.Lib
{
    [JsonObject]
    public class TvMazeShow
    {
        public static readonly DateTimeOffset Epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "cast")]
        public TvMazeShowCastMember[] Cast { get; set; }

        [JsonProperty(PropertyName = "updated")]
        public long Updated { get; set; }

        [JsonIgnore]
        public DateTimeOffset UpdatedTimestamp => Epoch.AddSeconds(Updated);
    }
}