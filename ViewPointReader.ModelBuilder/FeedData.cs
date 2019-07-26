using System;
using Microsoft.ML.Data;

namespace ViewPointReader.ModelBuilder
{
    public class FeedData
    {
        //public int Id;
        public string Title { get; set; }
        //public string Description { get; set; }
        public string[] KeyPhrases { get; set; }
        //public DateTime SubscribedDate { get; set; }
        //public string Url { get; set; }
        //public string ImageUrl { get; set; }
        //public DateTime? LastUpdated { get; set; }

        public bool Label;
    }
}
