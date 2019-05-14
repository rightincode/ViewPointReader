using CodeHollow.FeedReader;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ViewPointReader.Core.Interfaces;
using ViewPointReader.Core.Models;
using ViewPointReader.Data.Interfaces;
using ViewPointReader.Rss.Interfaces;
using Xamarin.Forms;

namespace ViewPointReader.ViewModels
{
    public class VprSearchViewModel
    {
        private readonly IViewPointRssReader _viewPointRssReader;
        private readonly IViewPointReaderRepository _viewPointReaderRepository;

        public ObservableCollection<Feed> SearchResults { get; set; }
        public string SearchPhrase { get; set; }

        public VprSearchViewModel(IViewPointRssReader viewPointRssReader, IViewPointReaderRepository viewPointReaderRepository)
        {
            _viewPointRssReader = viewPointRssReader;
            _viewPointReaderRepository = viewPointReaderRepository;

            SearchResults = new ObservableCollection<Feed>();
        }

        public ICommand FeedSearchCommand => new Command( async () => { await Search(); });

        private async Task Search()
        {
            SearchResults.Clear();

            SearchResults.Add(new Feed
            {
                Title = "Searching..."
            });

            var results = await _viewPointRssReader.SearchForFeedsAsync(SearchPhrase);

            SearchResults.Clear();

            foreach (var feed in results)
            {
                SearchResults.Add(feed);
            }

            if (SearchResults.Count == 0)
            {
                SearchResults.Add(new Feed
                {
                    Title = "No Feeds Found!"
                });
            }
        }

        public async Task<int> SaveSubscription(Feed feed)
        {
            var feedSubscription = new FeedSubscription
            {
                Title = feed.Title,
                Description = feed.Description,
                Url = feed.Link,
                ImageUrl = feed.ImageUrl,
                LastUpdated = feed.LastUpdatedDate,
                SubscribedDate = DateTime.Now
            };

            if (!string.IsNullOrEmpty(feedSubscription.Description))
            {
                feedSubscription.KeyPhrases = await _viewPointRssReader.ExtractKeyPhrasesAsync(feedSubscription.Description);
            }
            
            //return await _viewPointReaderRepository.SaveFeedSubscriptionAsync(feedSubscription);
            return 0;
        }
    }
}
