 using FunctionZero.Maui.MvvmZero;
using Microsoft.Extensions.Logging;
using SampleFlyoutApp.Mvvm.Pages;
using SampleFlyoutApp.Mvvm.Pages.Root;
using SampleFlyoutApp.Mvvm.PageViewModels;
using SampleFlyoutApp.Mvvm.PageViewModels.Root;

namespace SampleFlyoutApp
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
               .AddSingleton<FlyoutPage>()
               .AddSingleton<HomePage>()
               .AddSingleton<HomePageVm>()
               .AddSingleton<FlyoutContentPage>()
               .AddSingleton<FlyoutContentPageVm>()
               .AddSingleton<TreePage>()
               .AddSingleton<TreePageVm>()
               .AddSingleton<ListPage>()
               .AddSingleton<ListPageVm>()
               .AddSingleton<ListPlayPage>()
               .AddSingleton<ListPlayPageVm>()

               // TestPage/Vm are transient because there can be more than one on a navigation stack at any time.
               .AddTransient<TestPage>()
               .AddTransient<TestPageVm>()
               ;


            return builder.Build();
        }

        private static IPageServiceZero CreatePageService(IServiceProvider arg)
        {
            var retval = new PageServiceBuilder().
                // A lambda to get the current navigation page.
                SetNavigationGetter(() => ((FlyoutPage)App.Current.MainPage).Detail.Navigation)
                // A lambda to get instances of pages and viewmodels.
                // This is a proxy to the IoC container, and that is where lifetime (Singleton / Transient) is set.
                .SetTypeFactory((type) => arg.GetService(type))
                .Build();

            return retval;
        }
    }
}