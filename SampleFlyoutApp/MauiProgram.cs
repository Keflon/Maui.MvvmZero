using FunctionZero.Maui.MvvmZero;
using FunctionZero.Maui.Controls;
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
                    config.MapVmToView<FlyoutFlyoutPageVm, FlyoutFlyoutPage>();
                    config.MapVmToView<TestPageVm>(GetPageForTestVm);

                    config.MapVmToView<HomePageVm, HomePage>();
                    config.MapVmToView<ListPageVm, ListPage>();
                    config.MapVmToView<TreePageVm, TreePage>();

                    //config.SetFlyoutFactory(() => new FlyoutPage());  // Test SetFlyoutFactory by swapping AdaptedFlyoutPage for FlyoutPage.

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
               //.AddSingleton<TabbedPage>()
               .AddSingleton<MultiPage<Page>, AdaptedTabbedPage>()
               .AddSingleton<FlyoutPage, AdaptedFlyoutPage>()               
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

        private static IView GetPageForTestVm(ViewMapperParameters arg)
        {
            // Use arg to decide what type of page instance to return.
            return (IView)arg.PageService.GetView<TestPage>();
        }
    }
}