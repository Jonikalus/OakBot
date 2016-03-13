using Newtonsoft.Json;

namespace OakBot.Models
{
    [JsonObject("images")]
    public class Image
    {
        #region Public Properties

        [JsonProperty("emoticon_set")]
        public long EmoticonSet { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        #endregion Public Properties
    }
}