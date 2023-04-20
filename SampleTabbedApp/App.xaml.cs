using FunctionZero.Maui.Controls;
using FunctionZero.Maui.MvvmZero;
using SampleTabbedApp.Mvvm.PageViewModels;
using System.Collections;
using System.Diagnostics;

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
            //var rootPage = pageService.GetMultiPage<TabbedPage>(VmInitializer, typeof(ReadyPageVm), typeof(SteadyPageVm), typeof(GoPageVm));

            MainPage = rootPage;
        }
        private bool VmInitializer(object obj)
        {
            if (obj is ReadyPageVm)
                return false; // Test not wrapping the ReadyPage in a NavigationPage.

            return true;
        }
    }
}