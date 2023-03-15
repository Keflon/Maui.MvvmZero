using System;
using System.Threading.Tasks;

namespace FunctionZero.Maui.MvvmZero.Services
{
    public class LatchDelayMachine
    {
        private readonly TimeSpan _clockTimespan;
        private readonly int _clockTicksBeforeAction;
        private readonly Action _delayedAction;
        private readonly Func<bool> _delayStartedAction;
        private readonly Func<int, double, bool> _clockTick;
        private readonly Action _delayKilledAction;
        private int _counter;
        private readonly IDispatcherTimer _timer;


        // TODO: Either add a 'busy' flag and a BusyChanged event (e.g. to manage a 'Busy' flag in the consumer), or
        // TODO: Add a bool to delayedAction describing whether the delayedAction was killed.
        // TODO: Add a state-object.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="millisecondClock"></param>
        /// <param name="clockTicksBeforeAction"></param>
        /// <param name="delayedAction"></param>
        /// <param name="delayStartedAction"></param>
        /// <param name="clockTick"></param>
        public LatchDelayMachine(
            int millisecondClock,
            int clockTicksBeforeAction,
            Action delayedAction,
            Func<bool> delayStartedAction = null,
            Func<int, double, bool> clockTick = null,
            Action delayKilledAction = null
            )
        {
            _clockTimespan = new TimeSpan(0, 0, 0, 0, millisecondClock);
            _clockTicksBeforeAction = clockTicksBeforeAction;
            _delayedAction = delayedAction ?? (() => { });
            _delayStartedAction = delayStartedAction ?? (() => true);
            _clockTick = clockTick ?? ((count, progress) => true);
            _delayKilledAction = delayKilledAction ?? (() => { });

            _timer = Application.Current.Dispatcher.CreateTimer();
            _timer.Interval = _clockTimespan;
            _timer.Tick += AreWeReadyToRock;
        }


        /// <summary>
        /// Calling this starts or resets the timer whose expiry raises delayedActionAsync
        /// </summary>
        /// <returns>False if an existing timer was reset; true if no timer was running and a new one was created.</returns>
        public bool Poke()
        {
            _counter = 0;

            if (!_timer.IsRunning)
            {
                if (_delayStartedAction() == false)
                    return false;

                if (_clockTick(_counter, _counter / _clockTicksBeforeAction) == false)
                    return false;

                _timer.Start();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Prevents a poked timer from continuing
        /// Note: This 'requests' a timer to stop, therefore if the timer
        /// is running, _timerIsRunning will remain true until the next timer callback.
        /// </summary>
        /// <returns>True if there was a timer running</returns>
        public bool KillPoke()
        {
            var retval = _timer.IsRunning;

            if (!retval)
                return false;

            _timer.Stop();
            _delayKilledAction();
            return retval;
        }

        private void AreWeReadyToRock(object sender, EventArgs e)
        {
            _counter++;

            // If the clock callback returns false, kill the timer.
            if (_clockTick(_counter, _counter / (double)_clockTicksBeforeAction) == false)
                _timer.Stop();

            // Otherwise, if it's time to do the delayed action ...
            else if (_counter >= _clockTicksBeforeAction)
            {
                _timer.Stop();
                // NOTE: _delayAction may restart the timer.
                _delayedAction();
            }
        }
    }
}
