namespace FunctionZero.Maui.Services
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