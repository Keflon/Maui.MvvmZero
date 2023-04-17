 using FunctionZero.Maui.MvvmZero;
using FunctionZero.Maui.MvvmZero.Workaround;
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
                .UseMvvmZero(config =>
                {
                    config.MapVmToPage<FlyoutFlyoutPageVm, FlyoutFlyoutPage>();
                    config.MapVmToPage<TestPageVm>(GetPageForTestPage);
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
               .AddSingleton<FlyoutPage>()
               // because https://github.com/dotnet/maui/issues/14572
               .AddSingleton<AdaptedTabbedPage>()
               .AddSingleton<AdaptedFlyoutPage>()               
               .AddSingleton<HomePage>()
               .AddSingleton<HomePageVm>()
               .AddSingleton<FlyoutFlyoutPage>()
               .AddSingleton<FlyoutFlyoutPageVm>()
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

        private static IView GetPageForTestPage(ViewMapperParameters arg)
        {
            // Use arg to decide what type of page instance to return.
            return (IView)arg.PageService.GetPage<TestPage>();
        }
    }
}