using Newtonsoft.Json;
using System;

namespace OakBot.Models
{
    [JsonObject("subscriptions")]
    public class Subscription : TwitchResponse
    {
        #region Public Properties

        [JsonProperty("channel")]
        public Channel Channel { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("_id")]
        public string Id { get; set; }

        // user or channel is null. There are two types of subscription: user subscriptions and channel subscriptions
        [JsonProperty("user")]
        public User User { get; set; }

        #endregion Public Properties
    }
}