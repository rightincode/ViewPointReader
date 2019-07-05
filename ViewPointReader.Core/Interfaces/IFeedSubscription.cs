using System;
using System.Collections.Generic;
using ViewPointReader.Core.Models;

namespace ViewPointReader.Core.Interfaces
{
    public interface IFeedSubscription
    {
        int Id { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        string Url { get; set; }
        string ImageUrl { get; set; }
        DateTime? LastUpdated { get; set; }
        List<string> KeyPhrases { get; set; }
        DateTime SubscribedDate { get; set; }
        float RecommendationScore { get; set; }
        List<VprFeedItem> FeedItems { get; set; }
    }
}
