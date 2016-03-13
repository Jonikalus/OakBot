using Newtonsoft.Json;
using System;

namespace OakBot.Models
{
    [JsonObject("follows")]
    public class Follower
    {
        #region Public Properties

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("notifications")]
        public bool Notifications { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        #endregion Public Properties
    }
}