using FunctionZero.CommandZero;
using FunctionZero.Maui.MvvmZero;
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
    public class FlyoutContentPageVm : MvvmZeroBasePageVm
    {
        private DetailPageItemVm _selectedItem;

        public ICommand ItemTappedCommand { get; }
        public IList<DetailPageItemVm> Items { get; }
        public DetailPageItemVm SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
        public FlyoutContentPageVm(IPageServiceZero pageServiceZero)
        {
            ItemTappedCommand = new CommandBuilder().AddGuard(this).SetExecute(ItemTappedCommandExecute).Build();
            var items = new List<DetailPageItemVm>
            {
                new DetailPageItemVm("One", pageServiceZero.GetMvvmPage<HomePage, HomePageVm>().page, true),
                new DetailPageItemVm("Two", pageServiceZero.GetMvvmPage<ListPage, ListPageVm>().page, true),
                new DetailPageItemVm("Three", pageServiceZero.GetMvvmPage<TreePage, TreePageVm>().page, false)
            };
            Items = items;
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

                ((FlyoutPage)Application.Current.MainPage).Detail = SelectedItem.ThePage;
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
        public DetailPageItemVm(string name, Page page, bool wrapInNavigation = true)
        {
            Name = name;
            if (wrapInNavigation)
                ThePage = new NavigationPage(page);
            else
                ThePage = page;
        }

        public string Name { get; }
        public Page ThePage { get; }
    }
}
