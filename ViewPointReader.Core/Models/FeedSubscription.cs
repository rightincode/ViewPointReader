using System;
using ViewPointReader.Core.Interfaces;

namespace ViewPointReader.Core.Models
{
    public class FeedSubscription : IFeedSubscription
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string[] KeyPhrases { get; set; }
        public DateTime SubscribedDate { get; set; }

        public FeedSubscription()
        {

        }
    }
}
