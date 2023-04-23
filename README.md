# FunctionZero.Maui.MvvmZero

## Major Improvements
This is a first release for package 2.0.0. All functionality for building and navigating pages is complete. 
Some interfaces may not be final, though any changes will be simple to accommodate.  

[Nuget](https://www.nuget.org/packages/FunctionZero.Maui.MvvmZero),  [Source](https://github.com/Keflon/Maui.MvvmZero).  

`Maui.MvvmZero` is an evolution of [MvvmZero for Xamarin](https://github.com/Keflon/FunctionZero.MvvmZero)  

## Overview
This library provides an exceptionally lightweight and easy to use framework for building cross-platform MAUI 
apps using the MVVM design pattern.  
- No naming conventions are enforced, so you can use your own, or none at all.
    - There is a recommended naming convention and folder structure for those starting out fresh.
- Useful base classes are provided for your ViewModels, but they are not required or enforced. 
- The mapping of ViewModels to Views does not have to be 1:1
    - can be context-sensitive, or overridden at any time.
- Navigation by ViewModel is recommended and has first-class support. 
- Page navigation is also supported, as is mix and match MAUI navigation and MvvmZero page or ViewModel navigation without things breaking!
- Special support for Flyout and any derivatives.
- Special support for IMultiPage<Page> (e.g. TabbedPage) and any derivatives.
- ViewModel initialisation is typesafe, optional, and asynchronous if you want it to be.

## Ethos
`MvvmZero` is there to guide the way, not get in the way.  
If you understand the MVVM pattern, the aim is for MvvmZero to be intuitive, and to remain so if 
you go off the beaten track to do things your own way.


## QuickStart:
Configure `MauiProgram.cs` and launch in `App.xaml.cs`  

### MauiProgram.cs
In `MauiProgram.cs` configure `MvvmZero` and your container.  

```csharp
namespace SampleTabbedApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                *******************************
                *** CONFIGURE MVVMZERO HERE ***
                *******************************
                )
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            ********************************
            *** CONFIGURE CONTAINER HERE ***
            ********************************

            return builder.Build();
        }
    }
}
```
### Configure `MvvmZero`
```csharp
.UseMauiApp<App>()
.UseMvvmZero(
serviceBuilder =>
{
    // Configure MvvmZero ...
    serviceBuilder
    .MapVmToView<ReadyPageVm, ReadyPage>()      // Visualise a ReadyPageVm in a ReadyPage
    .MapVmToView<SteadyPageVm, SteadyPage>()
    .MapVmToView<GoPageVm, GoPage>();
    ...
}
```
### Configure your container
Register all your Pages and ViewModels in the Container.

`MvvmZero` registers `IPageService` and `NavigationPage` in the container for you.  
Unless you override the `TypeFactory` everything else `MvvmZero` is asked to instantiate **must** 
be registered in the container.  
This includes `FlyoutPage` and `TabbedPage` if you use them.  

```csharp
builder.Services
    // The root page is supplied by the container!
    // AdaptedTabbedPage Because https://github.com/dotnet/maui/issues/14572
    .AddSingleton<MultiPage<Page>, AdaptedTabbedPage>()

    .AddSingleton<ReadyPage>()
    .AddSingleton<SteadyPage>()
    .AddSingleton<GoPage>()

    .AddSingleton<ReadyPageVm>()
    .AddSingleton<SteadyPageVm>()
    .AddSingleton<GoPageVm>()

    // TestPage/Vm are transient because there can be more than one on any navigation stack at any time.
    .AddTransient<TestPage>()
    .AddTransient<TestPageVm>()
    ;
```
### App.xaml.cs

In `App.xaml.cs`
```csharp
public partial class App : Application
{
    public App(IPageServiceZero pageService)
    {
        InitializeComponent();
            
        pageService.Init(this);     // Required!

        // This app has a TabbedPage containing 3 tabs at the root.
        MainPage = pageService.GetMultiPage(VmInitializer, typeof(ReadyPageVm), typeof(SteadyPageVm), typeof(GoPageVm));
    }
    private bool VmInitializer(object viewModel)
    {
        if (viewModel is ReadyPageVm)
            return false; // Do not wrap the ReadyPage in a NavigationPage.

        return true;
    }
}
```

# Samples
There are 3 sample applications in the repo.
They are what I use for development, so they're not pretty.
1. SampleApp. Has a root navigation page and pages can be pushed / popped.
2. SampleFlyoutApp. This has a flyout managing multiple navigation pages, one of which is a TabbedPage.
3. SampleTabbedApp. This has a TabbedPage managing 3 Tabs, 2 of which are navigation pages.

For each sample application, look at:
1. MauiProgram.cs for registrations.
2. App.xaml.cs to get things off the ground.

### Workarounds
Currently the samples use `AdaptedTabbedPage` because [TabbedPageBug](https://github.com/dotnet/maui/issues/14572) 
and `AdaptedFlyoutPage` because [AdaptedPageBug](https://github.com/dotnet/maui/issues/13496)  
If you target WinUI, you'll want to make any _pushed_ pages _Transient_ until [this bug](https://github.com/dotnet/maui/issues/7698) is fixed.

The rest is basically the same as found in the [Xamarin Turorial](https://github.com/Keflon/MvvmZeroTutorialApp)

### Is anybody out there?

Full documentation and better samples are on the way! Give me encouragement by starring the repo!  
Whilst you're here, take a look at [Maui.zBind](https://github.com/Keflon/FunctionZero.Maui.zBind) and tell all your friends. It's already included in `MvvmZero`.  



