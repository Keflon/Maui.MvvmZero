using FunctionZero.Maui.MvvmZero;
using FunctionZero.Maui.MvvmZero.Workaround;
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
                .UsePageServiceZero(
                serviceBuilder =>
                {
                    serviceBuilder
                        .MapVmToPage<ReadyPageVm, ReadyPage>(true)
                        .MapVmToPage<SteadyPageVm, SteadyPage>(true)
                        .MapVmToPage<GoPageVm, GoPage>(true)
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
               .AddSingleton<AdaptedTabbedPage>()
               .AddSingleton<TabbedPage>()

               .AddTransient<ReadyPage>()
               .AddTransient<SteadyPage>()
               .AddTransient<GoPage>()

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