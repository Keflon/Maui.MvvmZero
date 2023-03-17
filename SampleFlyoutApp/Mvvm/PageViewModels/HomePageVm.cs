using FunctionZero.Maui.Showcase.Mvvm.PageViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleFlyoutApp.Mvvm.PageViewModels
{
    public class HomePageVm : BasePageVm
    {
        internal void Init(string initMessage)
        {
            Debug.WriteLine(initMessage);
        }
    }
}
