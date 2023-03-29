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

               // Get our root page from the container!
               .AddSingleton<TabbedPage>()

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

        private static IPageServiceZero CreatePageService(IServiceProvider arg)
        {
            var retval = new PageServiceZero
            (
                // A lambda to get the current navigation page.
                () => ((TabbedPage)App.Current.MainPage).CurrentPage.Navigation,
                // A lambda to get instances of pages and viewmodels.
                // This is a proxy to the IoC container, and that is where lifetime (Singleton / Transient) is set.
                (type) => arg.GetService(type)
            );

            return retval;
        }
    }
}