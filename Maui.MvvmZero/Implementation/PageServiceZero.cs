/*
MIT License

Copyright(c) 2016 - 2023 Function Zero Ltd

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FunctionZero.Maui.MvvmZero
{
    public class PageServiceZero : IPageServiceZero
    {
        bool _report = false;

        private readonly Func<INavigation> _navigationGetter;
        public Func<Type, object> TypeFactory { get; }

        private INavigation CurrentNavigationPage => _navigationGetter();

        private readonly List<Page> _pagesOnAnyNavigationStack;
        private readonly List<Page> _currentVisiblePageList;

        /// <summary>
        /// Creates a PageServiceZero associated with the provided NavigationPage.
        /// Uses a Func to get the INavigation for Push operations to allow
        /// multiple nav stacks when using a Flyout page or similar architecture.
        /// </summary>
        /// <param name="navigationGetter">A Func that returns the navigationPage to push to and pop from.</param>
        /// <param name="typeFactory">A Func that returns a requested type. Wire it directly to your IoC container if you have one.</param>
        public PageServiceZero(Func<INavigation> navigationGetter, Func<Type, object> typeFactory)
        {
            _navigationGetter = navigationGetter;
            TypeFactory = typeFactory;

            _pagesOnAnyNavigationStack = new();
            _currentVisiblePageList = new();
        }

        public void Init(Application currentApplication)
        {
            if (currentApplication == null)
                throw new ArgumentNullException(nameof(currentApplication));

            currentApplication.DescendantAdded += CurrentApplication_DescendantAdded;
            currentApplication.DescendantRemoved += CurrentApplication_DescendantRemoved;

            currentApplication.ModalPushed += CurrentApplication_ModalPushed;
            currentApplication.ModalPopped += CurrentApplication_ModalPopped;
        }

        private void CurrentApplication_DescendantAdded(object sender, ElementEventArgs e)
        {
            if (e.Element is Page cp)
            {

                if (_report) Debug.WriteLine($"Descendant Added: {cp}");

                cp.Disappearing += PageDisappearing;
                cp.Appearing += PageAppearing;

                var hop = cp.BindingContext as IHasOwnerPage;
                hop?.OnOwnerPageAddedToVisualTree();

                if (cp.Navigation != null)
                {
                    bool isOnNavigationStack = cp.Navigation.NavigationStack.Contains(cp);
                    bool isOnAnyNavigationStack = _pagesOnAnyNavigationStack.Contains(cp);

                    if (isOnNavigationStack && (!isOnAnyNavigationStack))
                    {
                        _pagesOnAnyNavigationStack.Add(cp);
                        hop?.OnOwnerPagePushed(false);
                    }
                    else if (!isOnNavigationStack && isOnAnyNavigationStack)
                        throw new InvalidOperationException($"Page {cp} is 'counted' when not on navigation stack!");
                }
            }
        }

        private async void CurrentApplication_DescendantRemoved(object sender, ElementEventArgs e)
        {
            if (e.Element is Page cp)
            {
                if (_report) Debug.WriteLine($"Descendant Removed: {cp}");

                var hop = cp.BindingContext as IHasOwnerPage;

                bool isOnNavigationStack = cp.Navigation.NavigationStack.Contains(cp);
                bool isOnAnyNavigationStack = _pagesOnAnyNavigationStack.Contains(cp);

                if ((!isOnNavigationStack) && isOnAnyNavigationStack)
                {
                    _pagesOnAnyNavigationStack.Remove(cp);
                    hop?.OnOwnerPagePopped(false);
                }
                else if (isOnNavigationStack && (!isOnAnyNavigationStack))
                    throw new InvalidOperationException($"Removed Page {cp} is not 'counted' when is on navigation stack!");

                hop?.OnOwnerPageRemovedFromVisualTree();

                cp.Appearing -= PageAppearing;

                await Task.Yield();     // Is this still necessary? Probably. Reason - I expect the disappearing event is raised after DescendantRemoved.

                cp.Disappearing -= PageDisappearing;
            }
        }

        private void PageAppearing(object sender, EventArgs e)
        {
            var page = (Page)sender;

            if (_report) Debug.WriteLine($"Page Appearing: {sender}");
            if (_currentVisiblePageList.Contains(page))
                throw new InvalidOperationException("Page already in _currentVisiblePageList");

            _currentVisiblePageList.Add(page);
            if (page.BindingContext is IHasOwnerPage hop)
                hop.OnOwnerPageAppearing();

        }

        private void PageDisappearing(object sender, EventArgs e)
        {
            var page = (Page)sender;
            if (!_currentVisiblePageList.Contains(page))
                throw new InvalidOperationException("Page not in _currentVisiblePageList");

            _currentVisiblePageList.Remove(page);
            if (_report) Debug.WriteLine($"Page Disappearing: {sender}");
            if (((Page)sender).BindingContext is IHasOwnerPage hop)
                hop.OnOwnerPageDisappearing();
        }

        public int GetVisiblePageCountForVm(object vm)
        {
            return _currentVisiblePageList.Where(page => page.BindingContext == vm).Count();
        }

        private void CurrentApplication_ModalPopped(object sender, ModalPoppedEventArgs e)
        {
            if (_report) Debug.WriteLine($"Modal Removed: {e.Modal}");

            if (e.Modal is Page page)
                if (page.BindingContext is IHasOwnerPage hop)
                    hop.OnOwnerPagePopped(true);
        }

        private void CurrentApplication_ModalPushed(object sender, ModalPushedEventArgs e)
        {
            if (_report) Debug.WriteLine($"Modal Added: {e.Modal}");

            if (e.Modal is Page page)
                if (page.BindingContext is IHasOwnerPage hop)
                    hop.OnOwnerPagePushed(true);
        }


        #region MyRegion

        public (TPage page, TViewModel viewModel) GetMvvmPage<TPage, TViewModel>()
            where TPage : Page
            where TViewModel : class
        {
            TPage page = GetPage<TPage>();
            var vm = GetViewModel<TViewModel>();

            try
            {

                page.BindingContext = vm;

            }
            catch (Exception ex)
            {

                if (_report) Debug.WriteLine(ex.Message);
            }
            return (page, vm);
        }

        public TPage GetPage<TPage>() where TPage : Page
        {
            TPage page = (TPage)TypeFactory(typeof(TPage));
            return page;
        }

        public TViewModel GetViewModel<TViewModel>() where TViewModel : class
        {
            TViewModel vm = (TViewModel)TypeFactory(typeof(TViewModel));
            return vm;
        }

        public async Task<TViewModel> PushPageAsync<TPage, TViewModel>(Func<TViewModel, Task> initViewModelActionAsync, bool isModal, bool animated)
            where TPage : Page
            where TViewModel : class
        {
            var mvvmPage = GetMvvmPage<TPage, TViewModel>();

            if (initViewModelActionAsync != null)
                await initViewModelActionAsync(mvvmPage.viewModel);

            await PushPageAsync(mvvmPage.page, isModal, animated);

            return mvvmPage.viewModel;
        }

        public async Task<TViewModel> PushPageAsync<TPage, TViewModel>(Action<TViewModel> initViewModelAction, bool isModal, bool animated)
            where TPage : Page
            where TViewModel : class
        {
            var mvvmPage = GetMvvmPage<TPage, TViewModel>();

            if (initViewModelAction != null)
                initViewModelAction(mvvmPage.viewModel);

            await PushPageAsync(mvvmPage.page, isModal, animated);

            return mvvmPage.viewModel;
        }

        public async Task PushPageAsync(Page page, bool isModal, bool animated)
        {
            if (isModal == false)
                await CurrentNavigationPage.PushAsync(page, animated);
            else
                await CurrentNavigationPage.PushModalAsync(page, animated);
        }

        public async Task<Page> PushPageAsync<TPage>(Func<TPage, Task> setStateAction, bool isModal = false, bool isAnimated = true) where TPage : Page
        {
            TPage page = GetPage<TPage>();

            await setStateAction(page);
            await PushPageAsync(page, isModal, isAnimated);
            return page;
        }

        // Don't do anything fancy in PopAsync because the system can bypass this method and pop stuff directly.
        public async Task PopAsync(bool isModal, bool animated = true)
        {
            if (!isModal)
                await CurrentNavigationPage.PopAsync(animated);
            else
                await CurrentNavigationPage.PopModalAsync(animated);
        }

        public async Task PopToRootAsync(bool animated = false)
        {
            var navStack = CurrentNavigationPage.NavigationStack;

#if false // This has been fixed.
            // CurrentNavigationPage.PopToRootAsync does not raise OnPageDisappearing on the top page,
            // so we must do it here.
            // Odd, given the appearing / disappearing methods are called when backgrounding, foregrounding etc.
            if (navStack.Count > 1)
            {
                var topPage = navStack[navStack.Count - 1];
                (topPage.BindingContext as IHasOwnerPage)?.OnOwnerPageDisappearing();
            }
#endif
            await CurrentNavigationPage.PopToRootAsync(animated);
        }

        public void RemovePageBelowTop()
        {
            if (CurrentNavigationPage != null)
            {
                int index = CurrentNavigationPage.NavigationStack.Count - 2;
                if (index >= 0)
                {
                    var page = CurrentNavigationPage.NavigationStack[index];
                    CurrentNavigationPage.RemovePage(page);
                }
            }
        }

        public TViewModel FindAncestorPageVm<TViewModel>() where TViewModel : class
        {
            for (int c = CurrentNavigationPage.NavigationStack.Count - 1; c >= 0; c--)
                if (CurrentNavigationPage.NavigationStack[c].BindingContext is TViewModel theVm)
                    return theVm;

            return null;
        }

        //private INavigation GetNavigationForPage(Page thePage)
        //{
        //    Element current = thePage;

        //    while (current != null)
        //    {
        //        if (current is INavigation nav)
        //            return nav;

        //        current = current.Parent;
        //    }
        //    return null;
        //} 
        #endregion
    }
}
