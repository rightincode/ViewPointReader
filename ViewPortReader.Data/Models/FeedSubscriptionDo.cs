using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using ViewPointReader.Data.Interfaces;

namespace ViewPointReader.Data.Models
{
    public class FeedSubscriptionDo : IFeedSubscriptionDo
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get ; set ; }
        public string Title { get ; set ; }
        public string Description { get ; set ; }
        public string[] KeyPhrases { get ; set ; }
        public DateTime SubscribedDate { get ; set ; }

        public FeedSubscriptionDo() { }

    }
}
