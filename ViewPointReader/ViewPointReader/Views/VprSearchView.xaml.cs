using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
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

        public VprSearchViewModel VprSearchViewModel { get; }

        public VprSearchView()
        {
            InitializeComponent();

            VprSearchViewModel = new VprSearchViewModel(_viewPointRssReader);

            BindingContext = this;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            //await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
