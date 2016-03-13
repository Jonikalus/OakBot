using Newtonsoft.Json;

namespace OakBot.Models
{
    public class StreamResult : TwitchResponse
    {
        #region Public Properties

        [JsonProperty("stream")]
        public Stream Stream { get; set; }

        #endregion Public Properties
    }
}