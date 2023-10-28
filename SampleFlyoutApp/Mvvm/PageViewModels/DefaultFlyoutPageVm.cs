using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleFlyoutApp.Mvvm.PageViewModels
{
    public class DefaultFlyoutPageVm : BasePageVm
    {
        private int _count;
        public int Count { get => _count; set => SetProperty(ref _count, value); }

        public DefaultFlyoutPageVm()
        {
            // PageTimers run only when page is presented.
            AddPageTimer(16, FlyoutTimerCallback, null, null);
        }

        private void FlyoutTimerCallback(object obj)
        {
            Count++;
        }
    }
}
