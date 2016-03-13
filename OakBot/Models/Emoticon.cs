using Newtonsoft.Json;
using System.Collections.Generic;

namespace OakBot.Models
{
    [JsonObject("emoticons")]
    public class Emoticon
    {
        #region Public Properties

        [JsonProperty("images")]
        public List<Image> Images { get; set; }

        [JsonProperty("regex")]
        public string Regex { get; set; }

        #endregion Public Properties
    }
}