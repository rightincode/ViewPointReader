using SQLite;
using System;

namespace ViewPointReader.Data.Interfaces
{
    public interface IFeedSubscriptionDo
    {
        [PrimaryKey, AutoIncrement]
        int Id { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        string Url { get; set; }
        string ImageUrl { get; set; }
        DateTime? LastUpdated { get; set; }
        string KeyPhrases { get; set; }
        DateTime SubscribedDate { get; set; }
    }
}

