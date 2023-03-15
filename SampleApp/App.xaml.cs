using FunctionZero.Maui.MvvmZero;
using SampleApp.Mvvm.Pages;
using SampleApp.Mvvm.PageViewModels;

namespace SampleApp
{
    public partial class App : Application
    {
        public App(NavigationPage mainPage, IPageServiceZero pageService)
        {
            InitializeComponent();

            pageService.Init(this);
            MainPage = mainPage;
            pageService.PushPageAsync<HomePage, HomePageVm>(vm => vm.Init("Main screen turn on"));
        }
    }
}