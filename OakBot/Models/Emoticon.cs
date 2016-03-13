using Newtonsoft.Json;
using System.Collections.Generic;

namespace OakBot.Models
{
    [JsonObject("emoticons")]
    public class Emoticon
    {
        [JsonProperty("regex")]
        public string Regex { get; set; }

        [JsonProperty("images")]
        public List<Image> Images { get; set; }
    }
}