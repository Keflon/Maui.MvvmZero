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
    public class FlyoutController : IFlyoutController
    {
        private FlyoutPage _flyoutPage;

        private bool _hasFlyout;
        private Page _flyout;
        private bool _isPresented;
        private bool _isGestureEnabled;
        private Page _detail;
        private FlyoutLayoutBehavior _flyoutLayoutBehavior;

        public bool HasFlyout { get => _hasFlyout; set => SetProperty(ref _hasFlyout, value); }

        public Page Flyout { get => _flyout; set => SetProperty(ref _flyout, value); }

        public bool IsPresented { get => _isPresented; set => SetProperty(ref _isPresented, value); }

        public bool IsGestureEnabled { get => _isGestureEnabled; set => SetProperty(ref _isGestureEnabled, value); }

        public Page Detail { get => _detail; set => SetProperty(ref _detail, value); }
        public FlyoutLayoutBehavior FlyoutLayoutBehavior { get => _flyoutLayoutBehavior; set => SetProperty(ref _flyoutLayoutBehavior, value); }

        public FlyoutController()
        {
        }

        internal void SetFlyoutPage(FlyoutPage flyoutPage)
        {
            if (_flyoutPage != null)
                Detach();

            _flyoutPage = flyoutPage;

            if (_flyoutPage != null)
            {
                HasFlyout = flyoutPage != null;

                _flyoutPage.Flyout = Flyout ?? _flyoutPage.Flyout;
                _flyoutPage.IsPresented = IsPresented;
                _flyoutPage.IsGestureEnabled = IsGestureEnabled;
                _flyoutPage.Detail = Detail ?? _flyoutPage.Detail;
                _flyoutPage.FlyoutLayoutBehavior = FlyoutLayoutBehavior;

                Attach(_flyoutPage);
            }
        }

        internal void Attach(FlyoutPage flyoutPage)
        {
            _flyoutPage = flyoutPage;
            _flyoutPage.PropertyChanged += _flyout_PropertyChanged;
        }

        internal void Detach()
        {
            _flyoutPage.PropertyChanged -= _flyout_PropertyChanged;
            _flyoutPage = null;
        }

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
        #endregion

    }
}
