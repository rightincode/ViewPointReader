using System.Collections.Generic;
using System.Threading.Tasks;
using ViewPointReader.Core.Interfaces;
using ViewPointReader.Core.Models;

namespace ViewPointReader.Data.Interfaces
{
    public interface IViewPointReaderRepository
    {
        Task<int> DeleteFeedSubscriptionAsync(IFeedSubscription feedSubscription);
        Task<FeedSubscription> GetFeedSubscriptionAsync(int id);
        Task<List<FeedSubscription>> GetFeedSubscriptionsAsync();
        Task<int> SaveFeedSubscriptionAsync(IFeedSubscription feedSubscription);
        Task<List<VprFeedItem>> GetFeedItemsForFeedAsync(int subscriptionId);
    }
}
