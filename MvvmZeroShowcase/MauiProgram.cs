using FunctionZero.Maui.MvvmZero;
using Microsoft.Extensions.Logging;
using MvvmZeroShowcase.Mvvm.Pages;
using MvvmZeroShowcase.Mvvm.Pages.Root;
using MvvmZeroShowcase.Mvvm.PageViewModels;
using MvvmZeroShowcase.Mvvm.PageViewModels.Root;

namespace MvvmZeroShowcase
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
                    config.MapVmToPage<FlyoutPlaceholderContentPageVm, FlyoutPlaceholderContentPage>();
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
                .AddSingleton<FlyoutPage>()
                .AddSingleton<FlyoutFlyoutPage>()
                .AddSingleton<FlyoutFlyoutPageVm>()
                .AddSingleton<FlyoutPlaceholderContentPage>()
                .AddSingleton<FlyoutPlaceholderContentPageVm>()
                .AddSingleton<IntroductionPage>()
                .AddSingleton<IntroductionPageVm>()



                ;
            return builder.Build();
        }
    }
}