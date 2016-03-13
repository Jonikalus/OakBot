using Newtonsoft.Json;
using System;

namespace OakBot.Models
{
    [JsonObject("blocks")]
    public class Block
    {
        #region Public Properties

        [JsonProperty("_id")]
        public long Id { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        #endregion Public Properties
    }
}