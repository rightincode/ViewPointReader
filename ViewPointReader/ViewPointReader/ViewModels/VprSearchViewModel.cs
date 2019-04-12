using CodeHollow.FeedReader;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ViewPointReader.Rss.Interfaces;
using Xamarin.Forms;

namespace ViewPointReader.ViewModels
{
    public class VprSearchViewModel
    {
        private readonly IViewPointRssReader _viewPointRssReader;

        public ObservableCollection<Feed> SearchResults { get; set; }
        public string SearchPhrase { get; set; }

        public VprSearchViewModel(IViewPointRssReader viewPointRssReader)
        {
            _viewPointRssReader = viewPointRssReader;

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

            var results = await _viewPointRssReader.SearchForFeeds(SearchPhrase);

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
    }
}
