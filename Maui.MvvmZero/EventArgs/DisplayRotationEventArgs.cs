namespace FunctionZero.Maui.MvvmZero.EventArgs
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