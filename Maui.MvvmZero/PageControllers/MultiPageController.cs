using FunctionZero.Maui.Controls;
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
        private readonly Func<MultiPage<Page>> _multiPageFinder;

        public MultiPageController(Func<MultiPage<Page>> multiPageFinder)
        {
            _multiPageFinder = multiPageFinder;
        }
        public bool HasMultiPage => _multiPageFinder() != null;

        //public ObservableCollection<object> ItemsSource => _pageService.MultiPageFinder()?.ItemsSource as ObservableCollection<object>;

        public ObservableCollection<object> ItemsSource
        {
            get
            {
                // TODO: _multiPageFinder may return null.
                var multiPage = _multiPageFinder();
                if (multiPage is AdaptedTabbedPage adaptedMultiPage)
                    return adaptedMultiPage.ItemsSource as ObservableCollection<object>;
                else
                    return multiPage.ItemsSource as ObservableCollection<object>;
            }
        }

        public object SelectedItem { get => _multiPageFinder()?.SelectedItem; set {  _multiPageFinder().SelectedItem = value; } }
    }
}
