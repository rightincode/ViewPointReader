using System;
using System.Collections.Generic;
using ViewPointReader.Core.Interfaces;

namespace ViewPointReader.Core.Models
{
    public class FeedSubscription : IFeedSubscription
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> KeyPhrases { get; set; }
        public DateTime SubscribedDate { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? LastUpdated { get; set; }
        public float RecommendationScore { get; set; }

        public FeedSubscription()
        {

        }
    }
}
