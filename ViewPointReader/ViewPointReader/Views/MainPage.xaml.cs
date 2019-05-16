using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewPointReader.Core.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace ViewPointReader.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : Xamarin.Forms.TabbedPage
    {
        public MainPage()
        {
            InitializeComponent();

            On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
        }

        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
            if (((NavigationPage)this.CurrentPage).CurrentPage is VprSubscribedView)
            {
                //reload subscribed feeds
                ((VprSubscribedView)((NavigationPage)this.CurrentPage).CurrentPage).VM.LoadSubscribedFeeds();
            }

            if (((NavigationPage)this.CurrentPage).CurrentPage is VprRecommendedView)
            {
                //reload subscribed feeds
                ((VprRecommendedView)((NavigationPage)this.CurrentPage).CurrentPage).VM.LoadRecommendedFeeds();
            }
        }
    }
}
