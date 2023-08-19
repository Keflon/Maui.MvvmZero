using FunctionZero.Maui.Controls;
using FunctionZero.Maui.MvvmZero;
using FunctionZero.Maui.MvvmZero.Interfaces;
using FunctionZero.Maui.MvvmZero.PageControllers;
using FunctionZero.Maui.Services;
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
                        //.MapVmToView<GoPageVm, GoPage>()
                        .MapVmToView<GoPageVm>((thing) => PageGetter(thing))
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
               .AddSingleton<GoPortraitPage>()

               .AddSingleton<ReadyPageVm>()
               .AddSingleton<SteadyPageVm>()
               .AddSingleton<GoPageVm>()

               .AddSingleton<IDisplayService, DisplayService>()


               .AddTransient<ProxyPage>()

               // TestPage/Vm are transient because there can be more than one on a navigation stack at any time.
               .AddTransient<TestPage>()
               .AddTransient<TestPageVm>()
               ;

            return builder.Build();
        }

        //private static IView PageGetter(ViewMapperParameters thing)
        //{
        //    // Not really a ViewModel!
        //    var proxy = (ProxyPage)(thing.PageService.GetViewModel<ProxyPage>());

        //    var innerPage1 = thing.PageService.GetView<GoPage>();
        //    var innerPage2 = thing.PageService.GetView<TestPage>();

        //    DoTheThing(proxy, innerPage1, innerPage2);

        //    proxy.CurrentPage = innerPage2;
        //    return (IView)proxy;

        //}
        private static IView PageGetter(ViewMapperParameters parameters)
        {
            Dictionary<string, Type> viewMapper = new()
            {
                {"Landscape", typeof(GoPage) },
                {"Portrait", typeof(GoPortraitPage) }
            };

            var proxyPage = parameters.PageService.GetIdiomPage(parameters.VmType, viewMapper);
            return proxyPage;
        }

        private static async void DoTheThing(ProxyPage proxy)
        {
            while (true)
            {
                await Task.Delay(3000);
                proxy.Resources["IdiomZero"] = "Landscape";
                await Task.Delay(3000);
                proxy.Resources["IdiomZero"] = "Portrait";

            }
        }
    }
}