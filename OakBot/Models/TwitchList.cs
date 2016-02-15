using System.Collections.Generic;
using Newtonsoft.Json;
using OakBot.Helpers;

namespace OakBot.Models
{
    //@author gibletto

    [JsonObject(ItemConverterType = typeof (TwitchListConverter))]
    public class TwitchList<T> : TwitchResponse
    {
        public List<T> List { get; set; }
    }
}