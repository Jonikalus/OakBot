using Newtonsoft.Json;

namespace OakBot.Models
{
    [JsonObject("games")]
    public class Game
    {
        #region Public Properties

        [JsonProperty("box")]
        public ScaledImage Box { get; set; }

        [JsonProperty("giantbomb_id")]
        public long GiantbombId { get; set; }

        [JsonProperty("_id")]
        public long Id { get; set; }

        [JsonProperty("logo")]
        public ScaledImage Logo { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("popularity")]
        public long Popularity { get; set; }

        #endregion Public Properties
    }
}