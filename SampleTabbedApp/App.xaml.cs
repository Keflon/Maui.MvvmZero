using FunctionZero.Maui.MvvmZero;
using FunctionZero.Maui.MvvmZero.Workaround;
using SampleTabbedApp.Mvvm.PageViewModels;
using System.Collections;

namespace SampleTabbedApp
{
    // Use AdaptedTabbedPage over TabbedPage because https://github.com/dotnet/maui/issues/14572

    public partial class App : Application
    {
        public App(IPageServiceZero pageService)
        {
            InitializeComponent();

            pageService.Init(this);

            var rootPage = pageService.GetMultiPage<AdaptedTabbedPage>(VmInitializer, typeof(ReadyPageVm), typeof(SteadyPageVm), typeof(GoPageVm));

            // To modify, e.g. ... ((IList)rootPage.ItemsSource).Add(pageService.TypeFactory(typeof(ReadyPageVm)));

            MainPage = rootPage;
        }

        private bool VmInitializer(object obj)
        {
            return true;
        }
    }
}