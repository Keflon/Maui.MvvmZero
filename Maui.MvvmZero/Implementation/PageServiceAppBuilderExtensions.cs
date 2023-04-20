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
            var pageServiceBuilder = new PageServiceBuilder(DefaultNavigationFinder, DefaultMultiPageFinder, (type) => appBuilder.Services.BuildServiceProvider().GetService(type));

            // If there is a configuration callback provided by the user, call it and pass in the pageServiceBuilder.
            configureDelegate?.Invoke(pageServiceBuilder);

            // Generate the PageService.
            var pageService = pageServiceBuilder.Build();

            // Add it to the Container as a Singleton.
            appBuilder.Services.AddSingleton<IPageServiceZero>(pageService);

            // Add a FlyoutController to the Container.
            appBuilder.Services.AddSingleton<IFlyoutController, FlyoutController>();

            // NavigationPage is required to wrap NavigationPage and MultiPage items.
            // It must be transient because there may be multiple navigation stacks,
            // e.g. on Flyout.Detail and each Tab of a TabbedPage.
            appBuilder.Services.AddTransient<NavigationPage>();

            return appBuilder;
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
                // navPage.Navigation is a candidate. Recurse with it.
                return DefaultNavigationFinder(navPage.CurrentPage, navPage.Navigation);

            if (current is Page)
                // If the Page instance is on a nav-stack, lastNavigation will not be null.
                return lastNavigation;

            return null;
        }

        // TODO: What do we want to do here?
        // TODO: If we return page.Navigation when there is no NavigationPage set, we get an INavigation but I don't know from where.
        // TODO: This INavigation accepts pages pushed onto it, but we don't see them anywhere.
        // TODO: Scanning up for a NavigationPage *seems* better.
        // TODO: Problems:
        // TODO: If the stack is FlyoutPage->TabbedPage1[selectedTab]->NavigationPage->TabbedPage2[selectedTab]->ContentPage1
        // TODO: then pushing a page from ContentPage1 will push it onto the TabbedPage1[selectedTab] stack, and won't be seen until 
        // TODO: we pop back to it.
        // TODO: I think it better to return null in this case. 
        //private static INavigation GetNavigationPageForPage(Page page)
        //{
        //    while (page != null)
        //    {
        //        if (page is NavigationPage navigationPage)
        //            return navigationPage.Navigation;
        //        else if (page is MultiPage<Page>)
        //            return null;

        //            page = page.Parent as Page;
        //    }
        //    return null;
        //}
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
