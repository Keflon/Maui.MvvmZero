using FunctionZero.CommandZero;
using FunctionZero.Maui.MvvmZero;
using SampleTabbedApp.Mvvm.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SampleTabbedApp.Mvvm.PageViewModels
{
    public class ReadyPageVm : BasePageVm
    {
        private readonly IPageServiceZero _pageService;

        public ICommand PushTestPageCommand { get; }

        public ReadyPageVm(IPageServiceZero pageService)
        {
            _pageService = pageService;

            PushTestPageCommand = new CommandBuilder().AddGuard(this).SetName("Push test page").SetExecuteAsync(PushTestPageExecuteAsync).Build();
        }

        private async Task PushTestPageExecuteAsync(object arg)
        {
            if(
            await _pageService.PushPageAsync<TestPage, TestPageVm>((vm) => { }) == null)
                { 
                Debug.WriteLine("No stack to push to.");
            }
                ;
        }
    }
}