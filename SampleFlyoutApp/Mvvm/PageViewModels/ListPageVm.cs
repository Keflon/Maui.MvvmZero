using FunctionZero.CommandZero;
using FunctionZero.Maui.MvvmZero;
using FunctionZero.Maui.Showcase.Mvvm.PageViewModels;
using SampleFlyoutApp.Mvvm.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SampleFlyoutApp.Mvvm.PageViewModels
{
    public class ListPageVm : BasePageVm
    {
        private readonly IPageServiceZero _pageService;

        public ICommand Test1Command { get; }
        public ICommand Test2Command { get; }
        public ListPageVm(IPageServiceZero pageService)
        {
            _pageService = pageService;

            Test1Command = new CommandBuilder().AddGuard(this).SetName("Test").SetExecuteAsync(Test1CommandExecuteAsync).Build();
            Test2Command = new CommandBuilder().AddGuard(this).SetExecuteAsync(Test2CommandExecuteAsync).Build();
        }

        private async Task Test1CommandExecuteAsync(object arg)
        {
            await _pageService.PushPageAsync<ListPlayPage, ListPlayPageVm>(vm => { });
        }

        private async Task Test2CommandExecuteAsync(object arg)
        {
            throw new NotImplementedException();
        }

        public override void OnOwnerPageDisappearing()
        {
            base.OnOwnerPageDisappearing();
        }

        public override void OnOwnerPagePopped(bool isModal)
        {
            base.OnOwnerPagePopped(isModal);
        }
    }
}
