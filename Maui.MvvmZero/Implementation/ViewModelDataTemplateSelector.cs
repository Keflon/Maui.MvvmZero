using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionZero.Maui.MvvmZero
{
    internal class ViewDataTemplateSelector : DataTemplateSelector
    {
        private readonly IPageServiceZero _pageService;

        public ViewDataTemplateSelector(IPageServiceZero pageService)
        {
            _pageService = pageService;
        }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return new DataTemplate(()=> GetTemplate(item));
        }
        private object GetTemplate(object item)
        {
            // 'item' is  our view-model. Get the corresponding Page.
            var page = (Page)((PageServiceZero)_pageService)._viewFinder.Invoke(item.GetType(), null);
            return page;
        }
    }
}
