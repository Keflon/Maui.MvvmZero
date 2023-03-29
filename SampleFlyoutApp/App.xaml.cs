using FunctionZero.Maui.MvvmZero;
using SampleFlyoutApp.Mvvm.Pages.Root;
using SampleFlyoutApp.Mvvm.PageViewModels.Root;

namespace SampleFlyoutApp
{
    public partial class App : Application
    {
        public App(FlyoutPage mainPage, IPageServiceZero pageService)
        {
            InitializeComponent();

            pageService.Init(this);

            mainPage.Flyout = pageService.GetMvvmPage<FlyoutContentPage, FlyoutContentPageVm>().page;
            // Detail cannot be null. Will be overwritten immediately after being presented.
            mainPage.Detail = new ContentPage();
            MainPage = mainPage;
        }
    }
}