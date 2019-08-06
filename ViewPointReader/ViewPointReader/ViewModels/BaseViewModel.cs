using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ViewPointReader.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected BaseViewModel() 
        { 
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
        protected BaseViewModel() : base() 
        {
        }
        public override async Task Init()
        {
            await Init (default(TParameter));
        }
        public abstract Task Init (TParameter parameter);
    }
}
