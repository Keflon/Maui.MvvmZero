using FunctionZero.Maui.MvvmZero;
using FunctionZero.Maui.Controls;
using Microsoft.Extensions.Logging;
using SampleFlyoutApp.Mvvm.Pages;
using SampleFlyoutApp.Mvvm.Pages.Root;
using SampleFlyoutApp.Mvvm.PageViewModels;
using SampleFlyoutApp.Mvvm.PageViewModels.Root;
using FunctionZero.Maui.Services;
using static FunctionZero.Maui.Services.TranslationService;

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
                    config.MapVmToView<DefaultFlyoutPageVm, DefaultFlyoutPage>();
                    config.MapVmToView<TestAnimationZeroPageVm, TestAnimationZeroPage>();
                    
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

               .AddSingleton<DefaultFlyoutPageVm>()
               .AddSingleton<DefaultFlyoutPage>()


               // TestPage/Vm are transient because there can be more than one on a navigation stack at any time.
               .AddTransient<TestPage>()
               .AddTransient<TestPageVm>()


               .AddSingleton<TestAnimationZeroPageVm>()
               .AddSingleton<TestAnimationZeroPage>()

            // Services ...
            .AddSingleton<TranslationService>(GetConfiguredLanguageService);

            ;


            return builder.Build();
        }

        private static IView GetPageForTestVm(ViewMapperParameters arg)
        {
            // Use arg to decide what type of page instance to return.
            return (IView)arg.PageService.GetView<TestPage>();
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
    }
}