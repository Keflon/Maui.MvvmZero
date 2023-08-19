using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FunctionZero.Maui.MvvmZero.Interfaces;
using FunctionZero.Maui.MvvmZero.Services;

namespace FunctionZero.Maui.Services
{
    public class DisplayService : IDisplayService
    {
        public DisplayRotation CurrentRotation { get; protected set; }

        public event EventHandler<DisplayRotationEventArgs> RotationChanged;
        public DisplayService()
        {
            CurrentRotation = DeviceDisplay.Current.MainDisplayInfo.Rotation;
            DeviceDisplay.Current.MainDisplayInfoChanged += InfoChanged;
        }

        private void InfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            if (CurrentRotation != e.DisplayInfo.Rotation)
            {
                CurrentRotation = e.DisplayInfo.Rotation;
                RotationChanged?.Invoke(this, new DisplayRotationEventArgs(CurrentRotation));
            }
        }
    }
}
