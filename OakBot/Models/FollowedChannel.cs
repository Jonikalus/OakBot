using Newtonsoft.Json;
using System;

namespace OakBot.Models
{
    [JsonObject("follows")]
    public class FollowedChannel : TwitchResponse
    {
        #region Public Properties

        [JsonProperty("channel")]
        public Channel Channel { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("notifications")]
        public bool Notifications { get; set; }

        #endregion Public Properties
    }
}