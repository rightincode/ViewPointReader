using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ViewPointReader.Interfaces;
using ViewPointReader.ViewModels;
using Xamarin.Forms;

namespace ViewPointReader.Services
{
    public class VprNavigationService : INavService
    {
        public event PropertyChangedEventHandler CanGoBackChanged;

        public INavigation Navigation { get; set; }

        public readonly IDictionary<Type, Type> ViewMapping = new Dictionary<Type, Type>();

        public void RegisterViewMapping(Type viewModel, Type view)
        {
            ViewMapping.Add(viewModel, view);
        }

        public bool CanGoBack => Navigation.NavigationStack != null && Navigation.NavigationStack.Count > 0;

        public void ClearBackStack()
        {
            if (Navigation.NavigationStack.Count <= 1)
            {
                return;
            }

            for (var x = 0; x < Navigation.NavigationStack.Count - 1; x++)
            {
                Navigation.RemovePage(Navigation.NavigationStack[x]);
            }
        }

        public async Task GoBack()
        {
            if (CanGoBack)
            {
                await Navigation.PopAsync(true);
            }

            OnCanGoBackChanged();
        }

        public async Task NavigateTo<TVM>() where TVM : BaseViewModel
        {
            await NavigateToView(typeof(TVM));
            if (Navigation.NavigationStack.Last().BindingContext is BaseViewModel)
            {
                await ((BaseViewModel)(Navigation
                    .NavigationStack.Last().BindingContext)).Init();
            }
        }

        public async Task NavigateTo<TVM, TParameter>(TParameter parameter) where TVM : BaseViewModel<TParameter>
        {
            await NavigateToView(typeof(TVM));
            if (Navigation.NavigationStack.Last().BindingContext is BaseViewModel<TParameter>)
            {
                await ((BaseViewModel<TParameter>)(Navigation
                    .NavigationStack.Last().BindingContext)).Init(parameter);
            }
        }

        public void NavigateToUri(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentException("Invalid URI");
            }
            Device.OpenUri(uri);
        }

        public void RemoveLastView()
        {
            if (!Navigation.NavigationStack.Any()) return;
            var lastView = Navigation
                .NavigationStack[Navigation.NavigationStack.Count - 2];
            Navigation.RemovePage(lastView);
        }

        private async Task NavigateToView(Type viewModelType)
        {
            if (!ViewMapping.TryGetValue(viewModelType, out var viewType))
            {
                throw new ArgumentException("No view found in View Mapping for " + viewModelType.FullName + ".");
            }
            var constructor = viewType.GetTypeInfo()
                .DeclaredConstructors
                .FirstOrDefault(dc => !dc.GetParameters().Any());
            var view = constructor.Invoke(null) as Page;
            await Navigation.PushAsync(view, true);
        }

        private void OnCanGoBackChanged()
        {
            CanGoBackChanged?.Invoke(this, new PropertyChangedEventArgs("CanGoBack"));
        }
    }
}
