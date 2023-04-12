using FunctionZero.Maui.MvvmZero;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Embedding;
using System.Diagnostics.CodeAnalysis;

namespace SampleViewFinderApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UsePageServiceZero()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
		builder.Logging.AddDebug();
#endif

            builder.Services
    .AddSingleton<NavigationPage>((arg) => new NavigationPage())
    //.AddSingleton<HomePageVm>()
    //.AddSingleton<HomePage>()
    //.AddSingleton<CabbagesPageVm>()
    //.AddSingleton<CabbagesPage>()
    //.AddSingleton<OnionsPageVm>()
    //.AddSingleton<OnionsPage>()
    //.AddSingleton<ResultsPageVm>()
    //.AddSingleton<ResultsPage>();

    ;

            return builder.Build();
        }
    }
}
