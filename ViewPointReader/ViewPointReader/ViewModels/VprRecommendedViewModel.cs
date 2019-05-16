using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewPointReader.Core.Interfaces;
using ViewPointReader.Data.Interfaces;
using ViewPointReader.Rss.Interfaces;
using Xamarin.Forms;

namespace ViewPointReader.ViewModels
{
    public class VprRecommendedViewModel: INotifyPropertyChanged
    {
        private readonly IViewPointRssReader _viewPointRssReader;
        private readonly IViewPointReaderRepository _viewPointReaderRepository;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<IFeedSubscription> FeedRecommendations { get; private set; }

        public VprRecommendedViewModel(IViewPointRssReader viewPointRssReader, IViewPointReaderRepository viewPointReaderRepository)
        {
            _viewPointRssReader = viewPointRssReader;
            _viewPointReaderRepository = viewPointReaderRepository;

            FeedRecommendations = new ObservableCollection<IFeedSubscription>();
        }

        public async Task<int> SaveSubscription(IFeedSubscription feed)
        {
            if (!string.IsNullOrEmpty(feed.Description))
            {
                feed.KeyPhrases = await _viewPointRssReader.ExtractKeyPhrasesAsync(feed.Description);
            }
            
            return await _viewPointReaderRepository.SaveFeedSubscriptionAsync(feed);
        }

        public void LoadRecommendedFeeds()
        {
            FeedRecommendations.Clear();
            var recommendedFeeds = ((App) Application.Current).RecommendedFeeds?.OrderByDescending(fr => fr.RecommendationScore);

            if (recommendedFeeds == null) return;
            foreach (var feedSubscription in recommendedFeeds)
            {
                FeedRecommendations.Add(feedSubscription);
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FeedRecommendations"));
        }
    }
}
