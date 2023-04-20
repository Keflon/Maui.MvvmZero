using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionZero.Maui.MvvmZero.PageControllers
{
    public interface IMultiPageController
    {
        bool HasMultiPage { get; }
        ObservableCollection<object> ItemsSource { get; }
        object SelectedItem { get; set; }

    }
}
