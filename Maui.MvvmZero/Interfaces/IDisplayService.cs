using FunctionZero.Maui.MvvmZero.EventArgs;

namespace FunctionZero.Maui.MvvmZero.Interfaces
{
    public interface IDisplayService
    {
        DisplayRotation CurrentRotation { get; }

        event EventHandler<DisplayRotationEventArgs> RotationChanged;
    }
}