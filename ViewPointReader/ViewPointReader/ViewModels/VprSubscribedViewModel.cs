using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ViewPointReader.Core.Interfaces;
using ViewPointReader.Data.Interfaces;

namespace ViewPointReader.ViewModels
{
    public class VprSubscribedViewModel
    {
        private readonly IViewPointReaderRepository _viewPointReaderRepository;

        public ObservableCollection<IFeedSubscription> FeedSubscriptions { get; }

        public VprSubscribedViewModel(IViewPointReaderRepository viewPointReaderRepository)
        {
            _viewPointReaderRepository = viewPointReaderRepository;
            FeedSubscriptions = new ObservableCollection<IFeedSubscription>();
        }

        public async Task LoadSubscribedFeeds()
        {
            var feeds = await _viewPointReaderRepository.GetFeedSubscriptionsAsync();
            FeedSubscriptions.Clear();
            feeds.ToList().ForEach(FeedSubscriptions.Add);
        }
    }
}
