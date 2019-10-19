using System.Collections.Generic;
using System.Threading.Tasks;
using ViewPointReader.Core.Models;

namespace ViewPointReader.Rss.Interfaces
{
    public interface IViewPointRssReader
    {
        Task<List<FeedSubscription>> SearchForFeedsAsync(string queryText);
        Task<List<string>> ExtractKeyPhrasesAsync(string feedDescription);
    }
}
