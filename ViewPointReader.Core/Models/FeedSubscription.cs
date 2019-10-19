using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ViewPointReader.Core.Interfaces;

namespace ViewPointReader.Core.Models
{
    public class FeedSubscription : IFeedSubscription
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("keyphrases")]
        public List<string> KeyPhrases { get; set; }
        [JsonProperty("subscribeddate")]
        public DateTime SubscribedDate { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("imageurl")]
        public string ImageUrl { get; set; }
        [JsonProperty("lastupdated")]
        public DateTime? LastUpdated { get; set; }
        [JsonProperty("recommendationscore")]
        public float RecommendationScore { get; set; }
        [JsonProperty("feeditems")]
        public List<VprFeedItem> FeedItems { get; set; }

        public FeedSubscription()
        {

        }
    }
}
