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

        public VprSubscribedViewModel Vm { get; }

        public VprSubscribedView()
        {
            InitializeComponent();
            //On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            Vm = new VprSubscribedViewModel(_viewPointReaderRepository, _navService);
            BindingContext = Vm;
        }

        protected override async void OnAppearing()
        {
            if (Vm != null)
            {
                await Vm.Init();
            }
        }

        public async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            //Deselect Item
            ((ListView)sender).SelectedItem = null;

            var subscription = (FeedSubscription) e.Item;
            await Vm.HandleFeedSelection(subscription.Id);
        }
    }
}
