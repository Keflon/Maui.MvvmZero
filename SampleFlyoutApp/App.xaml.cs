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

            var flyout = pageService.GetMvvmPage<FlyoutContentPage, FlyoutContentPageVm>().page;

            //mainPage.Flyout = new NavigationPage(flyout) { Title = "Hello" };
            mainPage.Flyout = flyout;
            mainPage.Flyout.Navigation.PushAsync(pageService.GetMvvmPage<FlyoutContentPage, FlyoutContentPageVm>().page);
            // Detail cannot be null. Will be overwritten immediately after being presented.
            mainPage.Detail = new ContentPage();
            MainPage = mainPage;

            /*
            Something like:

            pageService.PushPageAsync(
                pageService.BuildTabbedPage(
                    pageService.GetMvvmPage<Tab1Page, Tab1PageVm>(),
                    pageService.GetMvvmPage<Tab2Page, Tab2PageVm>(),
                    pageService.GetMvvmPage<Tab3Page, Tab3PageVm>())
                , false
                , true
            );


            
            pageService.PushPageAsync<TabbedPage, TabbedPageViewModel>((vm)=>vm.Init(typeof(Tab1PageVm), typeof(Tab2PageVm), typeof(Tab3PageVm)), blah);

            Where the TabbedPage is registered with
            1. ItemsSource = {Binding ItemsSource}
            2. ItemTemplate = ItemTemplateSelector, where the return value is pageService.TypeFactory(typeof(vm));






            */
        }
    }
}