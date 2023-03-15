using Maui.MvvmZero;
using Microsoft.Extensions.Logging;
using SampleApp.Mvvm.Pages;
using SampleApp.Mvvm.PageViewModels;

namespace SampleApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services
                .AddSingleton<IPageServiceZero>(CreatePageService)
                .AddSingleton<NavigationPage>((arg) => new NavigationPage())
                .AddSingleton<HomePageVm>()
                .AddSingleton<HomePage>()
                .AddSingleton<CabbagesPageVm>()
                .AddSingleton<CabbagesPage>()
                .AddSingleton<OnionsPageVm>()
                .AddSingleton<OnionsPage>()
                .AddSingleton<ResultsPageVm>()
                .AddSingleton<ResultsPage>();

            return builder.Build();
        }

        private static IPageServiceZero CreatePageService(IServiceProvider arg)
        {
            var retval = new PageServiceZero
            (
                () => App.Current.MainPage.Navigation,
                (type) => arg.GetService(type)
            );

            return retval;
        }
    }
}