using System;
using System.ComponentModel;
using System.Threading.Tasks;
using ViewPointReader.ViewModels;
using Xamarin.Forms;

namespace ViewPointReader.Interfaces
{
    public interface INavService
    {
        INavigation Navigation { get; set; }

        void RegisterViewMapping(Type viewModel, Type view);
        bool CanGoBack { get; }
        Task GoBack();
        Task NavigateTo<TVM>() 
            where TVM : BaseViewModel;
        Task NavigateTo<TVM, TParameter>(TParameter parameter) 
            where TVM : BaseViewModel<TParameter>;
        void RemoveLastView();
        void ClearBackStack();
        void NavigateToUri(Uri uri);
        event PropertyChangedEventHandler CanGoBackChanged;
    }
}
