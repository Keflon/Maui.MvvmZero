using FunctionZero.Maui.MvvmZero;
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
        private readonly IPageServiceZero _pageService;
        private int _count;
        public int Count
        { get => _count; set => base.SetProperty(ref _count, value); }

        public HomePageVm(IPageServiceZero pageService)
        {
            _pageService = pageService;

            base.AddPageTimer(16, pageTimerCallback, null, "hello");
        }
        internal void Init(string initMessage)
        {
            Debug.WriteLine(initMessage);
        }

        private void pageTimerCallback(object state)
        {
            Count++;
        }
    }
}
