using Newtonsoft.Json;

namespace OakBot.Models
{
    public class StreamSummary : TwitchResponse
    {
        [JsonProperty("viewers")]
        public long Viewers { get; set; }

        [JsonProperty("channels")]
        public long Channels { get; set; }
    }
}