using FunctionZero.Maui.MvvmZero;
using SampleTabbedApp.Mvvm.Pages;
using SampleTabbedApp.Mvvm.PageViewModels;

namespace SampleTabbedApp
{
    public partial class App : Application
    {
        public App(TabbedPage rootPage, IPageServiceZero pageService)
        {
            InitializeComponent();

            pageService.Init(this);

            // Construct your TabbedPage any way you like. This pattern keeps it all MVVM.
            rootPage.Children.Add(new NavigationPage(pageService.GetMvvmPage<ReadyPage, ReadyPageVm>().page) { Title="Ready"});
            rootPage.Children.Add(new NavigationPage(pageService.GetMvvmPage<SteadyPage, SteadyPageVm>().page) { Title = "Steady" });
            rootPage.Children.Add(new NavigationPage(pageService.GetMvvmPage<GoPage, GoPageVm>().page) { Title = "Go"});

            MainPage = rootPage;
        }
    }
}