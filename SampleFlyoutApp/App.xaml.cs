using FunctionZero.Maui.MvvmZero;
using FunctionZero.Maui.MvvmZero.Workaround;
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

            // Use AdaptedFlyoutPage because https://github.com/dotnet/maui/issues/13496
            //var flyoutPage = pageService.GetFlyoutPage<FlyoutPage, FlyoutFlyoutPageVm>();
            var flyoutPage = pageService.GetFlyoutPage<AdaptedFlyoutPage, FlyoutFlyoutPageVm>();
            MainPage = flyoutPage;
        }
    }
}
