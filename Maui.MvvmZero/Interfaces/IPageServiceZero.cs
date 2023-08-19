/*
MIT License

Copyright(c) 2016 - 2022 Function Zero Ltd

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

using FunctionZero.Maui.MvvmZero.PageControllers;
using System.Collections;

namespace FunctionZero.Maui.MvvmZero
{
    public interface IPageServiceZero
    {
        /// <summary>
        /// Call this after Application.Current is set, 
        /// e.g. from the App.xaml.cs constructor.
        /// </summary>
        /// <param name="currentApplication"></param>
        void Init(Application currentApplication);

        IFlyoutController FlyoutController { get; }
        IMultiPageController MultiPageController { get; }

        /// <summary>
        /// Gets a ViewModel of a given type.
        /// Typically this is delegated to a factory that delegates to your IoC container.
        /// It is then the container's responsibility to provide Singletons or Transient instances.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the return value.</typeparam>
        /// <returns>An instance of the specified type.</returns>
        TViewModel GetViewModel<TViewModel>() where TViewModel : class;

        /// <summary>
        /// Pops a Page from the navigation stack.
        /// </summary>
        /// <param name="isModal">Whether to pop from the modal or non-modal stack.</param>
        /// <param name="animated">Whether to animate the pop.</param>
        /// <returns></returns>
        Task PopAsync(bool isModal, bool animated = true);
        /// <summary>
        /// Pops all Pages from the navigation stack.
        /// </summary>
        /// <param name="isModal">Whether to pop-all from the modal or non-modal stack.</param>
        /// <param name="animated">Whether to animate the pop.</param>
        /// <returns></returns>
        Task PopToRootAsync(bool isModal = false, bool animated = true);
        /// <summary>
        /// Removes the page below the top page on the non-modal stack.
        /// </summary>
        void RemovePageBelowTop();

        /// <summary>
        /// Walks up the non-modal navigation stack looking for a Page with a BindingContext matching the provided ViewModel type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of ViewModel whose Page we're looking for.</typeparam>
        /// <returns>The ViewModel, or null if no match is found.</returns>
        TViewModel FindAncestorPageVm<TViewModel>() where TViewModel : class;

        /// <summary>
        /// Retrieves the number of Pages known about that have the specified ViewModel instance as their BindingContext.
        /// Note that this counts ViewModels across all non-modal navigation stacks, 
        /// e.g. in the case where the root page is a Flyout with multiple Navigation stacks.
        /// Note Different Pages can legitimately share the same viewmodel instance.
        /// </summary>
        /// <param name="vm">The ViewModel instance to match.</param>
        /// <returns>A count of all matches.</returns>
        int GetVisiblePageCountForVm(object vm);
        Task<TViewModel> PushVmAsync<TViewModel>(Action<TViewModel> initViewModelAction, object hint = null, bool isModal = false, bool isAnimated = true) where TViewModel : class;

        IView GetViewForVm(Type viewModel, object hint);

        Func<Type, object> TypeFactory { get; }

        /// <summary>
        /// Makes a TPage with a BindingContext set to a TViewModel
        /// </summary>
        /// <typeparam name="TPage">The type of page to return</typeparam>
        /// <typeparam name="TViewModel">The type of ViewModel found on the page BindingContext</typeparam>
        /// <returns>A Tuple containing a reference to the TPage and a reference to the TViewModel found on the page BindingContext</returns>
        (TPage page, TViewModel viewModel) GetMvvmPage<TPage, TViewModel>()
            where TPage : Page
            where TViewModel : class;

        /// <summary>
        /// Retrieves a Page instance and a ViewModel instance,
        /// sets the Page BindingContext to the ViewModel,
        /// then pushes the Page onto an existing INavigation Page.
        /// </summary>
        /// <typeparam name="TPage">The type of Page to push</typeparam>
        /// <typeparam name="TViewModel">The type of ViewModel to bind to the Page</typeparam>
        /// <param name="initViewModelActionAsync">An async lambda that is called immediately before the Page is pushed.
        /// Use this to provide any initialisation to the ViewModel found in the argument.</param>
        /// <param name="isModal">Whether to push to the Modal stack</param>
        /// <param name="animated">Whether to animate the push</param>
        /// <returns>The ViewModel associated with the Page instance</returns>
        Task<TViewModel> PushPageAsync<TPage, TViewModel>(Func<TViewModel, Task> initViewModelActionAsync, bool isModal = false, bool animated = true)
            where TPage : Page
            where TViewModel : class;

        /// <summary>
        /// Retrieves a Page instance and a ViewModel instance,
        /// sets the Page BindingContext to the ViewModel,
        /// then pushes the Page onto an existing INavigation Page.
        /// </summary>
        /// <typeparam name="TPage">The type of Page to push</typeparam>
        /// <typeparam name="TViewModel">The type of ViewModel to bind to the Page</typeparam>
        /// <param name="initViewModelAction">A lambda that is called immediately before the Page is pushed.
        /// Use this to provide any initialisation to the ViewModel found in the argument.</param>
        /// <param name="isModal">Whether to push to the Modal stack.</param>
        /// <param name="animated">Whether to animate the push.</param>
        /// <returns>The ViewModel associated with the Page instance</returns>
        Task<TViewModel> PushPageAsync<TPage, TViewModel>(Action<TViewModel> initViewModelAction, bool isModal = false, bool animated = true)
            where TPage : Page
            where TViewModel : class;


        /// <summary>
        /// Retrieves a Page instance then pushes the Page onto an existing INavigation Page.
        /// </summary>
        /// <typeparam name="TPage">The type of Page to push</typeparam>
        /// <param name="setStateAction">A lambda that is called immediately before the Page is pushed.
        /// Use this to provide any initialisation to the Page found in the argument.</param>
        /// <param name="isModal">Whether to push to the Modal stack.</param>
        /// <param name="isAnimated">Whether to animate the push.</param>
        /// <returns>The Page that was pushed.</returns>
        Task<Page> PushPageAsync<TPage>(Func<TPage, Task> setStateAction, bool isModal = false, bool isAnimated = true) where TPage : Page;

        Task<bool> PushPageAsync(Page page, bool isModal, bool animated = true);

        /// <summary>
        /// Gets an IView of a given type.
        /// Typically this is delegated to a factory that delegates to your IoC container.
        /// It is then the container's responsibility to provide Singletons or Transient instances.
        /// </summary>
        /// <typeparam name="TView">The type of the return value.</typeparam>
        /// <returns>A page of the specified type.</returns>
        TView GetView<TView>() where TView : IView;


        //void RemovePageAtIndex(int index);
        //void GetNavigationStackCount(bool isModal = false);

        #region Special page stuff

        MultiPage<Page> GetMultiPage(Func<object, bool> vmInitializer, IEnumerable itemsSource);
        MultiPage<Page> GetMultiPage(Func<object, bool> vmInitializer, params Type[] types);
        MultiPage<Page> GetMultiPage<TMultiPage>(Func<object, bool> vmInitializer, IEnumerable itemsSource) where TMultiPage : MultiPage<Page>;
        MultiPage<Page> GetMultiPage<TMultiPage>(Func<object, bool> vmInitializer, params Type[] types) where TMultiPage : MultiPage<Page>;
        FlyoutPage GetFlyoutPage<TFlyoutFlyoutVm, TFlyoutDetailVm>()
            where TFlyoutFlyoutVm : class
            where TFlyoutDetailVm : class;

        FlyoutPage GetFlyoutPage<TFlyoutFlyoutVm>()
            where TFlyoutFlyoutVm : class;

        public ProxyPage GetIdiomPage(Type viewModelType, IDictionary<string, Func<Page>> lookup) /*where TPage : Page*/;
        public ProxyPage GetIdiomPage(Type viewModelType, IDictionary<string, Type> lookup);
        public ProxyPage GetIdiomPage<TViewModel>(IDictionary<string, Type> lookup) where TViewModel : class;

        #endregion

    }
}
