using AnimationZero.Clock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationZero.Animation
{
    public class AnimationZero : IClockEventHandler
    {
        private readonly Func<double> _getStartValue;
        private readonly Func<double> _getEndValue;
        private readonly Func<double, double> _easingFunc;
        private readonly Action<double> _setValue;
        private readonly int _duration;
        private readonly int _delayTime;
        private readonly Action _startAction;
        private readonly Action<bool> _endAction;

        private int _totalElapsedTime;
        private bool _isDead;

        // https://github.com/dotnet/maui/blob/main/src/Core/src/Animations/Easing.cs
        public static readonly Func<double, double> CubicInOut = (x => x < 0.5f ? Math.Pow(x * 2.0f, 3.0f) / 2.0f : (Math.Pow((x - 1) * 2.0f, 3.0f) + 2.0f) / 2.0f);

        public AnimationZero(Func<double> getStartValue,
                          Func<double> getEndValue,
                          Func<double, double> easingFunc,
                          Action<double> setValue,
                          int duration,
                          int delayTime,
                          Action startAction,
                          Action<bool> endAction
                          )
        {
            _getStartValue = getStartValue;
            _getEndValue = getEndValue;
            _easingFunc = easingFunc;
            _setValue = setValue;
            _duration = duration;
            _delayTime = delayTime;
            _startAction = startAction ?? (() => { });
            _endAction = endAction ?? (wasForceKilled => { });

            _totalElapsedTime = 0;
        }

        public bool ClockTicked(int elapsedTime)
        {
            if (_isDead)
                return false;

            var isAnimationStarted = _totalElapsedTime != 0;

            _totalElapsedTime += elapsedTime;

            // This will be -ve if _delayTime has not elapsed.
            var elapsedAnimationTime = _totalElapsedTime - _delayTime;

            // This will be -ve if _delayTime has not elapsed.
            if (elapsedAnimationTime > 0)  // > in case elapsedTime is zero. Don't pass in zero!
            {
                var endValue = _getEndValue();

                _isDead = elapsedAnimationTime >= _duration;

                if (_isDead)
                {
                    _setValue(endValue);
                    _endAction(false);
                }
                else
                {
                    var startValue = _getStartValue();
                    if (isAnimationStarted == false)
                    {
                        isAnimationStarted = true;
                        _startAction();
                    }
                    var progress = (double)elapsedAnimationTime / _duration;
                    if (progress >= 1)
                        progress = 1;

                    var diff = endValue - startValue;
                    double offset = _easingFunc(progress) * diff;
                    _setValue(startValue + offset);
                }
            }
            return !_isDead;
        }

        public void Kill()
        {
            _isDead = true;
            _endAction(true);
        }
    }
}
