﻿using FunctionZero.Maui.MvvmZero.PageControllers;
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
                return GetNavigationPageForPage(page);

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
        // TODO: 
        private static INavigation GetNavigationPageForPage(Page page)
        {
            while (page != null)
            {
                if (page is NavigationPage navigationPage)
                    return navigationPage.Navigation;
                else if (page is MultiPage<Page> multiPage)
                    return null;

                    page = page.Parent as Page;
            }
            return null;
        }
    }
}
