using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ViewPointReader.Core.Models;
using ViewPointReader.Data.Interfaces;
using ViewPointReader.Interfaces;

namespace ViewPointReader.ViewModels
{
    public class VprFeedArticlesViewModel : BaseViewModel<int>
    {
        private readonly IViewPointReaderRepository _viewPointReaderRepository;
        public ObservableCollection<VprFeedItem> FeedItems { get; }

        public VprFeedArticlesViewModel(IViewPointReaderRepository viewPointReaderRepository, INavService navService)
            :base(navService)
        {
            _viewPointReaderRepository = viewPointReaderRepository;
            NavService = navService;
            FeedItems = new ObservableCollection<VprFeedItem>();
        }

        public override async Task Init(int subscriptionId)
        {
            await LoadFeedItems(subscriptionId);
        }

        public async Task LoadFeedItems(int subscriptionId)
        {
            var feedItems = await _viewPointReaderRepository.GetFeedItemsForFeedAsync(subscriptionId);
            FeedItems.Clear();
            feedItems.ToList().ForEach(FeedItems.Add);
        }
    }
}
