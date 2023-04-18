using FunctionZero.Maui.MvvmZero;
using MvvmZeroShowcase.Mvvm.Pages.Root;
using MvvmZeroShowcase.Mvvm.PageViewModels.Root;

namespace MvvmZeroShowcase
{
    public partial class App : Application
    {
        public App(IPageServiceZero pageService)
        {
            InitializeComponent();

            MainPage = pageService.GetFlyoutPage<FlyoutFlyoutPageVm, FlyoutPlaceholderContentPageVm>();
        }
    }
}