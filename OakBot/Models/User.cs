using Newtonsoft.Json;
using System;

namespace OakBot.Models
{
    [JsonObject("users")]
    public class User : TwitchResponse
    {
        #region Public Properties

        [JsonProperty("bio")]
        public string Bio { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("_id")]
        public long Id { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("notifications")]
        public Notification Notification { get; set; }

        [JsonProperty("partnered")]
        public bool Partnered { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        #endregion Public Properties
    }
}