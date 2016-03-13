using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;

namespace OakBot.Helpers
{
    // @author gibletto
    public class DynamicJsonDeserializer : IDeserializer
    {
        #region Public Properties

        public string DateFormat { get; set; }
        public string Namespace { get; set; }
        public string RootElement { get; set; }

        #endregion Public Properties

        #region Public Methods

        public T Deserialize<T>(IRestResponse response)
        {
            return JsonConvert.DeserializeObject<T>(response.Content, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new JsonConverter[] { new TwitchListConverter() } });
        }

        #endregion Public Methods
    }
}