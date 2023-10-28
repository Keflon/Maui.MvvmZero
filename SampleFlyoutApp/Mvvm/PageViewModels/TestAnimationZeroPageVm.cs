using AnimationZero.Clock;
using Microsoft.Maui;
using SampleFlyoutApp.Mvvm.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleFlyoutApp.Mvvm.PageViewModels
{



    public class TestAnimationZeroPageVm : BasePageVm
    {
        public ObservableCollection<TestItemVm> Items { get; }

        public TestAnimationZeroPageVm()
        {
            Items = new();


            var manager = new ClockEventManager();


            Items.Add(new TestItemVm(manager, "Ready?", 1000, AnimationZero.Animation.AnimationZero.CubicInOut, 700));
            Items.Add(new TestItemVm(manager, "Steady?", 2000, AnimationZero.Animation.AnimationZero.CubicInOut, 600));
            Items.Add(new TestItemVm(manager, "GO!", 3000, AnimationZero.Animation.AnimationZero.CubicInOut, 500));
            Items.Add(new TestItemVm(manager, "BOING!", 0, AnimationZero.Animation.AnimationZero.CubicInOut, 500));

            AddPageTimer(16, (state) => manager.Tick(16), null, "Owner is TestAnimationZeroVm");

        }

    }


    public class TestItemVm : BaseVm
    {
        private double _tx;
        public double Tx { get => _tx; set => SetProperty(ref _tx, value); }

        private bool _state;
        public TestItemVm(ClockEventManager manager, string text, int delayTime, Func<double, double> easing, int duration)
        {
            _manager = manager;
            Text = text;
            _delayTime = delayTime;
            _easing = easing;
            _duration = duration;
            DoTheThing();
        }

        private void DoTheThing()
        {
            if(_state)
            _manager.Add(new AnimationZero.Animation.AnimationZero(() => 0, () => 400, _easing, (val) => Tx = val, _duration, _delayTime, null, (wasForceKilled)=>DoTheThing()));
            else
                _manager.Add(new AnimationZero.Animation.AnimationZero(() => 400, () => 0, _easing, (val) => Tx = val, _duration, _delayTime, null, (wasForceKilled) => DoTheThing()));

            _state = !_state;

        }

        public string Text { get; }

        private readonly int _delayTime;
        private readonly ClockEventManager _manager;
        private readonly Func<double, double> _easing;
        private readonly int _duration;
    }
}
