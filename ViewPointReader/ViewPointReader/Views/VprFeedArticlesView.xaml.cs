using ViewPointReader.Core.Models;
using ViewPointReader.Data.Interfaces;
using ViewPointReader.Interfaces;
using ViewPointReader.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ViewPointReader.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VprFeedArticlesView : ContentPage
    {
        //TODO: protect from usage directly from this codebehind file
        private readonly IViewPointReaderRepository _viewPointReaderRepository = ((App)Application.Current).ServiceProvider.GetService<IViewPointReaderRepository>();
        private readonly INavService _navService = ((App) Application.Current).ServiceProvider.GetService<INavService>();

        public VprFeedArticlesViewModel Vm { get; set; }

        public VprFeedArticlesView()
        {
            InitializeComponent();
            Vm = new VprFeedArticlesViewModel(_viewPointReaderRepository, _navService);
            BindingContext = Vm;
        }

        public async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;
            
            var feedItem = (VprFeedItem) e.Item;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet && !string.IsNullOrEmpty(feedItem.Link))
            {
                await Browser.OpenAsync(feedItem.Link, BrowserLaunchMode.SystemPreferred);
                
            }
            
            //Deselect Item
            ((ListView)sender).SelectedItem = null;

            //TODO: display msg: no link available
            
        }
    }
}
