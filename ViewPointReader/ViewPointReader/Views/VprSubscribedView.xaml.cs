using ViewPointReader.Core.Models;
using ViewPointReader.Data.Interfaces;
using ViewPointReader.Interfaces;
using ViewPointReader.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ViewPointReader.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VprSubscribedView : ContentPage
    {
        //TODO: protect from usage directly from this codebehind file
        private readonly IViewPointReaderRepository _viewPointReaderRepository = ((App)Application.Current).ServiceProvider.GetService<IViewPointReaderRepository>();
        private readonly INavService _navService = ((App) Application.Current).ServiceProvider.GetService<INavService>();

        public VprSubscribedViewModel VM { get; }

        public VprSubscribedView()
        {
            InitializeComponent();
            //On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            VM = new VprSubscribedViewModel(_viewPointReaderRepository, _navService);
            BindingContext = VM;
        }

        protected override async void OnAppearing()
        {
            if (VM != null)
            {
                await VM.Init();
            }
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            //Deselect Item
            ((ListView)sender).SelectedItem = null;

            var subscription = (FeedSubscription) e.Item;
            //await Navigation.PushAsync(new VprFeedArticlesView());

            await _navService.NavigateTo<VprFeedArticlesViewModel, int>(subscription.Id);
        }
    }
}
