using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ViewPointReader.Data.Interfaces;
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
        
        public VprSubscribedViewModel VM { get; }

        public VprSubscribedView()
        {
            InitializeComponent();
            //On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            VM = new VprSubscribedViewModel(_viewPointReaderRepository);
            BindingContext = VM;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
