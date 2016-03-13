using Newtonsoft.Json;

namespace OakBot.Models
{
    public class BadgeResult
    {
        #region Public Properties

        [JsonProperty("admin")]
        public Badge Admin { get; set; }

        [JsonProperty("broadcaster")]
        public Badge Broadcaster { get; set; }

        [JsonProperty("global_mod")]
        public Badge GlobalMod { get; set; }

        [JsonProperty("mod")]
        public Badge Mod { get; set; }

        [JsonProperty("staff")]
        public Badge Staff { get; set; }

        [JsonProperty("subscriber")]
        public Badge Subscriber { get; set; }

        [JsonProperty("turbo")]
        public Badge Turbo { get; set; }

        #endregion Public Properties
    }
}