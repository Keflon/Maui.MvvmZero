using FunctionZero.CommandZero;
using FunctionZero.Maui.MvvmZero;
using SampleTabbedApp.Mvvm.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SampleTabbedApp.Mvvm.PageViewModels
{
    public class SteadyPageVm : BasePageVm
    {
        private readonly IPageServiceZero _pageService;

        public ICommand PushTestPageCommand { get; }
        public ICommand SwapToReadyPageCommand { get; }
        public ICommand SwapToGoPageCommand { get; }

        public SteadyPageVm(IPageServiceZero pageService)
        {
            _pageService = pageService;

            PushTestPageCommand = new CommandBuilder().AddGuard(this).SetName("Push test page").SetExecuteAsync(() => pageService.PushPageAsync<TestPage, TestPageVm>((vm) => { })).Build();
            SwapToReadyPageCommand = new CommandBuilder().AddGuard(this).SetName("Swap to Ready").SetExecuteAsync(SwapToReadyPageCommandExecute).Build();
            SwapToGoPageCommand = new CommandBuilder().AddGuard(this).SetName("Swap to Go").SetExecuteAsync(SwapToReadyGoCommandExecute).Build();
        }
        private async Task SwapToReadyPageCommandExecute(object arg)
        {
            _pageService.MultiPageController.SelectedItem = _pageService.MultiPageController.ItemsSource[0];
        }

        private async Task SwapToReadyGoCommandExecute(object arg)
        {
            _pageService.MultiPageController.SelectedItem = _pageService.MultiPageController.ItemsSource[2];
        }


    }
}
