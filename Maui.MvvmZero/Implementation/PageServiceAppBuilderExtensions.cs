﻿using FunctionZero.Maui.MvvmZero.Implementation;
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
            // Add an initialisation service for MAUI to call. Haven't found a use for it yet ...
            appBuilder.Services.AddSingleton<IMauiInitializeService, PageServiceInitializationService>();

            // Build the PageService and add it to the Container ...
            var pageServiceBuilder = new PageServiceBuilder(DefaultNavigationGetter, (type) => appBuilder.Services.BuildServiceProvider().GetService(type));
            // If there is a configuration callback provided by the user, call it.
            configureDelegate?.Invoke(pageServiceBuilder);

            // Generate the PageService ...
            var pageService = pageServiceBuilder.Build();

            // Add it to the Container as a Singleton.
            appBuilder.Services.AddSingleton<IPageServiceZero>(pageService);
            return appBuilder;
        }

        /// <summary>
        /// This ought to find the correct INavigation for most people.
        /// It can be replaced in the UsePageService delegate if necessary.
        /// </summary>
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