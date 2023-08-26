using System.ComponentModel;

namespace FunctionZero.Maui.MvvmZero.PageControllers
{
    public interface IFlyoutController
    {
        Page Detail { get; set; }
        Page Flyout { get; set; }
        bool HasFlyout { get; }
        bool IsGestureEnabled { get; set; }
        bool IsPresented { get; set; }
        FlyoutLayoutBehavior FlyoutLayoutBehavior { get; set; }

        event PropertyChangedEventHandler PropertyChanged;

        void SetDetailVm<TViewModel>(bool wrapInNavigation, Action<TViewModel> initViewModelAction, object hint=null) where TViewModel : class;
        [Obsolete]
        void SetDetailVm(Type vmType, bool wrapInNavigation, object hint=null);
        void SetDetailMultiPage(Func<object, bool> initializer, params Type[] vmTypes);
    }
}