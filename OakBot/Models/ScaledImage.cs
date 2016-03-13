using Newtonsoft.Json;

namespace OakBot.Models
{
    public class ScaledImage
    {
        #region Public Properties

        [JsonProperty("large")]
        public string Large { get; set; }

        [JsonProperty("medium")]
        public string Medium { get; set; }

        [JsonProperty("small")]
        public string Small { get; set; }

        [JsonProperty("template")]
        public string Template { get; set; }

        #endregion Public Properties
    }
}