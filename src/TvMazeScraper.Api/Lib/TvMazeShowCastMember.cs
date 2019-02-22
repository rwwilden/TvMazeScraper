using Newtonsoft.Json;

namespace TvMazeScraper.Api.Lib
{
    [JsonObject]
    public class TvMazeShowCastMember
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "birthday")]
        public string Birthday { get; set; }
    }
}