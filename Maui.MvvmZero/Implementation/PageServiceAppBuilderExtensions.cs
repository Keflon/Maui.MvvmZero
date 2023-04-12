using FunctionZero.Maui.MvvmZero.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionZero.Maui.MvvmZero
{
    public static class PageServiceAppBuilderExtensions
    {
        public static MauiAppBuilder UsePageServiceZero(this MauiAppBuilder appBuilder, Action<PageServiceBuilder> configureDelegate = null)
        {
            appBuilder.Services.AddSingleton<IMauiInitializeService, PageServiceInitializationService>();
            var pageServiceBuilder = new PageServiceBuilder(DefaultNavigationGetter, (type) => appBuilder.Services.BuildServiceProvider().GetService(type));
            configureDelegate?.Invoke(pageServiceBuilder);
            var pageService = pageServiceBuilder.Build();
            appBuilder.Services.AddSingleton<IPageServiceZero>(pageService);
            return appBuilder;
        }

        private static INavigation DefaultNavigationGetter()
        {
            if (Application.Current.MainPage is FlyoutPage flyoutPage)
                return flyoutPage.Detail.Navigation; 

            if (Application.Current.MainPage is TabbedPage tabbedPage)
                return tabbedPage.CurrentPage.Navigation; 

            if (Application.Current.MainPage is Page page)
                return page.Navigation;

            return null;
        }
    }
}
