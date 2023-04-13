using FunctionZero.Maui.MvvmZero;
using FunctionZero.Maui.MvvmZero.Workaround;
using SampleTabbedApp.Mvvm.PageViewModels;
using System.Collections;

namespace SampleTabbedApp
{
    public partial class App : Application
    {
        private readonly IPageServiceZero _pageService;

        //public App(TabbedPage rootPage, IPageServiceZero pageService)
        //{
        //    InitializeComponent();

        //    pageService.Init(this);

        //    // Construct your TabbedPage any way you like. This pattern keeps it all MVVM.
        //    rootPage.Children.Add(new NavigationPage(pageService.GetMvvmPage<ReadyPage, ReadyPageVm>().page) { Title="Ready"});
        //    rootPage.Children.Add(new NavigationPage(pageService.GetMvvmPage<SteadyPage, SteadyPageVm>().page) { Title = "Steady" });
        //    rootPage.Children.Add(new NavigationPage(pageService.GetMvvmPage<GoPage, GoPageVm>().page) { Title = "Go"});

        //    MainPage = rootPage;
        //}

        public App(IPageServiceZero pageService)
        {
            _pageService = pageService;
            InitializeComponent();

            pageService.Init(this);

            var rootPage = pageService.GetMultiPage<AdaptedTabbedPage>(typeof(ReadyPageVm), typeof(SteadyPageVm), typeof(GoPageVm));
            //var rootPage = pageService.GetMultiPage<TabbedPage>(typeof(ReadyPageVm), typeof(SteadyPageVm), typeof(GoPageVm));

            Dispatcher.StartTimer(TimeSpan.FromMilliseconds(500), TimerTick);

            MainPage = rootPage;
        }

        private bool TimerTick()
        {
            var tp = (AdaptedTabbedPage)MainPage;
            var items = (IList)tp.ItemsSource;
            if(items.Count == 3)
            {
                items.RemoveAt(1);
            }
            else
            {
                items.Insert(1, _pageService.TypeFactory(typeof(SteadyPageVm)));
            }


            return true;
        }
    }
}