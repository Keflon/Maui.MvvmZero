using System.ComponentModel;

namespace FunctionZero.Maui.MvvmZero.Workaround
{
    public class AdaptedFlyoutPage : FlyoutPage
    {
        Page _oldFlyout;
        public AdaptedFlyoutPage()
        {
#if WINDOWS
            PropertyChanged += AdaptedFlyoutPage_PropertyChanged;
#endif
            _oldFlyout = null;
        }

        private void Flyout_Disappearing(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AdaptedFlyoutPage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FlyoutPage.Flyout))
            {
                if (_oldFlyout != null)
                    _oldFlyout.PropertyChanged -= FlyoutFlyout_PropertyChanged;

                if (Flyout != null)
                {
                    Flyout.PropertyChanged += FlyoutFlyout_PropertyChanged;
                    Flyout.Disappearing += Flyout_Disappearing;
                    Flyout.NavigatedFrom += Flyout_NavigatedFrom;
                    Flyout.Unloaded += Flyout_Unloaded;
                }

                _oldFlyout = Flyout;
            }
        }

        private void Flyout_Unloaded(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Flyout_NavigatedFrom(object sender, NavigatedFromEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void FlyoutFlyout_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Page.IsFocused))
            {
                if (FlyoutLayoutBehavior == FlyoutLayoutBehavior.Popover)
                {
                    if (Flyout.IsFocused == false)
                    {
                        // Delay is necessary otherwise closing the Flyout using the hamburger menu
                        // immediately repoens it, for no sane reason.
                        await Task.Delay(120);
                        IsPresented = false;
                    }
                }
            }
        }
    }
}

