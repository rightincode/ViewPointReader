﻿using System;
using System.Globalization;
using ViewPointReader.Core.Interfaces;
using ViewPointReader.Data.Interfaces;
using ViewPointReader.Interfaces;
using ViewPointReader.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ViewPointReader.Rss.Interfaces;

namespace ViewPointReader.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VprSearchView : ContentPage
    {
        private readonly IViewPointRssReader _viewPointRssReader = ((App)Application.Current).ServiceProvider.GetService<IViewPointRssReader>();

        //TODO: protect from usage directly from this codebehind file
        private readonly IViewPointReaderRepository _viewPointReaderRepository = ((App)Application.Current).ServiceProvider.GetService<IViewPointReaderRepository>();
        private readonly INavService _navService = ((App) Application.Current).ServiceProvider.GetService<INavService>();

        public VprSearchViewModel Vm { get; }

        public VprSearchView()
        {
            InitializeComponent();

            Vm = new VprSearchViewModel(_viewPointRssReader, _viewPointReaderRepository, _navService);

            BindingContext = Vm;
        }

        public async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await Vm.SubscribeToFeed((IFeedSubscription) e.Item);

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
        
    }
}
