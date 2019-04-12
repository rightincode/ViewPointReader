using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CodeHollow.FeedReader;
using ViewPointReader.Core.Interfaces;
using ViewPointReader.Data.Interfaces;
using ViewPointReader.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ViewPointReader.Rss;
using ViewPointReader.Rss.Interfaces;

namespace ViewPointReader.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VprSearchView : ContentPage
    {
        private readonly IViewPointRssReader _viewPointRssReader = ((App)Application.Current).ServiceProvider.GetService<IViewPointRssReader>();

        //TODO: protect from usage directly from this codebehind file
        private readonly IViewPointReaderRepository _viewPointReaderRepository = ((App)Application.Current).ServiceProvider.GetService<IViewPointReaderRepository>();

        public VprSearchViewModel VprSearchViewModel { get; }

        public VprSearchView()
        {
            InitializeComponent();

            VprSearchViewModel = new VprSearchViewModel(_viewPointRssReader, _viewPointReaderRepository);

            BindingContext = this;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await SaveSubscription((Feed)e.Item);

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private Task<int> SaveSubscription(Feed feed)
        {
            return VprSearchViewModel.SaveSubscription(feed);
        }
    }
}
