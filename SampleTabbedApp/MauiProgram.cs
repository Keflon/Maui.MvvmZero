using FunctionZero.Maui.Controls;
using FunctionZero.Maui.MvvmZero;
using Microsoft.Extensions.Logging;
using SampleTabbedApp.Mvvm.Pages;
using SampleTabbedApp.Mvvm.PageViewModels;

namespace SampleTabbedApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMvvmZero(
                serviceBuilder =>
                {
                    serviceBuilder
                        .MapVmToView<ReadyPageVm, ReadyPage>()
                        .MapVmToView<SteadyPageVm, SteadyPage>()
                        .MapVmToView<GoPageVm, GoPage>()
                        ;
                }
                )
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services
                // Get our root page from the container!
                // AdaptedTabbedPage Because https://github.com/dotnet/maui/issues/14572
               .AddSingleton<MultiPage<Page>, AdaptedTabbedPage>()

               .AddSingleton<ReadyPage>()
               .AddSingleton<SteadyPage>()
               .AddSingleton<GoPage>()

               .AddSingleton<ReadyPageVm>()
               .AddSingleton<SteadyPageVm>()
               .AddSingleton<GoPageVm>()

               // TestPage/Vm are transient because there can be more than one on a navigation stack at any time.
               .AddTransient<TestPage>()
               .AddTransient<TestPageVm>()
               ;

            return builder.Build();
        }
    }
}