using Newtonsoft.Json;
using OakBot.Helpers;
using System.Collections.Generic;

namespace OakBot.Models
{
    //@author gibletto

    [JsonObject(ItemConverterType = typeof(TwitchListConverter))]
    public class TwitchList<T> : TwitchResponse
    {
        public List<T> List { get; set; }
    }
}