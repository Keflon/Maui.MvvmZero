using FunctionZero.Maui.MvvmZero;

namespace SampleFlyoutApp
{
    public partial class App : Application
    {
        public App(FlyoutPage mainPage, IPageServiceZero pageService)
        {
            InitializeComponent();

            pageService.Init(this);
            MainPage = mainPage;
        }
    }
}