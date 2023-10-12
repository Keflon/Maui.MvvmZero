using FunctionZero.CommandZero;
using FunctionZero.Maui.MvvmZero;
using FunctionZero.Maui.MvvmZero.PageControllers;
using Microsoft.Maui;
using SampleFlyoutApp.Mvvm.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SampleFlyoutApp.Mvvm.PageViewModels.Root
{
    public class FlyoutFlyoutPageVm : MvvmZeroBasePageVm
    {
        private DetailPageItemVm _selectedItem;
        private readonly IPageServiceZero _pageService;

        public ICommand ItemTappedCommand { get; }
        public ObservableCollection<DetailPageItemVm> Items { get; }
        public DetailPageItemVm SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

        public FlyoutFlyoutPageVm(IPageServiceZero pageService)
        {
            _pageService = pageService;

            _pageService.FlyoutController.FlyoutLayoutBehavior = FlyoutLayoutBehavior.Popover;

            ItemTappedCommand = new CommandBuilder().AddGuard(this).SetExecute(ItemTappedCommandExecute).Build();
            Items = new()
            {
                new DetailPageItemVm("One", () => _pageService.FlyoutController.SetDetailVm<HomePageVm>(true, (vm)=>{ })),
                new DetailPageItemVm("Two", () => _pageService.FlyoutController.SetDetailVm<ListPageVm>(true, (vm)=>{ })),
                new DetailPageItemVm("Three", () => _pageService.FlyoutController.SetDetailVm<TreePageVm>(true, (vm)=>{ })),
                new DetailPageItemVm("Test",  () => _pageService.FlyoutController.SetDetailVm<TestPageVm>(true, (vm)=>{ })),
                //new DetailPageItemVm("Tabbed Test", () =>_pageService.FlyoutController.Detail = GetTabbedTestPage(pageService))
                new DetailPageItemVm("Tabbed Test", () =>_pageService.FlyoutController.SetDetailMultiPage(VmInitializer, typeof(TestPageVm), typeof(TestPageVm), typeof(TestPageVm)))
            };
        }
        private bool VmInitializer(object obj)
        {
            return true;
        }

        private void ItemTappedCommandExecute(object arg)
        {
            var thing = (DetailPageItemVm)arg;
            SelectedItem = thing;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(SelectedItem))
                SelectedItem.SelectedAction();
        }

        public override void OnOwnerPageAppearing()
        {
            base.OnOwnerPageAppearing();

            if (SelectedItem == null)
                SelectedItem = Items[0];
        }
    }

    public class DetailPageItemVm
    {
        public DetailPageItemVm(string name, Action selectedAction)
        {
            Name = name;
            SelectedAction = selectedAction;
        }

        public string Name { get; }
        public Action SelectedAction { get; }
    }
}
