using Newtonsoft.Json;
using System;

namespace OakBot.Models
{
    [JsonObject("videos")]
    public class Video : TwitchResponse
    {
        #region Public Properties

        [JsonProperty("broadcast_id")]
        public long BroadcastId { get; set; }

        [JsonProperty("broadcast_type")]
        public string BroadcastType { get; set; }

        [JsonProperty("channel")]
        public Channel Channel { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("game")]
        public string Game { get; set; }

        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("length")]
        public long Length { get; set; }

        [JsonProperty("preview")]
        public string Preview { get; set; }

        [JsonProperty("recorded_at")]
        public DateTime RecordedAt { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("tag_list")]
        public string TagList { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("views")]
        public long Views { get; set; }

        #endregion Public Properties
    }
}