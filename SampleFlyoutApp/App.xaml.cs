using FunctionZero.Maui.MvvmZero;
using SampleFlyoutApp.Mvvm.Pages.Root;
using SampleFlyoutApp.Mvvm.PageViewModels.Root;

namespace SampleFlyoutApp
{
    public partial class App : Application
    {
        //public App(FlyoutPage mainPage, IPageServiceZero pageService)
        //{
        //    InitializeComponent();

        //    pageService.Init(this);

        //    var flyout = pageService.GetMvvmPage<FlyoutContentPage, FlyoutContentPageVm>().page;

        //    //mainPage.Flyout = new NavigationPage(flyout) { Title = "Hello" };
        //    mainPage.Flyout = flyout;
        //    mainPage.Flyout.Navigation.PushAsync(pageService.GetMvvmPage<FlyoutContentPage, FlyoutContentPageVm>().page);
        //    // Detail cannot be null. Will be overwritten immediately after being presented.
        //    mainPage.Detail = new ContentPage();
        //    MainPage = mainPage;
        //}

        public App(IPageServiceZero pageService)
        {
            InitializeComponent();

            pageService.Init(this);

            var flyoutPage = pageService.GetFlyoutPage<FlyoutPage, FlyoutContentPageVm>();
            MainPage = flyoutPage;
        }
    }
}