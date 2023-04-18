using FunctionZero.CommandZero;
using FunctionZero.Maui.MvvmZero;
using FunctionZero.Maui.MvvmZero.PageControllers;
using FunctionZero.Maui.MvvmZero.Workaround;
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
                new DetailPageItemVm("One", typeof(HomePageVm), true),
                new DetailPageItemVm("Two", typeof(ListPageVm), true),
                new DetailPageItemVm("Three", typeof(TreePageVm), false),
                new DetailPageItemVm("Test", typeof(TestPageVm), true),
                //new DetailPageItemVm("Tabbed Test", GetTabbedTestPage(pageService), true)

        };
            Items = items;
        }

        private Page GetTabbedTestPage(IPageServiceZero pageService)
        {
            // Use ItemsSource and an ItemTemplate.
            var retval = pageService.GetMultiPage<AdaptedTabbedPage>(typeof(TestPageVm), typeof(TestPageVm), typeof(TestPageVm));

            //TODO: pageService.GetMultiPage<AdaptedTabbedPage>()
            //                                                  .AddVm<TViewModel>(vm=>Init(..))
            //                                                  .AddVm(typeof(SomeVm), vm=>Init(..)) ...
            //                                                  .AddMultiPage

            return retval;
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

                _pageService.FlyoutController.SetContentVm(SelectedItem.VmType, SelectedItem.WrapInNavigation);
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
        public DetailPageItemVm(string name, Type vmType, bool wrapInNavigation = true)
        {
            Name = name;
            VmType = vmType;
            WrapInNavigation = wrapInNavigation;
        }

        public string Name { get; }
        public Type VmType { get; }
        public bool WrapInNavigation { get; }
    }
}
