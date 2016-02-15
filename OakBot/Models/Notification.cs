using Newtonsoft.Json;

namespace OakBot.Models
{
    [JsonObject("notifications")]
    public class Notification
    {
        [JsonProperty("email")]
        public bool Email { get; set; }
        [JsonProperty("push")]
        public bool Push { get; set; }
    }
}