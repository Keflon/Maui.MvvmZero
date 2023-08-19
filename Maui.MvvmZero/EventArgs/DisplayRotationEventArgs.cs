namespace FunctionZero.Maui.MvvmZero.Services
{
    public class DisplayRotationEventArgs
    {
        public DisplayRotationEventArgs(DisplayRotation currentRotation)
        {
            CurrentRotation = currentRotation;
        }
        public DisplayRotation CurrentRotation { get; }
    }
}