using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace OakBot.Models
{
    [JsonObject("authorization")]
    public class Authorization
    {
        #region Public Properties

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("scopes")]
        public List<string> Scopes { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        #endregion Public Properties
    }
}