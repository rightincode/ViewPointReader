using CodeHollow.FeedReader;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ViewPointReader.Rss.Interfaces
{
    public interface IViewPointRssReader
    {
        Task<List<Feed>> SearchForFeedsAsync(string queryText);
        Task<List<FeedItem>> LoadSubscribedFeeds();
        Task<List<string>> ExtractKeyPhrasesAsync(string feedDescription);
    }
}
