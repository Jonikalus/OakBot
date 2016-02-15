using Newtonsoft.Json;

namespace OakBot.Models
{
    public class StreamResult : TwitchResponse
    {
        [JsonProperty("stream")]
        public Stream Stream { get; set; }
    }
}