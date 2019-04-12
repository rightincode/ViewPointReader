using Microsoft.Extensions.DependencyInjection;
using System;
using ViewPointReader.Rss;
using ViewPointReader.Rss.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ViewPointReader.Views;
using ViewPointReader.Data.Models;
using ViewPointReader.Data.Interfaces;
using ViewPointReader.Core.Interfaces;
using ViewPointReader.Core.Models;

namespace ViewPointReader
{
    public partial class App : Application
    {
        private readonly IFileHelper _fileHelper = DependencyService.Get<IFileHelper>();

        public IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            InitializeComponent();
            StartupConfiguration();
            MainPage = new MainPage();
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
            services.AddTransient<IViewPointReaderRepository>(s => new ViewPointReaderRepository(_fileHelper));
            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
