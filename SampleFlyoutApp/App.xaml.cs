using FunctionZero.Maui.MvvmZero;
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

            var flyoutPage = pageService.GetFlyoutPage<FlyoutPage, FlyoutFlyoutPageVm>();
            MainPage = flyoutPage;
        }
    }
}