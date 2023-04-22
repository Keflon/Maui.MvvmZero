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

            var rootPage = pageService.GetMultiPage(VmInitializer, typeof(ReadyPageVm), typeof(SteadyPageVm), typeof(GoPageVm));
            var a = new ContentPage();
            MainPage = rootPage;
        }
        private bool VmInitializer(object viewModel)
        {
            if (viewModel is ReadyPageVm)
                return false; // Do not wrap the ReadyPage in a NavigationPage.

            return true;
        }
    }
}