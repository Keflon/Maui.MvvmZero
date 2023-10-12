//using FunctionZero.Maui.Controls;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FunctionZero.Maui.MvvmZero.PageControllers
{
    /// <summary>
    /// This class is a proxy to FlyoutPage properties that are of interest to IPageService consumers.
    /// There can be only one FlyoutPage active at a time, and that must be the root UI page. 
    /// This is a requirement / limitation of MAUI.
    /// </summary>
    internal class FlyoutController : IFlyoutController
    {
        private FlyoutPage _flyoutPage;

        private bool _hasFlyout;
        private Page _flyout;
        private bool _isPresented;
        private bool _isGestureEnabled;
        private Page _detail;
        private FlyoutLayoutBehavior _flyoutLayoutBehavior;
        private readonly IPageServiceZero _pageService;

        public bool HasFlyout { get => _hasFlyout; set => SetProperty(ref _hasFlyout, value); }
        public Page Flyout { get => _flyout; set => SetProperty(ref _flyout, value); }
        public bool IsPresented { get => _isPresented; set => SetProperty(ref _isPresented, value); }
        public bool IsGestureEnabled { get => _isGestureEnabled; set => SetProperty(ref _isGestureEnabled, value); }
        public Page Detail { get => _detail; set => SetProperty(ref _detail, value); }
        public FlyoutLayoutBehavior FlyoutLayoutBehavior { get => _flyoutLayoutBehavior; set => SetProperty(ref _flyoutLayoutBehavior, value); }

        public FlyoutController(IPageServiceZero pageService)
        {
            _pageService = pageService;
        }

        internal void SetFlyoutPage(FlyoutPage flyoutPage)
        {
            if (_flyoutPage != null)
                Detach();

            _flyoutPage = flyoutPage;
            HasFlyout = flyoutPage != null;

            if (_flyoutPage != null)
            {
                // Flyout already has a Flyout set if it was created by PageServiceZero.GetFlyoutPage.
                // SMELL: Think this through.
                //if (Flyout != null)
                //    _flyoutPage.Flyout = Flyout;
                //else
                Flyout = _flyoutPage.Flyout;

                _flyoutPage.IsPresented = IsPresented;
                _flyoutPage.IsGestureEnabled = IsGestureEnabled;

                // Detail cannot be null. FlyoutPage will already have a Detail if it was created by PageServiceZero.GetFlyoutPage.
                // If 'this' also has one, use it.
                // SMELL: Think this through.
                if (Detail != null)
                    _flyoutPage.Detail = Detail;
                else
                    Detail = _flyoutPage.Detail;


                _flyoutPage.FlyoutLayoutBehavior = FlyoutLayoutBehavior;

                Attach(_flyoutPage);
            }
        }


        private void Attach(FlyoutPage flyoutPage)
        {
            _flyoutPage = flyoutPage;
            _flyoutPage.PropertyChanged += _flyout_PropertyChanged;
        }

        private void Detach()
        {
            _flyoutPage.PropertyChanged -= _flyout_PropertyChanged;
            _flyoutPage = null;
        }
        /// <summary>
        /// Something of interest on the FlyoutPage has changed. Copy it to here.
        /// </summary>
        private void _flyout_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Copy FlyoutPage properties to this.
            if (e.PropertyName == nameof(FlyoutPage.Flyout))
                Flyout = _flyoutPage.Flyout;

            else if (e.PropertyName == nameof(FlyoutPage.IsPresented))
                IsPresented = _flyoutPage.IsPresented;

            else if (e.PropertyName == nameof(FlyoutPage.IsGestureEnabled))
                IsGestureEnabled = _flyoutPage.IsGestureEnabled;

            else if (e.PropertyName == nameof(FlyoutPage.Detail))
                Detail = _flyoutPage.Detail;

            else if (e.PropertyName == nameof(FlyoutPage.FlyoutLayoutBehavior))
                FlyoutLayoutBehavior = _flyoutPage.FlyoutLayoutBehavior;
        }
        public void SetDetailVm<TViewModel>(bool wrapInNavigation, Action<TViewModel> initViewModelAction, object hint = null) where TViewModel : class
        {
            var page = (Page)_pageService.GetViewForVm(typeof(TViewModel), hint);
            var vm = _pageService.GetViewModel<TViewModel>();

            page.BindingContext = vm;

            if (initViewModelAction != null)
                initViewModelAction(vm);

            if (wrapInNavigation)
            {
                // Has our page been previously wrapped (by us)?
                if (page.Parent is NavigationPage navpage)
                {
                    page = navpage;
                }
                else
                {
                    var root = _pageService.GetView<NavigationPage>();
                    root.PushAsync(page, false);
                    page = root;
                }
            }
            this.Detail = page;
        }

        [Obsolete]
        public void SetDetailVm(Type vmType, bool wrapInNavigation, object hint = null)
        {
            var page = (Page)_pageService.GetViewForVm(vmType, hint);
            var vm = _pageService.TypeFactory(vmType);

            page.BindingContext = vm;

            if (wrapInNavigation)
            {
                // Has our page been previously wrapped (by us)?
                if (page.Parent is NavigationPage navpage)
                {
                    page = navpage;
                }
                else
                {
                    var root = _pageService.GetView<NavigationPage>();
                    root.PushAsync(page, false);
                    page = root;
                }
            }
            this.Detail = page;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        /// <summary>
        /// Something of interest here  has changed. Copy it to the FlyoutPage.
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (_flyoutPage != null)
            {
                // Copy this to FlyoutPage properties.
                if (propertyName == nameof(Flyout))
                    _flyoutPage.Flyout = Flyout;

                else if (propertyName == nameof(IsPresented))
                {
                    Debug.WriteLine($"Controller IsPresented changed to {IsPresented}");
                    _flyoutPage.IsPresented = IsPresented;
                }
                else if (propertyName == nameof(IsGestureEnabled))
                    _flyoutPage.IsGestureEnabled = IsGestureEnabled;

                else if (propertyName == nameof(Detail))
                    _flyoutPage.Detail = Detail;

                else if (propertyName == nameof(FlyoutLayoutBehavior))
                    _flyoutPage.FlyoutLayoutBehavior = FlyoutLayoutBehavior;
            }
        }

        public void SetDetailMultiPage(Func<object, bool> vmInitializer, params Type[] vmTypes)
        {
            var multiPage = _pageService.GetMultiPage(vmInitializer, vmTypes);
            Detail = multiPage;
        }
        #endregion
    }
}
