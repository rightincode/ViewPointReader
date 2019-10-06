using System;
using System.Threading.Tasks;
using ViewPointReader.Interfaces;

namespace ViewPointReader.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        public override Task Init()
        {
            throw new NotImplementedException();
        }

        public MainPageViewModel(INavService navService) : base(navService)
        {
            NavService = navService;
        }
    }
}
