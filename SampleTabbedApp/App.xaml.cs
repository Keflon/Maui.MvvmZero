using FunctionZero.Maui.Controls;
using FunctionZero.Maui.MvvmZero;
using FunctionZero.Maui.MvvmZero.EventArgs;
using FunctionZero.Maui.MvvmZero.Interfaces;
using SampleTabbedApp.Mvvm.PageViewModels;
using System.Collections;
using System.Diagnostics;

namespace SampleTabbedApp
{
    // Use AdaptedTabbedPage over TabbedPage because https://github.com/dotnet/maui/issues/14572

    public partial class App : Application
    {
        public App(IPageServiceZero pageService, IDisplayService displayService)
        {
            InitializeComponent();

            pageService.Init(this);
            this.Resources["IdiomZero"] = "Portrait";

            displayService.RotationChanged += DisplayService_RotationChanged;

            var rootPage = pageService.GetMultiPage(VmInitializer, typeof(ReadyPageVm), typeof(SteadyPageVm), typeof(GoPageVm));
            var a = new ContentPage();
            MainPage = rootPage;
        }
        private bool VmInitializer(object viewModel)
        {
            if (viewModel is ReadyPageVm)
                return false; // Do not wrap the ReadyPage in a NavigationPage.

            return true;
        }

        private void DisplayService_RotationChanged(object sender, DisplayRotationEventArgs e)
        {
            switch (e.CurrentRotation)
            {
                case DisplayRotation.Unknown:
                    this.Resources["IdiomZero"] = "Portrait";
                    break;
                case DisplayRotation.Rotation0:
                    this.Resources["IdiomZero"] = "Portrait";
                    break;
                case DisplayRotation.Rotation90:
                    this.Resources["IdiomZero"] = "Landscape";
                    break;
                case DisplayRotation.Rotation180:
                    this.Resources["IdiomZero"] = "Portrait";
                    break;
                case DisplayRotation.Rotation270:
                    this.Resources["IdiomZero"] = "Landscape";
                    break;
            }
        }
    }
}