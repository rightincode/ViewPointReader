using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ViewPointReader.Core.Models;
using ViewPointReader.Data.Interfaces;

namespace ViewPointReader.ViewModels
{
    public class VprFeedArticlesViewModel
    {
        private readonly IViewPointReaderRepository _viewPointReaderRepository;
        public ObservableCollection<VprFeedItem> FeedItems { get; }

        public VprFeedArticlesViewModel(IViewPointReaderRepository viewPointReaderRepository)
        {
            _viewPointReaderRepository = viewPointReaderRepository;
            FeedItems = new ObservableCollection<VprFeedItem>();
        }

        public async Task LoadFeedItems(int subscriptionId)
        {
            var feedItems = await _viewPointReaderRepository.GetFeedItemsForFeedAsync(subscriptionId);
            FeedItems.Clear();
            feedItems.ToList().ForEach(FeedItems.Add);
        }
    }
}
