using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewPointReader.Core.Interfaces;
using ViewPointReader.Data.Interfaces;
using ViewPointReader.Rss.Interfaces;
using ViewPointReader.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ViewPointReader.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VprRecommendedView : ContentPage
    {
        private readonly IViewPointRssReader _viewPointRssReader = ((App)Application.Current).ServiceProvider.GetService<IViewPointRssReader>();

        //TODO: protect from usage directly from this codebehind file
        private readonly IViewPointReaderRepository _viewPointReaderRepository = ((App)Application.Current).ServiceProvider.GetService<IViewPointReaderRepository>();

        public VprRecommendedViewModel VM { get; }

        public VprRecommendedView()
        {
            InitializeComponent();

            VM = new VprRecommendedViewModel(_viewPointRssReader, _viewPointReaderRepository);
            BindingContext = VM;
        }

        public async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await SaveSubscription((IFeedSubscription)e.Item);

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private async Task SaveSubscription(IFeedSubscription feed)
        {
            await VM.SaveSubscription(feed);
        }
    }
}