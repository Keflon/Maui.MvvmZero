using System;
using System.ComponentModel;
using System.Diagnostics;

namespace FunctionZero.Maui.MvvmZero.Services
{
    public class AutoPageTimer
    {
        private readonly IHasOwnerPage _ownerPageVm;
        private readonly IDispatcherTimer _timer;

        public int MillisecondInterval { get; }
        private readonly Action<object> _callback;
        private readonly Action<Exception> _exceptionHandler;
        public object State { get; }
        public bool IsPageActive => _ownerPageVm.IsOwnerPageVisible;

        public AutoPageTimer(IHasOwnerPage ownerPageVm, int millisecondInterval, Action<object> callback, Action<Exception> exceptionHandler = null, object state = null)
        {
            _ownerPageVm = ownerPageVm;
            MillisecondInterval = millisecondInterval;
            _callback = callback;
            _exceptionHandler = exceptionHandler;
            State = state;

            _timer = Application.Current.Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(MillisecondInterval);
            _timer.Tick += RawTimerCallback;


            _ownerPageVm.PropertyChanged += _ownerPage_PropertyChanged;
            if (_ownerPageVm.IsOwnerPageVisible)
                StartTimer();
        }

        private void _ownerPage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IHasOwnerPage.IsOwnerPageVisible))
            {
                var page = (IHasOwnerPage)sender;
                if (page.IsOwnerPageVisible)
                {
                    StartTimer();
                }
                else
                {
                    StopTimer();
                }
            }
        }

        private void StartTimer()
        {
            if (_timer.IsRunning)
            {
                throw new InvalidOperationException("Attempt to start a running timer!");
            }
            _timer.Start();
        }
        private void StopTimer()
        {
            if (!_timer.IsRunning)
            {
                throw new InvalidOperationException("Attempt to stop a running timer!");
            }
            _timer.Stop();
        }

        private void RawTimerCallback(object sender, EventArgs e)
        {
            if (IsPageActive)
            {
                try
                {
                    _callback(State);
                }
                catch (Exception ex)
                {
                    try
                    {
                        _exceptionHandler?.Invoke(ex);
                    }
                    catch { }
                }
            }
            else
            {
                throw new InvalidOperationException("Timer is active for an inactive page!");
            }
        }
    }
}
