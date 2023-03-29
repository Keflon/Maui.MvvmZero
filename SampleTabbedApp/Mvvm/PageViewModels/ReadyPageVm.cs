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
    public class ReadyPageVm : BasePageVm
    {
        public ICommand PushTestPageCommand { get; }

        public ReadyPageVm(IPageServiceZero pageService)
        {
            PushTestPageCommand = new CommandBuilder().AddGuard(this).SetName("Push test page").SetExecuteAsync(() => pageService.PushPageAsync<TestPage, TestPageVm>((vm) => { })).Build();
        }
    }
}