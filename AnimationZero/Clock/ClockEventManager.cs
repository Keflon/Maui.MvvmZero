using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationZero.Clock
{
    public class ClockEventManager
    {
        List<IClockEventHandler> _newList;
        List<IClockEventHandler> _killList;

        public bool IsRunning { get; private set; }

        private readonly bool _allowReentrancy;

        private readonly List<IClockEventHandler> _activeClockEvents;

        // Manages whether there is anything to update, ie.e wheter the external clock needs to run.

        public event EventHandler<IsRunningChangedEventArgs> IsRunningChanged;

        public ClockEventManager(bool allowReentrancy = false)
        {
            _allowReentrancy = allowReentrancy;

            _activeClockEvents = new List<IClockEventHandler>();
            _newList = new List<IClockEventHandler>();
            _killList = new List<IClockEventHandler>();
        }

        protected void SetIsRunning(bool isRunning)
        {
            if (IsRunning != isRunning)
            {
                IsRunning = isRunning;
                IsRunningChanged?.Invoke(this, new IsRunningChangedEventArgs(isRunning));
            }
        }

        public void Add(IClockEventHandler handler)
        {
            if (_activeClockEvents.Count == 0)
                SetIsRunning(true);

            _newList.Add(handler);
        }

        public void ForceRemove(IClockEventHandler handler)
        {
            handler.Kill();
        }

        static bool _reentrant;
        public void Tick(int clock)
        {
            if (!_allowReentrancy)
            {
                if (_reentrant)
                {
                    throw new InvalidOperationException();
                }
                _reentrant = true;
            }
            foreach (IClockEventHandler newHandler in _newList)
                _activeClockEvents.Add(newHandler);

            _newList.Clear();
            _killList.Clear();

            foreach (IClockEventHandler handler in _activeClockEvents)
                if (handler.ClockTicked(clock) == false)
                    _killList.Add(handler);

            foreach (IClockEventHandler deadHandler in _killList)
                _activeClockEvents.Remove(deadHandler);

            if (_activeClockEvents.Count == 0)
                SetIsRunning(false);
            _reentrant = false;
        }
    }
}
