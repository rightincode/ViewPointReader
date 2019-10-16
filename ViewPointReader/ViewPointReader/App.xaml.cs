using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using ViewPointReader.CognitiveServices;
using ViewPointReader.CognitiveServices.Interfaces;
using ViewPointReader.Rss;
using ViewPointReader.Rss.Interfaces;
using Xamarin.Forms;
using ViewPointReader.Views;
using ViewPointReader.Data.Models;
using ViewPointReader.Data.Interfaces;
using ViewPointReader.Core.Interfaces;
using ViewPointReader.Core.Models;
using ViewPointReader.Interfaces;
using ViewPointReader.Services;
using ViewPointReader.ViewModels;

namespace ViewPointReader
{
    public partial class App : Application
    {
        public readonly IFileHelper FileHelper = DependencyService.Get<IFileHelper>();
        //public ModelBuilder.ModelBuilder ModelBuilder;

        public IServiceProvider ServiceProvider { get; private set; }

        public List<IFeedSubscription> RecommendedFeeds { get; set; }

        public App()
        {
            InitializeComponent();
            StartupConfiguration();
            
            MainPage = new NavigationPage(new MainPage());
            ConfigureNavService();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private void StartupConfiguration()
        {
            var services = new ServiceCollection();
            services.AddTransient<IViewPointRssReader, ViewPointRssReader>();
            services.AddTransient<IFeedSubscription, FeedSubscription>();
            services.AddTransient<IViewPointReaderRepository>(s => new ViewPointReaderRepository(FileHelper));
            services.AddTransient<IVprWebSearchClient>(s => new VprWebSearchClient("62212ab381824133b4f2dfbeef5ddfb7")); //TODO: must secure key
            services.AddTransient<IVprTextAnalyticsClient>(s => new VprTextAnalyticsClient("0ae5b7dd8d584b3196516ce807b9aa4e")); //TODO: must secure key
            services.AddSingleton<INavService, VprNavigationService>();
            ServiceProvider = services.BuildServiceProvider();

            UpdateVprRecommendationModel();
        }

        private void UpdateVprRecommendationModel()
        {
            //ModelBuilder = new ModelBuilder.ModelBuilder(FileHelper);
            //ModelBuilder.BuildModel();
        }

        private void ConfigureNavService()
        {
            var navService = (INavService)ServiceProvider.GetService(typeof(INavService));

            if (navService == null) return;
            navService.Navigation = MainPage.Navigation;

            navService.RegisterViewMapping(typeof(VprSubscribedViewModel), typeof(VprSubscribedView));
            navService.RegisterViewMapping(typeof(VprSearchViewModel), typeof(VprSearchView));
            navService.RegisterViewMapping(typeof(VprFeedArticlesViewModel), typeof(VprFeedArticlesView));
        }
    }
}
