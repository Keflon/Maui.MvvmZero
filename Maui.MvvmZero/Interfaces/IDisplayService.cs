using FunctionZero.Maui.MvvmZero.Services;

namespace FunctionZero.Maui.MvvmZero.Interfaces
{
    public interface IDisplayService
    {
        DisplayRotation CurrentRotation { get; }

        event EventHandler<DisplayRotationEventArgs> RotationChanged;
    }
}