using FunctionZero.Maui.MvvmZero;
using FunctionZero.Maui.Services;
using SampleFlyoutApp.Mvvm.Pages.Root;
using SampleFlyoutApp.Mvvm.PageViewModels.Root;

namespace SampleFlyoutApp
{
    public partial class App : Application
    {
        public App(IPageServiceZero pageService)
        {
            InitializeComponent();

            pageService.Init(this);

            // Uses AdaptedFlyoutPage because https://github.com/dotnet/maui/issues/13496
            var flyoutPage = pageService.GetFlyoutPage<FlyoutFlyoutPageVm>();
            MainPage = flyoutPage;
        }
    }
}
