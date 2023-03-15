using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleApp.Mvvm.PageViewModels
{
    internal class HomePageVm
    {
        internal void Init(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
