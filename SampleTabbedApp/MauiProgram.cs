using FunctionZero.Maui.Controls;
using FunctionZero.Maui.MvvmZero;
using FunctionZero.Maui.MvvmZero.Interfaces;
using FunctionZero.Maui.MvvmZero.PageControllers;
using FunctionZero.Maui.Services;
using Microsoft.Extensions.Logging;
using SampleTabbedApp.Mvvm.Pages;
using SampleTabbedApp.Mvvm.PageViewModels;
using static FunctionZero.Maui.Services.TranslationService;

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
                        .MapVmToView<GoPageVm>((parameters) => PageGetter(parameters))
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


            // Services ...
            .AddSingleton<TranslationService>(GetConfiguredLanguageService)

            ;

            return builder.Build();
        }

        #region Language translation setup
        private static TranslationService GetConfiguredLanguageService(IServiceProvider provider)
        {
            var translationService = new TranslationService();
            translationService.RegisterLanguage("English", new LanguageProvider(GetEnglish, "English"));
            translationService.RegisterLanguage("German", new LanguageProvider(GetGerman, "Deutsch"));

            return translationService;
        }

        // Example
        private static string[] GetEnglish() => new string[] { "Hello", "World", "Welcome to the Moasure Playground!" };
        private static string[] GetGerman() => new string[] { "Hallo", "Welt", "Willkommen auf dem Moasure Spielplatz!" };

        #endregion

        private static IView PageGetter(ViewMapperParameters parameters)
        {
            Dictionary<string, Type> viewMapper = new()
            {
                {"Landscape", typeof(GoPage) },
                {"Portrait", typeof(GoPortraitPage) }
            };

            var proxyPage = parameters.PageService.GetIdiomPage(parameters.VmType, viewMapper);
            proxyPage.SetDynamicResource(ProxyPage.IdiomProperty, "IdiomZero");
            DoTheThing(proxyPage);
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