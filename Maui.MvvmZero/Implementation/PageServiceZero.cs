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
using FunctionZero.Maui.Controls;
using FunctionZero.Maui.MvvmZero;
using FunctionZero.Maui.MvvmZero.PageControllers;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FunctionZero.Maui.MvvmZero
{
    public class PageServiceZero : IPageServiceZero
    {
        bool _report = false;
        private readonly FlyoutController _flyoutController;
        private readonly MultiPageController _multiPageController;
        private readonly Func<Type, object, IView> _viewMapper;
        private readonly Func<INavigation> _navigationFinder;
        private readonly Func<MultiPage<Page>> _multiPageFinder;
        private readonly Func<FlyoutPage> _flyoutFactory;

        public Func<Type, object> TypeFactory { get; }


        private IView GetViewForViewModel<TViewModel>(object hint) where TViewModel : class
        {
            return _viewMapper(typeof(TViewModel), hint);
        }
        public IView GetViewForViewModel(Type viewModel, object hint)
        {
            return _viewMapper(viewModel, hint);
        }

        private TInstanceType GetInstance<TInstanceType>()
        {
            return (TInstanceType)GetInstance(typeof(TInstanceType));
        }
        internal object GetInstance(Type instanceType)
        {
            var retval = TypeFactory(instanceType);

            if (retval == null)
                throw new TypeFactoryException($"ERROR: Cannot get an instance of {instanceType}. Make sure you have registered it in your Container!", instanceType);

            return retval;
        }

        private INavigation CurrentNavigationPage => _navigationFinder();
        public IFlyoutController FlyoutController => _flyoutController;
        public IMultiPageController MultiPageController => _multiPageController;

        private readonly List<Page> _pagesOnAnyNavigationStack;
        private readonly List<Page> _currentVisiblePageList;

        /// <summary>
        /// Creates a PageServiceZero associated with the provided NavigationPage.
        /// Uses a Func to get the INavigation for Push operations to allow
        /// multiple nav stacks when using a Flyout page or similar architecture.
        /// </summary>
        /// <param name="navigationFinder">A Func that returns the navigationPage to push to and pop from.</param>
        /// <param name="typeFactory">A Func that returns a requested type. Wire it directly to your IoC container if you have one.</param>
        internal PageServiceZero(Func<Type, object> typeFactory, Func<FlyoutPage> flyoutFactory, Func<INavigation> navigationFinder, Func<MultiPage<Page>> multiPageFinder, Func<Type, object, IView> viewMapper)
        {
            TypeFactory = typeFactory;
            _flyoutFactory = flyoutFactory;
            _navigationFinder = navigationFinder;
            _multiPageFinder = multiPageFinder;
            _viewMapper = viewMapper;

            _pagesOnAnyNavigationStack = new();
            _currentVisiblePageList = new();

            _flyoutController = new FlyoutController(this);
            _multiPageController = new MultiPageController(_multiPageFinder);
        }

        public void Init(Application currentApplication)
        {
            if (currentApplication == null)
                throw new ArgumentNullException(nameof(currentApplication));

            currentApplication.DescendantAdded += CurrentApplication_DescendantAdded;
            currentApplication.DescendantRemoved += CurrentApplication_DescendantRemoved;

            currentApplication.ModalPushed += CurrentApplication_ModalPushed;
            currentApplication.ModalPopped += CurrentApplication_ModalPopped;

            currentApplication.PageAppearing += CurrentApplication_PageAppearing;
            currentApplication.PageDisappearing += CurrentApplication_PageDisappearing;
        }

        private void CurrentApplication_PageDisappearing(object sender, Page e)
        {
            Debug.WriteLine($"CurrentApplication_PageDisappearing: {e}");
        }

        private void CurrentApplication_PageAppearing(object sender, Page e)
        {
            // This is not called when a page is popped! https://github.com/dotnet/maui/issues/14092
            Debug.WriteLine($"CurrentApplication_PageAppearing: {e}");
        }

        private void CurrentApplication_DescendantAdded(object sender, ElementEventArgs e)
        {
            if (e.Element is Page cp)
            {
                if (_report) Debug.WriteLine($"Descendant Added: {cp}");

                if (e.Element is FlyoutPage fp)
                    _flyoutController.SetFlyoutPage(fp);

                cp.Disappearing += PageDisappearing;
                cp.Appearing += PageAppearing;

                var hop = cp.BindingContext as IHasOwnerPage;
                hop?.OnOwnerPageAddedToVisualTree();

                if (cp.Navigation != null)
                {
                    bool isOnNavigationStack = cp.Navigation.NavigationStack.Contains(cp);
                    bool isOnAnyNavigationStack = _pagesOnAnyNavigationStack.Contains(cp);

                    // If the page is on the navigation stack that we're not tracking, it has been pushed.
                    if (isOnNavigationStack && (!isOnAnyNavigationStack))
                    {
                        _pagesOnAnyNavigationStack.Add(cp);
                        hop?.OnOwnerPagePushed(false);
                    }
                    // Otherwise if we're tracking the page when it is not on the navigation stack, something has gone wrong!
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

                if (e.Element is FlyoutPage fp)
                    _flyoutController.SetFlyoutPage(null);

                var hop = cp.BindingContext as IHasOwnerPage;

                bool isOnNavigationStack = cp.Navigation.NavigationStack.Contains(cp);
                bool isOnAnyNavigationStack = _pagesOnAnyNavigationStack.Contains(cp);

                // If the page is not on the navigation stack and we are tracking it, it has been popped.
                if ((!isOnNavigationStack) && isOnAnyNavigationStack)
                {
                    _pagesOnAnyNavigationStack.Remove(cp);
                    hop?.OnOwnerPagePopped(false);
                }
                // Otherwise if the page is on the navigation stack and we're not tracking it, something has gone wrong!
                else if (isOnNavigationStack && (!isOnAnyNavigationStack))
                    throw new InvalidOperationException($"Removed Page {cp} is not 'counted' when is on navigation stack!");

                hop?.OnOwnerPageRemovedFromVisualTree();

                cp.Appearing -= PageAppearing;

                await Task.Yield();     // Reason: The disappearing event is (was?) raised after DescendantRemoved.

                cp.Disappearing -= PageDisappearing;
            }
        }

        private void WalkTree(IVisualTreeElement visualElement, Action<object> doTheThing)
        {
            doTheThing(visualElement);

            foreach (var item in visualElement.GetVisualChildren())
                WalkTree(item, doTheThing);
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

        public int GetVisiblePageCountForVm(object vm)
        {
            return _currentVisiblePageList.Where(page => page.BindingContext == vm).Count();
        }
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
                throw;
            }
            return (page, vm);
        }

        public TPage GetPage<TPage>() where TPage : Page
        {
            TPage page = GetInstance<TPage>();
            return page;
        }

        public TView GetView<TView>() where TView : IView // IView allows Page. View doesn't. => IView can do everything. Is that a good thing?
        {
            TView view = GetInstance<TView>();
            return view;
        }

        public TViewModel GetViewModel<TViewModel>() where TViewModel : class
        {
            return GetInstance<TViewModel>();
        }

        public async Task<TViewModel> PushPageAsync<TPage, TViewModel>(Func<TViewModel, Task> initViewModelActionAsync, bool isModal, bool animated)
            where TPage : Page
            where TViewModel : class
        {
            if (CurrentNavigationPage == null)
                return null;

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
            if (CurrentNavigationPage == null)
                return null;

            // Call the async overload with a synchronous action.
            return await PushPageAsync<TPage, TViewModel>(async (vm) => initViewModelAction(vm), isModal, animated);
        }

        public async Task<bool> PushPageAsync(Page page, bool isModal, bool animated)
        {
            var navigation = CurrentNavigationPage;
            if (navigation == null)
                return false;

            if (isModal == false)
                await CurrentNavigationPage.PushAsync(page, animated);
            else
                await CurrentNavigationPage.PushModalAsync(page, animated);

            return true;
        }

        public async Task<Page> PushPageAsync<TPage>(Func<TPage, Task> setStateAction, bool isModal = false, bool isAnimated = true) where TPage : Page
        {
            if (CurrentNavigationPage == null)
                return null;

            TPage page = GetPage<TPage>();

            await setStateAction(page);
            await PushPageAsync(page, isModal, isAnimated);
            return page;
        }

        public async Task<TViewModel> PushVmAsync<TViewModel>(Action<TViewModel> initViewModelAction, object hint = null, bool isModal = false, bool isAnimated = true) where TViewModel : class
        {
            if (CurrentNavigationPage == null)
                return null;

            var page = (Page)GetViewForViewModel<TViewModel>(hint);
            var vm = GetViewModel<TViewModel>();

            try
            {
                page.BindingContext = vm;
            }
            catch (Exception ex)
            {
                if (_report) Debug.WriteLine(ex.Message);
                throw;
            }
            await PushPageAsync(page, isModal, isAnimated);

            return vm;
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

        public MultiPage<Page> GetMultiPage<TPage>(Func<object, bool> initializer, IEnumerable vmCollection) where TPage : MultiPage<Page>
        {
            var page = GetInstance<TPage>();

            var multiPageItemTemplate = new ViewDataTemplateSelector(initializer, () => GetPage<NavigationPage>(), (viewModelType) => GetViewForViewModel(viewModelType, null));

            // AdaptedTabbedPage Because https://github.com/dotnet/maui/issues/14572
            if (page is AdaptedTabbedPage adaptedPage)
            {
                adaptedPage.ItemTemplate = multiPageItemTemplate;
                adaptedPage.ItemsSource = vmCollection;
            }
            else
            {
                page.ItemTemplate = multiPageItemTemplate;
                page.ItemsSource = vmCollection;
            }
            return page;
        }

        public MultiPage<Page> GetMultiPage<TPage>(Func<object, bool> initializer, params Type[] vmTypes) where TPage : MultiPage<Page>
        {
            var vmCollection = new ObservableCollection<object>();

            foreach (var vmType in vmTypes)
                vmCollection.Add(GetInstance(vmType));

            return GetMultiPage<TPage>(initializer, vmCollection);
        }

        private FlyoutPage GetPartialFlyoutPage<TFlyoutFlyoutVm>()
            where TFlyoutFlyoutVm : class
        {
            FlyoutPage flyoutPage = _flyoutFactory();

            var flyoutFlyoutPage = (Page)GetViewForViewModel<TFlyoutFlyoutVm>(null);
            flyoutFlyoutPage.BindingContext = GetViewModel<TFlyoutFlyoutVm>();
            flyoutPage.Title = flyoutPage.Title ?? string.Empty;
            flyoutFlyoutPage.Title = flyoutFlyoutPage.Title ?? string.Empty;
            flyoutPage.Flyout = flyoutFlyoutPage;
            return flyoutPage;
        }
        public FlyoutPage GetFlyoutPage<TFlyoutFlyoutVm, TFlyoutDetailVm>()
            where TFlyoutFlyoutVm : class
            where TFlyoutDetailVm : class
        {
            var flyoutPage = GetPartialFlyoutPage<TFlyoutFlyoutVm>();
            var flyoutDetailPage = (Page)GetViewForViewModel<TFlyoutDetailVm>(null);
            flyoutDetailPage.BindingContext = GetViewModel<TFlyoutDetailVm>();
            flyoutDetailPage.Title = flyoutDetailPage.Title ?? string.Empty;
            flyoutPage.Detail = flyoutDetailPage;
            return flyoutPage;
        }

        public FlyoutPage GetFlyoutPage<TFlyoutFlyoutVm>()
            where TFlyoutFlyoutVm : class
        {
            var retval = GetPartialFlyoutPage<TFlyoutFlyoutVm>();
            retval.Detail = new ContentPage();

            return retval;
        }
    }
}
