using FunctionZero.CommandZero;
using FunctionZero.Maui.MvvmZero;
using FunctionZero.Maui.MvvmZero.PageControllers;
using Microsoft.Maui;
using SampleFlyoutApp.Mvvm.Pages;
using SampleFlyoutApp.Mvvm.ViewModels;
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
        private readonly IPageServiceZero _pageService;

        public ICommand ItemTappedCommand { get; }
        public ObservableCollection<FlyoutItemVm> Items { get; }

        public FlyoutFlyoutPageVm(IPageServiceZero pageService)
        {
            _pageService = pageService;

            _pageService.FlyoutController.FlyoutLayoutBehavior = FlyoutLayoutBehavior.Popover;

            //ItemTappedCommand = new CommandBuilder().AddGuard(this).SetExecute(ItemTappedCommandExecute).Build();
            //Items = new()
            //{
            //    new DetailPageItemVm("One", () => _pageService.FlyoutController.SetDetailVm<HomePageVm>(true, (vm)=>{ })),
            //    new DetailPageItemVm("Two", () => _pageService.FlyoutController.SetDetailVm<ListPageVm>(true, (vm)=>{ })),
            //    new DetailPageItemVm("Three", () => _pageService.FlyoutController.SetDetailVm<TreePageVm>(true, (vm)=>{ })),
            //    new DetailPageItemVm("Test",  () => _pageService.FlyoutController.SetDetailVm<TestPageVm>(true, (vm)=>{ })),
            //    //new DetailPageItemVm("Tabbed Test", () =>_pageService.FlyoutController.Detail = GetTabbedTestPage(pageService))
            //    new DetailPageItemVm("Tabbed Test", () =>_pageService.FlyoutController.SetDetailMultiPage(VmInitializer, typeof(TestPageVm), typeof(TestPageVm), typeof(TestPageVm)))
            //};
            // Placeholder ...

            // Build a tree of data to represent all the options in the FlyoutFlyoutPage.
            // The FlyoutPage will databind to this and build the tree.
            // (If this data is modified at runtime, the Visualisation will track changes.)

            // FlyoutItemVm can be enhanced to support any other per-item properties, e.g. icon-names, animations etc.

            Items = new();

            var item1 = new FlyoutItemVm("Old stuff", null);
            var item2 = new FlyoutItemVm("Test", null);
            var item3 = new FlyoutItemVm("Settings", null);

            item1.Children.Add(new FlyoutItemVm("Home Page", (flyoutItem) => _pageService.FlyoutController.SetDetailVm<HomePageVm>(true, vm => vm.Init("Hello"))));
            item1.Children.Add(new FlyoutItemVm("List Page", (flyoutItem) => _pageService.FlyoutController.SetDetailVm<ListPageVm>(true, vm => { })));
            item1.Children.Add(new FlyoutItemVm("Tree Page", (flyoutItem) => _pageService.FlyoutController.SetDetailVm<TreePageVm>(true, vm => { })));
            item1.Children.Add(new FlyoutItemVm("Test Page", (flyoutItem) => _pageService.FlyoutController.SetDetailVm<TestPageVm>(true, vm => { })));
            item1.Children.Add(new FlyoutItemVm("Tabbed Test Page", (flyoutItem) => _pageService.FlyoutController.SetDetailMultiPage(VmInitializer, typeof(TestPageVm), typeof(TestPageVm), typeof(TestPageVm))));
            item1.Children.Add(new FlyoutItemVm("VM Animation Page", (flyoutItem) => _pageService.FlyoutController.SetDetailVm<TestAnimationZeroPageVm>(true, vm => { })));

            //var skiaTree = new FlyoutItemVm("Skia Tree Test", SelectSkiaTreePage);
            var testItem2 = new FlyoutItemVm("Test List 2", null);

            //skiaTree.Children.Add(new FlyoutItemVm("Item 1-1", null));
            //skiaTree.Children.Add(new FlyoutItemVm("Item 1-2", null));

            testItem2.Children.Add(new FlyoutItemVm("Item 2-1", null));
            testItem2.Children.Add(new FlyoutItemVm("Item 2-2", null));

            //item2.Children.Add(skiaTree);
            item2.Children.Add(testItem2);

            item3.Children.Add(new FlyoutItemVm("Settings Page 1", null));

            Items.Add(item1);
            Items.Add(item2);
            Items.Add(item3);
        }
        private bool VmInitializer(object obj)
        {
            return true;
        }

        public override void OnOwnerPageAppearing()
        {
            base.OnOwnerPageAppearing();
            _pageService.FlyoutController.SetDetailVm<DefaultFlyoutPageVm>(true, vm => { });

        }
    }
}
