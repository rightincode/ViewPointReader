using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace ViewPointReader.Data.Models
{
    public class VprFeedItemDo
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [ForeignKey(typeof(FeedSubscriptionDo))]
        public int FeedSubscriptionDoId { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string PublishingDateString { get; set; }
        public DateTime? PublishingDate { get; set; }
        public string Author { get; set; }

        //TODO: persist this data
        //public ICollection<string> Categories { get; set; }

        public string Content { get; set; }
       
    }
}
