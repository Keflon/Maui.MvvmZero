using FunctionZero.Maui.MvvmZero;
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
                .UsePageServiceZero(builder =>
                {
                    builder.MapVmToPage<CabbagesPageVm, CabbagesPage>();
                })

                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services
                .AddSingleton<NavigationPage>()
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
    }
}