using Newtonsoft.Json;

namespace OakBot.Models
{
    public class StreamSummary : TwitchResponse
    {
        #region Public Properties

        [JsonProperty("channels")]
        public long Channels { get; set; }

        [JsonProperty("viewers")]
        public long Viewers { get; set; }

        #endregion Public Properties
    }
}