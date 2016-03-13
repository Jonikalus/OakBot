using Newtonsoft.Json;

namespace OakBot.Models
{
    public class RootResult
    {
        #region Public Properties

        [JsonProperty("token")]
        public Token Token { get; set; }

        #endregion Public Properties
    }
}