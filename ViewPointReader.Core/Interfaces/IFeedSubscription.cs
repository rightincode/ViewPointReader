using System;
using System.Collections.Generic;
using System.Text;

namespace ViewPointReader.Core.Interfaces
{
    public interface IFeedSubscription
    {
        int Id { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        string[] KeyPhrases { get; set; }
        DateTime SubscribedDate { get; set; }
    }
}
