using System;

namespace AnimationZero.Clock
{
    public class IsRunningChangedEventArgs : EventArgs
    {
        public IsRunningChangedEventArgs(bool hasActiveEventHandler)
        {
            HasActiveEventHandler = hasActiveEventHandler;
        }

        public bool HasActiveEventHandler { get; }
    }
}