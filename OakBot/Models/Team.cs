using Newtonsoft.Json;
using System;

namespace OakBot.Models
{
    [JsonObject("teams")]
    public class Team : TwitchResponse
    {
        #region Public Properties

        [JsonProperty("background")]
        public string Background { get; set; }

        [JsonProperty("banner")]
        public string Banner { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("_id")]
        public long Id { get; set; }

        [JsonProperty("info")]
        public string Info { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        #endregion Public Properties
    }
}