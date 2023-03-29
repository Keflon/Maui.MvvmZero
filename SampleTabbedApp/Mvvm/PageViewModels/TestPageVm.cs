using FunctionZero.CommandZero;
using FunctionZero.Maui.MvvmZero;
using SampleTabbedApp.Mvvm.Pages;
using System.Windows.Input;

namespace SampleTabbedApp.Mvvm.PageViewModels
{
    public class TestPageVm : BasePageVm
    {
        private readonly IPageServiceZero _pageService;

        public ICommand PushPageCommand { get; }
        public ICommand PushModalPageCommand { get; }
        public ICommand PopPageCommand { get; }
        public ICommand PopModalPageCommand { get; }

        public TestPageVm(IPageServiceZero pageService)
        {
            _pageService = pageService;

            PushPageCommand = new CommandBuilder().AddGuard(this).SetName(nameof(PushPageCommand)).SetExecuteAsync(PushPageCommandExecuteAsync).Build();
            PushModalPageCommand = new CommandBuilder().AddGuard(this).SetName(nameof(PushModalPageCommand)).SetExecuteAsync(PushModalPageCommandExecuteAsync).Build();

            PopPageCommand = new CommandBuilder().AddGuard(this).SetName(nameof(PopPageCommand)).SetExecuteAsync(PopPageCommandExecuteAsync).Build();
            PopModalPageCommand = new CommandBuilder().AddGuard(this).SetName(nameof(PopModalPageCommand)).SetExecuteAsync(PopModalPageCommandExecuteAsync).Build();
        }

        private async Task PopModalPageCommandExecuteAsync(object arg)
        {
            await _pageService.PopAsync(true);
        }

        private async Task PopPageCommandExecuteAsync(object arg)
        {
            await _pageService.PopAsync(false);

        }

        private async Task PushModalPageCommandExecuteAsync(object arg)
        {
            await _pageService.PushPageAsync<TestPage, TestPageVm>(vm => { }, true);
        }

        private async Task PushPageCommandExecuteAsync()
        {
            await _pageService.PushPageAsync<TestPage, TestPageVm>(vm => { }, false);
        }
    }
}
