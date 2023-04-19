using FunctionZero.CommandZero;
using FunctionZero.Maui.Controls;
using FunctionZero.Maui.MvvmZero;
using FunctionZero.Maui.MvvmZero.PageControllers;
using Microsoft.Maui;
using SampleFlyoutApp.Mvvm.Pages;
using System;
using System.Collections.Generic;
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
        public IList<DetailPageItemVm> Items { get; }
        public DetailPageItemVm SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

        public FlyoutFlyoutPageVm(IPageServiceZero pageService)
        {
            _pageService = pageService;

            _pageService.FlyoutController.FlyoutLayoutBehavior = FlyoutLayoutBehavior.Popover;

            ItemTappedCommand = new CommandBuilder().AddGuard(this).SetExecute(ItemTappedCommandExecute).Build();
            var items = new List<DetailPageItemVm>
            {
                new DetailPageItemVm("One", ()=> _pageService.FlyoutController.SetContentVm(typeof(HomePageVm), true)),
                new DetailPageItemVm("Two", ()=> _pageService.FlyoutController.SetContentVm(typeof(ListPageVm), true)),
                new DetailPageItemVm("Three", ()=> _pageService.FlyoutController.SetContentVm(typeof(TreePageVm), true)),
                new DetailPageItemVm("Test",  ()=> _pageService.FlyoutController.SetContentVm(typeof(TestPageVm), true)),
                new DetailPageItemVm("Tabbed Test", ()=>_pageService.FlyoutController.Detail = GetTabbedTestPage(pageService))
            };
            Items = items;
        }

        private Page GetTabbedTestPage(IPageServiceZero pageService)
        {
            var retval = pageService.GetMultiPage<AdaptedTabbedPage>(VmInitializer, typeof(TestPageVm), typeof(TestPageVm), typeof(TestPageVm));

            return retval;
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
            {
                if (SelectedItem == null)
                    throw new InvalidOperationException("Null SelectedItem");

                SelectedItem.SelectedAction();
                //_pageService.FlyoutController.Detail = SelectedItem.ThePage;

                // TODO: _pageService.FlyoutController.SetDetail(typeof(ViewModel), vm => vm.Init(...));

                //((FlyoutPage)Application.Current.MainPage).Detail = SelectedItem.ThePage;
            }
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
