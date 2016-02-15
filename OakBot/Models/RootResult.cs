using Newtonsoft.Json;

namespace OakBot.Models
{
    public class RootResult
    {
        [JsonProperty("token")]
        public Token Token { get; set; }
    }
}