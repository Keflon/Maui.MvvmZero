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
               .AddSingleton<FlyoutPage>(CreateRootPage)
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

               .AddTransient<TestPage>()
               .AddTransient<TestPageVm>()
               ;


            return builder.Build();
        }

        private static FlyoutPage CreateRootPage(IServiceProvider arg)
        {
            var pageService = arg.GetService<IPageServiceZero>();

            var root = new FlyoutPage();
            root.Title = "Hello";

            return root;
        }

        private static IPageServiceZero CreatePageService(IServiceProvider arg)
        {
            var retval = new PageServiceZero
            (
                // A lambda to get the current navigation page.
                () => ((FlyoutPage)App.Current.MainPage).Detail.Navigation,

                // A lambda to get instances of pages and viewmodels.
                // This is a proxy to the IoC container, and that is where lifetime (Singleton / Transient) is set.
                (type) => arg.GetService(type)
            );

            return retval;
        }
    }
}