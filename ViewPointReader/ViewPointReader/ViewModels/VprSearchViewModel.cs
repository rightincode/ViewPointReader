using CodeHollow.FeedReader;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ViewPointReader.Core.Interfaces;
using ViewPointReader.Core.Models;
using ViewPointReader.Data.Interfaces;
using ViewPointReader.Data.Models;
using ViewPointReader.Rss.Interfaces;
using Xamarin.Forms;

namespace ViewPointReader.ViewModels
{
    public class VprSearchViewModel : INotifyPropertyChanged
    {
        private readonly IViewPointRssReader _viewPointRssReader;
        private readonly IViewPointReaderRepository _viewPointReaderRepository;
        private readonly ViewPointReaderCloudRepository _viewPointReaderCloudRepository = 
            new ViewPointReaderCloudRepository("DefaultEndpointsProtocol=https;AccountName=viewpointreaderdb;AccountKey=ryh54Nxxe99Ay2OjpcxfDB8SCTrWAYt0wOgZd01da2gajopsMAnMouvcgFCgwtNRprUWjbJKJzscHVFFTUVg3Q==;TableEndpoint=https://viewpointreaderdb.table.cosmos.azure.com:443/;");
        //private readonly ModelBuilder.ModelBuilder _modelBuilder;
        private string _searchPhrase;
        private bool _isClearSearchButtonVisible;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Feed> SearchResults { get; set; }
        public string SearchPhrase
        {
            get => _searchPhrase;
            set
            {
                _searchPhrase = value;
                OnPropertyChanged("SearchPhrase");
            } }

        public bool IsClearSearchButtonVisible
        {
            get => _isClearSearchButtonVisible;
            set
            {
                _isClearSearchButtonVisible = value;
                OnPropertyChanged("IsClearSearchButtonVisible");
            }
        }
        
        public VprSearchViewModel(IViewPointRssReader viewPointRssReader
            , IViewPointReaderRepository viewPointReaderRepository)
        {
            _viewPointRssReader = viewPointRssReader;
            _viewPointReaderRepository = viewPointReaderRepository;

            SearchResults = new ObservableCollection<Feed>();
            IsClearSearchButtonVisible = false;

            //_modelBuilder = ((App) Application.Current).ModelBuilder;
        }

        public ICommand FeedSearchCommand => new Command( async () => { await Search(); });
        public ICommand ClearResultsCommand => new Command(ClearSearchResults);

        public async Task<int> SaveSubscription(Feed feed)
        {
            var feedSubscription = await CovertFeedToIFeedSubscription(feed);
            var subId = await _viewPointReaderRepository.SaveFeedSubscriptionAsync(feedSubscription);

            await _viewPointReaderCloudRepository.SaveFeedSubscriptionAsync(feedSubscription);

            return subId;
        }

        public void RemoveFeedFromSearchResults(Feed feed)
        {
            SearchResults.Remove(feed);
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private async Task Search()
        {
            SearchResults.Clear();

            SearchResults.Add(new Feed
            {
                Title = "Searching..."
            });

            var results = await _viewPointRssReader.SearchForFeedsAsync(SearchPhrase);

            SearchResults.Clear();

            var scoreAndSaveTasks = new List<Task>();

            foreach (var feed in results)
            {
                SearchResults.Add(feed);
                scoreAndSaveTasks.Add(ScoreAndSave(feed));
            }

            await Task.WhenAll(scoreAndSaveTasks);

            if (SearchResults.Count == 0)
            {
                SearchResults.Add(new Feed
                {
                    Title = "No Feeds Found!"
                });

                IsClearSearchButtonVisible = false;
            }

            IsClearSearchButtonVisible = true;
        }

        private void ClearSearchResults()
        {
            SearchResults.Clear();
            SearchPhrase = string.Empty;
            IsClearSearchButtonVisible = false;
        }
        
        private async Task ScoreAndSave(Feed feed)
        {
            ((App)Application.Current).RecommendedFeeds = new List<IFeedSubscription>();

            var recommendedFeed = await CovertFeedToIFeedSubscription(feed);
            
            //recommendedFeed.RecommendationScore = _modelBuilder.ScoreFeed(recommendedFeed);

            ((App)Application.Current).RecommendedFeeds.Add(recommendedFeed);
        }

        private async Task<IFeedSubscription> CovertFeedToIFeedSubscription(Feed feed)
        {
            var feedSubscription =  new FeedSubscription
            {
                Title = feed.Title,
                Description = feed.Description,
                Url = feed.Link,
                ImageUrl = feed.ImageUrl,
                LastUpdated = feed.LastUpdatedDate,
                SubscribedDate = DateTime.Now,
                FeedItems = new List<VprFeedItem>()
            };
            
            if (!string.IsNullOrEmpty(feedSubscription.Description))
            {
                feedSubscription.KeyPhrases = await _viewPointRssReader.ExtractKeyPhrasesAsync(feedSubscription.Description);
            }

            if (feed.Items.Count == 0) return feedSubscription;

            foreach (var feedItem in feed.Items)
            {
                var vprFeedItem = new VprFeedItem
                {
                    Author = feedItem.Author,
                    Categories = feedItem.Categories,
                    Content = feedItem.Content,
                    Description = feedItem.Description,
                    Link = feedItem.Link,
                    PublishingDate = feedItem.PublishingDate,
                    PublishingDateString = feedItem.PublishingDateString,
                    Title = feedItem.Title
                };

                feedSubscription.FeedItems.Add(vprFeedItem);
            }

            return feedSubscription;
        }
    }
}
