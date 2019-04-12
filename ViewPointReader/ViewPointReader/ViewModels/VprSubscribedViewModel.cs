using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using ViewPointReader.Core.Interfaces;
using ViewPointReader.Data.Interfaces;

namespace ViewPointReader.ViewModels
{
    public class VprSubscribedViewModel : INotifyPropertyChanged
    {
        private readonly IViewPointReaderRepository _viewPointReaderRepository;

        public ObservableCollection<IFeedSubscription> FeedSubscriptions { get; private set; }

        public VprSubscribedViewModel(IViewPointReaderRepository viewPointReaderRepository)
        {
            _viewPointReaderRepository = viewPointReaderRepository;
            FeedSubscriptions = new ObservableCollection<IFeedSubscription>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task LoadSubscribedFeeds()
        {
            FeedSubscriptions =
                new ObservableCollection<IFeedSubscription>(
                    await _viewPointReaderRepository.GetFeedSubscriptionsAsync());
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FeedSubscriptions"));
        }
    }
}
