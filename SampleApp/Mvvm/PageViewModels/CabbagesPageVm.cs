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
using System.Xml.Linq;

namespace SampleApp.Mvvm.PageViewModels
{
    public class CabbagesPageVm : BasePageVm
    {
        private string _name;
        private IPageServiceZero _pageService;

        /// <summary>
        /// The UI can bind to this command and use it to get to the ResultsPage
        /// </summary>
        public ICommand NextCommand { get; }

        public string Name { get => _name; set => SetProperty(ref _name, value); }

        public CabbagesPageVm(IPageServiceZero pageService)
        {
            _pageService = pageService;

            // Initialise Name so we don't have to null-check it later
            Name = string.Empty;

            // Set up our Command for the UI to bind to ...
            NextCommand = new CommandBuilder()
                .SetExecuteAsync(NextCommandExecuteAsync)
                .SetCanExecute(NextCommandCanExecute)
                .SetName(GetCurrentName)
                // This command can enable or disable itself or change its Text if the 'Name' property changes
                .AddObservedProperty(this, nameof(Name))
                .Build();
        }

        // When the NextCommand is invoked (by the UI) this method is called to take us to the results page
        private async Task NextCommandExecuteAsync(object arg)
        {
            string payload = $"The Cabbages Page has been visited by {Name}";

            await _pageService.PushPageAsync<ResultsPage, ResultsPageVm>((vm) => vm.Init(payload));
        }

        // Returns true if the Name property is considered valid
        private bool NextCommandCanExecute()
        {
            return Name.Length >= 4;
        }

        // This is reevaluated (by the UI) every time the 'Name' property changes
        private string GetCurrentName()
        {
            if (NextCommandCanExecute() == false)
                return "Keep typing ...";

            return "Next";
        }
    }
}
