﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ViewPointReader.Core.Interfaces;
using ViewPointReader.Core.Models;
using ViewPointReader.Data.Interfaces;
using ViewPointReader.Data.Models;
using ViewPointReader.Interfaces;
using ViewPointReader.Rss.Interfaces;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace ViewPointReader.ViewModels
{
    public class VprSearchViewModel : BaseViewModel
    {
        private readonly IViewPointRssReader _viewPointRssReader;
        private readonly IViewPointReaderRepository _viewPointReaderRepository;
        private readonly ViewPointReaderCloudRepository _viewPointReaderCloudRepository = 
            new ViewPointReaderCloudRepository("DefaultEndpointsProtocol=https;AccountName=viewpointreaderdb;AccountKey=ryh54Nxxe99Ay2OjpcxfDB8SCTrWAYt0wOgZd01da2gajopsMAnMouvcgFCgwtNRprUWjbJKJzscHVFFTUVg3Q==;TableEndpoint=https://viewpointreaderdb.table.cosmos.azure.com:443/;");

        private string _searchPhrase;
        private bool _isClearSearchButtonVisible;
        private bool _isSearchEnabled;
        private bool _searching;
        private string _currentNetworkStatus;

        public ObservableCollection<IFeedSubscription> SearchResults { get; set; }
        public string SearchPhrase
        {
            get => _searchPhrase;
            set
            {
                _searchPhrase = value;
                OnPropertyChanged("SearchPhrase");

                if (_searchPhrase.Length > 0 && IsSearchEnabled == false)
                {
                    IsSearchEnabled = true;
                }

                if (_searchPhrase.Length == 0)
                {
                    IsSearchEnabled = false;
                }
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

        public bool IsSearchEnabled { get => _isSearchEnabled;
            set
            {
                _isSearchEnabled = value;
                OnPropertyChanged("IsSearchEnabled");
            }
        }
        public bool Searching { get => _searching;
            set
            {
                _searching = value;
                OnPropertyChanged("Searching");
            }
        }

        public string CurrentNetworkStatus
        {
            get => _currentNetworkStatus;
            set
            {
                _currentNetworkStatus = value;
                OnPropertyChanged("CurrentNetworkStatus");
            }
        }
        
        public VprSearchViewModel(IViewPointRssReader viewPointRssReader
            , IViewPointReaderRepository viewPointReaderRepository, INavService navService)
            : base(navService)
        {
            _viewPointRssReader = viewPointRssReader;
            _viewPointReaderRepository = viewPointReaderRepository;
            NavService = navService;

            SearchResults = new ObservableCollection<IFeedSubscription>();
            IsClearSearchButtonVisible = false;
            IsSearchEnabled = false;
            CurrentNetworkStatus = string.Empty;

            Connectivity.ConnectivityChanged += OnConnectivityChanged;
        }

        public override Task Init()
        {
            throw new NotImplementedException();
        }

        public ICommand FeedSearchCommand => new Command( async () => { await Search(); });
        public ICommand ClearResultsCommand => new Command(ClearSearchResults);

        public async Task<int> SubscribeToFeed(IFeedSubscription feed)
        {
            var saveTasks = new List<Task<int>>
            {
                _viewPointReaderRepository.SaveFeedSubscriptionAsync(feed),
                _viewPointReaderCloudRepository.SaveFeedSubscriptionAsync(feed)
            };

            var results =  await Task.WhenAll(saveTasks);
            SearchResults.Remove(feed);
            return results[0];
        }

        protected void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess != NetworkAccess.Internet)
            {
                IsSearchEnabled = false;

                CurrentNetworkStatus = "No network connectivity";
            }
            else
            {
                IsSearchEnabled = true;
                CurrentNetworkStatus = string.Empty;
            }
        }

        private async Task Search()
        {
            Searching = true;
            IsSearchEnabled = false;
            SearchResults.Clear();
            
            var results = new List<FeedSubscription>();

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                results = await _viewPointRssReader.SearchForFeedsAsync(SearchPhrase);
            }

            SearchResults.Clear();

            if (results.Count == 0)
            {
                SearchResults.Add(new FeedSubscription()
                {
                    Title = "No Feeds Found!"
                });

                IsClearSearchButtonVisible = false;
            }
            else
            {
                results.OrderByDescending(fs => fs.RecommendationScore).ToList().ForEach(x => SearchResults.Add(x));
            }

            IsClearSearchButtonVisible = true;
            Searching = false;
            IsSearchEnabled = true;
        }

        private void ClearSearchResults()
        {
            SearchResults.Clear();
            SearchPhrase = string.Empty;
            IsClearSearchButtonVisible = false;
        }

       }
}
