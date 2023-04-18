﻿using System;
using System.Collections.Generic;
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
            var page = (IView)_templateGetter(item.GetType());
            // initialize it. If the initializer returns true, wrap it in a NavigationPage.
            if (_initializer(page) == true)
            {
                var root = _navPageGetter();
                root.Title = ((Page)page).Title;
                root.PushAsync((Page)page, false);
                return root;
            }
            else
                return page;
        }
    }
}
