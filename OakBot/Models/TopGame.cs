using Newtonsoft.Json;

namespace OakBot.Models
{
    [JsonObject("top")]
    public class TopGame
    {
        #region Public Properties

        [JsonProperty("channels")]
        public long Channels { get; set; }

        [JsonProperty("game")]
        public Game Game { get; set; }

        [JsonProperty("viewers")]
        public long Viewers { get; set; }

        #endregion Public Properties
    }
}