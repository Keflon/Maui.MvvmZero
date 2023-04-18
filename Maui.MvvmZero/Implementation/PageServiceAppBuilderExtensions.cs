using FunctionZero.Maui.MvvmZero.PageControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionZero.Maui.MvvmZero
{
    public static class PageServiceAppBuilderExtensions
    {
        public static MauiAppBuilder UseMvvmZero(this MauiAppBuilder appBuilder, Action<PageServiceBuilder> configureDelegate = null)
        {
            // Add an initialisation service for MAUI to call. Haven't found a use for it yet ...
            appBuilder.Services.AddSingleton<IMauiInitializeService, PageServiceInitializationService>();

            // Create the pageServiceBuilder.
            var pageServiceBuilder = new PageServiceBuilder(DefaultNavigationGetter, (type) => appBuilder.Services.BuildServiceProvider().GetService(type));

            // If there is a configuration callback provided by the user, call it and pass in the pageServiceBuilder.
            configureDelegate?.Invoke(pageServiceBuilder);

            // Generate the PageService.
            var pageService = pageServiceBuilder.Build();

            // Add it to the Container as a Singleton.
            appBuilder.Services.AddSingleton<IPageServiceZero>(pageService);

            // Add a FlyoutController to the Container.
            appBuilder.Services.AddSingleton<IFlyoutController, FlyoutController>();
            return appBuilder;
        }

        /// <summary>
        /// This ought to find the correct INavigation for most people.
        /// It can be replaced in the UsePageService delegate if necessary.
        /// </summary>
        private static INavigation DefaultNavigationGetter()
        {
            return DefaultNavigationGetter(Application.Current.MainPage);
        }

        private static INavigation DefaultNavigationGetter(Page current)
        {
            if (current is FlyoutPage flyoutPage)
                return DefaultNavigationGetter(flyoutPage.Detail);

            if (current is MultiPage<Page> multiPage)
                return DefaultNavigationGetter(multiPage.CurrentPage);

            if (current is Page page)
                return page.Navigation;

            return null;
        }
    }
}
