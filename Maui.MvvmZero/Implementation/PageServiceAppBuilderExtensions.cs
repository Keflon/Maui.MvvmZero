using FunctionZero.Maui.MvvmZero.PageControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
            var pageServiceBuilder = new PageServiceBuilder(DefaultNavigationFinder, DefaultMultiPageFinder);

            // If there is a configuration callback provided by the user, call it and pass in the pageServiceBuilder.
            configureDelegate?.Invoke(pageServiceBuilder);

            // Add IPageServiceZero to the Container as a Singleton.
            appBuilder.Services.AddSingleton<IPageServiceZero>((serviceProvider)=>BuildPageService(serviceProvider, pageServiceBuilder));

            // Add a FlyoutController to the Container.
            appBuilder.Services.AddSingleton<IFlyoutController, FlyoutController>();

            // NavigationPage is required to wrap NavigationPage and MultiPage items.
            // It must be transient because there may be multiple navigation stacks,
            // e.g. on Flyout.Detail and each Tab of a TabbedPage.
            appBuilder.Services.AddTransient<NavigationPage>();

            return appBuilder;
        }

        /// <summary>
        /// The PageService needs to know IServiceProvider, unless pageServiceBuilder specifies a replacement.
        /// IServiceProvider is hidden from us prior to the ctor on App completing, so we get to it in a factory method registered to the container.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="pageServiceBuilder"></param>
        /// <returns></returns>
        private static IPageServiceZero BuildPageService(IServiceProvider serviceProvider, PageServiceBuilder pageServiceBuilder)
        {
            if (pageServiceBuilder.HasTypeFactory == false)
                pageServiceBuilder.SetTypeFactory(serviceProvider.GetService);

            var pageService = pageServiceBuilder.Build();

            return pageService;
        }

        #region NavigationFinder
        /// <summary>
        /// This ought to find the correct INavigation for most people.
        /// It can be replaced in the UsePageService delegate if necessary.
        /// Method: Find the current Page instance. 
        /// Method: If it is on a nav stack of a NavigationPage, return the INavigation.
        /// Method: 
        /// </summary>
        private static INavigation DefaultNavigationFinder()
        {
            return DefaultNavigationFinder(Application.Current.MainPage, null);
        }

        private static INavigation DefaultNavigationFinder(Page current, INavigation lastNavigation)
        {
            if (current is FlyoutPage flyoutPage)
                // Reset lastNavigation and recurse.
                return DefaultNavigationFinder(flyoutPage.Detail, null);

            if (current is MultiPage<Page> multiPage)
                // Reset lastNavigation and recurse.
                return DefaultNavigationFinder(multiPage.CurrentPage, null);

            if (current is NavigationPage navPage)
                if (navPage.CurrentPage == null)
                    // If navPage is empty, it's ready to be filled.
                    return navPage.Navigation;
            else
                // navPage.Navigation is a candidate. Recurse with it.
                return DefaultNavigationFinder(navPage.CurrentPage, navPage.Navigation);

            if(current is ProxyPage proxyPage)
                return DefaultNavigationFinder(proxyPage.CurrentPage, proxyPage.Navigation);

            if (current is Page)
                // If the Page instance is on a nav-stack, lastNavigation will not be null.
                return lastNavigation;

            return null;
        }

        #endregion

        #region MultiPageFinder

        private static MultiPage<Page> DefaultMultiPageFinder()
        {
            return DefaultMultiPageFinder(Application.Current.MainPage, null);

        }

        private static MultiPage<Page> DefaultMultiPageFinder(Page current, MultiPage<Page> lastMultiPage)
        {
            if (current is FlyoutPage flyoutPage)
                // Reset lastMultiPage and recurse.
                return DefaultMultiPageFinder(flyoutPage.Detail, null);

            if (current is MultiPage<Page> multiPage)
                // multiPage is a candidate. Recurse with it.
                return DefaultMultiPageFinder(multiPage.CurrentPage, multiPage);

            if (current is NavigationPage navPage)
                // if navPage is multiPage.Current, lastMultiPage is a viable candidate.
                if (navPage == lastMultiPage?.CurrentPage)
                    return DefaultMultiPageFinder(navPage.CurrentPage, lastMultiPage);
                else
                    // SMELL: Try this out and see what happens.
                    // For if a NavigationPage is pushed onto a NavigationPage. (Should never happen?)
                    return DefaultMultiPageFinder(navPage.CurrentPage, null);

            if (current is Page page)
                // lastMultiPage will be non-null only if Page is visible, and belongs to a MultiPage.
                return lastMultiPage;

            return null;
        }

        #endregion
    }
}
