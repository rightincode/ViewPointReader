using System;
using Microsoft.Azure.Cosmos.Table;
using ViewPointReader.Data.Interfaces;

namespace ViewPointReader.Data.Models.Cloud
{
    public class FeedSubscriptionEntity : TableEntity, IFeedSubscriptionDo
    {
        public FeedSubscriptionEntity(){}

        public int Id { get ; set ; }
        public string Title { get ; set ; }
        public string Description { get ; set ; }
        public string Url { get ; set ; }
        public string ImageUrl { get ; set ; }
        public DateTime? LastUpdated { get ; set ; }
        public string KeyPhrases { get ; set ; }
        public DateTime SubscribedDate { get ; set ; }
    }
}
