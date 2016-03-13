using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OakBot.Models;
using System;
using System.Reflection;

namespace OakBot.Helpers
{
    // @author gibletto
    internal class TwitchListConverter : JsonConverter
    {
        #region Public Methods

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType && typeof(TwitchList<>) == objectType.GetGenericTypeDefinition();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = Activator.CreateInstance(objectType) as TwitchResponse;
            var genericArg = objectType.GetGenericArguments()[0];
            var key = genericArg.GetCustomAttribute<JsonObjectAttribute>();
            if (value == null || key == null)
                return null;
            var jsonObject = JObject.Load(reader);
            value.Total = SetValue<long>(jsonObject["_total"]);
            value.Error = SetValue<string>(jsonObject["error"]);
            value.Message = SetValue<string>(jsonObject["message"]);
            var list = jsonObject[key.Id];
            var prop = value.GetType().GetProperty("List");
            if (prop != null && list != null)
            {
                prop.SetValue(value, list.ToObject(prop.PropertyType, serializer));
            }
            return value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods

        #region Private Methods

        private T SetValue<T>(JToken token)
        {
            if (token != null)
            {
                return (T)token.ToObject(typeof(T));
            }
            return default(T);
        }

        #endregion Private Methods
    }
}