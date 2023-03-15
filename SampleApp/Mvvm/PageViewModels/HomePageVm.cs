using FunctionZero.CommandZero;
using Maui.MvvmZero;
using Maui.MvvmZero.Services;
using SampleApp.Mvvm.Pages;
using SampleApp.Mvvm.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SampleApp.Mvvm.PageViewModels
{
    public class HomePageVm : BasePageVm
    {
        private int _count;
        private readonly IPageServiceZero _pageService;

        public ICommand CabbagesPageCommand { get; }
        public ICommand OnionsPageCommand { get; }

        public int Count
        { get => _count; set => base.SetProperty(ref _count, value); }

        public HomePageVm(IPageServiceZero pageService)
        {
            _pageService = pageService;

            base.AddPageTimer(16, pageTimerCallback, null, "hello");

            // Set up our commands for the UI to bind to ...
            CabbagesPageCommand = new CommandBuilder().SetExecuteAsync(CabbagesPageCommandExecuteAsync).SetName("Cabbages").Build();
            OnionsPageCommand = new CommandBuilder().SetExecuteAsync(OnionsPageCommandExecuteAsync).SetName("Onions").Build();
        }

        private void pageTimerCallback(object state)
        {
            Count++;
        }

        private async Task CabbagesPageCommandExecuteAsync(/* Optional : object arg */)
        {
            // Take us to the CabbagesPage page ...
            await _pageService.PushPageAsync<CabbagesPage, CabbagesPageVm>((vm) => { /* Initialize the vm in here if necessary */ });
        }

        private async Task OnionsPageCommandExecuteAsync(/* Optional : object arg */)
        {
            // Take us to the OnionsPage page ...
            await _pageService.PushPageAsync<OnionsPage, OnionsPageVm>((vm) => { /* Initialize the vm in here if necessary */ });
        }

        internal void Init(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
