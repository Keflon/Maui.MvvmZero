using FunctionZero.CommandZero;
using Maui.MvvmZero;
using SampleApp.Mvvm.Pages;
using SampleApp.Mvvm.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SampleApp.Mvvm.PageViewModels
{
    public class OnionsPageVm : BasePageVm
    {
        /// <summary>
        /// The UI can bind to this command and use it to get to the ResultsPage
        /// </summary>
        public ICommand NextCommand { get; }

        /// <summary>
        /// A very basic ViewModel
        /// </summary>
        public OnionsPageVm(IPageServiceZero pageService)
        {
            NextCommand = new CommandBuilder()
                .SetExecuteAsync(() => pageService.PushPageAsync<ResultsPage, ResultsPageVm>((vm) => vm.Init("Hello from the Onions Page!")))
                .SetName("Next")
                .Build();
        }
    }
}
