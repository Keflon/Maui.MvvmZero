using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionZero.Maui.MvvmZero
{
    internal class ViewDataTemplateSelector : DataTemplateSelector
    {
        private readonly Func<Type, IView> _templateGetter;
        private readonly Func<object, bool> _initializer;
        private readonly Func<NavigationPage> _navPageGetter;

        public ViewDataTemplateSelector(Func<object, bool> initializer, Func<NavigationPage> navPageGetter, Func<Type, IView> templateGetter)
        {
            _initializer = initializer ?? ((item) => true);
            _navPageGetter = navPageGetter;
            _templateGetter = templateGetter;
        }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return new DataTemplate(() => GetTemplate(item));
        }
        private object GetTemplate(object item)
        {
            // 'item' is  our view-model. Get the corresponding Page.
            var page = (Page)_templateGetter(item.GetType());
            // initialize it. If the initializer returns true, wrap it in a NavigationPage.
            if (_initializer(item) == true)
            {
                // Set the page BC ...
                page.BindingContext = item;
                var root = _navPageGetter();
                // And remove the Nav BC as soon as it is set.
                root.BindingContextChanged += Root_BindingContextChanged;
                root.Title = ((Page)page).Title;
                root.PushAsync((Page)page, false);
                return root;
            }
            else
                return page;
        }

        /// <summary>
        /// The caller to OnSelectTemplate will set the BindingContext of the object returned by GetTemplate.
        /// Normally this is fine, but if we've wrapped the return value in a NavigationPage, we do not want to set the 
        /// BindingContext of the NavigationPage, otherwise we'll e.g. get OwnerPageAppearing events raised twice for 
        /// the viewmodel associated with the NavigationPage root Page.
        /// To counter this, we remove the BindingContext as soon as it has been set.
        /// </summary>
        private void Root_BindingContextChanged(object sender, EventArgs e)
        {
            var nav = (NavigationPage)sender;
            // Prevent unintended side effects ...
            nav.BindingContextChanged -= Root_BindingContextChanged;

            nav.ClearValue(NavigationPage.BindingContextProperty);
        }
    }
}
