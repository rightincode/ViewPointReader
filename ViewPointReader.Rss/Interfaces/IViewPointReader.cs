using CodeHollow.FeedReader;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ViewPointReader.Rss.Interfaces
{
    public interface IViewPointReader
    {
        Task<List<Feed>> SearchForFeeds(string queryText);
        Task<List<FeedItem>> LoadSubscribedFeeds();
    }
}
