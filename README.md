# Maui.MvvmZero

[MvvmZero](https://github.com/Keflon/FunctionZero.MvvmZero) for MAUI.  

If you have used [MvvmZero](https://github.com/Keflon/FunctionZero.MvvmZero) for Xamarin, you'll probably be able to work out how to use this by looking at the SampleApp.  

## Features:
Ths is an exceptionally lightweight library based on `ViewModel` driven navigation.  

Setup and present any Page with any associated ViewModel ...
```csharp
pageService.PushPageAsync<ResultsPage, ResultsPageVm>((vm) => vm.Init("vm.Init (or any method) is called on your ResultsPageVm before the push" );
```
Connect the pageService to your IoC container to delegate object creation.  

Implement `IHasOwnerPage` or derive from `MvvmZeroBasePageVm` to expose lifecycle events and other goodies to your ViewModels automatically.  

Other goodies:

- PageTimer
- LatchDelayMachine
- and more...  

## QuickStart:
Look at MauiProgram.cs for registrations.  
Look at App.xaml.cs to get things off the ground.
The rest is basically the same as found in the [Xamarin Turorial](https://github.com/Keflon/MvvmZeroTutorialApp)

### Is anybody out there?
The more stars I get, the sooner I'm likely to write up some proper documentation, and even a sample app tutorial. ;)  
Whilst you're here, take a look at [Maui.zBind](https://github.com/Keflon/FunctionZero.Maui.zBind) and tell all your friends. It's already included in Maui.MvvmZero.  

If you target WinUI, you'll want to make your pages _Transient_ until [this MAUI bug](https://github.com/dotnet/maui/issues/7698) is fixed.


