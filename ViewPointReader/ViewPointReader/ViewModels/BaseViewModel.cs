using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ViewPointReader.Interfaces;

namespace ViewPointReader.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public INavService NavService { get; protected set; } 

        protected BaseViewModel(INavService navService)
        {
            NavService = navService;
        }
        
        protected virtual void OnPropertyChanged(
            [CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, 
                new PropertyChangedEventArgs(propertyName));
        }

        public abstract Task Init();
    }

    public abstract class BaseViewModel<TParameter> : BaseViewModel
    {
        protected BaseViewModel(INavService navService) : base(navService)
        {
        }

        public override async Task Init()
        {
            await Init (default);
        }
        public abstract Task Init (TParameter parameter);
    }
}
