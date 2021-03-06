﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ViewPointReader.Core.Interfaces;
using ViewPointReader.Data.Interfaces;
using ViewPointReader.Interfaces;

namespace ViewPointReader.ViewModels
{
    public class VprSubscribedViewModel : BaseViewModel
    {
        private readonly IViewPointReaderRepository _viewPointReaderRepository;

        public ObservableCollection<IFeedSubscription> FeedSubscriptions { get; }

        public VprSubscribedViewModel(IViewPointReaderRepository viewPointReaderRepository
            , INavService navService) : base(navService)
        {
            _viewPointReaderRepository = viewPointReaderRepository;
            NavService = navService;
            FeedSubscriptions = new ObservableCollection<IFeedSubscription>();
        }

        public override async Task Init()
        {
            await LoadSubscribedFeeds();
        }

        public async Task LoadSubscribedFeeds()
        {
            var feeds = await _viewPointReaderRepository.GetFeedSubscriptionsAsync();
            FeedSubscriptions.Clear();
            feeds.ToList().ForEach(FeedSubscriptions.Add);
        }

        public async Task HandleFeedSelection(int feedId)
        {
            await NavService.NavigateTo<VprFeedArticlesViewModel, int>(feedId);
        }
    }
}
