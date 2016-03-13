using Newtonsoft.Json;

namespace OakBot.Models
{
    [JsonObject("ingests")]
    public class Ingest
    {
        #region Public Properties

        [JsonProperty("availability")]
        public double Availability { get; set; }

        [JsonProperty("default")]
        public bool Default { get; set; }

        [JsonProperty("_id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url_template")]
        public string UrlTemplate { get; set; }

        #endregion Public Properties
    }
}