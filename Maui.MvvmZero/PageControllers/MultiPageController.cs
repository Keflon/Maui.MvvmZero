using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionZero.Maui.MvvmZero.PageControllers
{
    internal class MultiPageController : IMultiPageController
    {
        private readonly IPageServiceZero _pageService;

        public MultiPageController(IPageServiceZero pageService)
        {
            _pageService = pageService;
        }
        public bool HasMultiPage => throw new NotImplementedException();

        public ObservableCollection<object> ItemsSource => throw new NotImplementedException();

        public object SelectedItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


        internal MultiPage<Page> FindMultiPage()
        {
            var mp = _pageService.MultiPageFinder();
            return mp;
        }
    }
}
