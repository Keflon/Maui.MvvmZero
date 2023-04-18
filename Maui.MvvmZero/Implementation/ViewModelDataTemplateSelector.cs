using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionZero.Maui.MvvmZero
{
    internal class ViewDataTemplateSelector : DataTemplateSelector
    {
        private readonly Func<Type, IView> _templateGetter;
        private readonly Action<object> _initializer;

        public ViewDataTemplateSelector(Action<object> initializer, Func<Type, IView> templateGetter)
        {
            _initializer = initializer ?? ((item)=>{ });
            _templateGetter = templateGetter;
        }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return new DataTemplate(()=> GetTemplate(item));
        }
        private object GetTemplate(object item)
        {
            // 'item' is  our view-model. Get the corresponding Page.
            var page = (IView)_templateGetter(item.GetType());
            // initialize it.
            _initializer(page);
            return page;
        }
    }
}
