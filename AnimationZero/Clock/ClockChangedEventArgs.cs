using System;

namespace AnimationZero.Clock
{
    public class ClockChangedEventArgs : EventArgs
    {
        public ClockChangedEventArgs(int clockCount)
        {
            ClockCount = clockCount;
        }

        public int ClockCount { get; }
    }
}
